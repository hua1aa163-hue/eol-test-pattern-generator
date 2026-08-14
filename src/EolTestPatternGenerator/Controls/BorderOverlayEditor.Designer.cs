#nullable disable

namespace EolTestPatternGenerator.Controls;

partial class BorderOverlayEditor
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
        checkEnabled = new CheckBox();
        labelX = new Label();
        numericX = new NumericUpDown();
        labelY = new Label();
        numericY = new NumericUpDown();
        labelWidth = new Label();
        numericWidth = new NumericUpDown();
        labelHeight = new Label();
        numericHeight = new NumericUpDown();
        labelLineWidth = new Label();
        numericLineWidth = new NumericUpDown();
        buttonCenter = new Button();
        labelHelp = new Label();
        ((System.ComponentModel.ISupportInitialize)numericX).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericY).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericWidth).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericHeight).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericLineWidth).BeginInit();
        SuspendLayout();
        //
        // checkEnabled
        //
        checkEnabled.AutoSize = true;
        checkEnabled.Location = new Point(4, 4);
        checkEnabled.Name = "checkEnabled";
        checkEnabled.Size = new Size(121, 24);
        checkEnabled.TabIndex = 0;
        checkEnabled.Text = "在底图上叠加白框";
        checkEnabled.UseVisualStyleBackColor = true;
        checkEnabled.CheckedChanged += ParameterChanged;
        //
        // labelX
        //
        labelX.AutoSize = true;
        labelX.Location = new Point(4, 40);
        labelX.Name = "labelX";
        labelX.Size = new Size(19, 20);
        labelX.TabIndex = 1;
        labelX.Text = "左边";
        //
        // numericX
        //
        numericX.Location = new Point(54, 36);
        numericX.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericX.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
        numericX.Name = "numericX";
        numericX.Size = new Size(103, 27);
        numericX.TabIndex = 2;
        numericX.Value = new decimal(new int[] { 71, 0, 0, 0 });
        numericX.ValueChanged += ParameterChanged;
        //
        // labelY
        //
        labelY.AutoSize = true;
        labelY.Location = new Point(178, 40);
        labelY.Name = "labelY";
        labelY.Size = new Size(18, 20);
        labelY.TabIndex = 3;
        labelY.Text = "上边";
        //
        // numericY
        //
        numericY.Location = new Point(228, 36);
        numericY.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericY.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
        numericY.Name = "numericY";
        numericY.Size = new Size(103, 27);
        numericY.TabIndex = 4;
        numericY.Value = new decimal(new int[] { 226, 0, 0, 0 });
        numericY.ValueChanged += ParameterChanged;
        //
        // labelWidth
        //
        labelWidth.AutoSize = true;
        labelWidth.Location = new Point(4, 76);
        labelWidth.Name = "labelWidth";
        labelWidth.Size = new Size(41, 20);
        labelWidth.TabIndex = 5;
        labelWidth.Text = "右边";
        //
        // numericWidth
        //
        numericWidth.Location = new Point(54, 72);
        numericWidth.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericWidth.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
        numericWidth.Name = "numericWidth";
        numericWidth.Size = new Size(103, 27);
        numericWidth.TabIndex = 6;
        numericWidth.ThousandsSeparator = true;
        numericWidth.Value = new decimal(new int[] { 72, 0, 0, 0 });
        numericWidth.ValueChanged += ParameterChanged;
        //
        // labelHeight
        //
        labelHeight.AutoSize = true;
        labelHeight.Location = new Point(178, 76);
        labelHeight.Name = "labelHeight";
        labelHeight.Size = new Size(41, 20);
        labelHeight.TabIndex = 7;
        labelHeight.Text = "下边";
        //
        // numericHeight
        //
        numericHeight.Location = new Point(228, 72);
        numericHeight.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericHeight.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
        numericHeight.Name = "numericHeight";
        numericHeight.Size = new Size(103, 27);
        numericHeight.TabIndex = 8;
        numericHeight.ThousandsSeparator = true;
        numericHeight.Value = new decimal(new int[] { 227, 0, 0, 0 });
        numericHeight.ValueChanged += ParameterChanged;
        //
        // labelLineWidth
        //
        labelLineWidth.AutoSize = true;
        labelLineWidth.Location = new Point(4, 112);
        labelLineWidth.Name = "labelLineWidth";
        labelLineWidth.Size = new Size(41, 20);
        labelLineWidth.TabIndex = 9;
        labelLineWidth.Text = "线宽";
        //
        // numericLineWidth
        //
        numericLineWidth.Location = new Point(54, 108);
        numericLineWidth.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
        numericLineWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericLineWidth.Name = "numericLineWidth";
        numericLineWidth.Size = new Size(103, 27);
        numericLineWidth.TabIndex = 10;
        numericLineWidth.Value = new decimal(new int[] { 5, 0, 0, 0 });
        numericLineWidth.ValueChanged += ParameterChanged;
        //
        // buttonCenter
        //
        buttonCenter.Location = new Point(228, 105);
        buttonCenter.Name = "buttonCenter";
        buttonCenter.Size = new Size(103, 34);
        buttonCenter.TabIndex = 11;
        buttonCenter.Text = "保持尺寸居中";
        buttonCenter.UseVisualStyleBackColor = true;
        buttonCenter.Click += buttonCenter_Click;
        //
        // labelHelp
        //
        labelHelp.ForeColor = Color.DimGray;
        labelHelp.Location = new Point(4, 147);
        labelHelp.Name = "labelHelp";
        labelHelp.Size = new Size(327, 48);
        labelHelp.TabIndex = 12;
        labelHelp.Text = "实时白框尺寸：1777 × 627 px\n四边距允许为负值，越界部分会裁剪。";
        //
        // BorderOverlayEditor
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Controls.Add(labelHelp);
        Controls.Add(buttonCenter);
        Controls.Add(numericLineWidth);
        Controls.Add(labelLineWidth);
        Controls.Add(numericHeight);
        Controls.Add(labelHeight);
        Controls.Add(numericWidth);
        Controls.Add(labelWidth);
        Controls.Add(numericY);
        Controls.Add(labelY);
        Controls.Add(numericX);
        Controls.Add(labelX);
        Controls.Add(checkEnabled);
        Name = "BorderOverlayEditor";
        Size = new Size(335, 188);
        ((System.ComponentModel.ISupportInitialize)numericX).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericY).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericWidth).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericHeight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericLineWidth).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private CheckBox checkEnabled;
    private Label labelX;
    private NumericUpDown numericX;
    private Label labelY;
    private NumericUpDown numericY;
    private Label labelWidth;
    private NumericUpDown numericWidth;
    private Label labelHeight;
    private NumericUpDown numericHeight;
    private Label labelLineWidth;
    private NumericUpDown numericLineWidth;
    private Button buttonCenter;
    private Label labelHelp;
}
