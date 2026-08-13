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
        labelPatternHelp = new Label();
        numericPhase = new NumericUpDown();
        labelPhase = new Label();
        comboPattern = new ComboBox();
        buttonBrowseSourceImage = new Button();
        labelPatternType = new Label();
        groupCanvas = new GroupBox();
        numericCanvasHeight = new NumericUpDown();
        labelCanvasHeight = new Label();
        numericCanvasWidth = new NumericUpDown();
        labelCanvasWidth = new Label();
        groupPlacement = new GroupBox();
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
        buttonBatchExport = new Button();
        buttonSaveCurrent = new Button();
        buttonReset = new Button();
        buttonRefresh = new Button();
        buttonPhaseTool = new Button();
        buttonExportScreen1 = new Button();
        buttonExportScreen1Stereo = new Button();
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
        splitContainerMain.Size = new Size(1380, 838);
        splitContainerMain.SplitterDistance = 405;
        splitContainerMain.SplitterWidth = 6;
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
        settingsFlowPanel.Name = "settingsFlowPanel";
        settingsFlowPanel.Padding = new Padding(10, 10, 8, 16);
        settingsFlowPanel.Size = new Size(405, 838);
        settingsFlowPanel.TabIndex = 0;
        settingsFlowPanel.WrapContents = false;
        // 
        // groupPattern
        // 
        groupPattern.Controls.Add(labelPatternHelp);
        groupPattern.Controls.Add(buttonBrowseSourceImage);
        groupPattern.Controls.Add(numericPhase);
        groupPattern.Controls.Add(labelPhase);
        groupPattern.Controls.Add(comboPattern);
        groupPattern.Controls.Add(labelPatternType);
        groupPattern.Location = new Point(13, 13);
        groupPattern.Name = "groupPattern";
        groupPattern.Size = new Size(367, 122);
        groupPattern.TabIndex = 0;
        groupPattern.TabStop = false;
        groupPattern.Text = "图卡类型";
        // 
        // labelPatternHelp
        // 
        labelPatternHelp.ForeColor = Color.DimGray;
        labelPatternHelp.Location = new Point(18, 67);
        labelPatternHelp.Name = "labelPatternHelp";
        labelPatternHelp.Size = new Size(331, 43);
        labelPatternHelp.TabIndex = 4;
        labelPatternHelp.Text = "外框、相移、白图使用矩形区域；点阵使用首个圆心和圆心跨度。";
        // 
        // numericPhase
        // 
        numericPhase.Location = new Point(112, 66);
        numericPhase.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
        numericPhase.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPhase.Name = "numericPhase";
        numericPhase.Visible = false;
        numericPhase.Size = new Size(98, 27);
        numericPhase.TabIndex = 3;
        numericPhase.Value = new decimal(new int[] { 1, 0, 0, 0 });
        numericPhase.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPhase
        // 
        labelPhase.AutoSize = true;
        labelPhase.Location = new Point(18, 70);
        labelPhase.Name = "labelPhase";
        labelPhase.Visible = false;
        labelPhase.Size = new Size(75, 20);
        labelPhase.TabIndex = 2;
        labelPhase.Text = "相位(1-8)";
        // 
        // comboPattern
        // 
        comboPattern.DropDownStyle = ComboBoxStyle.DropDownList;
        comboPattern.FormattingEnabled = true;
        comboPattern.Location = new Point(112, 28);
        comboPattern.Name = "comboPattern";
        comboPattern.Size = new Size(237, 28);
        comboPattern.TabIndex = 1;
        comboPattern.SelectedIndexChanged += comboPattern_SelectedIndexChanged;
        //
        // buttonBrowseSourceImage
        //
        buttonBrowseSourceImage.Location = new Point(216, 66);
        buttonBrowseSourceImage.Name = "buttonBrowseSourceImage";
        buttonBrowseSourceImage.Size = new Size(133, 32);
        buttonBrowseSourceImage.TabIndex = 5;
        buttonBrowseSourceImage.Text = "选择底图...";
        buttonBrowseSourceImage.UseVisualStyleBackColor = true;
        buttonBrowseSourceImage.Visible = false;
        buttonBrowseSourceImage.Click += buttonBrowseSourceImage_Click;
        // 
        // labelPatternType
        // 
        labelPatternType.AutoSize = true;
        labelPatternType.Location = new Point(18, 32);
        labelPatternType.Name = "labelPatternType";
        labelPatternType.Size = new Size(69, 20);
        labelPatternType.TabIndex = 0;
        labelPatternType.Text = "图卡类型";
        // 
        // groupCanvas
        // 
        groupCanvas.Controls.Add(numericCanvasHeight);
        groupCanvas.Controls.Add(labelCanvasHeight);
        groupCanvas.Controls.Add(numericCanvasWidth);
        groupCanvas.Controls.Add(labelCanvasWidth);
        groupCanvas.Location = new Point(13, 176);
        groupCanvas.Name = "groupCanvas";
        groupCanvas.Size = new Size(367, 106);
        groupCanvas.TabIndex = 1;
        groupCanvas.TabStop = false;
        groupCanvas.Text = "输出画布（可修改）";
        // 
        // numericCanvasHeight
        // 
        numericCanvasHeight.Location = new Point(246, 43);
        numericCanvasHeight.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericCanvasHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericCanvasHeight.Name = "numericCanvasHeight";
        numericCanvasHeight.Size = new Size(103, 27);
        numericCanvasHeight.TabIndex = 3;
        numericCanvasHeight.ThousandsSeparator = true;
        numericCanvasHeight.Value = new decimal(new int[] { 1080, 0, 0, 0 });
        numericCanvasHeight.ValueChanged += Parameter_ValueChanged;
        // 
        // labelCanvasHeight
        // 
        labelCanvasHeight.AutoSize = true;
        labelCanvasHeight.Location = new Point(192, 47);
        labelCanvasHeight.Name = "labelCanvasHeight";
        labelCanvasHeight.Size = new Size(41, 20);
        labelCanvasHeight.TabIndex = 2;
        labelCanvasHeight.Text = "高度";
        // 
        // numericCanvasWidth
        // 
        numericCanvasWidth.Location = new Point(72, 43);
        numericCanvasWidth.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericCanvasWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericCanvasWidth.Name = "numericCanvasWidth";
        numericCanvasWidth.Size = new Size(103, 27);
        numericCanvasWidth.TabIndex = 1;
        numericCanvasWidth.ThousandsSeparator = true;
        numericCanvasWidth.Value = new decimal(new int[] { 1920, 0, 0, 0 });
        numericCanvasWidth.ValueChanged += Parameter_ValueChanged;
        // 
        // labelCanvasWidth
        // 
        labelCanvasWidth.AutoSize = true;
        labelCanvasWidth.Location = new Point(18, 47);
        labelCanvasWidth.Name = "labelCanvasWidth";
        labelCanvasWidth.Size = new Size(41, 20);
        labelCanvasWidth.TabIndex = 0;
        labelCanvasWidth.Text = "宽度";
        // 
        // groupPlacement
        // 
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
        groupPlacement.Location = new Point(13, 288);
        groupPlacement.Name = "groupPlacement";
        groupPlacement.Size = new Size(367, 205);
        groupPlacement.TabIndex = 2;
        groupPlacement.TabStop = false;
        groupPlacement.Text = "图案位置与区域尺寸（可修改）";
        // 
        // labelPlacementHelp
        // 
        labelPlacementHelp.ForeColor = Color.DimGray;
        labelPlacementHelp.Location = new Point(18, 158);
        labelPlacementHelp.Name = "labelPlacementHelp";
        labelPlacementHelp.Size = new Size(331, 39);
        labelPlacementHelp.TabIndex = 9;
        labelPlacementHelp.Text = "坐标允许为负数，超出画布的部分会被安全裁剪。";
        // 
        // buttonCenter
        // 
        buttonCenter.Location = new Point(246, 109);
        buttonCenter.Name = "buttonCenter";
        buttonCenter.Size = new Size(103, 34);
        buttonCenter.TabIndex = 8;
        buttonCenter.Text = "居中图案";
        buttonCenter.UseVisualStyleBackColor = true;
        buttonCenter.Click += buttonCenter_Click;
        // 
        // numericPatternHeight
        // 
        numericPatternHeight.Location = new Point(246, 70);
        numericPatternHeight.Maximum = new decimal(new int[] { 32768, 0, 0, 0 });
        numericPatternHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPatternHeight.Name = "numericPatternHeight";
        numericPatternHeight.Size = new Size(103, 27);
        numericPatternHeight.TabIndex = 7;
        numericPatternHeight.ThousandsSeparator = true;
        numericPatternHeight.Value = new decimal(new int[] { 627, 0, 0, 0 });
        numericPatternHeight.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternHeight
        // 
        labelPatternHeight.AutoSize = true;
        labelPatternHeight.Location = new Point(192, 74);
        labelPatternHeight.Name = "labelPatternHeight";
        labelPatternHeight.Size = new Size(41, 20);
        labelPatternHeight.TabIndex = 6;
        labelPatternHeight.Text = "高度";
        // 
        // numericPatternWidth
        // 
        numericPatternWidth.Location = new Point(72, 70);
        numericPatternWidth.Maximum = new decimal(new int[] { 32768, 0, 0, 0 });
        numericPatternWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPatternWidth.Name = "numericPatternWidth";
        numericPatternWidth.Size = new Size(103, 27);
        numericPatternWidth.TabIndex = 5;
        numericPatternWidth.ThousandsSeparator = true;
        numericPatternWidth.Value = new decimal(new int[] { 1777, 0, 0, 0 });
        numericPatternWidth.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternWidth
        // 
        labelPatternWidth.AutoSize = true;
        labelPatternWidth.Location = new Point(18, 74);
        labelPatternWidth.Name = "labelPatternWidth";
        labelPatternWidth.Size = new Size(41, 20);
        labelPatternWidth.TabIndex = 4;
        labelPatternWidth.Text = "宽度";
        // 
        // numericPatternY
        // 
        numericPatternY.Location = new Point(246, 31);
        numericPatternY.Maximum = new decimal(new int[] { 32768, 0, 0, 0 });
        numericPatternY.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
        numericPatternY.Name = "numericPatternY";
        numericPatternY.Size = new Size(103, 27);
        numericPatternY.TabIndex = 3;
        numericPatternY.Value = new decimal(new int[] { 226, 0, 0, 0 });
        numericPatternY.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternY
        // 
        labelPatternY.AutoSize = true;
        labelPatternY.Location = new Point(192, 35);
        labelPatternY.Name = "labelPatternY";
        labelPatternY.Size = new Size(18, 20);
        labelPatternY.TabIndex = 2;
        labelPatternY.Text = "Y";
        // 
        // numericPatternX
        // 
        numericPatternX.Location = new Point(72, 31);
        numericPatternX.Maximum = new decimal(new int[] { 32768, 0, 0, 0 });
        numericPatternX.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
        numericPatternX.Name = "numericPatternX";
        numericPatternX.Size = new Size(103, 27);
        numericPatternX.TabIndex = 1;
        numericPatternX.Value = new decimal(new int[] { 71, 0, 0, 0 });
        numericPatternX.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternX
        // 
        labelPatternX.AutoSize = true;
        labelPatternX.Location = new Point(18, 35);
        labelPatternX.Name = "labelPatternX";
        labelPatternX.Size = new Size(19, 20);
        labelPatternX.TabIndex = 0;
        labelPatternX.Text = "X";
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
        groupShape.Location = new Point(13, 499);
        groupShape.Name = "groupShape";
        groupShape.Size = new Size(367, 174);
        groupShape.TabIndex = 3;
        groupShape.TabStop = false;
        groupShape.Text = "图形参数";
        // 
        // numericColumns
        // 
        numericColumns.Location = new Point(246, 72);
        numericColumns.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        numericColumns.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericColumns.Name = "numericColumns";
        numericColumns.Size = new Size(103, 27);
        numericColumns.TabIndex = 7;
        numericColumns.Value = new decimal(new int[] { 3, 0, 0, 0 });
        numericColumns.ValueChanged += Parameter_ValueChanged;
        // 
        // labelColumns
        // 
        labelColumns.AutoSize = true;
        labelColumns.Location = new Point(192, 76);
        labelColumns.Name = "labelColumns";
        labelColumns.Size = new Size(25, 20);
        labelColumns.TabIndex = 6;
        labelColumns.Text = "列";
        // 
        // numericRows
        // 
        numericRows.Location = new Point(72, 72);
        numericRows.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        numericRows.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericRows.Name = "numericRows";
        numericRows.Size = new Size(103, 27);
        numericRows.TabIndex = 5;
        numericRows.Value = new decimal(new int[] { 3, 0, 0, 0 });
        numericRows.ValueChanged += Parameter_ValueChanged;
        // 
        // labelRows
        // 
        labelRows.AutoSize = true;
        labelRows.Location = new Point(18, 76);
        labelRows.Name = "labelRows";
        labelRows.Size = new Size(25, 20);
        labelRows.TabIndex = 4;
        labelRows.Text = "行";
        // 
        // numericLineWidth
        // 
        numericLineWidth.Location = new Point(246, 33);
        numericLineWidth.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
        numericLineWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericLineWidth.Name = "numericLineWidth";
        numericLineWidth.Size = new Size(103, 27);
        numericLineWidth.TabIndex = 3;
        numericLineWidth.Value = new decimal(new int[] { 5, 0, 0, 0 });
        numericLineWidth.ValueChanged += Parameter_ValueChanged;
        // 
        // labelLineWidth
        // 
        labelLineWidth.AutoSize = true;
        labelLineWidth.Location = new Point(192, 37);
        labelLineWidth.Name = "labelLineWidth";
        labelLineWidth.Size = new Size(41, 20);
        labelLineWidth.TabIndex = 2;
        labelLineWidth.Text = "线宽";
        // 
        // numericDotRadius
        // 
        numericDotRadius.Location = new Point(112, 33);
        numericDotRadius.Maximum = new decimal(new int[] { 2048, 0, 0, 0 });
        numericDotRadius.Name = "numericDotRadius";
        numericDotRadius.Size = new Size(63, 27);
        numericDotRadius.TabIndex = 1;
        numericDotRadius.Value = new decimal(new int[] { 4, 0, 0, 0 });
        numericDotRadius.ValueChanged += Parameter_ValueChanged;
        // 
        // labelDotRadius
        // 
        labelDotRadius.AutoSize = true;
        labelDotRadius.Location = new Point(18, 37);
        labelDotRadius.Name = "labelDotRadius";
        labelDotRadius.Size = new Size(87, 20);
        labelDotRadius.TabIndex = 0;
        labelDotRadius.Text = "圆点半径(px)";
        // 
        // labelDotHelp
        // 
        labelDotHelp.ForeColor = Color.DimGray;
        labelDotHelp.Location = new Point(18, 112);
        labelDotHelp.Name = "labelDotHelp";
        labelDotHelp.Size = new Size(331, 42);
        labelDotHelp.TabIndex = 8;
        labelDotHelp.Text = "默认半径 4，直径为 2r+1=9 px；点阵行列数也可调整。";
        //
        // groupBorderOverlay
        //
        groupBorderOverlay.Controls.Add(borderOverlayEditor);
        groupBorderOverlay.Location = new Point(13, 679);
        groupBorderOverlay.Name = "groupBorderOverlay";
        groupBorderOverlay.Size = new Size(367, 226);
        groupBorderOverlay.TabIndex = 4;
        groupBorderOverlay.TabStop = false;
        groupBorderOverlay.Text = "白框叠加层（可应用于任意底图）";
        //
        // borderOverlayEditor
        //
        borderOverlayEditor.CanvasSize = new Size(1920, 1080);
        borderOverlayEditor.Location = new Point(14, 26);
        borderOverlayEditor.Name = "borderOverlayEditor";
        borderOverlayEditor.Size = new Size(335, 188);
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
        groupExport.Location = new Point(13, 911);
        groupExport.Name = "groupExport";
        groupExport.Size = new Size(367, 145);
        groupExport.TabIndex = 5;
        groupExport.TabStop = false;
        groupExport.Text = "导出格式";
        // 
        // labelExportHelp
        // 
        labelExportHelp.ForeColor = Color.DimGray;
        labelExportHelp.Location = new Point(18, 99);
        labelExportHelp.Name = "labelExportHelp";
        labelExportHelp.Size = new Size(331, 37);
        labelExportHelp.TabIndex = 4;
        labelExportHelp.Text = "PNG/BMP/TIFF 无损；JPG 会改变精确像素，检测图建议优先 PNG。";
        // 
        // numericQuality
        // 
        numericQuality.Location = new Point(112, 66);
        numericQuality.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
        numericQuality.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericQuality.Name = "numericQuality";
        numericQuality.Size = new Size(98, 27);
        numericQuality.TabIndex = 3;
        numericQuality.Value = new decimal(new int[] { 95, 0, 0, 0 });
        // 
        // labelQuality
        // 
        labelQuality.AutoSize = true;
        labelQuality.Location = new Point(18, 70);
        labelQuality.Name = "labelQuality";
        labelQuality.Size = new Size(69, 20);
        labelQuality.TabIndex = 2;
        labelQuality.Text = "压缩质量";
        // 
        // comboOutputFormat
        // 
        comboOutputFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        comboOutputFormat.FormattingEnabled = true;
        comboOutputFormat.Items.AddRange(new object[] { "PNG（无损，推荐）", "JPEG / JPG", "BMP", "TIFF", "WebP" });
        comboOutputFormat.Location = new Point(112, 28);
        comboOutputFormat.Name = "comboOutputFormat";
        comboOutputFormat.Size = new Size(237, 28);
        comboOutputFormat.TabIndex = 1;
        comboOutputFormat.SelectedIndexChanged += comboOutputFormat_SelectedIndexChanged;
        // 
        // labelOutputFormat
        // 
        labelOutputFormat.AutoSize = true;
        labelOutputFormat.Location = new Point(18, 32);
        labelOutputFormat.Name = "labelOutputFormat";
        labelOutputFormat.Size = new Size(69, 20);
        labelOutputFormat.TabIndex = 0;
        labelOutputFormat.Text = "图片格式";
        // 
        // groupActions
        // 
        groupActions.Controls.Add(buttonBatchExport);
        groupActions.Controls.Add(buttonSaveCurrent);
        groupActions.Controls.Add(buttonReset);
        groupActions.Controls.Add(buttonRefresh);
        groupActions.Controls.Add(buttonPhaseTool);
        groupActions.Controls.Add(buttonExportScreen1);
        groupActions.Controls.Add(buttonExportScreen1Stereo);
        groupActions.Location = new Point(13, 1062);
        groupActions.Name = "groupActions";
        groupActions.Size = new Size(367, 226);
        groupActions.TabIndex = 6;
        groupActions.TabStop = false;
        groupActions.Text = "生成与保存";
        // 
        // buttonBatchExport
        // 
        buttonBatchExport.Location = new Point(192, 75);
        buttonBatchExport.Name = "buttonBatchExport";
        buttonBatchExport.Size = new Size(157, 38);
        buttonBatchExport.TabIndex = 3;
        buttonBatchExport.Text = "批量导出主图 10 张";
        buttonBatchExport.UseVisualStyleBackColor = true;
        buttonBatchExport.Click += buttonBatchExport_Click;
        //
        // buttonPhaseTool
        //
        buttonPhaseTool.Location = new Point(18, 122);
        buttonPhaseTool.Name = "buttonPhaseTool";
        buttonPhaseTool.Size = new Size(331, 38);
        buttonPhaseTool.TabIndex = 4;
        buttonPhaseTool.Text = "打开 RGB 八步相移条纹工具...";
        buttonPhaseTool.UseVisualStyleBackColor = true;
        buttonPhaseTool.Click += buttonPhaseTool_Click;
        //
        // buttonExportScreen1
        //
        buttonExportScreen1.Location = new Point(18, 169);
        buttonExportScreen1.Name = "buttonExportScreen1";
        buttonExportScreen1.Size = new Size(331, 38);
        buttonExportScreen1.TabIndex = 5;
        buttonExportScreen1.Text = "导出1号屏参考图（3张）";
        buttonExportScreen1.UseVisualStyleBackColor = true;
        buttonExportScreen1.Click += buttonExportScreen1_Click;
        //
        // buttonExportScreen1Stereo
        //
        buttonExportScreen1Stereo.Location = new Point(192, 169);
        buttonExportScreen1Stereo.Name = "buttonExportScreen1Stereo";
        buttonExportScreen1Stereo.Size = new Size(157, 38);
        buttonExportScreen1Stereo.TabIndex = 6;
        buttonExportScreen1Stereo.Text = "3D系列（暂未启用）";
        buttonExportScreen1Stereo.UseVisualStyleBackColor = true;
        buttonExportScreen1Stereo.Visible = false;
        buttonExportScreen1Stereo.Click += buttonExportScreen1Stereo_Click;
        // 
        // buttonSaveCurrent
        // 
        buttonSaveCurrent.Location = new Point(18, 75);
        buttonSaveCurrent.Name = "buttonSaveCurrent";
        buttonSaveCurrent.Size = new Size(157, 38);
        buttonSaveCurrent.TabIndex = 2;
        buttonSaveCurrent.Text = "保存当前图卡...";
        buttonSaveCurrent.UseVisualStyleBackColor = true;
        buttonSaveCurrent.Click += buttonSaveCurrent_Click;
        // 
        // buttonReset
        // 
        buttonReset.Location = new Point(192, 28);
        buttonReset.Name = "buttonReset";
        buttonReset.Size = new Size(157, 38);
        buttonReset.TabIndex = 1;
        buttonReset.Text = "恢复样图默认参数";
        buttonReset.UseVisualStyleBackColor = true;
        buttonReset.Click += buttonReset_Click;
        // 
        // buttonRefresh
        // 
        buttonRefresh.Location = new Point(18, 28);
        buttonRefresh.Name = "buttonRefresh";
        buttonRefresh.Size = new Size(157, 38);
        buttonRefresh.TabIndex = 0;
        buttonRefresh.Text = "立即刷新预览";
        buttonRefresh.UseVisualStyleBackColor = true;
        buttonRefresh.Click += buttonRefresh_Click;
        // 
        // imagePreviewControl
        // 
        imagePreviewControl.BackColor = Color.FromArgb(224, 228, 234);
        imagePreviewControl.Dock = DockStyle.Fill;
        imagePreviewControl.Location = new Point(0, 44);
        imagePreviewControl.Name = "imagePreviewControl";
        imagePreviewControl.Size = new Size(969, 794);
        imagePreviewControl.TabIndex = 1;
        // 
        // previewHeaderPanel
        // 
        previewHeaderPanel.BackColor = Color.FromArgb(42, 45, 50);
        previewHeaderPanel.Controls.Add(labelPreviewInfo);
        previewHeaderPanel.Dock = DockStyle.Top;
        previewHeaderPanel.Location = new Point(0, 0);
        previewHeaderPanel.Name = "previewHeaderPanel";
        previewHeaderPanel.Size = new Size(969, 44);
        previewHeaderPanel.TabIndex = 0;
        // 
        // labelPreviewInfo
        // 
        labelPreviewInfo.AutoSize = true;
        labelPreviewInfo.ForeColor = Color.WhiteSmoke;
        labelPreviewInfo.Location = new Point(16, 12);
        labelPreviewInfo.Name = "labelPreviewInfo";
        labelPreviewInfo.Size = new Size(126, 20);
        labelPreviewInfo.TabIndex = 0;
        labelPreviewInfo.Text = "预览：1920 × 1080";
        // 
        // statusStrip
        // 
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 838);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1380, 22);
        statusStrip.TabIndex = 1;
        // 
        // statusLabel
        // 
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(126, 17);
        statusLabel.Text = "正在初始化 OpenCV...";
        // 
        // previewTimer
        // 
        previewTimer.Interval = 160;
        previewTimer.Tick += previewTimer_Tick;
        // 
        // saveFileDialog
        // 
        saveFileDialog.AddExtension = true;
        saveFileDialog.OverwritePrompt = true;
        saveFileDialog.Title = "保存图卡";
        // 
        // folderBrowserDialog
        // 
        folderBrowserDialog.Description = "选择批量导出目录";
        folderBrowserDialog.UseDescriptionForTitle = true;
        //
        // openImageDialog
        //
        openImageDialog.CheckFileExists = true;
        openImageDialog.Filter = "图片文件|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff;*.webp|所有文件|*.*";
        openImageDialog.Title = "选择要添加白框的底图";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1380, 860);
        Controls.Add(splitContainerMain);
        Controls.Add(statusStrip);
        Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        MinimumSize = new Size(1100, 700);
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
    private Button buttonBatchExport;
    private Button buttonSaveCurrent;
    private Button buttonReset;
    private Button buttonRefresh;
    private Button buttonPhaseTool;
    private Button buttonExportScreen1;
    private Button buttonExportScreen1Stereo;
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
}
