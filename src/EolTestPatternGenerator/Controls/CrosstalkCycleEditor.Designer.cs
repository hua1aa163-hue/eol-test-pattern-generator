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
        labelColumnAdvance = new Label();
        numericColumnAdvance = new NumericUpDown();
        labelTiltAngle = new Label();
        numericTiltAngle = new NumericUpDown();
        gridCycle = new DataGridView();
        columnIndex = new DataGridViewTextBoxColumn();
        columnRed = new DataGridViewCheckBoxColumn();
        columnGreen = new DataGridViewCheckBoxColumn();
        columnBlue = new DataGridViewCheckBoxColumn();
        labelHelp = new Label();
        ((System.ComponentModel.ISupportInitialize)numericPeriodLength).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericColumnAdvance).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericTiltAngle).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridCycle).BeginInit();
        SuspendLayout();
        //
        // labelPreset
        //
        labelPreset.AutoSize = true;
        labelPreset.Location = new Point(0, 8);
        labelPreset.Name = "labelPreset";
        labelPreset.Size = new Size(68, 20);
        labelPreset.TabIndex = 0;
        labelPreset.Text = "兼容预设";
        //
        // comboLegacyPreset
        //
        comboLegacyPreset.DropDownStyle = ComboBoxStyle.DropDownList;
        comboLegacyPreset.FormattingEnabled = true;
        comboLegacyPreset.Items.AddRange(new object[] { "旧版 RGB", "旧版 RBG", "旧版 GRB", "旧版 GBR", "旧版 BRG", "旧版 BGR", "自定义周期" });
        comboLegacyPreset.Location = new Point(86, 4);
        comboLegacyPreset.Name = "comboLegacyPreset";
        comboLegacyPreset.Size = new Size(112, 28);
        comboLegacyPreset.TabIndex = 1;
        comboLegacyPreset.SelectedIndexChanged += comboLegacyPreset_SelectedIndexChanged;
        //
        // labelPeriodLength
        //
        labelPeriodLength.AutoSize = true;
        labelPeriodLength.Location = new Point(201, 8);
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
        // labelColumnAdvance
        //
        labelColumnAdvance.AutoSize = true;
        labelColumnAdvance.Location = new Point(0, 48);
        labelColumnAdvance.Name = "labelColumnAdvance";
        labelColumnAdvance.Size = new Size(68, 20);
        labelColumnAdvance.TabIndex = 4;
        labelColumnAdvance.Text = "横向步进";
        //
        // numericColumnAdvance
        //
        numericColumnAdvance.Location = new Point(86, 44);
        numericColumnAdvance.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
        numericColumnAdvance.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
        numericColumnAdvance.Name = "numericColumnAdvance";
        numericColumnAdvance.Size = new Size(68, 27);
        numericColumnAdvance.TabIndex = 5;
        numericColumnAdvance.Value = new decimal(new int[] { 3, 0, 0, int.MinValue });
        numericColumnAdvance.ValueChanged += numericAdvance_ValueChanged;
        //
        // labelTiltAngle
        //
        labelTiltAngle.AutoSize = true;
        labelTiltAngle.Location = new Point(162, 48);
        labelTiltAngle.Name = "labelTiltAngle";
        labelTiltAngle.Size = new Size(83, 20);
        labelTiltAngle.TabIndex = 6;
        labelTiltAngle.Text = "倾斜角(°)";
        //
        // numericTiltAngle
        //
        numericTiltAngle.DecimalPlaces = 6;
        numericTiltAngle.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
        numericTiltAngle.Location = new Point(249, 44);
        numericTiltAngle.Maximum = new decimal(new int[] { 89, 0, 0, 0 });
        numericTiltAngle.Minimum = new decimal(new int[] { 89, 0, 0, int.MinValue });
        numericTiltAngle.Name = "numericTiltAngle";
        numericTiltAngle.Size = new Size(84, 27);
        numericTiltAngle.TabIndex = 7;
        numericTiltAngle.Value = new decimal(new int[] { 18435, 0, 0, 196608 });
        numericTiltAngle.ValueChanged += numericAdvance_ValueChanged;
        //
        // gridCycle
        //
        gridCycle.AllowUserToAddRows = false;
        gridCycle.AllowUserToDeleteRows = false;
        gridCycle.AllowUserToResizeRows = false;
        gridCycle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        gridCycle.BackgroundColor = SystemColors.Window;
        gridCycle.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        gridCycle.Columns.AddRange(new DataGridViewColumn[] { columnIndex, columnRed, columnGreen, columnBlue });
        gridCycle.Location = new Point(0, 87);
        gridCycle.MultiSelect = false;
        gridCycle.Name = "gridCycle";
        gridCycle.RowHeadersVisible = false;
        gridCycle.RowHeadersWidth = 51;
        gridCycle.RowTemplate.Height = 29;
        gridCycle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        gridCycle.Size = new Size(331, 230);
        gridCycle.TabIndex = 8;
        gridCycle.CellValueChanged += gridCycle_CellValueChanged;
        gridCycle.CurrentCellDirtyStateChanged += gridCycle_CurrentCellDirtyStateChanged;
        //
        // columnIndex
        //
        columnIndex.FillWeight = 145F;
        columnIndex.HeaderText = "周期内像素序号";
        columnIndex.MinimumWidth = 6;
        columnIndex.Name = "columnIndex";
        columnIndex.ReadOnly = true;
        columnIndex.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // columnRed
        //
        columnRed.HeaderText = "R=255";
        columnRed.MinimumWidth = 6;
        columnRed.Name = "columnRed";
        columnRed.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // columnGreen
        //
        columnGreen.HeaderText = "G=255";
        columnGreen.MinimumWidth = 6;
        columnGreen.Name = "columnGreen";
        columnGreen.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // columnBlue
        //
        columnBlue.HeaderText = "B=255";
        columnBlue.MinimumWidth = 6;
        columnBlue.Name = "columnBlue";
        columnBlue.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // labelHelp
        //
        labelHelp.ForeColor = Color.DimGray;
        labelHelp.Location = new Point(0, 326);
        labelHelp.Name = "labelHelp";
        labelHelp.Size = new Size(331, 48);
        labelHelp.TabIndex = 9;
        labelHelp.Text = "通道未选=0、选中=255；每行位移=3×tan(倾角)。横向步进不为 -3 时，视觉斜率也会变化。";
        //
        // CrosstalkCycleEditor
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Controls.Add(labelHelp);
        Controls.Add(gridCycle);
        Controls.Add(numericTiltAngle);
        Controls.Add(labelTiltAngle);
        Controls.Add(numericColumnAdvance);
        Controls.Add(labelColumnAdvance);
        Controls.Add(numericPeriodLength);
        Controls.Add(labelPeriodLength);
        Controls.Add(comboLegacyPreset);
        Controls.Add(labelPreset);
        Name = "CrosstalkCycleEditor";
        Size = new Size(335, 378);
        ((System.ComponentModel.ISupportInitialize)numericPeriodLength).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericColumnAdvance).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericTiltAngle).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridCycle).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelPreset;
    private ComboBox comboLegacyPreset;
    private Label labelPeriodLength;
    private NumericUpDown numericPeriodLength;
    private Label labelColumnAdvance;
    private NumericUpDown numericColumnAdvance;
    private Label labelTiltAngle;
    private NumericUpDown numericTiltAngle;
    private DataGridView gridCycle;
    private DataGridViewTextBoxColumn columnIndex;
    private DataGridViewCheckBoxColumn columnRed;
    private DataGridViewCheckBoxColumn columnGreen;
    private DataGridViewCheckBoxColumn columnBlue;
    private Label labelHelp;
}
