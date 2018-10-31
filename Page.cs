//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        http://www.cliversoft.com
//********************************************************************************************
using System;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Cliver.PdfDocumentParser
{
    /// <summary>
    /// pdf page parsing API
    /// </summary>
    public class Page : IDisposable
    {
        internal Page(PageCollection pageCollection, int pageI)
        {
            this.pageCollection = pageCollection;
            this.pageI = pageI;
        }
        int pageI;
        PageCollection pageCollection;

        ~Page()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (_bitmap != null)
                {
                    _bitmap.Dispose();
                    _bitmap = null;
                }
                if (_activeTemplateBitmap != null)
                {
                    _activeTemplateBitmap.Dispose();
                    _activeTemplateBitmap = null;
                }
                if (_activeTemplateImageData != null)
                {
                    //_activeTemplateImageData.Dispose();
                    _activeTemplateImageData = null;
                }
                if (_pdfCharBoxs != null)
                {
                    _pdfCharBoxs = null;
                }
                if (_activeTemplateOcrCharBoxs != null)
                {
                    _activeTemplateOcrCharBoxs = null;
                }
            }
        }

        internal Bitmap Bitmap
        {
            get
            {
                if (_bitmap == null)
                    _bitmap = Pdf.RenderBitmap(pageCollection.PdfFile, pageI, Settings.Constants.PdfPageImageResolution);
                return _bitmap;
            }
        }
        Bitmap _bitmap;

        internal void OnActiveTemplateUpdating(Template newTemplate)
        {
            if (pageCollection.ActiveTemplate == null)
                return;

            if (newTemplate.PageRotation != pageCollection.ActiveTemplate.PageRotation || newTemplate.AutoDeskew != pageCollection.ActiveTemplate.AutoDeskew)
            {
                if (_activeTemplateImageData != null)
                {
                    //_activeTemplateImageData.Dispose();
                    _activeTemplateImageData = null;
                }
                if (_activeTemplateBitmap != null)
                {
                    _activeTemplateBitmap.Dispose();
                    _activeTemplateBitmap = null;
                }
                if (_activeTemplateOcrCharBoxs != null)
                    _activeTemplateOcrCharBoxs = null;

                anchorHashes2anchorRectangles.Clear();
            }

            //if (pageCollection.ActiveTemplate.Name != newTemplate.Name)
            //    anchorValueStrings2rectangles.Clear();
        }

        Bitmap getRectangleFromActiveTemplateBitmap(float x, float y, float w, float h)
        {
            Rectangle r = new Rectangle(0, 0, ActiveTemplateBitmap.Width, ActiveTemplateBitmap.Height);
            r.Intersect(new Rectangle((int)x, (int)y, (int)w, (int)h));
            return ActiveTemplateBitmap.Clone(r, System.Drawing.Imaging.PixelFormat.Undefined);
            //return ImageRoutines.GetCopy(ActiveTemplateBitmap, new RectangleF(x, y, w, h));
        }

        internal Bitmap ActiveTemplateBitmap
        {
            get
            {
                if (_activeTemplateBitmap == null)
                {
                    if (pageCollection.ActiveTemplate == null)
                        return null;

                    //From stackoverflow:
                    //Using the Graphics class to modify the clone (created with .Clone()) will not modify the original.
                    //Similarly, using the LockBits method yields different memory blocks for the original and clone.
                    //...change one random pixel to a random color on the clone... seems to trigger a copy of all pixel data from the original.
                    Bitmap b = Bitmap.Clone(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), System.Drawing.Imaging.PixelFormat.Undefined);

                    switch (pageCollection.ActiveTemplate.PageRotation)
                    {
                        case Template.PageRotations.NONE:
                            break;
                        case Template.PageRotations.Clockwise90:
                            b.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case Template.PageRotations.Clockwise180:
                            b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case Template.PageRotations.Clockwise270:
                            b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case Template.PageRotations.AutoDetection:
                            float confidence;
                            Tesseract.Orientation o = Ocr.This.DetectOrientation(b, out confidence);
                            switch (o)
                            {
                                case Tesseract.Orientation.PageUp:
                                    break;
                                case Tesseract.Orientation.PageRight:
                                    b.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;
                                case Tesseract.Orientation.PageDown:
                                    b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    break;
                                case Tesseract.Orientation.PageLeft:
                                    b.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    break;
                                default:
                                    throw new Exception("Unknown option: " + o);
                            }
                            break;
                        default:
                            throw new Exception("Unknown option: " + pageCollection.ActiveTemplate.PageRotation);
                    }

                    if (pageCollection.ActiveTemplate.AutoDeskew)
                    {
                        using (ImageMagick.MagickImage image = new ImageMagick.MagickImage(b))
                        {
                            //image.Density = new PointD(600, 600);
                            //image.AutoLevel();
                            //image.Negate();
                            //image.AdaptiveThreshold(10, 10, new ImageMagick.Percentage(20));
                            //image.Negate();
                            image.Deskew(new ImageMagick.Percentage(pageCollection.ActiveTemplate.AutoDeskewThreshold));
                            //image.AutoThreshold(AutoThresholdMethod.OTSU);
                            //image.Despeckle();
                            //image.WhiteThreshold(new Percentage(20));
                            //image.Trim();
                            b.Dispose();
                            b = image.ToBitmap();
                        }
                    }

                    _activeTemplateBitmap = b;
                }
                return _activeTemplateBitmap;
            }
        }

        Bitmap _activeTemplateBitmap = null;

        internal PointF? GetAnchorPoint0(int anchorId)
        {
            Template.Anchor a = pageCollection.ActiveTemplate.Anchors.Find(x => x.Id == anchorId);
            List<RectangleF> rs = GetAnchorRectangles(a);
            if (rs == null || rs.Count < 1)
                return null;
            return new PointF(rs[0].X, rs[0].Y);
        }

        internal List<RectangleF> GetAnchorRectangles(Template.Anchor a)
        {
            List<RectangleF> rs;
            StringBuilder sb = new StringBuilder(SerializationRoutines.Json.Serialize(a));
            for (int? id = a.ParentAnchorId; id != null;)
            {
                Template.Anchor pa = pageCollection.ActiveTemplate.Anchors.Find(x => x.Id == id);
                if (a == pa)
                    throw new Exception("Anchor[Id=" + a.Id + "] is referenced by an ancestor anchor.");
                sb.Append(SerializationRoutines.Json.Serialize(pa));
                id = pa.ParentAnchorId;
            }
            string sa = sb.ToString();
            if (!anchorHashes2anchorRectangles.TryGetValue(sa, out rs))
            {
                rs = findAnchor(a);
                anchorHashes2anchorRectangles[sa] = rs;
            }
            return rs;
        }
        Dictionary<string, List<RectangleF>> anchorHashes2anchorRectangles = new Dictionary<string, List<RectangleF>>();

        List<RectangleF> findAnchor(Template.Anchor a)
        {
            if (a == null)
                return null;

            RectangleF mainElementSearchRectangle;
            RectangleF mainElementInitialRectangle = a.MainElementInitialRectangle();
            if (a.ParentAnchorId != null)
            {
                Template.Anchor pa = pageCollection.ActiveTemplate.Anchors.Find(x => x.Id == a.ParentAnchorId);
                List<RectangleF> rs = GetAnchorRectangles(pa);
                if (rs == null || rs.Count < 1)
                    return null;
                RectangleF paInitialRectangle = pa.MainElementInitialRectangle();
                mainElementSearchRectangle = getSearchRectangle(new RectangleF(mainElementInitialRectangle.X + paInitialRectangle.X - rs[0].X, mainElementInitialRectangle.Y + paInitialRectangle.Y - rs[0].Y, mainElementInitialRectangle.Width, mainElementInitialRectangle.Height), a.SearchRectangleMargin);
            }
            else
                mainElementSearchRectangle = getSearchRectangle(mainElementInitialRectangle, a.SearchRectangleMargin);

            switch (a.Type)
            {
                case Template.Types.PdfText:
                    {
                        Template.Anchor.PdfText ptv = (Template.Anchor.PdfText)a;
                        List<Template.Anchor.PdfText.CharBox> aes = ptv.CharBoxs;
                        if (aes.Count < 1)
                            return null;
                        IEnumerable<Pdf.CharBox> bt0s;
                        if (ptv.SearchRectangleMargin < 0)
                            bt0s = PdfCharBoxs.Where(x => x.Char == aes[0].Char);
                        else
                            bt0s = PdfCharBoxs.Where(x => x.Char == aes[0].Char && mainElementSearchRectangle.Contains(x.R));
                        List<Pdf.CharBox> bts = new List<Pdf.CharBox>();
                        foreach (Pdf.CharBox bt0 in bt0s)
                        {
                            bts.Clear();
                            bts.Add(bt0);
                            for (int i = 1; i < aes.Count; i++)
                            {
                                PointF p;
                                if (ptv.PositionDeviationIsAbsolute)
                                    p = new PointF(bt0.R.X + aes[i].Rectangle.X - aes[0].Rectangle.X, bt0.R.Y + aes[i].Rectangle.Y - aes[0].Rectangle.Y);
                                else
                                    p = new PointF(bts[i - 1].R.X + aes[i].Rectangle.X - aes[i - 1].Rectangle.X, bts[i - 1].R.Y + aes[i].Rectangle.Y - aes[i - 1].Rectangle.Y);
                                foreach (Pdf.CharBox bt in PdfCharBoxs.Where(x => x.Char == aes[i].Char))
                                    if (Math.Abs(bt.R.X - p.X) <= ptv.PositionDeviation && Math.Abs(bt.R.Y - p.Y) <= ptv.PositionDeviation)
                                    {
                                        bts.Add(bt);
                                        break;
                                    }
                                if (bts.Count - 1 < i)
                                    break;
                            }
                            if (bts.Count == aes.Count)
                                return bts.Select(x => x.R).ToList();
                        }
                    }
                    return null;
                case Template.Types.OcrText:
                    {
                        Template.Anchor.OcrText otv = (Template.Anchor.OcrText)a;
                        List<Template.Anchor.OcrText.CharBox> aes = otv.CharBoxs;
                        if (aes.Count < 1)
                            return null;
                        //List<Ocr.CharBox> contaningOcrCharBoxs;//does not work properly because Tesseract recongnizes a big fragment and a small fragment differently!
                        IEnumerable<Ocr.CharBox> bt0s;
                        if (otv.SearchRectangleMargin < 0)
                        {
                            //contaningOcrCharBoxs = ActiveTemplateOcrCharBoxs;
                            bt0s = ActiveTemplateOcrCharBoxs.Where(x => x.Char == aes[0].Char);
                        }
                        else
                        {
                            //RectangleF contaningRectangle = mainElementSearchRectangle;
                            //for (int i = 1; i < otv.CharBoxs.Count; i++)
                            //    contaningRectangle = RectangleF.Union(contaningRectangle, getSearchRectangle(otv.CharBoxs[i].Rectangle.GetSystemRectangleF(), a.SearchRectangleMargin));
                            //contaningOcrCharBoxs = Ocr.This.GetCharBoxs(getRectangleFromActiveTemplateBitmap(contaningRectangle.X / Settings.Constants.Image2PdfResolutionRatio, contaningRectangle.Y / Settings.Constants.Image2PdfResolutionRatio, contaningRectangle.Width / Settings.Constants.Image2PdfResolutionRatio, contaningRectangle.Height / Settings.Constants.Image2PdfResolutionRatio));
                            //bt0s = contaningOcrCharBoxs.Where(x => x.Char == aes[0].Char && mainElementSearchRectangle.Contains(x.R));
                            bt0s = ActiveTemplateOcrCharBoxs.Where(x => x.Char == aes[0].Char && mainElementSearchRectangle.Contains(x.R));
                        }
                        List<Ocr.CharBox> bts = new List<Ocr.CharBox>();
                        foreach (Ocr.CharBox bt0 in bt0s)
                        {
                            bts.Clear();
                            bts.Add(bt0);
                            for (int i = 1; i < aes.Count; i++)
                            {
                                PointF p;
                                if (otv.PositionDeviationIsAbsolute)
                                    p = new PointF(bt0.R.X + aes[i].Rectangle.X - aes[0].Rectangle.X, bt0.R.Y + aes[i].Rectangle.Y - aes[0].Rectangle.Y);
                                else
                                    p = new PointF(bts[i - 1].R.X + aes[i].Rectangle.X - aes[i - 1].Rectangle.X, bts[i - 1].R.Y + aes[i].Rectangle.Y - aes[i - 1].Rectangle.Y);
                                foreach (Ocr.CharBox bt in /*contaningOcrCharBoxs*/ActiveTemplateOcrCharBoxs.Where(x => x.Char == aes[i].Char))
                                    if (Math.Abs(bt.R.X - p.X) <= otv.PositionDeviation && Math.Abs(bt.R.Y - p.Y) <= otv.PositionDeviation)
                                    {
                                        bts.Add(bt);
                                        break;
                                    }
                                if (bts.Count - 1 < i)
                                    break;
                            }
                            if (bts.Count == aes.Count)
                                return bts.Select(x => x.R).ToList();
                        }
                    }
                    return null;
                case Template.Types.ImageData:
                    {
                        Template.Anchor.ImageData idv = (Template.Anchor.ImageData)a;
                        List<Template.Anchor.ImageData.ImageBox> ibs = idv.ImageBoxs;
                        if (ibs.Count < 1)
                            return null;
                        Point shift;
                        ImageData id0;
                        if (idv.SearchRectangleMargin < 0)
                        {
                            id0 = ActiveTemplateImageData;
                            shift = new Point(0, 0);
                        }
                        else
                        {
                            id0 = new ImageData(getRectangleFromActiveTemplateBitmap(mainElementSearchRectangle.X / Settings.Constants.Image2PdfResolutionRatio, mainElementSearchRectangle.Y / Settings.Constants.Image2PdfResolutionRatio, mainElementSearchRectangle.Width / Settings.Constants.Image2PdfResolutionRatio, mainElementSearchRectangle.Height / Settings.Constants.Image2PdfResolutionRatio));
                            shift = new Point(mainElementSearchRectangle.X < 0 ? 0 : (int)mainElementSearchRectangle.X, mainElementSearchRectangle.Y < 0 ? 0 : (int)mainElementSearchRectangle.Y);
                        }
                        List<RectangleF> bestRs = null;
                        float minDeviation = float.MaxValue;
                        ibs[0].ImageData.FindWithinImage(id0, idv.BrightnessTolerance, idv.DifferentPixelNumberTolerance, (Point point0, float deviation) =>
                        {
                            point0 = new Point(shift.X + point0.X, shift.Y + point0.Y);
                            List<RectangleF> rs = new List<RectangleF>();
                            rs.Add(new RectangleF(point0, new SizeF(ibs[0].Rectangle.Width, ibs[0].Rectangle.Height)));
                            for (int i = 1; i < ibs.Count; i++)
                            {
                                Template.RectangleF r;
                                if (idv.PositionDeviationIsAbsolute)
                                    r = new Template.RectangleF(point0.X + ibs[i].Rectangle.X - ibs[0].Rectangle.X - idv.PositionDeviation, point0.Y + ibs[i].Rectangle.Y - ibs[0].Rectangle.Y - idv.PositionDeviation, ibs[i].Rectangle.Width + 2 * idv.PositionDeviation, ibs[i].Rectangle.Height + 2 * idv.PositionDeviation);
                                else
                                    r = new Template.RectangleF(rs[i - 1].X + ibs[i].Rectangle.X - ibs[i - 1].Rectangle.X - idv.PositionDeviation, rs[i - 1].Y + ibs[i].Rectangle.Y - ibs[i - 1].Rectangle.Y - idv.PositionDeviation, ibs[i].Rectangle.Width + 2 * idv.PositionDeviation, ibs[i].Rectangle.Height + 2 * idv.PositionDeviation);
                                using (Bitmap rb = getRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                                {
                                    if (null == ibs[i].ImageData.FindWithinImage(new ImageData(rb), idv.BrightnessTolerance, idv.DifferentPixelNumberTolerance, false))
                                        return true;
                                }
                                rs.Add(r.GetSystemRectangleF());
                            }
                            if (!idv.FindBestImageMatch)
                            {
                                bestRs = rs;
                                return false;
                            }
                            if (deviation < minDeviation)
                            {
                                minDeviation = deviation;
                                bestRs = rs;
                            }
                            return true;
                        });
                        return bestRs;
                    }
                default:
                    throw new Exception("Unknown option: " + a.Type);
            }
        }
        RectangleF getSearchRectangle(RectangleF rectangle0, int margin/*, System.Drawing.RectangleF pageRectangle*/)
        {
            RectangleF r = new RectangleF(rectangle0.X - margin, rectangle0.Y - margin, rectangle0.Width + 2 * margin, rectangle0.Height + 2 * margin);
            //r.Intersect(pageRectangle);
            return r;
        }

        internal ImageData ActiveTemplateImageData
        {
            get
            {
                if (_activeTemplateImageData == null)
                    _activeTemplateImageData = new ImageData(ActiveTemplateBitmap);
                return _activeTemplateImageData;
            }
        }
        ImageData _activeTemplateImageData;

        internal List<Pdf.CharBox> PdfCharBoxs
        {
            get
            {
                if (_pdfCharBoxs == null)
                    _pdfCharBoxs = Pdf.GetCharBoxsFromPage(pageCollection.PdfReader, pageI);
                return _pdfCharBoxs;
            }
        }
        List<Pdf.CharBox> _pdfCharBoxs;

        internal List<Ocr.CharBox> ActiveTemplateOcrCharBoxs
        {
            get
            {
                if (_activeTemplateOcrCharBoxs == null)
                {
                    _activeTemplateOcrCharBoxs = Ocr.This.GetCharBoxs(ActiveTemplateBitmap);
                }
                return _activeTemplateOcrCharBoxs;
            }
        }
        List<Ocr.CharBox> _activeTemplateOcrCharBoxs;

        public bool IsAnchorGroupFound(string anchorGroup)//to be used instead of IsDocumentFirstPage
        {
            if (string.IsNullOrWhiteSpace(anchorGroup))
                return false;
            List<Template.Anchor> as_ = pageCollection.ActiveTemplate.Anchors.Where(a => a.Group == anchorGroup).ToList();
            if (as_.Count < 1)
                return false;
            foreach (Template.Anchor a in as_)
                if (GetAnchorRectangles(a) == null)
                    return false;
            return true;
        }

        public object GetValue(int? anchorId, Template.RectangleF r, Template.Types valueType, out string error)
        {
            if (r == null)
            {
                error = "Rectangular is not defined.";
                return null;
            }
            if (r.Width <= Settings.Constants.CoordinateDeviationMargin || r.Height <= Settings.Constants.CoordinateDeviationMargin)
            {
                error = "Rectangular is malformed.";
                return null;
            }
            if (anchorId != null)
            {
                PointF? p0_;
                p0_ = GetAnchorPoint0((int)anchorId);
                if (p0_ == null)
                {
                    error = "Anchor[" + anchorId + "] is not found.";
                    return null;
                }
                PointF p0 = (PointF)p0_;
                Template.Anchor a = pageCollection.ActiveTemplate.Anchors.Find(x => x.Id == anchorId);
                RectangleF air = a.MainElementInitialRectangle();
                r.X += p0.X - air.X;
                r.Y += p0.Y - air.Y;
            }
            error = null;
            switch (valueType)
            {
                case Template.Types.PdfText:
                    return Pdf.GetTextByTopLeftCoordinates(PdfCharBoxs, r.GetSystemRectangleF());
                case Template.Types.OcrText:
                    //return Ocr.GetTextByTopLeftCoordinates(ActiveTemplateOcrCharBoxs, r.GetSystemRectangleF());//for unknown reason tesseract often parses a whole page much worse than a fragment and so ActiveTemplateOcrCharBoxs give not reliable result.
                    return Ocr.This.GetText(ActiveTemplateBitmap, r.GetSystemRectangleF());
                case Template.Types.ImageData:
                    using (Bitmap rb = getRectangleFromActiveTemplateBitmap(r.X / Settings.Constants.Image2PdfResolutionRatio, r.Y / Settings.Constants.Image2PdfResolutionRatio, r.Width / Settings.Constants.Image2PdfResolutionRatio, r.Height / Settings.Constants.Image2PdfResolutionRatio))
                    {
                        return new ImageData(rb);
                    }
                default:
                    throw new Exception("Unknown option: " + valueType);
            }
            //}
            //catch(Exception e)
            //{
            //    error = Log.GetExceptionMessage(e);
            //}
            //return null;
        }
        static Dictionary<Bitmap, ImageData> bs2id = new Dictionary<Bitmap, ImageData>();

        Dictionary<string, string> fieldNames2texts = new Dictionary<string, string>();

        public static string NormalizeText(string value)
        {
            if (value == null)
                return null;
            value = FieldPreparation.RemoveNonPrintablesRegex.Replace(value, " ");
            value = Regex.Replace(value, @"\s+", " ");
            value = value.Trim();
            return value;
        }
    }
}