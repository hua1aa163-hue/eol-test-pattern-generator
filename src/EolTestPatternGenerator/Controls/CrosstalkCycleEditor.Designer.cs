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
        labelTiltAngle = new Label();
        numericTiltAngle = new NumericUpDown();
        labelHelp = new Label();
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
        labelPeriodLength.Size = new Size(124, 20);
        labelPeriodLength.TabIndex = 2;
        labelPeriodLength.Text = "固定周期：8 像素";
        //
        // labelTiltAngle
        //
        labelTiltAngle.AutoSize = true;
        labelTiltAngle.Location = new Point(0, 48);
        labelTiltAngle.Name = "labelTiltAngle";
        labelTiltAngle.Size = new Size(83, 20);
        labelTiltAngle.TabIndex = 3;
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
        numericTiltAngle.TabIndex = 4;
        numericTiltAngle.Value = new decimal(new int[] { 18435, 0, 0, 196608 });
        numericTiltAngle.ValueChanged += numericTiltAngle_ValueChanged;
        //
        // labelHelp
        //
        labelHelp.ForeColor = Color.DimGray;
        labelHelp.Location = new Point(0, 84);
        labelHelp.Name = "labelHelp";
        labelHelp.Size = new Size(331, 48);
        labelHelp.TabIndex = 5;
        labelHelp.Text = "固定 8 像素周期；颜色通道严格为 0/255；每行位移=3×tan(倾角)。";
        //
        // CrosstalkCycleEditor
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Controls.Add(labelHelp);
        Controls.Add(numericTiltAngle);
        Controls.Add(labelTiltAngle);
        Controls.Add(labelPeriodLength);
        Controls.Add(comboLegacyPreset);
        Controls.Add(labelPreset);
        Name = "CrosstalkCycleEditor";
        Size = new Size(335, 136);
        ((System.ComponentModel.ISupportInitialize)numericTiltAngle).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelPreset;
    private ComboBox comboLegacyPreset;
    private Label labelPeriodLength;
    private Label labelTiltAngle;
    private NumericUpDown numericTiltAngle;
    private Label labelHelp;
}
