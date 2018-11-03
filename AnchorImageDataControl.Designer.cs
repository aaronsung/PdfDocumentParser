﻿namespace Cliver.PdfDocumentParser
{
    partial class AnchorImageDataControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label20 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.FindBestImageMatch = new System.Windows.Forms.CheckBox();
            this.BrightnessTolerance = new System.Windows.Forms.NumericUpDown();
            this.DifferentPixelNumberTolerance = new System.Windows.Forms.NumericUpDown();
            this.pictures = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.PositionDeviationIsAbsolute = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cSearchRectangleMargin = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SearchRectangleMargin = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.PositionDeviation = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentPixelNumberTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionDeviation)).BeginInit();
            this.SuspendLayout();
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(3, 1);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(119, 13);
            this.label20.TabIndex = 66;
            this.label20.Text = "Find Best Image Match:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 43);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(163, 13);
            this.label13.TabIndex = 63;
            this.label13.Text = "Different Pixel NumberTolerance:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(110, 13);
            this.label11.TabIndex = 61;
            this.label11.Text = "Brightness Tolerance:";
            // 
            // FindBestImageMatch
            // 
            this.FindBestImageMatch.AutoSize = true;
            this.FindBestImageMatch.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.FindBestImageMatch.Location = new System.Drawing.Point(174, 1);
            this.FindBestImageMatch.Name = "FindBestImageMatch";
            this.FindBestImageMatch.Size = new System.Drawing.Size(15, 14);
            this.FindBestImageMatch.TabIndex = 65;
            this.FindBestImageMatch.UseVisualStyleBackColor = true;
            // 
            // BrightnessTolerance
            // 
            this.BrightnessTolerance.DecimalPlaces = 2;
            this.BrightnessTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.BrightnessTolerance.Location = new System.Drawing.Point(175, 18);
            this.BrightnessTolerance.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BrightnessTolerance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.BrightnessTolerance.Name = "BrightnessTolerance";
            this.BrightnessTolerance.Size = new System.Drawing.Size(47, 20);
            this.BrightnessTolerance.TabIndex = 62;
            this.BrightnessTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // DifferentPixelNumberTolerance
            // 
            this.DifferentPixelNumberTolerance.DecimalPlaces = 2;
            this.DifferentPixelNumberTolerance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.DifferentPixelNumberTolerance.Location = new System.Drawing.Point(175, 40);
            this.DifferentPixelNumberTolerance.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DifferentPixelNumberTolerance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.DifferentPixelNumberTolerance.Name = "DifferentPixelNumberTolerance";
            this.DifferentPixelNumberTolerance.Size = new System.Drawing.Size(47, 20);
            this.DifferentPixelNumberTolerance.TabIndex = 64;
            this.DifferentPixelNumberTolerance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // pictures
            // 
            this.pictures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictures.AutoSize = true;
            this.pictures.Location = new System.Drawing.Point(0, 122);
            this.pictures.Name = "pictures";
            this.pictures.Size = new System.Drawing.Size(223, 53);
            this.pictures.TabIndex = 75;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 76;
            this.label4.Text = "Pattern:";
            // 
            // PositionDeviationIsAbsolute
            // 
            this.PositionDeviationIsAbsolute.AutoSize = true;
            this.PositionDeviationIsAbsolute.Location = new System.Drawing.Point(151, 64);
            this.PositionDeviationIsAbsolute.Name = "PositionDeviationIsAbsolute";
            this.PositionDeviationIsAbsolute.Size = new System.Drawing.Size(15, 14);
            this.PositionDeviationIsAbsolute.TabIndex = 91;
            this.PositionDeviationIsAbsolute.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(163, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 98;
            this.label1.Text = ")";
            // 
            // cSearchRectangleMargin
            // 
            this.cSearchRectangleMargin.AutoSize = true;
            this.cSearchRectangleMargin.Location = new System.Drawing.Point(151, 86);
            this.cSearchRectangleMargin.Name = "cSearchRectangleMargin";
            this.cSearchRectangleMargin.Size = new System.Drawing.Size(15, 14);
            this.cSearchRectangleMargin.TabIndex = 97;
            this.cSearchRectangleMargin.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 95;
            this.label3.Text = "Search Rectangle Margin:";
            // 
            // SearchRectangleMargin
            // 
            this.SearchRectangleMargin.Location = new System.Drawing.Point(175, 84);
            this.SearchRectangleMargin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.SearchRectangleMargin.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.SearchRectangleMargin.Name = "SearchRectangleMargin";
            this.SearchRectangleMargin.Size = new System.Drawing.Size(47, 20);
            this.SearchRectangleMargin.TabIndex = 96;
            this.SearchRectangleMargin.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(98, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 94;
            this.label2.Text = "(absolute:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 13);
            this.label5.TabIndex = 92;
            this.label5.Text = "Position Deviation:";
            // 
            // PositionDeviation
            // 
            this.PositionDeviation.DecimalPlaces = 1;
            this.PositionDeviation.Location = new System.Drawing.Point(175, 62);
            this.PositionDeviation.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.PositionDeviation.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.PositionDeviation.Name = "PositionDeviation";
            this.PositionDeviation.Size = new System.Drawing.Size(47, 20);
            this.PositionDeviation.TabIndex = 93;
            this.PositionDeviation.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // AnchorImageDataControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PositionDeviationIsAbsolute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cSearchRectangleMargin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SearchRectangleMargin);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.PositionDeviation);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictures);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.FindBestImageMatch);
            this.Controls.Add(this.BrightnessTolerance);
            this.Controls.Add(this.DifferentPixelNumberTolerance);
            this.Name = "AnchorImageDataControl";
            this.Size = new System.Drawing.Size(226, 178);
            ((System.ComponentModel.ISupportInitialize)(this.BrightnessTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DifferentPixelNumberTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchRectangleMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PositionDeviation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.CheckBox FindBestImageMatch;
        public System.Windows.Forms.NumericUpDown BrightnessTolerance;
        public System.Windows.Forms.NumericUpDown DifferentPixelNumberTolerance;
        private System.Windows.Forms.FlowLayoutPanel pictures;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.CheckBox PositionDeviationIsAbsolute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cSearchRectangleMargin;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown SearchRectangleMargin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.NumericUpDown PositionDeviation;
    }
}
