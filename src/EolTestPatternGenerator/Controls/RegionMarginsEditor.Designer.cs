#nullable disable

namespace EolTestPatternGenerator.Controls;

partial class RegionMarginsEditor
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Component Designer generated code

    private void InitializeComponent()
    {
        labelLeft = new Label();
        numericLeft = new NumericUpDown();
        labelTop = new Label();
        numericTop = new NumericUpDown();
        labelRight = new Label();
        numericRight = new NumericUpDown();
        labelBottom = new Label();
        numericBottom = new NumericUpDown();
        buttonCenter = new Button();
        labelComputedSize = new Label();
        labelHelp = new Label();
        ((System.ComponentModel.ISupportInitialize)numericLeft).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericTop).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericRight).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericBottom).BeginInit();
        SuspendLayout();
        //
        // labelLeft
        //
        labelLeft.AutoSize = true;
        labelLeft.Location = new Point(4, 8);
        labelLeft.Name = "labelLeft";
        labelLeft.Size = new Size(39, 20);
        labelLeft.TabIndex = 0;
        labelLeft.Text = "左边";
        //
        // numericLeft
        //
        numericLeft.Location = new Point(54, 4);
        numericLeft.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericLeft.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
        numericLeft.Name = "numericLeft";
        numericLeft.Size = new Size(103, 27);
        numericLeft.TabIndex = 1;
        numericLeft.Value = new decimal(new int[] { 71, 0, 0, 0 });
        numericLeft.ValueChanged += numericMargin_ValueChanged;
        //
        // labelTop
        //
        labelTop.AutoSize = true;
        labelTop.Location = new Point(178, 8);
        labelTop.Name = "labelTop";
        labelTop.Size = new Size(39, 20);
        labelTop.TabIndex = 2;
        labelTop.Text = "上边";
        //
        // numericTop
        //
        numericTop.Location = new Point(228, 4);
        numericTop.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericTop.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
        numericTop.Name = "numericTop";
        numericTop.Size = new Size(103, 27);
        numericTop.TabIndex = 3;
        numericTop.Value = new decimal(new int[] { 226, 0, 0, 0 });
        numericTop.ValueChanged += numericMargin_ValueChanged;
        //
        // labelRight
        //
        labelRight.AutoSize = true;
        labelRight.Location = new Point(4, 44);
        labelRight.Name = "labelRight";
        labelRight.Size = new Size(39, 20);
        labelRight.TabIndex = 4;
        labelRight.Text = "右边";
        //
        // numericRight
        //
        numericRight.Location = new Point(54, 40);
        numericRight.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericRight.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
        numericRight.Name = "numericRight";
        numericRight.Size = new Size(103, 27);
        numericRight.TabIndex = 5;
        numericRight.Value = new decimal(new int[] { 72, 0, 0, 0 });
        numericRight.ValueChanged += numericMargin_ValueChanged;
        //
        // labelBottom
        //
        labelBottom.AutoSize = true;
        labelBottom.Location = new Point(178, 44);
        labelBottom.Name = "labelBottom";
        labelBottom.Size = new Size(39, 20);
        labelBottom.TabIndex = 6;
        labelBottom.Text = "下边";
        //
        // numericBottom
        //
        numericBottom.Location = new Point(228, 40);
        numericBottom.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericBottom.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
        numericBottom.Name = "numericBottom";
        numericBottom.Size = new Size(103, 27);
        numericBottom.TabIndex = 7;
        numericBottom.Value = new decimal(new int[] { 227, 0, 0, 0 });
        numericBottom.ValueChanged += numericMargin_ValueChanged;
        //
        // buttonCenter
        //
        buttonCenter.Location = new Point(228, 78);
        buttonCenter.Name = "buttonCenter";
        buttonCenter.Size = new Size(103, 34);
        buttonCenter.TabIndex = 8;
        buttonCenter.Text = "保持尺寸居中";
        buttonCenter.UseVisualStyleBackColor = true;
        buttonCenter.Click += buttonCenter_Click;
        //
        // labelComputedSize
        //
        labelComputedSize.AutoSize = true;
        labelComputedSize.ForeColor = Color.DimGray;
        labelComputedSize.Location = new Point(4, 84);
        labelComputedSize.Name = "labelComputedSize";
        labelComputedSize.Size = new Size(194, 20);
        labelComputedSize.TabIndex = 9;
        labelComputedSize.Text = "实时图案尺寸：1777 × 627 px";
        //
        // labelHelp
        //
        labelHelp.ForeColor = Color.DimGray;
        labelHelp.Location = new Point(4, 120);
        labelHelp.Name = "labelHelp";
        labelHelp.Size = new Size(327, 48);
        labelHelp.TabIndex = 10;
        labelHelp.Text = "距离按图案最外缘计算；0 表示贴边，负值表示越过画布后裁剪。";
        //
        // RegionMarginsEditor
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Controls.Add(labelHelp);
        Controls.Add(labelComputedSize);
        Controls.Add(buttonCenter);
        Controls.Add(numericBottom);
        Controls.Add(labelBottom);
        Controls.Add(numericRight);
        Controls.Add(labelRight);
        Controls.Add(numericTop);
        Controls.Add(labelTop);
        Controls.Add(numericLeft);
        Controls.Add(labelLeft);
        Name = "RegionMarginsEditor";
        Size = new Size(335, 171);
        ((System.ComponentModel.ISupportInitialize)numericLeft).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericTop).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericRight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericBottom).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelLeft;
    private NumericUpDown numericLeft;
    private Label labelTop;
    private NumericUpDown numericTop;
    private Label labelRight;
    private NumericUpDown numericRight;
    private Label labelBottom;
    private NumericUpDown numericBottom;
    private Button buttonCenter;
    private Label labelComputedSize;
    private Label labelHelp;
}
