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
        labelRowAdvance = new Label();
        numericRowAdvance = new NumericUpDown();
        gridCycle = new DataGridView();
        columnIndex = new DataGridViewTextBoxColumn();
        columnRed = new DataGridViewCheckBoxColumn();
        columnGreen = new DataGridViewCheckBoxColumn();
        columnBlue = new DataGridViewCheckBoxColumn();
        labelHelp = new Label();
        ((System.ComponentModel.ISupportInitialize)numericPeriodLength).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericColumnAdvance).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericRowAdvance).BeginInit();
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
        labelPeriodLength.Location = new Point(207, 8);
        labelPeriodLength.Name = "labelPeriodLength";
        labelPeriodLength.Size = new Size(81, 20);
        labelPeriodLength.TabIndex = 2;
        labelPeriodLength.Text = "周期像素";
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
        // labelRowAdvance
        //
        labelRowAdvance.AutoSize = true;
        labelRowAdvance.Location = new Point(170, 48);
        labelRowAdvance.Name = "labelRowAdvance";
        labelRowAdvance.Size = new Size(68, 20);
        labelRowAdvance.TabIndex = 6;
        labelRowAdvance.Text = "纵向步进";
        //
        // numericRowAdvance
        //
        numericRowAdvance.Location = new Point(256, 44);
        numericRowAdvance.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
        numericRowAdvance.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
        numericRowAdvance.Name = "numericRowAdvance";
        numericRowAdvance.Size = new Size(68, 27);
        numericRowAdvance.TabIndex = 7;
        numericRowAdvance.Value = new decimal(new int[] { 1, 0, 0, 0 });
        numericRowAdvance.ValueChanged += numericAdvance_ValueChanged;
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
        labelHelp.Text = "每行代表周期内一个像素，可同时勾选多个通道；未勾选时该像素为全黑。所有通道只写入 0 或 255。";
        //
        // CrosstalkCycleEditor
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Controls.Add(labelHelp);
        Controls.Add(gridCycle);
        Controls.Add(numericRowAdvance);
        Controls.Add(labelRowAdvance);
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
        ((System.ComponentModel.ISupportInitialize)numericRowAdvance).EndInit();
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
    private Label labelRowAdvance;
    private NumericUpDown numericRowAdvance;
    private DataGridView gridCycle;
    private DataGridViewTextBoxColumn columnIndex;
    private DataGridViewCheckBoxColumn columnRed;
    private DataGridViewCheckBoxColumn columnGreen;
    private DataGridViewCheckBoxColumn columnBlue;
    private Label labelHelp;
}
