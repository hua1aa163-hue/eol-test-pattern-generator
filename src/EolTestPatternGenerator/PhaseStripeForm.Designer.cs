using EolTestPatternGenerator.Controls;

namespace EolTestPatternGenerator;

#nullable disable

partial class PhaseStripeForm
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
        groupCanvas = new GroupBox();
        numericCanvasHeight = new NumericUpDown();
        labelCanvasHeight = new Label();
        numericCanvasWidth = new NumericUpDown();
        labelCanvasWidth = new Label();
        groupRegion = new GroupBox();
        buttonCenter = new Button();
        numericPatternHeight = new NumericUpDown();
        labelPatternHeight = new Label();
        numericPatternWidth = new NumericUpDown();
        labelPatternWidth = new Label();
        numericPatternY = new NumericUpDown();
        labelPatternY = new Label();
        numericPatternX = new NumericUpDown();
        labelPatternX = new Label();
        groupPhase = new GroupBox();
        labelOrderHelp = new Label();
        comboPixelOrder = new ComboBox();
        labelPixelOrder = new Label();
        numericPhase = new NumericUpDown();
        labelPhase = new Label();
        borderOverlayEditor = new BorderOverlayEditor();
        groupExport = new GroupBox();
        labelExportHelp = new Label();
        numericQuality = new NumericUpDown();
        labelQuality = new Label();
        comboOutputFormat = new ComboBox();
        labelOutputFormat = new Label();
        groupActions = new GroupBox();
        buttonNonIntegerFusion = new Button();
        buttonBatchExport = new Button();
        buttonSaveCurrent = new Button();
        buttonPreview = new Button();
        previewPanel = new Panel();
        previewControl = new ImagePreviewControl();
        previewHeaderPanel = new Panel();
        labelPreviewInfo = new Label();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        previewTimer = new System.Windows.Forms.Timer(components);
        saveFileDialog = new SaveFileDialog();
        folderBrowserDialog = new FolderBrowserDialog();
        toolTip = new ToolTip(components);
        ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
        splitContainerMain.Panel1.SuspendLayout();
        splitContainerMain.Panel2.SuspendLayout();
        splitContainerMain.SuspendLayout();
        settingsFlowPanel.SuspendLayout();
        groupCanvas.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericCanvasHeight).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericCanvasWidth).BeginInit();
        groupRegion.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericPatternHeight).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternWidth).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternY).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternX).BeginInit();
        groupPhase.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericPhase).BeginInit();
        groupExport.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericQuality).BeginInit();
        groupActions.SuspendLayout();
        previewPanel.SuspendLayout();
        previewHeaderPanel.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // splitContainerMain
        // 
        splitContainerMain.Dock = DockStyle.Fill;
        splitContainerMain.FixedPanel = FixedPanel.Panel1;
        splitContainerMain.Location = new Point(0, 0);
        splitContainerMain.Margin = new Padding(4, 4, 4, 4);
        splitContainerMain.Name = "splitContainerMain";
        // 
        // splitContainerMain.Panel1
        // 
        splitContainerMain.Panel1.Controls.Add(settingsFlowPanel);
        splitContainerMain.Panel1MinSize = 405;
        // 
        // splitContainerMain.Panel2
        // 
        splitContainerMain.Panel2.Controls.Add(previewPanel);
        splitContainerMain.Size = new Size(2130, 1319);
        splitContainerMain.SplitterDistance = 608;
        splitContainerMain.SplitterWidth = 6;
        splitContainerMain.TabIndex = 0;
        // 
        // settingsFlowPanel
        // 
        settingsFlowPanel.AutoScroll = true;
        settingsFlowPanel.Controls.Add(groupCanvas);
        settingsFlowPanel.Controls.Add(groupRegion);
        settingsFlowPanel.Controls.Add(groupPhase);
        settingsFlowPanel.Controls.Add(borderOverlayEditor);
        settingsFlowPanel.Controls.Add(groupExport);
        settingsFlowPanel.Controls.Add(groupActions);
        settingsFlowPanel.Dock = DockStyle.Fill;
        settingsFlowPanel.FlowDirection = FlowDirection.TopDown;
        settingsFlowPanel.Location = new Point(0, 0);
        settingsFlowPanel.Margin = new Padding(4, 4, 4, 4);
        settingsFlowPanel.Name = "settingsFlowPanel";
        settingsFlowPanel.Padding = new Padding(15, 15, 12, 24);
        settingsFlowPanel.Size = new Size(608, 1319);
        settingsFlowPanel.TabIndex = 0;
        settingsFlowPanel.WrapContents = false;
        // 
        // groupCanvas
        // 
        groupCanvas.Controls.Add(numericCanvasHeight);
        groupCanvas.Controls.Add(labelCanvasHeight);
        groupCanvas.Controls.Add(numericCanvasWidth);
        groupCanvas.Controls.Add(labelCanvasWidth);
        groupCanvas.Location = new Point(19, 19);
        groupCanvas.Margin = new Padding(4, 4, 4, 4);
        groupCanvas.Name = "groupCanvas";
        groupCanvas.Padding = new Padding(4, 4, 4, 4);
        groupCanvas.Size = new Size(550, 159);
        groupCanvas.TabIndex = 0;
        groupCanvas.TabStop = false;
        groupCanvas.Text = "画布尺寸";
        // 
        // numericCanvasHeight
        // 
        numericCanvasHeight.Location = new Point(168, 99);
        numericCanvasHeight.Margin = new Padding(4, 4, 4, 4);
        numericCanvasHeight.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericCanvasHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericCanvasHeight.Name = "numericCanvasHeight";
        numericCanvasHeight.Size = new Size(225, 30);
        numericCanvasHeight.TabIndex = 3;
        numericCanvasHeight.Value = new decimal(new int[] { 1080, 0, 0, 0 });
        numericCanvasHeight.ValueChanged += Parameter_ValueChanged;
        // 
        // labelCanvasHeight
        // 
        labelCanvasHeight.AutoSize = true;
        labelCanvasHeight.Location = new Point(27, 105);
        labelCanvasHeight.Margin = new Padding(4, 0, 4, 0);
        labelCanvasHeight.Name = "labelCanvasHeight";
        labelCanvasHeight.Size = new Size(82, 24);
        labelCanvasHeight.TabIndex = 2;
        labelCanvasHeight.Text = "画布高度";
        // 
        // numericCanvasWidth
        // 
        numericCanvasWidth.Location = new Point(168, 42);
        numericCanvasWidth.Margin = new Padding(4, 4, 4, 4);
        numericCanvasWidth.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericCanvasWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericCanvasWidth.Name = "numericCanvasWidth";
        numericCanvasWidth.Size = new Size(225, 30);
        numericCanvasWidth.TabIndex = 1;
        numericCanvasWidth.Value = new decimal(new int[] { 1920, 0, 0, 0 });
        numericCanvasWidth.ValueChanged += Parameter_ValueChanged;
        // 
        // labelCanvasWidth
        // 
        labelCanvasWidth.AutoSize = true;
        labelCanvasWidth.Location = new Point(27, 48);
        labelCanvasWidth.Margin = new Padding(4, 0, 4, 0);
        labelCanvasWidth.Name = "labelCanvasWidth";
        labelCanvasWidth.Size = new Size(82, 24);
        labelCanvasWidth.TabIndex = 0;
        labelCanvasWidth.Text = "画布宽度";
        // 
        // groupRegion
        // 
        groupRegion.Controls.Add(buttonCenter);
        groupRegion.Controls.Add(numericPatternHeight);
        groupRegion.Controls.Add(labelPatternHeight);
        groupRegion.Controls.Add(numericPatternWidth);
        groupRegion.Controls.Add(labelPatternWidth);
        groupRegion.Controls.Add(numericPatternY);
        groupRegion.Controls.Add(labelPatternY);
        groupRegion.Controls.Add(numericPatternX);
        groupRegion.Controls.Add(labelPatternX);
        groupRegion.Location = new Point(19, 186);
        groupRegion.Margin = new Padding(4, 4, 4, 4);
        groupRegion.Name = "groupRegion";
        groupRegion.Padding = new Padding(4, 4, 4, 4);
        groupRegion.Size = new Size(550, 225);
        groupRegion.TabIndex = 1;
        groupRegion.TabStop = false;
        groupRegion.Text = "条纹区域";
        // 
        // buttonCenter
        // 
        buttonCenter.Location = new Point(375, 156);
        buttonCenter.Margin = new Padding(4, 4, 4, 4);
        buttonCenter.Name = "buttonCenter";
        buttonCenter.Size = new Size(148, 45);
        buttonCenter.TabIndex = 8;
        buttonCenter.Text = "区域居中";
        buttonCenter.UseVisualStyleBackColor = true;
        buttonCenter.Click += buttonCenter_Click;
        // 
        // numericPatternHeight
        // 
        numericPatternHeight.Location = new Point(374, 99);
        numericPatternHeight.Margin = new Padding(4, 4, 4, 4);
        numericPatternHeight.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericPatternHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPatternHeight.Name = "numericPatternHeight";
        numericPatternHeight.Size = new Size(150, 30);
        numericPatternHeight.TabIndex = 7;
        numericPatternHeight.Value = new decimal(new int[] { 627, 0, 0, 0 });
        numericPatternHeight.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternHeight
        // 
        labelPatternHeight.AutoSize = true;
        labelPatternHeight.Location = new Point(290, 105);
        labelPatternHeight.Margin = new Padding(4, 0, 4, 0);
        labelPatternHeight.Name = "labelPatternHeight";
        labelPatternHeight.Size = new Size(46, 24);
        labelPatternHeight.TabIndex = 6;
        labelPatternHeight.Text = "高度";
        // 
        // numericPatternWidth
        // 
        numericPatternWidth.Location = new Point(114, 99);
        numericPatternWidth.Margin = new Padding(4, 4, 4, 4);
        numericPatternWidth.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericPatternWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPatternWidth.Name = "numericPatternWidth";
        numericPatternWidth.Size = new Size(150, 30);
        numericPatternWidth.TabIndex = 5;
        numericPatternWidth.Value = new decimal(new int[] { 1777, 0, 0, 0 });
        numericPatternWidth.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternWidth
        // 
        labelPatternWidth.AutoSize = true;
        labelPatternWidth.Location = new Point(27, 105);
        labelPatternWidth.Margin = new Padding(4, 0, 4, 0);
        labelPatternWidth.Name = "labelPatternWidth";
        labelPatternWidth.Size = new Size(46, 24);
        labelPatternWidth.TabIndex = 4;
        labelPatternWidth.Text = "宽度";
        // 
        // numericPatternY
        // 
        numericPatternY.Location = new Point(374, 42);
        numericPatternY.Margin = new Padding(4, 4, 4, 4);
        numericPatternY.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericPatternY.Minimum = new decimal(new int[] { 16384, 0, 0, int.MinValue });
        numericPatternY.Name = "numericPatternY";
        numericPatternY.Size = new Size(150, 30);
        numericPatternY.TabIndex = 3;
        numericPatternY.Value = new decimal(new int[] { 226, 0, 0, 0 });
        numericPatternY.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternY
        // 
        labelPatternY.AutoSize = true;
        labelPatternY.Location = new Point(290, 48);
        labelPatternY.Margin = new Padding(4, 0, 4, 0);
        labelPatternY.Name = "labelPatternY";
        labelPatternY.Size = new Size(62, 24);
        labelPatternY.TabIndex = 2;
        labelPatternY.Text = "左上 Y";
        // 
        // numericPatternX
        // 
        numericPatternX.Location = new Point(114, 42);
        numericPatternX.Margin = new Padding(4, 4, 4, 4);
        numericPatternX.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericPatternX.Minimum = new decimal(new int[] { 16384, 0, 0, int.MinValue });
        numericPatternX.Name = "numericPatternX";
        numericPatternX.Size = new Size(150, 30);
        numericPatternX.TabIndex = 1;
        numericPatternX.Value = new decimal(new int[] { 71, 0, 0, 0 });
        numericPatternX.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPatternX
        // 
        labelPatternX.AutoSize = true;
        labelPatternX.Location = new Point(27, 48);
        labelPatternX.Margin = new Padding(4, 0, 4, 0);
        labelPatternX.Name = "labelPatternX";
        labelPatternX.Size = new Size(63, 24);
        labelPatternX.TabIndex = 0;
        labelPatternX.Text = "左上 X";
        // 
        // groupPhase
        // 
        groupPhase.Controls.Add(labelOrderHelp);
        groupPhase.Controls.Add(comboPixelOrder);
        groupPhase.Controls.Add(labelPixelOrder);
        groupPhase.Controls.Add(numericPhase);
        groupPhase.Controls.Add(labelPhase);
        groupPhase.Location = new Point(19, 419);
        groupPhase.Margin = new Padding(4, 4, 4, 4);
        groupPhase.Name = "groupPhase";
        groupPhase.Padding = new Padding(4, 4, 4, 4);
        groupPhase.Size = new Size(550, 212);
        groupPhase.TabIndex = 2;
        groupPhase.TabStop = false;
        groupPhase.Text = "相位与颜色排列";
        // 
        // labelOrderHelp
        // 
        labelOrderHelp.ForeColor = Color.DimGray;
        labelOrderHelp.Location = new Point(27, 153);
        labelOrderHelp.Margin = new Padding(4, 0, 4, 0);
        labelOrderHelp.Name = "labelOrderHelp";
        labelOrderHelp.Size = new Size(496, 45);
        labelOrderHelp.TabIndex = 4;
        labelOrderHelp.Text = "默认 RGB 与原始 1–8.png 保持一致。";
        // 
        // comboPixelOrder
        // 
        comboPixelOrder.DropDownStyle = ComboBoxStyle.DropDownList;
        comboPixelOrder.FormattingEnabled = true;
        comboPixelOrder.Items.AddRange(new object[] { "RGB", "RBG", "GRB", "GBR", "BRG", "BGR" });
        comboPixelOrder.Location = new Point(168, 99);
        comboPixelOrder.Margin = new Padding(4, 4, 4, 4);
        comboPixelOrder.Name = "comboPixelOrder";
        comboPixelOrder.Size = new Size(354, 32);
        comboPixelOrder.TabIndex = 3;
        comboPixelOrder.SelectedIndexChanged += comboPixelOrder_SelectedIndexChanged;
        // 
        // labelPixelOrder
        // 
        labelPixelOrder.AutoSize = true;
        labelPixelOrder.Location = new Point(27, 105);
        labelPixelOrder.Margin = new Padding(4, 0, 4, 0);
        labelPixelOrder.Name = "labelPixelOrder";
        labelPixelOrder.Size = new Size(82, 24);
        labelPixelOrder.TabIndex = 2;
        labelPixelOrder.Text = "像素排列";
        // 
        // numericPhase
        // 
        numericPhase.Location = new Point(168, 42);
        numericPhase.Margin = new Padding(4, 4, 4, 4);
        numericPhase.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
        numericPhase.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPhase.Name = "numericPhase";
        numericPhase.Size = new Size(147, 30);
        numericPhase.TabIndex = 1;
        numericPhase.Value = new decimal(new int[] { 1, 0, 0, 0 });
        numericPhase.ValueChanged += Parameter_ValueChanged;
        // 
        // labelPhase
        // 
        labelPhase.AutoSize = true;
        labelPhase.Location = new Point(27, 48);
        labelPhase.Margin = new Padding(4, 0, 4, 0);
        labelPhase.Name = "labelPhase";
        labelPhase.Size = new Size(88, 24);
        labelPhase.TabIndex = 0;
        labelPhase.Text = "相位(1-8)";
        // 
        // borderOverlayEditor
        // 
        borderOverlayEditor.CanvasSize = new Size(1920, 1080);
        borderOverlayEditor.Location = new Point(21, 641);
        borderOverlayEditor.Margin = new Padding(6, 6, 6, 6);
        borderOverlayEditor.Name = "borderOverlayEditor";
        borderOverlayEditor.Size = new Size(550, 345);
        borderOverlayEditor.TabIndex = 3;
        borderOverlayEditor.SettingsChanged += borderOverlayEditor_SettingsChanged;
        // 
        // groupExport
        // 
        groupExport.Controls.Add(labelExportHelp);
        groupExport.Controls.Add(numericQuality);
        groupExport.Controls.Add(labelQuality);
        groupExport.Controls.Add(comboOutputFormat);
        groupExport.Controls.Add(labelOutputFormat);
        groupExport.Location = new Point(19, 996);
        groupExport.Margin = new Padding(4, 4, 4, 4);
        groupExport.Name = "groupExport";
        groupExport.Padding = new Padding(4, 4, 4, 4);
        groupExport.Size = new Size(550, 208);
        groupExport.TabIndex = 4;
        groupExport.TabStop = false;
        groupExport.Text = "导出设置";
        // 
        // labelExportHelp
        // 
        labelExportHelp.ForeColor = Color.DimGray;
        labelExportHelp.Location = new Point(27, 153);
        labelExportHelp.Margin = new Padding(4, 0, 4, 0);
        labelExportHelp.Name = "labelExportHelp";
        labelExportHelp.Size = new Size(496, 45);
        labelExportHelp.TabIndex = 4;
        labelExportHelp.Text = "PNG、BMP、TIFF 可保持精确的 0/255 像素。";
        // 
        // numericQuality
        // 
        numericQuality.Location = new Point(168, 99);
        numericQuality.Margin = new Padding(4, 4, 4, 4);
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
        comboOutputFormat.Margin = new Padding(4, 4, 4, 4);
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
        groupActions.Controls.Add(buttonNonIntegerFusion);
        groupActions.Controls.Add(buttonBatchExport);
        groupActions.Controls.Add(buttonSaveCurrent);
        groupActions.Controls.Add(buttonPreview);
        groupActions.Location = new Point(19, 1212);
        groupActions.Margin = new Padding(4, 4, 4, 4);
        groupActions.Name = "groupActions";
        groupActions.Padding = new Padding(4, 4, 4, 4);
        groupActions.Size = new Size(550, 243);
        groupActions.TabIndex = 5;
        groupActions.TabStop = false;
        groupActions.Text = "生成与保存";
        // 
        // buttonNonIntegerFusion
        // 
        buttonNonIntegerFusion.Location = new Point(27, 169);
        buttonNonIntegerFusion.Margin = new Padding(4, 4, 4, 4);
        buttonNonIntegerFusion.Name = "buttonNonIntegerFusion";
        buttonNonIntegerFusion.Size = new Size(496, 51);
        buttonNonIntegerFusion.TabIndex = 3;
        buttonNonIntegerFusion.Text = "非整数融合与 LightTools（C#）...";
        buttonNonIntegerFusion.UseVisualStyleBackColor = true;
        buttonNonIntegerFusion.Click += buttonNonIntegerFusion_Click;
        // 
        // buttonBatchExport
        // 
        buttonBatchExport.Location = new Point(288, 104);
        buttonBatchExport.Margin = new Padding(4, 4, 4, 4);
        buttonBatchExport.Name = "buttonBatchExport";
        buttonBatchExport.Size = new Size(236, 54);
        buttonBatchExport.TabIndex = 2;
        buttonBatchExport.Text = "批量导出相位 1–8";
        buttonBatchExport.UseVisualStyleBackColor = true;
        buttonBatchExport.Click += buttonBatchExport_Click;
        // 
        // buttonSaveCurrent
        // 
        buttonSaveCurrent.Location = new Point(27, 104);
        buttonSaveCurrent.Margin = new Padding(4, 4, 4, 4);
        buttonSaveCurrent.Name = "buttonSaveCurrent";
        buttonSaveCurrent.Size = new Size(236, 54);
        buttonSaveCurrent.TabIndex = 1;
        buttonSaveCurrent.Text = "保存当前相位...";
        buttonSaveCurrent.UseVisualStyleBackColor = true;
        buttonSaveCurrent.Click += buttonSaveCurrent_Click;
        // 
        // buttonPreview
        // 
        buttonPreview.Location = new Point(27, 40);
        buttonPreview.Margin = new Padding(4, 4, 4, 4);
        buttonPreview.Name = "buttonPreview";
        buttonPreview.Size = new Size(496, 51);
        buttonPreview.TabIndex = 0;
        buttonPreview.Text = "立即刷新预览";
        buttonPreview.UseVisualStyleBackColor = true;
        buttonPreview.Click += buttonPreview_Click;
        // 
        // previewPanel
        // 
        previewPanel.BackColor = SystemColors.ControlDark;
        previewPanel.Controls.Add(previewControl);
        previewPanel.Controls.Add(previewHeaderPanel);
        previewPanel.Dock = DockStyle.Fill;
        previewPanel.Location = new Point(0, 0);
        previewPanel.Margin = new Padding(4, 4, 4, 4);
        previewPanel.Name = "previewPanel";
        previewPanel.Size = new Size(1516, 1319);
        previewPanel.TabIndex = 0;
        // 
        // previewControl
        // 
        previewControl.BackColor = SystemColors.Control;
        previewControl.Dock = DockStyle.Fill;
        previewControl.Location = new Point(0, 66);
        previewControl.Margin = new Padding(6, 6, 6, 6);
        previewControl.Name = "previewControl";
        previewControl.Size = new Size(1516, 1253);
        previewControl.TabIndex = 1;
        // 
        // previewHeaderPanel
        // 
        previewHeaderPanel.BackColor = Color.FromArgb(42, 45, 50);
        previewHeaderPanel.Controls.Add(labelPreviewInfo);
        previewHeaderPanel.Dock = DockStyle.Top;
        previewHeaderPanel.Location = new Point(0, 0);
        previewHeaderPanel.Margin = new Padding(4, 4, 4, 4);
        previewHeaderPanel.Name = "previewHeaderPanel";
        previewHeaderPanel.Size = new Size(1516, 66);
        previewHeaderPanel.TabIndex = 0;
        // 
        // labelPreviewInfo
        // 
        labelPreviewInfo.AutoSize = true;
        labelPreviewInfo.ForeColor = Color.WhiteSmoke;
        labelPreviewInfo.Location = new Point(24, 18);
        labelPreviewInfo.Margin = new Padding(4, 0, 4, 0);
        labelPreviewInfo.Name = "labelPreviewInfo";
        labelPreviewInfo.Size = new Size(293, 24);
        labelPreviewInfo.TabIndex = 0;
        labelPreviewInfo.Text = "预览：1920 × 1080 | 相位 1 | RGB";
        // 
        // statusStrip
        // 
        statusStrip.ImageScalingSize = new Size(24, 24);
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 1319);
        statusStrip.Name = "statusStrip";
        statusStrip.Padding = new Padding(2, 0, 21, 0);
        statusStrip.Size = new Size(2130, 31);
        statusStrip.TabIndex = 1;
        // 
        // statusLabel
        // 
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(130, 24);
        statusLabel.Text = "正在生成预览...";
        // 
        // previewTimer
        // 
        previewTimer.Interval = 160;
        previewTimer.Tick += previewTimer_Tick;
        // 
        // saveFileDialog
        // 
        saveFileDialog.Title = "保存 RGB 相移图卡";
        // 
        // folderBrowserDialog
        // 
        folderBrowserDialog.Description = "选择 RGB 八步相移图卡的导出目录";
        folderBrowserDialog.UseDescriptionForTitle = true;
        // 
        // PhaseStripeForm
        // 
        AutoScaleDimensions = new SizeF(144F, 144F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(2130, 1350);
        Controls.Add(splitContainerMain);
        Controls.Add(statusStrip);
        Font = new Font("Microsoft YaHei UI", 9F);
        Margin = new Padding(4, 4, 4, 4);
        MinimumSize = new Size(1669, 1052);
        Name = "PhaseStripeForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "串扰图卡";
        FormClosing += PhaseStripeForm_FormClosing;
        FormClosed += PhaseStripeForm_FormClosed;
        splitContainerMain.Panel1.ResumeLayout(false);
        splitContainerMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
        splitContainerMain.ResumeLayout(false);
        settingsFlowPanel.ResumeLayout(false);
        groupCanvas.ResumeLayout(false);
        groupCanvas.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericCanvasHeight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericCanvasWidth).EndInit();
        groupRegion.ResumeLayout(false);
        groupRegion.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericPatternHeight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternWidth).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternY).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericPatternX).EndInit();
        groupPhase.ResumeLayout(false);
        groupPhase.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericPhase).EndInit();
        groupExport.ResumeLayout(false);
        groupExport.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericQuality).EndInit();
        groupActions.ResumeLayout(false);
        previewPanel.ResumeLayout(false);
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
    private GroupBox groupCanvas;
    private NumericUpDown numericCanvasHeight;
    private Label labelCanvasHeight;
    private NumericUpDown numericCanvasWidth;
    private Label labelCanvasWidth;
    private GroupBox groupRegion;
    private Button buttonCenter;
    private NumericUpDown numericPatternHeight;
    private Label labelPatternHeight;
    private NumericUpDown numericPatternWidth;
    private Label labelPatternWidth;
    private NumericUpDown numericPatternY;
    private Label labelPatternY;
    private NumericUpDown numericPatternX;
    private Label labelPatternX;
    private GroupBox groupPhase;
    private Label labelOrderHelp;
    private ComboBox comboPixelOrder;
    private Label labelPixelOrder;
    private NumericUpDown numericPhase;
    private Label labelPhase;
    private BorderOverlayEditor borderOverlayEditor;
    private GroupBox groupExport;
    private Label labelExportHelp;
    private NumericUpDown numericQuality;
    private Label labelQuality;
    private ComboBox comboOutputFormat;
    private Label labelOutputFormat;
    private GroupBox groupActions;
    private Button buttonNonIntegerFusion;
    private Button buttonBatchExport;
    private Button buttonSaveCurrent;
    private Button buttonPreview;
    private Panel previewPanel;
    private ImagePreviewControl previewControl;
    private Panel previewHeaderPanel;
    private Label labelPreviewInfo;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusLabel;
    private System.Windows.Forms.Timer previewTimer;
    private SaveFileDialog saveFileDialog;
    private FolderBrowserDialog folderBrowserDialog;
    private ToolTip toolTip;
}
