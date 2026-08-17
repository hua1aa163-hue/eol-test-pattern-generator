#nullable disable

namespace EolTestPatternGenerator.Controls;

partial class CrosstalkCycleEditor
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
        labelPreset = new Label();
        comboLegacyPreset = new ComboBox();
        labelPeriodLength = new Label();
        numericPeriodLength = new NumericUpDown();
        labelTiltAngle = new Label();
        numericTiltAngle = new NumericUpDown();
        labelHelp = new Label();
        ((System.ComponentModel.ISupportInitialize)numericPeriodLength).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericTiltAngle).BeginInit();
        SuspendLayout();
        //
        // labelPreset
        //
        labelPreset.AutoSize = true;
        labelPreset.Location = new Point(0, 8);
        labelPreset.Name = "labelPreset";
        labelPreset.Size = new Size(68, 20);
        labelPreset.TabIndex = 0;
        labelPreset.Text = "像素排列";
        //
        // comboLegacyPreset
        //
        comboLegacyPreset.DropDownStyle = ComboBoxStyle.DropDownList;
        comboLegacyPreset.FormattingEnabled = true;
        comboLegacyPreset.Items.AddRange(new object[] { "RGB", "RBG", "GRB", "GBR", "BRG", "BGR" });
        comboLegacyPreset.Location = new Point(86, 4);
        comboLegacyPreset.Name = "comboLegacyPreset";
        comboLegacyPreset.Size = new Size(112, 28);
        comboLegacyPreset.TabIndex = 1;
        comboLegacyPreset.SelectedIndexChanged += comboLegacyPreset_SelectedIndexChanged;
        //
        // labelPeriodLength
        //
        labelPeriodLength.AutoSize = true;
        labelPeriodLength.Location = new Point(207, 8);
        labelPeriodLength.Name = "labelPeriodLength";
        labelPeriodLength.Size = new Size(64, 20);
        labelPeriodLength.TabIndex = 2;
        labelPeriodLength.Text = "周期(px)";
        //
        // numericPeriodLength
        //
        numericPeriodLength.Location = new Point(273, 4);
        numericPeriodLength.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
        numericPeriodLength.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPeriodLength.Name = "numericPeriodLength";
        numericPeriodLength.Size = new Size(60, 27);
        numericPeriodLength.TabIndex = 3;
        numericPeriodLength.Value = new decimal(new int[] { 8, 0, 0, 0 });
        numericPeriodLength.ValueChanged += numericPeriodLength_ValueChanged;
        //
        // labelTiltAngle
        //
        labelTiltAngle.AutoSize = true;
        labelTiltAngle.Location = new Point(0, 48);
        labelTiltAngle.Name = "labelTiltAngle";
        labelTiltAngle.Size = new Size(83, 20);
        labelTiltAngle.TabIndex = 4;
        labelTiltAngle.Text = "倾斜角(°)";
        //
        // numericTiltAngle
        //
        numericTiltAngle.DecimalPlaces = 6;
        numericTiltAngle.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
        numericTiltAngle.Location = new Point(86, 44);
        numericTiltAngle.Maximum = new decimal(new int[] { 89, 0, 0, 0 });
        numericTiltAngle.Minimum = new decimal(new int[] { 89, 0, 0, int.MinValue });
        numericTiltAngle.Name = "numericTiltAngle";
        numericTiltAngle.Size = new Size(112, 27);
        numericTiltAngle.TabIndex = 5;
        numericTiltAngle.Value = new decimal(new int[] { 18435, 0, 0, 196608 });
        numericTiltAngle.ValueChanged += numericTiltAngle_ValueChanged;
        //
        // labelHelp
        //
        labelHelp.ForeColor = Color.DimGray;
        labelHelp.Location = new Point(0, 84);
        labelHelp.Name = "labelHelp";
        labelHelp.Size = new Size(331, 48);
        labelHelp.TabIndex = 6;
        labelHelp.Text = "周期可调；颜色通道严格为 0/255；每行位移=3×tan(倾角)。";
        //
        // CrosstalkCycleEditor
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Controls.Add(labelHelp);
        Controls.Add(numericTiltAngle);
        Controls.Add(labelTiltAngle);
        Controls.Add(numericPeriodLength);
        Controls.Add(labelPeriodLength);
        Controls.Add(comboLegacyPreset);
        Controls.Add(labelPreset);
        Name = "CrosstalkCycleEditor";
        Size = new Size(335, 136);
        ((System.ComponentModel.ISupportInitialize)numericPeriodLength).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericTiltAngle).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelPreset;
    private ComboBox comboLegacyPreset;
    private Label labelPeriodLength;
    private NumericUpDown numericPeriodLength;
    private Label labelTiltAngle;
    private NumericUpDown numericTiltAngle;
    private Label labelHelp;
}
