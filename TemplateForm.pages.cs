﻿//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// template editor GUI
    /// </summary>
    public partial class TemplateForm : Form
    {
        PageCollection pages = null;

        void reloadPageBitmaps()
        {
            if (pages == null)
                return;
            pages.Clear();
            pages.ActiveTemplate = getTemplateFromUI(false);
            showPage(currentPage);
        }

        void setScaledImage()
        {
            if (pages == null)
                return;
            if (scaledCurrentPageBitmap != null)
                scaledCurrentPageBitmap.Dispose();
            if(pages[currentPage].ActiveTemplateBitmap == null)
                pages.ActiveTemplate = getTemplateFromUI(false);
            scaledCurrentPageBitmap = ImageRoutines.GetScaled(pages[currentPage].ActiveTemplateBitmap, (float)pictureScale.Value * Settings.Constants.Image2PdfResolutionRatio);
            if (picture.Image != null)
                picture.Image.Dispose();
            picture.Image = new Bitmap(scaledCurrentPageBitmap);
        }
        Bitmap scaledCurrentPageBitmap;

        PointF? findAndDrawAnchor(int anchorId, bool renewImage = true)
        {
            DataGridViewRow row;
            Template.Anchor a = getAnchor(anchorId, out row);
            if (a == null || row == null)
                throw new Exception("Anchor[Id=" + anchorId + "] does not exist.");

            if (pages == null)
                return null;

            pages.ActiveTemplate = getTemplateFromUI(false);
            a = pages.ActiveTemplate.Anchors.FirstOrDefault(x => x.Id == anchorId);
            if (a == null)
                throw new Exception("Anchor[Id=" + a.Id + "] is not defined.");

            PointF? p0 = null;
            for (Template.Anchor a_ = a; a_ != null; a_ = pages.ActiveTemplate.Anchors.FirstOrDefault(x => x.Id == a_.ParentAnchorId))
            {
                DataGridViewRow r;
                getAnchor(a_.Id, out r);
                if (!a_.IsSet())
                {
                    setRowStatus(statuses.WARNING, r, "Not set");
                    if (a == a_)
                    {
                        clearPicture(renewImage);
                        return null;
                    }
                    continue;
                }
                List<RectangleF> rs = pages[currentPage].GetAnchorRectangles(a_);
                if (rs == null || rs.Count < 1)
                {
                    setRowStatus(statuses.ERROR, r, "Not found");
                    if (a == a_)
                    {
                        clearPicture(renewImage);
                        return null;
                    }
                }
                setRowStatus(statuses.SUCCESS, r, "Found");

                drawBoxes(Settings.Appearance.AnchorMasterBoxColor, new List<System.Drawing.RectangleF> { rs[0] }, a == a_ ? renewImage : false);
                if (rs.Count > 1)
                    drawBoxes(Settings.Appearance.AnchorSecondaryBoxColor, rs.GetRange(1, rs.Count - 1), false);

                if (a == a_)
                    p0 = new PointF(rs[0].X, rs[0].Y);
            }
            return p0;
        }

        object extractValueAndDrawSelectionBox(int? anchorId, Template.RectangleF r, Template.Types valueType, bool renewImage = true)
        {
            try
            {
                if (pages == null)
                    return null;
                pages.ActiveTemplate = getTemplateFromUI(false);

                float x = r.X;
                float y = r.Y;
                if (anchorId != null)
                {
                    PointF? p0_ = findAndDrawAnchor((int)anchorId);
                    if (p0_ == null)
                        return null;
                    PointF p0 = (PointF)p0_;
                    DataGridViewRow row;
                    Template.Anchor a = getAnchor(anchorId, out row);
                    RectangleF air = a.MainElementInitialRectangle();
                    x += p0.X - air.X;
                    y += p0.Y - air.Y;

                    renewImage = false;
                }

                if (r == null)
                    return null;

                RectangleF r_ = new RectangleF(x, y, r.Width, r.Height);
                drawBoxes(Settings.Appearance.SelectionBoxColor, new List<RectangleF> { r_ }, renewImage);

                string error;
                object v = pages[currentPage].GetValue(null, new Template.RectangleF(x, y, r.Width, r.Height), valueType, out error);
                switch (valueType)
                {
                    case Template.Types.PdfText:
                        return Page.NormalizeText((string)v);
                    case Template.Types.OcrText:
                        return Page.NormalizeText((string)v);
                    case Template.Types.ImageData:
                        return v;
                    default:
                        throw new Exception("Unknown option: " + valueType);
                }
            }
            catch (Exception ex)
            {
                //LogMessage.Error("Rectangle", ex);
                LogMessage.Error(ex);
            }
            return null;
        }

        void drawBoxes(Color c, IEnumerable<System.Drawing.RectangleF> rs, bool renewImage)
        {
            if (pages == null)
                return;

            Bitmap bm;
            if (renewImage)
                bm = new Bitmap(scaledCurrentPageBitmap);
            else
                bm = new Bitmap(picture.Image);

            using (Graphics gr = Graphics.FromImage(bm))
            {
                float factor = (float)pictureScale.Value;
                Pen p = new Pen(c);
                foreach (System.Drawing.RectangleF r in rs)
                {
                    System.Drawing.Rectangle r_ = new System.Drawing.Rectangle((int)(r.X * factor), (int)(r.Y * factor), (int)(r.Width * factor), (int)(r.Height * factor));
                    //if (invertColor)
                    //{
                    //    for (int i = r_.X; i <= r_.X + r_.Width; i++)
                    //        for (int j = r_.Y; j <= r_.Y + r_.Height; j++)
                    //        {
                    //            Color rgb = bm.GetPixel(i, j);
                    //            rgb = Color.FromArgb(255 - rgb.R, 255 - rgb.G, 255 - rgb.B);
                    //            bm.SetPixel(i, j, rgb);
                    //        }
                    //}
                    gr.DrawRectangle(p, r_);
                }
            }
            if (picture.Image != null)
                picture.Image.Dispose();
            picture.Image = bm;
        }
        Point selectionBoxPoint0, selectionBoxPoint1, selectionBoxPoint2;
        bool drawingSelectionBox = false;

        void clearPicture(bool renewImage)
        {
            if (pages == null)
                return;

            Bitmap bm;
            if (renewImage)
            {
                bm = new Bitmap(scaledCurrentPageBitmap);
                if (picture.Image != null)
                    picture.Image.Dispose();
                picture.Image = bm;
                return;
            }
        }

        void showPage(int page_i)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(testFile.Text) || 0 >= page_i || totalPageNumber < page_i)
                    return;

                foreach (DataGridViewRow r in fields.Rows)
                    r.Cells["Value"].Value = null;

                currentPage = page_i;
                tCurrentPage.Text = currentPage.ToString();

                setScaledImage();
                enableNavigationButtons();

                anchors.CurrentCell = null;//1-st row is autoselected
                fields.CurrentCell = null;//1-st row is autoselected
                anchors.ClearSelection();//1-st row is autoselected
                fields.ClearSelection();//1-st row is autoselected
                //setCurrentAnchorRow(null, true);
                //setCurrentMarkRow(null);
                //setCurrentFieldRow(null);
                loadingTemplate = false;

                if (ExtractFieldsAutomaticallyWhenPageChanged.Checked)
                {
                    foreach (DataGridViewRow row in fields.Rows)
                    {
                        Template.Field f = (Template.Field)row.Tag;
                        if (f == null)
                            continue;
                        if (f.IsSet())
                        {
                            row.Cells["Value"].Value = extractValueAndDrawSelectionBox(f.AnchorId, f.Rectangle, f.Type);
                            if (row.Cells["Value"].Value != null)
                                setRowStatus(statuses.SUCCESS, row, "Found");
                            else
                                setRowStatus(statuses.ERROR, row, "Not found");
                        }
                        else
                            setRowStatus(statuses.WARNING, row, "Not set");
                    }
                }

                if (CheckAnchorGroupsAutomaticallyWhenPageChanged.Checked)
                    setAnchorGroupStatuses();
            }
            catch (Exception e)
            {
                LogMessage.Error(e);
            }
        }
        int currentPage;
        int totalPageNumber;

        private void bPrevPage_Click(object sender, EventArgs e)
        {
            showPage(currentPage - 1);
        }

        private void bNextPage_Click(object sender, EventArgs e)
        {
            showPage(currentPage + 1);
        }

        void enableNavigationButtons()
        {
            bPrevPage.Enabled = currentPage > 1;
            bNextPage.Enabled = currentPage < totalPageNumber;
        }

        private void tCurrentPage_Leave(object sender, EventArgs e)
        {
            changeCurrentPage();
        }

        private void changeCurrentPage()
        {
            int i = 0;
            if (int.TryParse(tCurrentPage.Text, out i))
            {
                if (i != currentPage)
                    showPage(i);
            }
            else
            {
                LogMessage.Error("Page is not a number.");
                tCurrentPage.Text = currentPage.ToString();
            }
        }

        private void tCurrentPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                changeCurrentPage();
        }
    }
}