#nullable disable

namespace EolTestPatternGenerator;

partial class MainForm
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

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        splitContainerMain = new SplitContainer();
        settingsFlowPanel = new FlowLayoutPanel();
        groupPattern = new GroupBox();
        buttonPhaseTool = new Button();
        buttonExportScreen1 = new Button();
        labelPatternHelp = new Label();
        buttonBrowseSourceImage = new Button();
        numericPhase = new NumericUpDown();
        labelPhase = new Label();
        comboPattern = new ComboBox();
        labelPatternType = new Label();
        groupCanvas = new GroupBox();
        numericCanvasHeight = new NumericUpDown();
        labelCanvasHeight = new Label();
        numericCanvasWidth = new NumericUpDown();
        labelCanvasWidth = new Label();
        groupPlacement = new GroupBox();
        regionMarginsEditor = new EolTestPatternGenerator.Controls.RegionMarginsEditor();
        labelPlacementHelp = new Label();
        buttonCenter = new Button();
        numericPatternHeight = new NumericUpDown();
        labelPatternHeight = new Label();
        numericPatternWidth = new NumericUpDown();
        labelPatternWidth = new Label();
        numericPatternY = new NumericUpDown();
        labelPatternY = new Label();
        numericPatternX = new NumericUpDown();
        labelPatternX = new Label();
        groupShape = new GroupBox();
        numericColumns = new NumericUpDown();
        labelColumns = new Label();
        numericRows = new NumericUpDown();
        labelRows = new Label();
        numericLineWidth = new NumericUpDown();
        labelLineWidth = new Label();
        numericDotRadius = new NumericUpDown();
        labelDotRadius = new Label();
        labelDotHelp = new Label();
        groupBorderOverlay = new GroupBox();
        borderOverlayEditor = new EolTestPatternGenerator.Controls.BorderOverlayEditor();
        groupExport = new GroupBox();
        labelExportHelp = new Label();
        numericQuality = new NumericUpDown();
        labelQuality = new Label();
        comboOutputFormat = new ComboBox();
        labelOutputFormat = new Label();
        groupActions = new GroupBox();
        buttonBatchAddBorder = new Button();
        buttonBatchExport = new Button();
        buttonSaveCurrent = new Button();
        buttonReset = new Button();
        buttonRefresh = new Button();
        imagePreviewControl = new EolTestPatternGenerator.Controls.ImagePreviewControl();
        previewHeaderPanel = new Panel();
        labelPreviewInfo = new Label();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        previewTimer = new System.Windows.Forms.Timer(components);
        saveFileDialog = new SaveFileDialog();
        folderBrowserDialog = new FolderBrowserDialog();
        openImageDialog = new OpenFileDialog();
        toolTip = new ToolTip(components);
        ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
        splitContainerMain.Panel1.SuspendLayout();
        splitContainerMain.Panel2.SuspendLayout();
        splitContainerMain.SuspendLayout();
        settingsFlowPanel.SuspendLayout();
        groupPattern.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericPhase).BeginInit();
        groupCanvas.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericCanvasHeight).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericCanvasWidth).BeginInit();
        groupPlacement.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericPatternHeight).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternWidth).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternY).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternX).BeginInit();
        groupShape.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericColumns).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericRows).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericLineWidth).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericDotRadius).BeginInit();
        groupBorderOverlay.SuspendLayout();
        groupExport.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericQuality).BeginInit();
        groupActions.SuspendLayout();
        previewHeaderPanel.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // splitContainerMain
        // 
        splitContainerMain.Dock = DockStyle.Fill;
        splitContainerMain.FixedPanel = FixedPanel.Panel1;
        splitContainerMain.Location = new Point(0, 0);
        splitContainerMain.Margin = new Padding(4);
        splitContainerMain.Name = "splitContainerMain";
        // 
        // splitContainerMain.Panel1
        // 
        splitContainerMain.Panel1.BackColor = Color.FromArgb(245, 247, 250);
        splitContainerMain.Panel1.Controls.Add(settingsFlowPanel);
        splitContainerMain.Panel1MinSize = 390;
        // 
        // splitContainerMain.Panel2
        // 
        splitContainerMain.Panel2.BackColor = Color.FromArgb(224, 228, 234);
        splitContainerMain.Panel2.Controls.Add(imagePreviewControl);
        splitContainerMain.Panel2.Controls.Add(previewHeaderPanel);
        splitContainerMain.Panel2MinSize = 500;
        splitContainerMain.Size = new Size(2070, 1259);
        splitContainerMain.SplitterDistance = 608;
        splitContainerMain.SplitterWidth = 9;
        splitContainerMain.TabIndex = 0;
        // 
        // settingsFlowPanel
        // 
        settingsFlowPanel.AutoScroll = true;
        settingsFlowPanel.Controls.Add(groupPattern);
        settingsFlowPanel.Controls.Add(groupCanvas);
        settingsFlowPanel.Controls.Add(groupPlacement);
        settingsFlowPanel.Controls.Add(groupShape);
        settingsFlowPanel.Controls.Add(groupBorderOverlay);
        settingsFlowPanel.Controls.Add(groupExport);
        settingsFlowPanel.Controls.Add(groupActions);
        settingsFlowPanel.Dock = DockStyle.Fill;
        settingsFlowPanel.FlowDirection = FlowDirection.TopDown;
        settingsFlowPanel.Location = new Point(0, 0);
        settingsFlowPanel.Margin = new Padding(4);
        settingsFlowPanel.Name = "settingsFlowPanel";
        settingsFlowPanel.Padding = new Padding(15, 15, 12, 24);
        settingsFlowPanel.Size = new Size(608, 1259);
        settingsFlowPanel.TabIndex = 0;
        settingsFlowPanel.WrapContents = false;
        // 
        // groupPattern
        // 
        groupPattern.Controls.Add(buttonPhaseTool);
        groupPattern.Controls.Add(buttonExportScreen1);
        groupPattern.Controls.Add(labelPatternHelp);
        groupPattern.Controls.Add(buttonBrowseSourceImage);
        groupPattern.Controls.Add(numericPhase);
        groupPattern.Controls.Add(labelPhase);
        groupPattern.Controls.Add(comboPattern);
        groupPattern.Controls.Add(labelPatternType);
        groupPattern.Location = new Point(19, 19);
        groupPattern.Margin = new Padding(4);
        groupPattern.Name = "groupPattern";
        groupPattern.Padding = new Padding(4);
        groupPattern.Size = new Size(550, 257);
        groupPattern.TabIndex = 0;
        groupPattern.TabStop = false;
        groupPattern.Text = "图卡类型";
        // 
        // buttonPhaseTool
        // 
        buttonPhaseTool.Location = new Point(46, 168);
        buttonPhaseTool.Margin = new Padding(4);
        buttonPhaseTool.Name = "buttonPhaseTool";
        buttonPhaseTool.Size = new Size(216, 57);
        buttonPhaseTool.TabIndex = 6;
        buttonPhaseTool.Text = "串扰像素排列";
        buttonPhaseTool.UseVisualStyleBackColor = true;
        buttonPhaseTool.Click += buttonPhaseTool_Click_1;
        // 
        // buttonExportScreen1
        // 
        buttonExportScreen1.Location = new Point(305, 168);
        buttonExportScreen1.Margin = new Padding(4);
        buttonExportScreen1.Name = "buttonExportScreen1";
        buttonExportScreen1.Size = new Size(217, 57);
        buttonExportScreen1.TabIndex = 7;
        buttonExportScreen1.Text = "显示器";
        buttonExportScreen1.UseVisualStyleBackColor = true;
        buttonExportScreen1.Click += buttonExportScreen1_Click_1;
        // 
        // labelPatternHelp
        // 
        labelPatternHelp.ForeColor = Color.DimGray;
        labelPatternHelp.Location = new Point(27, 100);
        labelPatternHelp.Margin = new Padding(4, 0, 4, 0);
        labelPatternHelp.Name = "labelPatternHelp";
        labelPatternHelp.Size = new Size(496, 64);
        labelPatternHelp.TabIndex = 4;
        labelPatternHelp.Text = "所有图案按最外缘四边距定义；点阵还可使用圆心坐标和圆心跨度。";
        // 
        // buttonBrowseSourceImage
        // 
        buttonBrowseSourceImage.Location = new Point(324, 99);
        buttonBrowseSourceImage.Margin = new Padding(4);
        buttonBrowseSourceImage.Name = "buttonBrowseSourceImage";
        buttonBrowseSourceImage.Size = new Size(200, 48);
        buttonBrowseSourceImage.TabIndex = 5;
        buttonBrowseSourceImage.Text = "选择底图...";
        buttonBrowseSourceImage.UseVisualStyleBackColor = true;
        buttonBrowseSourceImage.Visible = false;
        buttonBrowseSourceImage.Click += buttonBrowseSourceImage_Click;
        // 
        // numericPhase
        // 
        numericPhase.Location = new Point(168, 99);
        numericPhase.Margin = new Padding(4);
        numericPhase.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
        numericPhase.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPhase.Name = "numericPhase";
        numericPhase.Size = new Size(147, 30);
        numericPhase.TabIndex = 3;
        numericPhase.Value = new decimal(new int[] { 1, 0, 0, 0 });
        numericPhase.Visible = false;
        numericPhase.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPhase
        // 
        labelPhase.AutoSize = true;
        labelPhase.Location = new Point(27, 105);
        labelPhase.Margin = new Padding(4, 0, 4, 0);
        labelPhase.Name = "labelPhase";
        labelPhase.Size = new Size(88, 24);
        labelPhase.TabIndex = 2;
        labelPhase.Text = "兼容相位";
        labelPhase.Visible = false;
        // 
        // comboPattern
        // 
        comboPattern.DropDownStyle = ComboBoxStyle.DropDownList;
        comboPattern.FormattingEnabled = true;
        comboPattern.Items.AddRange(new object[] { "外框（兼容 0.png）", "九点图", "畸变点阵（27×7）", "上下校正十字", "白色矩形", "全黑图（RGB 仅 0）", "全白图（RGB 仅 255）", "全红图（255,0,0）", "全绿图（0,255,0）", "全蓝图（0,0,255）", "导入图片并添加白框" });
        comboPattern.Location = new Point(168, 42);
        comboPattern.Margin = new Padding(4);
        comboPattern.Name = "comboPattern";
        comboPattern.Size = new Size(354, 32);
        comboPattern.TabIndex = 1;
        comboPattern.SelectedIndexChanged += comboPattern_SelectedIndexChanged;
        // 
        // labelPatternType
        // 
        labelPatternType.AutoSize = true;
        labelPatternType.Location = new Point(27, 48);
        labelPatternType.Margin = new Padding(4, 0, 4, 0);
        labelPatternType.Name = "labelPatternType";
        labelPatternType.Size = new Size(82, 24);
        labelPatternType.TabIndex = 0;
        labelPatternType.Text = "图卡类型";
        // 
        // groupCanvas
        // 
        groupCanvas.Controls.Add(numericCanvasHeight);
        groupCanvas.Controls.Add(labelCanvasHeight);
        groupCanvas.Controls.Add(numericCanvasWidth);
        groupCanvas.Controls.Add(labelCanvasWidth);
        groupCanvas.Location = new Point(19, 284);
        groupCanvas.Margin = new Padding(4);
        groupCanvas.Name = "groupCanvas";
        groupCanvas.Padding = new Padding(4);
        groupCanvas.Size = new Size(550, 159);
        groupCanvas.TabIndex = 1;
        groupCanvas.TabStop = false;
        groupCanvas.Text = "输出画布（可修改）";
        // 
        // numericCanvasHeight
        // 
        numericCanvasHeight.Location = new Point(369, 64);
        numericCanvasHeight.Margin = new Padding(4);
        numericCanvasHeight.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericCanvasHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericCanvasHeight.Name = "numericCanvasHeight";
        numericCanvasHeight.Size = new Size(154, 30);
        numericCanvasHeight.TabIndex = 3;
        numericCanvasHeight.ThousandsSeparator = true;
        numericCanvasHeight.Value = new decimal(new int[] { 1080, 0, 0, 0 });
        numericCanvasHeight.ValueChanged += Parameter_ValueChanged;
        // 
        // labelCanvasHeight
        // 
        labelCanvasHeight.AutoSize = true;
        labelCanvasHeight.Location = new Point(288, 70);
        labelCanvasHeight.Margin = new Padding(4, 0, 4, 0);
        labelCanvasHeight.Name = "labelCanvasHeight";
        labelCanvasHeight.Size = new Size(46, 24);
        labelCanvasHeight.TabIndex = 2;
        labelCanvasHeight.Text = "高度";
        // 
        // numericCanvasWidth
        // 
        numericCanvasWidth.Location = new Point(108, 64);
        numericCanvasWidth.Margin = new Padding(4);
        numericCanvasWidth.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericCanvasWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericCanvasWidth.Name = "numericCanvasWidth";
        numericCanvasWidth.Size = new Size(154, 30);
        numericCanvasWidth.TabIndex = 1;
        numericCanvasWidth.ThousandsSeparator = true;
        numericCanvasWidth.Value = new decimal(new int[] { 1920, 0, 0, 0 });
        numericCanvasWidth.ValueChanged += Parameter_ValueChanged;
        // 
        // labelCanvasWidth
        // 
        labelCanvasWidth.AutoSize = true;
        labelCanvasWidth.Location = new Point(27, 70);
        labelCanvasWidth.Margin = new Padding(4, 0, 4, 0);
        labelCanvasWidth.Name = "labelCanvasWidth";
        labelCanvasWidth.Size = new Size(46, 24);
        labelCanvasWidth.TabIndex = 0;
        labelCanvasWidth.Text = "宽度";
        // 
        // groupPlacement
        // 
        groupPlacement.Controls.Add(regionMarginsEditor);
        groupPlacement.Controls.Add(labelPlacementHelp);
        groupPlacement.Controls.Add(buttonCenter);
        groupPlacement.Controls.Add(numericPatternHeight);
        groupPlacement.Controls.Add(labelPatternHeight);
        groupPlacement.Controls.Add(numericPatternWidth);
        groupPlacement.Controls.Add(labelPatternWidth);
        groupPlacement.Controls.Add(numericPatternY);
        groupPlacement.Controls.Add(labelPatternY);
        groupPlacement.Controls.Add(numericPatternX);
        groupPlacement.Controls.Add(labelPatternX);
        groupPlacement.Location = new Point(19, 451);
        groupPlacement.Margin = new Padding(4);
        groupPlacement.Name = "groupPlacement";
        groupPlacement.Padding = new Padding(4);
        groupPlacement.Size = new Size(550, 500);
        groupPlacement.TabIndex = 2;
        groupPlacement.TabStop = false;
        groupPlacement.Text = "图案外缘四边距与圆心参数";
        //
        // regionMarginsEditor
        //
        regionMarginsEditor.CanvasSize = new Size(1920, 1080);
        regionMarginsEditor.Location = new Point(27, 38);
        regionMarginsEditor.Margin = new Padding(4);
        regionMarginsEditor.Name = "regionMarginsEditor";
        regionMarginsEditor.Size = new Size(496, 256);
        regionMarginsEditor.TabIndex = 0;
        regionMarginsEditor.MarginsChanged += regionMarginsEditor_MarginsChanged;
        // 
        // labelPlacementHelp
        // 
        labelPlacementHelp.ForeColor = Color.DimGray;
        labelPlacementHelp.Location = new Point(27, 436);
        labelPlacementHelp.Margin = new Padding(4, 0, 4, 0);
        labelPlacementHelp.Name = "labelPlacementHelp";
        labelPlacementHelp.Size = new Size(496, 58);
        labelPlacementHelp.TabIndex = 9;
        labelPlacementHelp.Text = "圆心坐标/跨度与上方外缘四边距双向同步。";
        // 
        // buttonCenter
        // 
        buttonCenter.Location = new Point(369, 436);
        buttonCenter.Margin = new Padding(4);
        buttonCenter.Name = "buttonCenter";
        buttonCenter.Size = new Size(154, 51);
        buttonCenter.TabIndex = 8;
        buttonCenter.Text = "居中图案";
        buttonCenter.UseVisualStyleBackColor = true;
        buttonCenter.Click += buttonCenter_Click;
        // 
        // numericPatternHeight
        // 
        numericPatternHeight.Location = new Point(369, 374);
        numericPatternHeight.Margin = new Padding(4);
        numericPatternHeight.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericPatternHeight.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
        numericPatternHeight.Name = "numericPatternHeight";
        numericPatternHeight.Size = new Size(154, 30);
        numericPatternHeight.TabIndex = 7;
        numericPatternHeight.ThousandsSeparator = true;
        numericPatternHeight.Value = new decimal(new int[] { 627, 0, 0, 0 });
        numericPatternHeight.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternHeight
        // 
        labelPatternHeight.AutoSize = true;
        labelPatternHeight.Location = new Point(267, 380);
        labelPatternHeight.Margin = new Padding(4, 0, 4, 0);
        labelPatternHeight.Name = "labelPatternHeight";
        labelPatternHeight.Size = new Size(46, 24);
        labelPatternHeight.TabIndex = 6;
        labelPatternHeight.Text = "圆心 Y 跨度";
        // 
        // numericPatternWidth
        // 
        numericPatternWidth.Location = new Point(108, 374);
        numericPatternWidth.Margin = new Padding(4);
        numericPatternWidth.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericPatternWidth.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
        numericPatternWidth.Name = "numericPatternWidth";
        numericPatternWidth.Size = new Size(154, 30);
        numericPatternWidth.TabIndex = 5;
        numericPatternWidth.ThousandsSeparator = true;
        numericPatternWidth.Value = new decimal(new int[] { 1777, 0, 0, 0 });
        numericPatternWidth.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternWidth
        // 
        labelPatternWidth.AutoSize = true;
        labelPatternWidth.Location = new Point(27, 380);
        labelPatternWidth.Margin = new Padding(4, 0, 4, 0);
        labelPatternWidth.Name = "labelPatternWidth";
        labelPatternWidth.Size = new Size(46, 24);
        labelPatternWidth.TabIndex = 4;
        labelPatternWidth.Text = "圆心 X 跨度";
        // 
        // numericPatternY
        // 
        numericPatternY.Location = new Point(369, 315);
        numericPatternY.Margin = new Padding(4);
        numericPatternY.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericPatternY.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
        numericPatternY.Name = "numericPatternY";
        numericPatternY.Size = new Size(154, 30);
        numericPatternY.TabIndex = 3;
        numericPatternY.Value = new decimal(new int[] { 226, 0, 0, 0 });
        numericPatternY.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternY
        // 
        labelPatternY.AutoSize = true;
        labelPatternY.Location = new Point(288, 321);
        labelPatternY.Margin = new Padding(4, 0, 4, 0);
        labelPatternY.Name = "labelPatternY";
        labelPatternY.Size = new Size(21, 24);
        labelPatternY.TabIndex = 2;
        labelPatternY.Text = "首圆心 Y";
        // 
        // numericPatternX
        // 
        numericPatternX.Location = new Point(108, 315);
        numericPatternX.Margin = new Padding(4);
        numericPatternX.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numericPatternX.Minimum = new decimal(new int[] { int.MinValue, 0, 0, int.MinValue });
        numericPatternX.Name = "numericPatternX";
        numericPatternX.Size = new Size(154, 30);
        numericPatternX.TabIndex = 1;
        numericPatternX.Value = new decimal(new int[] { 71, 0, 0, 0 });
        numericPatternX.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternX
        // 
        labelPatternX.AutoSize = true;
        labelPatternX.Location = new Point(27, 321);
        labelPatternX.Margin = new Padding(4, 0, 4, 0);
        labelPatternX.Name = "labelPatternX";
        labelPatternX.Size = new Size(22, 24);
        labelPatternX.TabIndex = 0;
        labelPatternX.Text = "首圆心 X";
        // 
        // groupShape
        // 
        groupShape.Controls.Add(numericColumns);
        groupShape.Controls.Add(labelColumns);
        groupShape.Controls.Add(numericRows);
        groupShape.Controls.Add(labelRows);
        groupShape.Controls.Add(numericLineWidth);
        groupShape.Controls.Add(labelLineWidth);
        groupShape.Controls.Add(numericDotRadius);
        groupShape.Controls.Add(labelDotRadius);
        groupShape.Controls.Add(labelDotHelp);
        groupShape.Location = new Point(19, 767);
        groupShape.Margin = new Padding(4);
        groupShape.Name = "groupShape";
        groupShape.Padding = new Padding(4);
        groupShape.Size = new Size(550, 261);
        groupShape.TabIndex = 3;
        groupShape.TabStop = false;
        groupShape.Text = "图形参数";
        // 
        // numericColumns
        // 
        numericColumns.Location = new Point(369, 108);
        numericColumns.Margin = new Padding(4);
        numericColumns.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        numericColumns.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericColumns.Name = "numericColumns";
        numericColumns.Size = new Size(154, 30);
        numericColumns.TabIndex = 7;
        numericColumns.Value = new decimal(new int[] { 3, 0, 0, 0 });
        numericColumns.ValueChanged += Parameter_ValueChanged;
        // 
        // labelColumns
        // 
        labelColumns.AutoSize = true;
        labelColumns.Location = new Point(288, 114);
        labelColumns.Margin = new Padding(4, 0, 4, 0);
        labelColumns.Name = "labelColumns";
        labelColumns.Size = new Size(28, 24);
        labelColumns.TabIndex = 6;
        labelColumns.Text = "列";
        // 
        // numericRows
        // 
        numericRows.Location = new Point(108, 108);
        numericRows.Margin = new Padding(4);
        numericRows.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        numericRows.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericRows.Name = "numericRows";
        numericRows.Size = new Size(154, 30);
        numericRows.TabIndex = 5;
        numericRows.Value = new decimal(new int[] { 3, 0, 0, 0 });
        numericRows.ValueChanged += Parameter_ValueChanged;
        // 
        // labelRows
        // 
        labelRows.AutoSize = true;
        labelRows.Location = new Point(27, 114);
        labelRows.Margin = new Padding(4, 0, 4, 0);
        labelRows.Name = "labelRows";
        labelRows.Size = new Size(28, 24);
        labelRows.TabIndex = 4;
        labelRows.Text = "行";
        // 
        // numericLineWidth
        // 
        numericLineWidth.Location = new Point(369, 50);
        numericLineWidth.Margin = new Padding(4);
        numericLineWidth.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
        numericLineWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericLineWidth.Name = "numericLineWidth";
        numericLineWidth.Size = new Size(154, 30);
        numericLineWidth.TabIndex = 3;
        numericLineWidth.Value = new decimal(new int[] { 5, 0, 0, 0 });
        numericLineWidth.ValueChanged += Parameter_ValueChanged;
        // 
        // labelLineWidth
        // 
        labelLineWidth.AutoSize = true;
        labelLineWidth.Location = new Point(288, 56);
        labelLineWidth.Margin = new Padding(4, 0, 4, 0);
        labelLineWidth.Name = "labelLineWidth";
        labelLineWidth.Size = new Size(46, 24);
        labelLineWidth.TabIndex = 2;
        labelLineWidth.Text = "线宽";
        // 
        // numericDotRadius
        // 
        numericDotRadius.Location = new Point(168, 50);
        numericDotRadius.Margin = new Padding(4);
        numericDotRadius.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
        numericDotRadius.Name = "numericDotRadius";
        numericDotRadius.Size = new Size(94, 30);
        numericDotRadius.TabIndex = 1;
        numericDotRadius.Value = new decimal(new int[] { 4, 0, 0, 0 });
        numericDotRadius.ValueChanged += Parameter_ValueChanged;
        // 
        // labelDotRadius
        // 
        labelDotRadius.AutoSize = true;
        labelDotRadius.Location = new Point(27, 56);
        labelDotRadius.Margin = new Padding(4, 0, 4, 0);
        labelDotRadius.Name = "labelDotRadius";
        labelDotRadius.Size = new Size(115, 24);
        labelDotRadius.TabIndex = 0;
        labelDotRadius.Text = "圆点半径(px)";
        // 
        // labelDotHelp
        // 
        labelDotHelp.ForeColor = Color.DimGray;
        labelDotHelp.Location = new Point(27, 168);
        labelDotHelp.Margin = new Padding(4, 0, 4, 0);
        labelDotHelp.Name = "labelDotHelp";
        labelDotHelp.Size = new Size(496, 63);
        labelDotHelp.TabIndex = 8;
        labelDotHelp.Text = "默认半径 4，直径为 2r+1=9 px；点阵行列数也可调整。";
        // 
        // groupBorderOverlay
        // 
        groupBorderOverlay.Controls.Add(borderOverlayEditor);
        groupBorderOverlay.Location = new Point(19, 1036);
        groupBorderOverlay.Margin = new Padding(4);
        groupBorderOverlay.Name = "groupBorderOverlay";
        groupBorderOverlay.Padding = new Padding(4);
        groupBorderOverlay.Size = new Size(550, 339);
        groupBorderOverlay.TabIndex = 4;
        groupBorderOverlay.TabStop = false;
        groupBorderOverlay.Text = "白框叠加层（可应用于任意底图）";
        // 
        // borderOverlayEditor
        // 
        borderOverlayEditor.CanvasSize = new Size(1920, 1080);
        borderOverlayEditor.Location = new Point(21, 39);
        borderOverlayEditor.Margin = new Padding(6);
        borderOverlayEditor.Name = "borderOverlayEditor";
        borderOverlayEditor.Size = new Size(502, 282);
        borderOverlayEditor.TabIndex = 0;
        borderOverlayEditor.SettingsChanged += borderOverlayEditor_SettingsChanged;
        // 
        // groupExport
        // 
        groupExport.Controls.Add(labelExportHelp);
        groupExport.Controls.Add(numericQuality);
        groupExport.Controls.Add(labelQuality);
        groupExport.Controls.Add(comboOutputFormat);
        groupExport.Controls.Add(labelOutputFormat);
        groupExport.Location = new Point(19, 1383);
        groupExport.Margin = new Padding(4);
        groupExport.Name = "groupExport";
        groupExport.Padding = new Padding(4);
        groupExport.Size = new Size(550, 218);
        groupExport.TabIndex = 5;
        groupExport.TabStop = false;
        groupExport.Text = "导出格式";
        // 
        // labelExportHelp
        // 
        labelExportHelp.ForeColor = Color.DimGray;
        labelExportHelp.Location = new Point(27, 148);
        labelExportHelp.Margin = new Padding(4, 0, 4, 0);
        labelExportHelp.Name = "labelExportHelp";
        labelExportHelp.Size = new Size(496, 56);
        labelExportHelp.TabIndex = 4;
        labelExportHelp.Text = "PNG/BMP/TIFF 无损；JPG 会改变精确像素，检测图建议优先 PNG。";
        // 
        // numericQuality
        // 
        numericQuality.Location = new Point(168, 99);
        numericQuality.Margin = new Padding(4);
        numericQuality.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericQuality.Name = "numericQuality";
        numericQuality.Size = new Size(147, 30);
        numericQuality.TabIndex = 3;
        numericQuality.Value = new decimal(new int[] { 95, 0, 0, 0 });
        // 
        // labelQuality
        // 
        labelQuality.AutoSize = true;
        labelQuality.Location = new Point(27, 105);
        labelQuality.Margin = new Padding(4, 0, 4, 0);
        labelQuality.Name = "labelQuality";
        labelQuality.Size = new Size(82, 24);
        labelQuality.TabIndex = 2;
        labelQuality.Text = "压缩质量";
        // 
        // comboOutputFormat
        // 
        comboOutputFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        comboOutputFormat.FormattingEnabled = true;
        comboOutputFormat.Items.AddRange(new object[] { "PNG（无损，推荐）", "JPEG / JPG", "BMP", "TIFF", "WebP" });
        comboOutputFormat.Location = new Point(168, 42);
        comboOutputFormat.Margin = new Padding(4);
        comboOutputFormat.Name = "comboOutputFormat";
        comboOutputFormat.Size = new Size(354, 32);
        comboOutputFormat.TabIndex = 1;
        comboOutputFormat.SelectedIndexChanged += comboOutputFormat_SelectedIndexChanged;
        // 
        // labelOutputFormat
        // 
        labelOutputFormat.AutoSize = true;
        labelOutputFormat.Location = new Point(27, 48);
        labelOutputFormat.Margin = new Padding(4, 0, 4, 0);
        labelOutputFormat.Name = "labelOutputFormat";
        labelOutputFormat.Size = new Size(82, 24);
        labelOutputFormat.TabIndex = 0;
        labelOutputFormat.Text = "图片格式";
        // 
        // groupActions
        // 
        groupActions.Controls.Add(buttonBatchAddBorder);
        groupActions.Controls.Add(buttonBatchExport);
        groupActions.Controls.Add(buttonSaveCurrent);
        groupActions.Controls.Add(buttonReset);
        groupActions.Controls.Add(buttonRefresh);
        groupActions.Location = new Point(19, 1609);
        groupActions.Margin = new Padding(4);
        groupActions.Name = "groupActions";
        groupActions.Padding = new Padding(4);
        groupActions.Size = new Size(550, 266);
        groupActions.TabIndex = 6;
        groupActions.TabStop = false;
        groupActions.Text = "生成与保存";
        //
        // buttonBatchAddBorder
        //
        buttonBatchAddBorder.Location = new Point(27, 182);
        buttonBatchAddBorder.Margin = new Padding(4);
        buttonBatchAddBorder.Name = "buttonBatchAddBorder";
        buttonBatchAddBorder.Size = new Size(497, 57);
        buttonBatchAddBorder.TabIndex = 4;
        buttonBatchAddBorder.Text = "批量给文件夹图片加白框...";
        buttonBatchAddBorder.UseVisualStyleBackColor = true;
        buttonBatchAddBorder.Click += buttonBatchAddBorder_Click;
        // 
        // buttonBatchExport
        // 
        buttonBatchExport.Location = new Point(288, 112);
        buttonBatchExport.Margin = new Padding(4);
        buttonBatchExport.Name = "buttonBatchExport";
        buttonBatchExport.Size = new Size(236, 57);
        buttonBatchExport.TabIndex = 3;
        buttonBatchExport.Text = "批量导出基础图 10 张";
        buttonBatchExport.UseVisualStyleBackColor = true;
        buttonBatchExport.Click += buttonBatchExport_Click;
        // 
        // buttonSaveCurrent
        // 
        buttonSaveCurrent.Location = new Point(27, 112);
        buttonSaveCurrent.Margin = new Padding(4);
        buttonSaveCurrent.Name = "buttonSaveCurrent";
        buttonSaveCurrent.Size = new Size(236, 57);
        buttonSaveCurrent.TabIndex = 2;
        buttonSaveCurrent.Text = "保存当前图卡...";
        buttonSaveCurrent.UseVisualStyleBackColor = true;
        buttonSaveCurrent.Click += buttonSaveCurrent_Click;
        // 
        // buttonReset
        // 
        buttonReset.Location = new Point(288, 42);
        buttonReset.Margin = new Padding(4);
        buttonReset.Name = "buttonReset";
        buttonReset.Size = new Size(236, 57);
        buttonReset.TabIndex = 1;
        buttonReset.Text = "恢复样图默认参数";
        buttonReset.UseVisualStyleBackColor = true;
        buttonReset.Click += buttonReset_Click;
        // 
        // buttonRefresh
        // 
        buttonRefresh.Location = new Point(27, 42);
        buttonRefresh.Margin = new Padding(4);
        buttonRefresh.Name = "buttonRefresh";
        buttonRefresh.Size = new Size(236, 57);
        buttonRefresh.TabIndex = 0;
        buttonRefresh.Text = "立即刷新预览";
        buttonRefresh.UseVisualStyleBackColor = true;
        buttonRefresh.Click += buttonRefresh_Click;
        // 
        // imagePreviewControl
        // 
        imagePreviewControl.BackColor = Color.FromArgb(224, 228, 234);
        imagePreviewControl.Dock = DockStyle.Fill;
        imagePreviewControl.Location = new Point(0, 66);
        imagePreviewControl.Margin = new Padding(6);
        imagePreviewControl.Name = "imagePreviewControl";
        imagePreviewControl.Size = new Size(1453, 1193);
        imagePreviewControl.TabIndex = 1;
        // 
        // previewHeaderPanel
        // 
        previewHeaderPanel.BackColor = Color.FromArgb(42, 45, 50);
        previewHeaderPanel.Controls.Add(labelPreviewInfo);
        previewHeaderPanel.Dock = DockStyle.Top;
        previewHeaderPanel.Location = new Point(0, 0);
        previewHeaderPanel.Margin = new Padding(4);
        previewHeaderPanel.Name = "previewHeaderPanel";
        previewHeaderPanel.Size = new Size(1453, 66);
        previewHeaderPanel.TabIndex = 0;
        // 
        // labelPreviewInfo
        // 
        labelPreviewInfo.AutoSize = true;
        labelPreviewInfo.ForeColor = Color.WhiteSmoke;
        labelPreviewInfo.Location = new Point(24, 18);
        labelPreviewInfo.Margin = new Padding(4, 0, 4, 0);
        labelPreviewInfo.Name = "labelPreviewInfo";
        labelPreviewInfo.Size = new Size(175, 24);
        labelPreviewInfo.TabIndex = 0;
        labelPreviewInfo.Text = "预览：1920 × 1080";
        // 
        // statusStrip
        // 
        statusStrip.ImageScalingSize = new Size(24, 24);
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 1259);
        statusStrip.Name = "statusStrip";
        statusStrip.Padding = new Padding(2, 0, 21, 0);
        statusStrip.Size = new Size(2070, 31);
        statusStrip.TabIndex = 1;
        // 
        // statusLabel
        // 
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(189, 24);
        statusLabel.Text = "正在初始化 OpenCV...";
        // 
        // previewTimer
        // 
        previewTimer.Interval = 160;
        previewTimer.Tick += previewTimer_Tick;
        // 
        // saveFileDialog
        // 
        saveFileDialog.Title = "保存图卡";
        // 
        // folderBrowserDialog
        // 
        folderBrowserDialog.Description = "选择批量导出目录";
        folderBrowserDialog.UseDescriptionForTitle = true;
        // 
        // openImageDialog
        // 
        openImageDialog.Filter = "图片文件|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff;*.webp|所有文件|*.*";
        openImageDialog.Title = "选择要添加白框的底图";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(144F, 144F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(2070, 1290);
        Controls.Add(splitContainerMain);
        Controls.Add(statusStrip);
        Font = new Font("Microsoft YaHei UI", 9F);
        Margin = new Padding(4);
        MinimumSize = new Size(1639, 1022);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "EOL 图卡生成器 - OpenCvSharp";
        FormClosing += MainForm_FormClosing;
        FormClosed += MainForm_FormClosed;
        splitContainerMain.Panel1.ResumeLayout(false);
        splitContainerMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
        splitContainerMain.ResumeLayout(false);
        settingsFlowPanel.ResumeLayout(false);
        groupPattern.ResumeLayout(false);
        groupPattern.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericPhase).EndInit();
        groupCanvas.ResumeLayout(false);
        groupCanvas.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericCanvasHeight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericCanvasWidth).EndInit();
        groupPlacement.ResumeLayout(false);
        groupPlacement.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericPatternHeight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternWidth).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternY).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternX).EndInit();
        groupShape.ResumeLayout(false);
        groupShape.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericColumns).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericRows).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericLineWidth).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericDotRadius).EndInit();
        groupBorderOverlay.ResumeLayout(false);
        groupExport.ResumeLayout(false);
        groupExport.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericQuality).EndInit();
        groupActions.ResumeLayout(false);
        previewHeaderPanel.ResumeLayout(false);
        previewHeaderPanel.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private SplitContainer splitContainerMain;
    private FlowLayoutPanel settingsFlowPanel;
    private GroupBox groupPattern;
    private Label labelPatternHelp;
    private NumericUpDown numericPhase;
    private Label labelPhase;
    private ComboBox comboPattern;
    private Button buttonBrowseSourceImage;
    private Label labelPatternType;
    private GroupBox groupCanvas;
    private NumericUpDown numericCanvasHeight;
    private Label labelCanvasHeight;
    private NumericUpDown numericCanvasWidth;
    private Label labelCanvasWidth;
    private GroupBox groupPlacement;
    private EolTestPatternGenerator.Controls.RegionMarginsEditor regionMarginsEditor;
    private Label labelPlacementHelp;
    private Button buttonCenter;
    private NumericUpDown numericPatternHeight;
    private Label labelPatternHeight;
    private NumericUpDown numericPatternWidth;
    private Label labelPatternWidth;
    private NumericUpDown numericPatternY;
    private Label labelPatternY;
    private NumericUpDown numericPatternX;
    private Label labelPatternX;
    private GroupBox groupShape;
    private NumericUpDown numericColumns;
    private Label labelColumns;
    private NumericUpDown numericRows;
    private Label labelRows;
    private NumericUpDown numericLineWidth;
    private Label labelLineWidth;
    private NumericUpDown numericDotRadius;
    private Label labelDotRadius;
    private Label labelDotHelp;
    private GroupBox groupBorderOverlay;
    private EolTestPatternGenerator.Controls.BorderOverlayEditor borderOverlayEditor;
    private GroupBox groupExport;
    private Label labelExportHelp;
    private NumericUpDown numericQuality;
    private Label labelQuality;
    private ComboBox comboOutputFormat;
    private Label labelOutputFormat;
    private GroupBox groupActions;
    private Button buttonBatchAddBorder;
    private Button buttonBatchExport;
    private Button buttonSaveCurrent;
    private Button buttonReset;
    private Button buttonRefresh;
    private EolTestPatternGenerator.Controls.ImagePreviewControl imagePreviewControl;
    private Panel previewHeaderPanel;
    private Label labelPreviewInfo;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusLabel;
    private System.Windows.Forms.Timer previewTimer;
    private SaveFileDialog saveFileDialog;
    private FolderBrowserDialog folderBrowserDialog;
    private OpenFileDialog openImageDialog;
    private ToolTip toolTip;
    private Button buttonPhaseTool;
    private Button buttonExportScreen1;
}
