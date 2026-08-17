#nullable disable

namespace EolTestPatternGenerator.Controls;

partial class CrosstalkPixelGridEditor
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
        components = new System.ComponentModel.Container();
        labelPeriodLength = new Label();
        numericPeriodLength = new NumericUpDown();
        labelColumnAdvance = new Label();
        numericColumnAdvance = new NumericUpDown();
        buttonColumnAdvanceHelp = new Button();
        labelTiltAngle = new Label();
        numericTiltAngle = new NumericUpDown();
        gridCycle = new DataGridView();
        columnIndex = new DataGridViewTextBoxColumn();
        columnRed = new DataGridViewCheckBoxColumn();
        columnGreen = new DataGridViewCheckBoxColumn();
        columnBlue = new DataGridViewCheckBoxColumn();
        labelHelp = new Label();
        toolTip = new ToolTip(components);
        ((System.ComponentModel.ISupportInitialize)numericPeriodLength).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericColumnAdvance).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericTiltAngle).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridCycle).BeginInit();
        SuspendLayout();
        //
        // labelPeriodLength
        //
        labelPeriodLength.AutoSize = true;
        labelPeriodLength.Location = new Point(0, 8);
        labelPeriodLength.Name = "labelPeriodLength";
        labelPeriodLength.Size = new Size(68, 20);
        labelPeriodLength.TabIndex = 0;
        labelPeriodLength.Text = "周期(px)";
        //
        // numericPeriodLength
        //
        numericPeriodLength.Location = new Point(86, 4);
        numericPeriodLength.Maximum = new decimal(new int[] { 1024, 0, 0, 0 });
        numericPeriodLength.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPeriodLength.Name = "numericPeriodLength";
        numericPeriodLength.Size = new Size(70, 27);
        numericPeriodLength.TabIndex = 1;
        numericPeriodLength.Value = new decimal(new int[] { 8, 0, 0, 0 });
        numericPeriodLength.ValueChanged += numericPeriodLength_ValueChanged;
        //
        // labelColumnAdvance
        //
        labelColumnAdvance.AutoSize = true;
        labelColumnAdvance.Location = new Point(0, 48);
        labelColumnAdvance.Name = "labelColumnAdvance";
        labelColumnAdvance.Size = new Size(68, 20);
        labelColumnAdvance.TabIndex = 2;
        labelColumnAdvance.Text = "横向步进";
        //
        // numericColumnAdvance
        //
        numericColumnAdvance.Location = new Point(86, 44);
        numericColumnAdvance.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
        numericColumnAdvance.Minimum = new decimal(new int[] { 4096, 0, 0, int.MinValue });
        numericColumnAdvance.Name = "numericColumnAdvance";
        numericColumnAdvance.Size = new Size(70, 27);
        numericColumnAdvance.TabIndex = 3;
        numericColumnAdvance.Value = new decimal(new int[] { 3, 0, 0, int.MinValue });
        numericColumnAdvance.ValueChanged += numericAdvance_ValueChanged;
        //
        // buttonColumnAdvanceHelp
        //
        buttonColumnAdvanceHelp.Location = new Point(163, 42);
        buttonColumnAdvanceHelp.Name = "buttonColumnAdvanceHelp";
        buttonColumnAdvanceHelp.Size = new Size(31, 31);
        buttonColumnAdvanceHelp.TabIndex = 4;
        buttonColumnAdvanceHelp.Text = "?";
        toolTip.SetToolTip(buttonColumnAdvanceHelp, "查看横向步进的定义和 -3 的含义。");
        buttonColumnAdvanceHelp.UseVisualStyleBackColor = true;
        buttonColumnAdvanceHelp.Click += buttonColumnAdvanceHelp_Click;
        //
        // labelTiltAngle
        //
        labelTiltAngle.AutoSize = true;
        labelTiltAngle.Location = new Point(0, 88);
        labelTiltAngle.Name = "labelTiltAngle";
        labelTiltAngle.Size = new Size(83, 20);
        labelTiltAngle.TabIndex = 5;
        labelTiltAngle.Text = "倾斜角(°)";
        //
        // numericTiltAngle
        //
        numericTiltAngle.DecimalPlaces = 6;
        numericTiltAngle.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
        numericTiltAngle.Location = new Point(86, 84);
        numericTiltAngle.Maximum = new decimal(new int[] { 89, 0, 0, 0 });
        numericTiltAngle.Minimum = new decimal(new int[] { 89, 0, 0, int.MinValue });
        numericTiltAngle.Name = "numericTiltAngle";
        numericTiltAngle.Size = new Size(108, 27);
        numericTiltAngle.TabIndex = 6;
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
        gridCycle.Location = new Point(0, 127);
        gridCycle.MultiSelect = false;
        gridCycle.Name = "gridCycle";
        gridCycle.RowHeadersVisible = false;
        gridCycle.RowHeadersWidth = 51;
        gridCycle.RowTemplate.Height = 29;
        gridCycle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        gridCycle.Size = new Size(331, 248);
        gridCycle.TabIndex = 7;
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
        labelHelp.Location = new Point(0, 384);
        labelHelp.Name = "labelHelp";
        labelHelp.Size = new Size(331, 50);
        labelHelp.TabIndex = 8;
        labelHelp.Text = "序号表示水平方向周期位置；通道未选=0、选中=255。每行位移=3×tan(倾斜角)。";
        //
        // CrosstalkPixelGridEditor
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Controls.Add(labelHelp);
        Controls.Add(gridCycle);
        Controls.Add(numericTiltAngle);
        Controls.Add(labelTiltAngle);
        Controls.Add(buttonColumnAdvanceHelp);
        Controls.Add(numericColumnAdvance);
        Controls.Add(labelColumnAdvance);
        Controls.Add(numericPeriodLength);
        Controls.Add(labelPeriodLength);
        Name = "CrosstalkPixelGridEditor";
        Size = new Size(335, 438);
        ((System.ComponentModel.ISupportInitialize)numericPeriodLength).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericColumnAdvance).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericTiltAngle).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridCycle).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelPeriodLength;
    private NumericUpDown numericPeriodLength;
    private Label labelColumnAdvance;
    private NumericUpDown numericColumnAdvance;
    private Button buttonColumnAdvanceHelp;
    private Label labelTiltAngle;
    private NumericUpDown numericTiltAngle;
    private DataGridView gridCycle;
    private DataGridViewTextBoxColumn columnIndex;
    private DataGridViewCheckBoxColumn columnRed;
    private DataGridViewCheckBoxColumn columnGreen;
    private DataGridViewCheckBoxColumn columnBlue;
    private Label labelHelp;
    private ToolTip toolTip;
}
