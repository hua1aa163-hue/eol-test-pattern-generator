using EolTestPatternGenerator.Controls;

namespace EolTestPatternGenerator;

#nullable disable

partial class NonIntegerFusionForm
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
        tabModes = new TabControl();
        tabContinuous = new TabPage();
        continuousFlowPanel = new FlowLayoutPanel();
        groupContinuousCanvas = new GroupBox();
        numericContinuousHeight = new NumericUpDown();
        labelContinuousHeight = new Label();
        numericContinuousWidth = new NumericUpDown();
        labelContinuousWidth = new Label();
        groupContinuousFusion = new GroupBox();
        checkContinuousReverse = new CheckBox();
        comboContinuousSourceMode = new ComboBox();
        labelContinuousSourceMode = new Label();
        numericContinuousDuty = new NumericUpDown();
        labelContinuousDuty = new Label();
        numericContinuousOffset = new NumericUpDown();
        labelContinuousOffset = new Label();
        numericContinuousMultiplier = new NumericUpDown();
        labelContinuousMultiplier = new Label();
        numericContinuousPeriod = new NumericUpDown();
        labelContinuousPeriod = new Label();
        numericContinuousAngle = new NumericUpDown();
        labelContinuousAngle = new Label();
        groupContinuousSources = new GroupBox();
        comboContinuousInputOrder = new ComboBox();
        labelContinuousInputOrder = new Label();
        buttonBrowseContinuousRight = new Button();
        textContinuousRightPath = new TextBox();
        labelContinuousRightPath = new Label();
        buttonBrowseContinuousLeft = new Button();
        textContinuousLeftPath = new TextBox();
        labelContinuousLeftPath = new Label();
        groupContinuousMetadata = new GroupBox();
        numericContinuousPixelHeight = new NumericUpDown();
        labelContinuousPixelHeight = new Label();
        numericContinuousPixelWidth = new NumericUpDown();
        labelContinuousPixelWidth = new Label();
        groupContinuousExport = new GroupBox();
        checkContinuousWriteMesh = new CheckBox();
        checkContinuousSaveSources = new CheckBox();
        textContinuousPrefix = new TextBox();
        labelContinuousPrefix = new Label();
        numericContinuousQuality = new NumericUpDown();
        labelContinuousQuality = new Label();
        comboContinuousFormat = new ComboBox();
        labelContinuousFormat = new Label();
        groupContinuousActions = new GroupBox();
        buttonContinuousBatch = new Button();
        buttonContinuousSave = new Button();
        buttonContinuousPreview = new Button();
        tabDiscrete = new TabPage();
        discreteFlowPanel = new FlowLayoutPanel();
        groupDiscreteCanvas = new GroupBox();
        numericDiscreteHeight = new NumericUpDown();
        labelDiscreteHeight = new Label();
        numericDiscreteWidth = new NumericUpDown();
        labelDiscreteWidth = new Label();
        groupDiscretePattern = new GroupBox();
        checkDiscretePositiveDirection = new CheckBox();
        numericDiscreteTranslation = new NumericUpDown();
        labelDiscreteTranslation = new Label();
        numericDiscreteStrategyAngle = new NumericUpDown();
        labelDiscreteStrategyAngle = new Label();
        numericDiscretePartitionWidth = new NumericUpDown();
        labelDiscretePartitionWidth = new Label();
        numericDiscreteGroup = new NumericUpDown();
        labelDiscreteGroup = new Label();
        numericDiscreteLitCount = new NumericUpDown();
        labelDiscreteLitCount = new Label();
        numericDiscretePeriod = new NumericUpDown();
        labelDiscretePeriod = new Label();
        numericDiscreteAngle = new NumericUpDown();
        labelDiscreteAngle = new Label();
        groupDiscreteMetadata = new GroupBox();
        numericDiscreteScreenSize = new NumericUpDown();
        labelDiscreteScreenSize = new Label();
        numericDiscretePixelY = new NumericUpDown();
        labelDiscretePixelY = new Label();
        numericDiscretePixelX = new NumericUpDown();
        labelDiscretePixelX = new Label();
        groupDiscreteSources = new GroupBox();
        buttonBrowseDiscreteB = new Button();
        textDiscreteSourceB = new TextBox();
        labelDiscreteSourceB = new Label();
        buttonBrowseDiscreteA = new Button();
        textDiscreteSourceA = new TextBox();
        labelDiscreteSourceA = new Label();
        comboDiscreteResultType = new ComboBox();
        labelDiscreteResultType = new Label();
        groupDiscreteExport = new GroupBox();
        checkDiscreteSingleSource = new CheckBox();
        numericDiscreteQuality = new NumericUpDown();
        labelDiscreteQuality = new Label();
        comboDiscreteFormat = new ComboBox();
        labelDiscreteFormat = new Label();
        groupDiscreteActions = new GroupBox();
        buttonDiscreteLightTools = new Button();
        buttonDiscreteBatch = new Button();
        buttonDiscreteSave = new Button();
        buttonDiscretePreview = new Button();
        tabImageConverter = new TabPage();
        converterFlowPanel = new FlowLayoutPanel();
        groupConverterSource = new GroupBox();
        buttonBrowseConverterImage = new Button();
        textConverterImagePath = new TextBox();
        labelConverterImagePath = new Label();
        groupConverterMetadata = new GroupBox();
        numericConverterPixelY = new NumericUpDown();
        labelConverterPixelY = new Label();
        numericConverterPixelX = new NumericUpDown();
        labelConverterPixelX = new Label();
        checkConverterSingleSource = new CheckBox();
        groupConverterActions = new GroupBox();
        buttonConverterExport = new Button();
        buttonConverterPreview = new Button();
        borderOverlayEditor = new BorderOverlayEditor();
        previewPanel = new Panel();
        previewControl = new ImagePreviewControl();
        previewHeaderPanel = new Panel();
        labelPreviewInfo = new Label();
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        previewTimer = new System.Windows.Forms.Timer(components);
        openImageDialog = new OpenFileDialog();
        saveFileDialog = new SaveFileDialog();
        folderBrowserDialog = new FolderBrowserDialog();
        toolTip = new ToolTip(components);
        ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
        splitContainerMain.Panel1.SuspendLayout();
        splitContainerMain.Panel2.SuspendLayout();
        splitContainerMain.SuspendLayout();
        settingsFlowPanel.SuspendLayout();
        tabModes.SuspendLayout();
        tabContinuous.SuspendLayout();
        continuousFlowPanel.SuspendLayout();
        groupContinuousCanvas.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericContinuousHeight).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousWidth).BeginInit();
        groupContinuousFusion.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericContinuousDuty).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousOffset).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousMultiplier).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousPeriod).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousAngle).BeginInit();
        groupContinuousSources.SuspendLayout();
        groupContinuousMetadata.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericContinuousPixelHeight).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousPixelWidth).BeginInit();
        groupContinuousExport.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericContinuousQuality).BeginInit();
        groupContinuousActions.SuspendLayout();
        tabDiscrete.SuspendLayout();
        discreteFlowPanel.SuspendLayout();
        groupDiscreteCanvas.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteHeight).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteWidth).BeginInit();
        groupDiscretePattern.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteTranslation).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteStrategyAngle).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscretePartitionWidth).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteGroup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteLitCount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscretePeriod).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteAngle).BeginInit();
        groupDiscreteMetadata.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteScreenSize).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscretePixelY).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscretePixelX).BeginInit();
        groupDiscreteSources.SuspendLayout();
        groupDiscreteExport.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteQuality).BeginInit();
        groupDiscreteActions.SuspendLayout();
        tabImageConverter.SuspendLayout();
        converterFlowPanel.SuspendLayout();
        groupConverterSource.SuspendLayout();
        groupConverterMetadata.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericConverterPixelY).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericConverterPixelX).BeginInit();
        groupConverterActions.SuspendLayout();
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
        splitContainerMain.Name = "splitContainerMain";
        splitContainerMain.Panel1.Controls.Add(settingsFlowPanel);
        splitContainerMain.Panel1MinSize = 600;
        splitContainerMain.Panel2.Controls.Add(previewPanel);
        splitContainerMain.Size = new Size(2250, 1349);
        splitContainerMain.SplitterDistance = 720;
        splitContainerMain.SplitterWidth = 6;
        splitContainerMain.TabIndex = 0;
        //
        // settingsFlowPanel
        //
        settingsFlowPanel.AutoScroll = true;
        settingsFlowPanel.Controls.Add(tabModes);
        settingsFlowPanel.Controls.Add(borderOverlayEditor);
        settingsFlowPanel.Dock = DockStyle.Fill;
        settingsFlowPanel.FlowDirection = FlowDirection.TopDown;
        settingsFlowPanel.Location = new Point(0, 0);
        settingsFlowPanel.Name = "settingsFlowPanel";
        settingsFlowPanel.Padding = new Padding(15, 15, 12, 24);
        settingsFlowPanel.Size = new Size(720, 1349);
        settingsFlowPanel.TabIndex = 0;
        settingsFlowPanel.WrapContents = false;
        //
        // tabModes
        //
        tabModes.Controls.Add(tabContinuous);
        tabModes.Controls.Add(tabDiscrete);
        tabModes.Controls.Add(tabImageConverter);
        tabModes.Location = new Point(19, 19);
        tabModes.Name = "tabModes";
        tabModes.SelectedIndex = 0;
        tabModes.Size = new Size(660, 960);
        tabModes.TabIndex = 0;
        tabModes.SelectedIndexChanged += tabModes_SelectedIndexChanged;
        //
        // tabContinuous
        //
        tabContinuous.Controls.Add(continuousFlowPanel);
        tabContinuous.Location = new Point(4, 33);
        tabContinuous.Name = "tabContinuous";
        tabContinuous.Padding = new Padding(3);
        tabContinuous.Size = new Size(652, 923);
        tabContinuous.TabIndex = 0;
        tabContinuous.Text = "非整数连续融合";
        tabContinuous.UseVisualStyleBackColor = true;
        //
        // continuousFlowPanel
        //
        continuousFlowPanel.AutoScroll = true;
        continuousFlowPanel.Controls.Add(groupContinuousCanvas);
        continuousFlowPanel.Controls.Add(groupContinuousFusion);
        continuousFlowPanel.Controls.Add(groupContinuousSources);
        continuousFlowPanel.Controls.Add(groupContinuousMetadata);
        continuousFlowPanel.Controls.Add(groupContinuousExport);
        continuousFlowPanel.Controls.Add(groupContinuousActions);
        continuousFlowPanel.Dock = DockStyle.Fill;
        continuousFlowPanel.FlowDirection = FlowDirection.TopDown;
        continuousFlowPanel.Location = new Point(3, 3);
        continuousFlowPanel.Name = "continuousFlowPanel";
        continuousFlowPanel.Padding = new Padding(10);
        continuousFlowPanel.Size = new Size(646, 917);
        continuousFlowPanel.TabIndex = 0;
        continuousFlowPanel.WrapContents = false;
        //
        // groupContinuousCanvas
        //
        groupContinuousCanvas.Controls.Add(numericContinuousHeight);
        groupContinuousCanvas.Controls.Add(labelContinuousHeight);
        groupContinuousCanvas.Controls.Add(numericContinuousWidth);
        groupContinuousCanvas.Controls.Add(labelContinuousWidth);
        groupContinuousCanvas.Location = new Point(13, 13);
        groupContinuousCanvas.Name = "groupContinuousCanvas";
        groupContinuousCanvas.Size = new Size(590, 110);
        groupContinuousCanvas.TabIndex = 0;
        groupContinuousCanvas.TabStop = false;
        groupContinuousCanvas.Text = "画布尺寸";
        //
        // numericContinuousHeight
        //
        numericContinuousHeight.Location = new Point(390, 44);
        numericContinuousHeight.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericContinuousHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericContinuousHeight.Name = "numericContinuousHeight";
        numericContinuousHeight.Size = new Size(160, 30);
        numericContinuousHeight.TabIndex = 3;
        numericContinuousHeight.Value = new decimal(new int[] { 1080, 0, 0, 0 });
        numericContinuousHeight.ValueChanged += ParameterChanged;
        //
        // labelContinuousHeight
        //
        labelContinuousHeight.AutoSize = true;
        labelContinuousHeight.Location = new Point(305, 48);
        labelContinuousHeight.Name = "labelContinuousHeight";
        labelContinuousHeight.Size = new Size(46, 24);
        labelContinuousHeight.TabIndex = 2;
        labelContinuousHeight.Text = "高度";
        //
        // numericContinuousWidth
        //
        numericContinuousWidth.Location = new Point(105, 44);
        numericContinuousWidth.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericContinuousWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericContinuousWidth.Name = "numericContinuousWidth";
        numericContinuousWidth.Size = new Size(160, 30);
        numericContinuousWidth.TabIndex = 1;
        numericContinuousWidth.Value = new decimal(new int[] { 1920, 0, 0, 0 });
        numericContinuousWidth.ValueChanged += ParameterChanged;
        //
        // labelContinuousWidth
        //
        labelContinuousWidth.AutoSize = true;
        labelContinuousWidth.Location = new Point(25, 48);
        labelContinuousWidth.Name = "labelContinuousWidth";
        labelContinuousWidth.Size = new Size(46, 24);
        labelContinuousWidth.TabIndex = 0;
        labelContinuousWidth.Text = "宽度";
        //
        // groupContinuousFusion
        //
        groupContinuousFusion.Controls.Add(checkContinuousReverse);
        groupContinuousFusion.Controls.Add(comboContinuousSourceMode);
        groupContinuousFusion.Controls.Add(labelContinuousSourceMode);
        groupContinuousFusion.Controls.Add(numericContinuousDuty);
        groupContinuousFusion.Controls.Add(labelContinuousDuty);
        groupContinuousFusion.Controls.Add(numericContinuousOffset);
        groupContinuousFusion.Controls.Add(labelContinuousOffset);
        groupContinuousFusion.Controls.Add(numericContinuousMultiplier);
        groupContinuousFusion.Controls.Add(labelContinuousMultiplier);
        groupContinuousFusion.Controls.Add(numericContinuousPeriod);
        groupContinuousFusion.Controls.Add(labelContinuousPeriod);
        groupContinuousFusion.Controls.Add(numericContinuousAngle);
        groupContinuousFusion.Controls.Add(labelContinuousAngle);
        groupContinuousFusion.Location = new Point(13, 129);
        groupContinuousFusion.Name = "groupContinuousFusion";
        groupContinuousFusion.Size = new Size(590, 315);
        groupContinuousFusion.TabIndex = 1;
        groupContinuousFusion.TabStop = false;
        groupContinuousFusion.Text = "MATLAB 兼容融合参数";
        //
        // checkContinuousReverse
        //
        checkContinuousReverse.AutoSize = true;
        checkContinuousReverse.Location = new Point(305, 264);
        checkContinuousReverse.Name = "checkContinuousReverse";
        checkContinuousReverse.Size = new Size(144, 28);
        checkContinuousReverse.TabIndex = 12;
        checkContinuousReverse.Text = "交换左右图源";
        checkContinuousReverse.UseVisualStyleBackColor = true;
        checkContinuousReverse.CheckedChanged += ParameterChanged;
        //
        // comboContinuousSourceMode
        //
        comboContinuousSourceMode.DropDownStyle = ComboBoxStyle.DropDownList;
        comboContinuousSourceMode.FormattingEnabled = true;
        comboContinuousSourceMode.Items.AddRange(new object[] { "全白 / 全黑", "固定 1 子像素", "可调点亮比例", "红 / 蓝", "自定义左右图片" });
        comboContinuousSourceMode.Location = new Point(160, 259);
        comboContinuousSourceMode.Name = "comboContinuousSourceMode";
        comboContinuousSourceMode.Size = new Size(135, 32);
        comboContinuousSourceMode.TabIndex = 11;
        comboContinuousSourceMode.SelectedIndexChanged += ParameterChanged;
        //
        // labelContinuousSourceMode
        //
        labelContinuousSourceMode.AutoSize = true;
        labelContinuousSourceMode.Location = new Point(25, 264);
        labelContinuousSourceMode.Name = "labelContinuousSourceMode";
        labelContinuousSourceMode.Size = new Size(82, 24);
        labelContinuousSourceMode.TabIndex = 10;
        labelContinuousSourceMode.Text = "图源模式";
        //
        // numericContinuousDuty
        //
        numericContinuousDuty.DecimalPlaces = 4;
        numericContinuousDuty.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
        numericContinuousDuty.Location = new Point(390, 199);
        numericContinuousDuty.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
        numericContinuousDuty.Name = "numericContinuousDuty";
        numericContinuousDuty.Size = new Size(160, 30);
        numericContinuousDuty.TabIndex = 9;
        numericContinuousDuty.Value = new decimal(new int[] { 25, 0, 0, 131072 });
        numericContinuousDuty.ValueChanged += ParameterChanged;
        //
        // labelContinuousDuty
        //
        labelContinuousDuty.AutoSize = true;
        labelContinuousDuty.Location = new Point(305, 203);
        labelContinuousDuty.Name = "labelContinuousDuty";
        labelContinuousDuty.Size = new Size(46, 24);
        labelContinuousDuty.TabIndex = 8;
        labelContinuousDuty.Text = "比例";
        //
        // numericContinuousOffset
        //
        numericContinuousOffset.DecimalPlaces = 4;
        numericContinuousOffset.Location = new Point(160, 199);
        numericContinuousOffset.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        numericContinuousOffset.Minimum = new decimal(new int[] { 100000, 0, 0, int.MinValue });
        numericContinuousOffset.Name = "numericContinuousOffset";
        numericContinuousOffset.Size = new Size(135, 30);
        numericContinuousOffset.TabIndex = 7;
        numericContinuousOffset.ValueChanged += ParameterChanged;
        //
        // labelContinuousOffset
        //
        labelContinuousOffset.AutoSize = true;
        labelContinuousOffset.Location = new Point(25, 203);
        labelContinuousOffset.Name = "labelContinuousOffset";
        labelContinuousOffset.Size = new Size(82, 24);
        labelContinuousOffset.TabIndex = 6;
        labelContinuousOffset.Text = "循环移位";
        //
        // numericContinuousMultiplier
        //
        numericContinuousMultiplier.DecimalPlaces = 6;
        numericContinuousMultiplier.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
        numericContinuousMultiplier.Location = new Point(390, 139);
        numericContinuousMultiplier.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        numericContinuousMultiplier.Minimum = new decimal(new int[] { 1, 0, 0, 393216 });
        numericContinuousMultiplier.Name = "numericContinuousMultiplier";
        numericContinuousMultiplier.Size = new Size(160, 30);
        numericContinuousMultiplier.TabIndex = 5;
        numericContinuousMultiplier.Value = new decimal(new int[] { 1, 0, 0, 0 });
        numericContinuousMultiplier.ValueChanged += ParameterChanged;
        //
        // labelContinuousMultiplier
        //
        labelContinuousMultiplier.AutoSize = true;
        labelContinuousMultiplier.Location = new Point(305, 143);
        labelContinuousMultiplier.Name = "labelContinuousMultiplier";
        labelContinuousMultiplier.Size = new Size(66, 24);
        labelContinuousMultiplier.TabIndex = 4;
        labelContinuousMultiplier.Text = "周期 m";
        //
        // numericContinuousPeriod
        //
        numericContinuousPeriod.DecimalPlaces = 6;
        numericContinuousPeriod.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
        numericContinuousPeriod.Location = new Point(160, 139);
        numericContinuousPeriod.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        numericContinuousPeriod.Minimum = new decimal(new int[] { 1, 0, 0, 393216 });
        numericContinuousPeriod.Name = "numericContinuousPeriod";
        numericContinuousPeriod.Size = new Size(135, 30);
        numericContinuousPeriod.TabIndex = 3;
        numericContinuousPeriod.Value = new decimal(new int[] { 8, 0, 0, 0 });
        numericContinuousPeriod.ValueChanged += ParameterChanged;
        //
        // labelContinuousPeriod
        //
        labelContinuousPeriod.AutoSize = true;
        labelContinuousPeriod.Location = new Point(25, 143);
        labelContinuousPeriod.Name = "labelContinuousPeriod";
        labelContinuousPeriod.Size = new Size(118, 24);
        labelContinuousPeriod.TabIndex = 2;
        labelContinuousPeriod.Text = "子像素周期";
        //
        // numericContinuousAngle
        //
        numericContinuousAngle.DecimalPlaces = 6;
        numericContinuousAngle.Increment = new decimal(new int[] { 1, 0, 0, 196608 });
        numericContinuousAngle.Location = new Point(160, 79);
        numericContinuousAngle.Maximum = new decimal(new int[] { 89, 0, 0, 0 });
        numericContinuousAngle.Minimum = new decimal(new int[] { 89, 0, 0, int.MinValue });
        numericContinuousAngle.Name = "numericContinuousAngle";
        numericContinuousAngle.Size = new Size(135, 30);
        numericContinuousAngle.TabIndex = 1;
        numericContinuousAngle.Value = new decimal(new int[] { 18435, 0, 0, 196608 });
        numericContinuousAngle.ValueChanged += ParameterChanged;
        //
        // labelContinuousAngle
        //
        labelContinuousAngle.AutoSize = true;
        labelContinuousAngle.Location = new Point(25, 83);
        labelContinuousAngle.Name = "labelContinuousAngle";
        labelContinuousAngle.Size = new Size(100, 24);
        labelContinuousAngle.TabIndex = 0;
        labelContinuousAngle.Text = "倾斜角(°)";
        //
        // groupContinuousSources
        //
        groupContinuousSources.Controls.Add(comboContinuousInputOrder);
        groupContinuousSources.Controls.Add(labelContinuousInputOrder);
        groupContinuousSources.Controls.Add(buttonBrowseContinuousRight);
        groupContinuousSources.Controls.Add(textContinuousRightPath);
        groupContinuousSources.Controls.Add(labelContinuousRightPath);
        groupContinuousSources.Controls.Add(buttonBrowseContinuousLeft);
        groupContinuousSources.Controls.Add(textContinuousLeftPath);
        groupContinuousSources.Controls.Add(labelContinuousLeftPath);
        groupContinuousSources.Location = new Point(13, 450);
        groupContinuousSources.Name = "groupContinuousSources";
        groupContinuousSources.Size = new Size(590, 225);
        groupContinuousSources.TabIndex = 2;
        groupContinuousSources.TabStop = false;
        groupContinuousSources.Text = "自定义图源与输入通道";
        //
        // comboContinuousInputOrder
        //
        comboContinuousInputOrder.DropDownStyle = ComboBoxStyle.DropDownList;
        comboContinuousInputOrder.FormattingEnabled = true;
        comboContinuousInputOrder.Items.AddRange(new object[] { "正常 RGB", "兼容旧 MATLAB（交换 R/B）" });
        comboContinuousInputOrder.Location = new Point(160, 164);
        comboContinuousInputOrder.Name = "comboContinuousInputOrder";
        comboContinuousInputOrder.Size = new Size(390, 32);
        comboContinuousInputOrder.TabIndex = 7;
        comboContinuousInputOrder.SelectedIndexChanged += ParameterChanged;
        //
        // labelContinuousInputOrder
        //
        labelContinuousInputOrder.AutoSize = true;
        labelContinuousInputOrder.Location = new Point(25, 169);
        labelContinuousInputOrder.Name = "labelContinuousInputOrder";
        labelContinuousInputOrder.Size = new Size(82, 24);
        labelContinuousInputOrder.TabIndex = 6;
        labelContinuousInputOrder.Text = "通道顺序";
        //
        // buttonBrowseContinuousRight
        //
        buttonBrowseContinuousRight.Location = new Point(488, 104);
        buttonBrowseContinuousRight.Name = "buttonBrowseContinuousRight";
        buttonBrowseContinuousRight.Size = new Size(62, 36);
        buttonBrowseContinuousRight.TabIndex = 5;
        buttonBrowseContinuousRight.Text = "...";
        buttonBrowseContinuousRight.UseVisualStyleBackColor = true;
        buttonBrowseContinuousRight.Click += buttonBrowseContinuousRight_Click;
        //
        // textContinuousRightPath
        //
        textContinuousRightPath.Location = new Point(160, 106);
        textContinuousRightPath.Name = "textContinuousRightPath";
        textContinuousRightPath.Size = new Size(315, 30);
        textContinuousRightPath.TabIndex = 4;
        textContinuousRightPath.TextChanged += ParameterChanged;
        //
        // labelContinuousRightPath
        //
        labelContinuousRightPath.AutoSize = true;
        labelContinuousRightPath.Location = new Point(25, 110);
        labelContinuousRightPath.Name = "labelContinuousRightPath";
        labelContinuousRightPath.Size = new Size(88, 24);
        labelContinuousRightPath.TabIndex = 3;
        labelContinuousRightPath.Text = "右图 / B";
        //
        // buttonBrowseContinuousLeft
        //
        buttonBrowseContinuousLeft.Location = new Point(488, 47);
        buttonBrowseContinuousLeft.Name = "buttonBrowseContinuousLeft";
        buttonBrowseContinuousLeft.Size = new Size(62, 36);
        buttonBrowseContinuousLeft.TabIndex = 2;
        buttonBrowseContinuousLeft.Text = "...";
        buttonBrowseContinuousLeft.UseVisualStyleBackColor = true;
        buttonBrowseContinuousLeft.Click += buttonBrowseContinuousLeft_Click;
        //
        // textContinuousLeftPath
        //
        textContinuousLeftPath.Location = new Point(160, 49);
        textContinuousLeftPath.Name = "textContinuousLeftPath";
        textContinuousLeftPath.Size = new Size(315, 30);
        textContinuousLeftPath.TabIndex = 1;
        textContinuousLeftPath.TextChanged += ParameterChanged;
        //
        // labelContinuousLeftPath
        //
        labelContinuousLeftPath.AutoSize = true;
        labelContinuousLeftPath.Location = new Point(25, 53);
        labelContinuousLeftPath.Name = "labelContinuousLeftPath";
        labelContinuousLeftPath.Size = new Size(84, 24);
        labelContinuousLeftPath.TabIndex = 0;
        labelContinuousLeftPath.Text = "左图 / A";
        //
        // groupContinuousMetadata
        //
        groupContinuousMetadata.Controls.Add(numericContinuousPixelHeight);
        groupContinuousMetadata.Controls.Add(labelContinuousPixelHeight);
        groupContinuousMetadata.Controls.Add(numericContinuousPixelWidth);
        groupContinuousMetadata.Controls.Add(labelContinuousPixelWidth);
        groupContinuousMetadata.Location = new Point(13, 681);
        groupContinuousMetadata.Name = "groupContinuousMetadata";
        groupContinuousMetadata.Size = new Size(590, 120);
        groupContinuousMetadata.TabIndex = 3;
        groupContinuousMetadata.TabStop = false;
        groupContinuousMetadata.Text = "LightTools 物理尺寸（仅用于 MESH）";
        //
        // numericContinuousPixelHeight
        //
        numericContinuousPixelHeight.DecimalPlaces = 6;
        numericContinuousPixelHeight.Location = new Point(390, 55);
        numericContinuousPixelHeight.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        numericContinuousPixelHeight.Minimum = new decimal(new int[] { 1, 0, 0, 393216 });
        numericContinuousPixelHeight.Name = "numericContinuousPixelHeight";
        numericContinuousPixelHeight.Size = new Size(160, 30);
        numericContinuousPixelHeight.TabIndex = 3;
        numericContinuousPixelHeight.Value = new decimal(new int[] { 576, 0, 0, 262144 });
        numericContinuousPixelHeight.ValueChanged += ExportOptionChanged;
        //
        // labelContinuousPixelHeight
        //
        labelContinuousPixelHeight.AutoSize = true;
        labelContinuousPixelHeight.Location = new Point(305, 59);
        labelContinuousPixelHeight.Name = "labelContinuousPixelHeight";
        labelContinuousPixelHeight.Size = new Size(73, 24);
        labelContinuousPixelHeight.TabIndex = 2;
        labelContinuousPixelHeight.Text = "pixH (mm)";
        //
        // numericContinuousPixelWidth
        //
        numericContinuousPixelWidth.DecimalPlaces = 6;
        numericContinuousPixelWidth.Location = new Point(105, 55);
        numericContinuousPixelWidth.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        numericContinuousPixelWidth.Minimum = new decimal(new int[] { 1, 0, 0, 393216 });
        numericContinuousPixelWidth.Name = "numericContinuousPixelWidth";
        numericContinuousPixelWidth.Size = new Size(160, 30);
        numericContinuousPixelWidth.TabIndex = 1;
        numericContinuousPixelWidth.Value = new decimal(new int[] { 192, 0, 0, 262144 });
        numericContinuousPixelWidth.ValueChanged += ExportOptionChanged;
        //
        // labelContinuousPixelWidth
        //
        labelContinuousPixelWidth.AutoSize = true;
        labelContinuousPixelWidth.Location = new Point(25, 59);
        labelContinuousPixelWidth.Name = "labelContinuousPixelWidth";
        labelContinuousPixelWidth.Size = new Size(74, 24);
        labelContinuousPixelWidth.TabIndex = 0;
        labelContinuousPixelWidth.Text = "pixL (mm)";
        //
        // groupContinuousExport
        //
        groupContinuousExport.Controls.Add(checkContinuousWriteMesh);
        groupContinuousExport.Controls.Add(checkContinuousSaveSources);
        groupContinuousExport.Controls.Add(textContinuousPrefix);
        groupContinuousExport.Controls.Add(labelContinuousPrefix);
        groupContinuousExport.Controls.Add(numericContinuousQuality);
        groupContinuousExport.Controls.Add(labelContinuousQuality);
        groupContinuousExport.Controls.Add(comboContinuousFormat);
        groupContinuousExport.Controls.Add(labelContinuousFormat);
        groupContinuousExport.Location = new Point(13, 807);
        groupContinuousExport.Name = "groupContinuousExport";
        groupContinuousExport.Size = new Size(590, 220);
        groupContinuousExport.TabIndex = 4;
        groupContinuousExport.TabStop = false;
        groupContinuousExport.Text = "图像与融合结果输出";
        //
        // checkContinuousWriteMesh
        //
        checkContinuousWriteMesh.AutoSize = true;
        checkContinuousWriteMesh.Checked = true;
        checkContinuousWriteMesh.CheckState = CheckState.Checked;
        checkContinuousWriteMesh.Location = new Point(305, 169);
        checkContinuousWriteMesh.Name = "checkContinuousWriteMesh";
        checkContinuousWriteMesh.Size = new Size(173, 28);
        checkContinuousWriteMesh.TabIndex = 7;
        checkContinuousWriteMesh.Text = "同时输出 MESH TXT";
        checkContinuousWriteMesh.UseVisualStyleBackColor = true;
        checkContinuousWriteMesh.CheckedChanged += ExportOptionChanged;
        //
        // checkContinuousSaveSources
        //
        checkContinuousSaveSources.AutoSize = true;
        checkContinuousSaveSources.Location = new Point(25, 169);
        checkContinuousSaveSources.Name = "checkContinuousSaveSources";
        checkContinuousSaveSources.Size = new Size(162, 28);
        checkContinuousSaveSources.TabIndex = 6;
        checkContinuousSaveSources.Text = "同时保存左右图源";
        checkContinuousSaveSources.UseVisualStyleBackColor = true;
        checkContinuousSaveSources.CheckedChanged += ExportOptionChanged;
        //
        // textContinuousPrefix
        //
        textContinuousPrefix.Location = new Point(160, 112);
        textContinuousPrefix.Name = "textContinuousPrefix";
        textContinuousPrefix.Size = new Size(390, 30);
        textContinuousPrefix.TabIndex = 5;
        textContinuousPrefix.Text = "fused";
        textContinuousPrefix.TextChanged += ExportOptionChanged;
        //
        // labelContinuousPrefix
        //
        labelContinuousPrefix.AutoSize = true;
        labelContinuousPrefix.Location = new Point(25, 116);
        labelContinuousPrefix.Name = "labelContinuousPrefix";
        labelContinuousPrefix.Size = new Size(82, 24);
        labelContinuousPrefix.TabIndex = 4;
        labelContinuousPrefix.Text = "文件前缀";
        //
        // numericContinuousQuality
        //
        numericContinuousQuality.Location = new Point(390, 52);
        numericContinuousQuality.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericContinuousQuality.Name = "numericContinuousQuality";
        numericContinuousQuality.Size = new Size(160, 30);
        numericContinuousQuality.TabIndex = 3;
        numericContinuousQuality.Value = new decimal(new int[] { 95, 0, 0, 0 });
        numericContinuousQuality.ValueChanged += ExportOptionChanged;
        //
        // labelContinuousQuality
        //
        labelContinuousQuality.AutoSize = true;
        labelContinuousQuality.Location = new Point(305, 56);
        labelContinuousQuality.Name = "labelContinuousQuality";
        labelContinuousQuality.Size = new Size(46, 24);
        labelContinuousQuality.TabIndex = 2;
        labelContinuousQuality.Text = "质量";
        //
        // comboContinuousFormat
        //
        comboContinuousFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        comboContinuousFormat.FormattingEnabled = true;
        comboContinuousFormat.Items.AddRange(new object[] { "PNG", "JPEG", "BMP", "TIFF", "WebP" });
        comboContinuousFormat.Location = new Point(160, 50);
        comboContinuousFormat.Name = "comboContinuousFormat";
        comboContinuousFormat.Size = new Size(135, 32);
        comboContinuousFormat.TabIndex = 1;
        comboContinuousFormat.SelectedIndexChanged += ExportOptionChanged;
        //
        // labelContinuousFormat
        //
        labelContinuousFormat.AutoSize = true;
        labelContinuousFormat.Location = new Point(25, 55);
        labelContinuousFormat.Name = "labelContinuousFormat";
        labelContinuousFormat.Size = new Size(82, 24);
        labelContinuousFormat.TabIndex = 0;
        labelContinuousFormat.Text = "图像格式";
        //
        // groupContinuousActions
        //
        groupContinuousActions.Controls.Add(buttonContinuousBatch);
        groupContinuousActions.Controls.Add(buttonContinuousSave);
        groupContinuousActions.Controls.Add(buttonContinuousPreview);
        groupContinuousActions.Location = new Point(13, 1033);
        groupContinuousActions.Name = "groupContinuousActions";
        groupContinuousActions.Size = new Size(590, 160);
        groupContinuousActions.TabIndex = 5;
        groupContinuousActions.TabStop = false;
        groupContinuousActions.Text = "预览与导出";
        //
        // buttonContinuousBatch
        //
        buttonContinuousBatch.Location = new Point(305, 94);
        buttonContinuousBatch.Name = "buttonContinuousBatch";
        buttonContinuousBatch.Size = new Size(245, 45);
        buttonContinuousBatch.TabIndex = 2;
        buttonContinuousBatch.Text = "批量融合 _L / _R 文件夹...";
        buttonContinuousBatch.UseVisualStyleBackColor = true;
        buttonContinuousBatch.Click += buttonContinuousBatch_Click;
        //
        // buttonContinuousSave
        //
        buttonContinuousSave.Location = new Point(25, 94);
        buttonContinuousSave.Name = "buttonContinuousSave";
        buttonContinuousSave.Size = new Size(245, 45);
        buttonContinuousSave.TabIndex = 1;
        buttonContinuousSave.Text = "保存当前融合图...";
        buttonContinuousSave.UseVisualStyleBackColor = true;
        buttonContinuousSave.Click += buttonContinuousSave_Click;
        //
        // buttonContinuousPreview
        //
        buttonContinuousPreview.Location = new Point(25, 38);
        buttonContinuousPreview.Name = "buttonContinuousPreview";
        buttonContinuousPreview.Size = new Size(525, 45);
        buttonContinuousPreview.TabIndex = 0;
        buttonContinuousPreview.Text = "刷新连续融合预览";
        buttonContinuousPreview.UseVisualStyleBackColor = true;
        buttonContinuousPreview.Click += buttonContinuousPreview_Click;
        //
        // tabDiscrete
        //
        tabDiscrete.Controls.Add(discreteFlowPanel);
        tabDiscrete.Location = new Point(4, 33);
        tabDiscrete.Name = "tabDiscrete";
        tabDiscrete.Padding = new Padding(3);
        tabDiscrete.Size = new Size(652, 923);
        tabDiscrete.TabIndex = 1;
        tabDiscrete.Text = "离散光源图";
        tabDiscrete.UseVisualStyleBackColor = true;
        //
        // discreteFlowPanel
        //
        discreteFlowPanel.AutoScroll = true;
        discreteFlowPanel.Controls.Add(groupDiscreteCanvas);
        discreteFlowPanel.Controls.Add(groupDiscretePattern);
        discreteFlowPanel.Controls.Add(groupDiscreteMetadata);
        discreteFlowPanel.Controls.Add(groupDiscreteSources);
        discreteFlowPanel.Controls.Add(groupDiscreteExport);
        discreteFlowPanel.Controls.Add(groupDiscreteActions);
        discreteFlowPanel.Dock = DockStyle.Fill;
        discreteFlowPanel.FlowDirection = FlowDirection.TopDown;
        discreteFlowPanel.Location = new Point(3, 3);
        discreteFlowPanel.Name = "discreteFlowPanel";
        discreteFlowPanel.Padding = new Padding(10);
        discreteFlowPanel.Size = new Size(646, 917);
        discreteFlowPanel.TabIndex = 0;
        discreteFlowPanel.WrapContents = false;
        //
        // groupDiscreteCanvas
        //
        groupDiscreteCanvas.Controls.Add(numericDiscreteHeight);
        groupDiscreteCanvas.Controls.Add(labelDiscreteHeight);
        groupDiscreteCanvas.Controls.Add(numericDiscreteWidth);
        groupDiscreteCanvas.Controls.Add(labelDiscreteWidth);
        groupDiscreteCanvas.Location = new Point(13, 13);
        groupDiscreteCanvas.Name = "groupDiscreteCanvas";
        groupDiscreteCanvas.Size = new Size(590, 110);
        groupDiscreteCanvas.TabIndex = 0;
        groupDiscreteCanvas.TabStop = false;
        groupDiscreteCanvas.Text = "画布尺寸";
        //
        // numericDiscreteHeight
        //
        numericDiscreteHeight.Location = new Point(390, 44);
        numericDiscreteHeight.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericDiscreteHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericDiscreteHeight.Name = "numericDiscreteHeight";
        numericDiscreteHeight.Size = new Size(160, 30);
        numericDiscreteHeight.TabIndex = 3;
        numericDiscreteHeight.Value = new decimal(new int[] { 1080, 0, 0, 0 });
        numericDiscreteHeight.ValueChanged += ParameterChanged;
        //
        // labelDiscreteHeight
        //
        labelDiscreteHeight.AutoSize = true;
        labelDiscreteHeight.Location = new Point(305, 48);
        labelDiscreteHeight.Name = "labelDiscreteHeight";
        labelDiscreteHeight.Size = new Size(46, 24);
        labelDiscreteHeight.TabIndex = 2;
        labelDiscreteHeight.Text = "高度";
        //
        // numericDiscreteWidth
        //
        numericDiscreteWidth.Location = new Point(105, 44);
        numericDiscreteWidth.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericDiscreteWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericDiscreteWidth.Name = "numericDiscreteWidth";
        numericDiscreteWidth.Size = new Size(160, 30);
        numericDiscreteWidth.TabIndex = 1;
        numericDiscreteWidth.Value = new decimal(new int[] { 1920, 0, 0, 0 });
        numericDiscreteWidth.ValueChanged += ParameterChanged;
        //
        // labelDiscreteWidth
        //
        labelDiscreteWidth.AutoSize = true;
        labelDiscreteWidth.Location = new Point(25, 48);
        labelDiscreteWidth.Name = "labelDiscreteWidth";
        labelDiscreteWidth.Size = new Size(46, 24);
        labelDiscreteWidth.TabIndex = 0;
        labelDiscreteWidth.Text = "宽度";
        //
        // groupDiscretePattern
        //
        groupDiscretePattern.Controls.Add(checkDiscretePositiveDirection);
        groupDiscretePattern.Controls.Add(numericDiscreteTranslation);
        groupDiscretePattern.Controls.Add(labelDiscreteTranslation);
        groupDiscretePattern.Controls.Add(numericDiscreteStrategyAngle);
        groupDiscretePattern.Controls.Add(labelDiscreteStrategyAngle);
        groupDiscretePattern.Controls.Add(numericDiscretePartitionWidth);
        groupDiscretePattern.Controls.Add(labelDiscretePartitionWidth);
        groupDiscretePattern.Controls.Add(numericDiscreteGroup);
        groupDiscretePattern.Controls.Add(labelDiscreteGroup);
        groupDiscretePattern.Controls.Add(numericDiscreteLitCount);
        groupDiscretePattern.Controls.Add(labelDiscreteLitCount);
        groupDiscretePattern.Controls.Add(numericDiscretePeriod);
        groupDiscretePattern.Controls.Add(labelDiscretePeriod);
        groupDiscretePattern.Controls.Add(numericDiscreteAngle);
        groupDiscretePattern.Controls.Add(labelDiscreteAngle);
        groupDiscretePattern.Location = new Point(13, 129);
        groupDiscretePattern.Name = "groupDiscretePattern";
        groupDiscretePattern.Size = new Size(590, 350);
        groupDiscretePattern.TabIndex = 1;
        groupDiscretePattern.TabStop = false;
        groupDiscretePattern.Text = "离散周期、分区与方向";
        //
        // checkDiscretePositiveDirection
        //
        checkDiscretePositiveDirection.AutoSize = true;
        checkDiscretePositiveDirection.Checked = true;
        checkDiscretePositiveDirection.CheckState = CheckState.Checked;
        checkDiscretePositiveDirection.Location = new Point(305, 291);
        checkDiscretePositiveDirection.Name = "checkDiscretePositiveDirection";
        checkDiscretePositiveDirection.Size = new Size(196, 28);
        checkDiscretePositiveDirection.TabIndex = 14;
        checkDiscretePositiveDirection.Text = "方向：默认左侧 +1";
        checkDiscretePositiveDirection.UseVisualStyleBackColor = true;
        checkDiscretePositiveDirection.CheckedChanged += ParameterChanged;
        //
        // numericDiscreteTranslation
        //
        numericDiscreteTranslation.DecimalPlaces = 4;
        numericDiscreteTranslation.Location = new Point(160, 286);
        numericDiscreteTranslation.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        numericDiscreteTranslation.Minimum = new decimal(new int[] { 100000, 0, 0, int.MinValue });
        numericDiscreteTranslation.Name = "numericDiscreteTranslation";
        numericDiscreteTranslation.Size = new Size(135, 30);
        numericDiscreteTranslation.TabIndex = 13;
        numericDiscreteTranslation.ValueChanged += ParameterChanged;
        //
        // labelDiscreteTranslation
        //
        labelDiscreteTranslation.AutoSize = true;
        labelDiscreteTranslation.Location = new Point(25, 290);
        labelDiscreteTranslation.Name = "labelDiscreteTranslation";
        labelDiscreteTranslation.Size = new Size(100, 24);
        labelDiscreteTranslation.TabIndex = 12;
        labelDiscreteTranslation.Text = "平移量(px)";
        //
        // numericDiscreteStrategyAngle
        //
        numericDiscreteStrategyAngle.DecimalPlaces = 4;
        numericDiscreteStrategyAngle.Location = new Point(390, 226);
        numericDiscreteStrategyAngle.Maximum = new decimal(new int[] { 89, 0, 0, 0 });
        numericDiscreteStrategyAngle.Minimum = new decimal(new int[] { 89, 0, 0, int.MinValue });
        numericDiscreteStrategyAngle.Name = "numericDiscreteStrategyAngle";
        numericDiscreteStrategyAngle.Size = new Size(160, 30);
        numericDiscreteStrategyAngle.TabIndex = 11;
        numericDiscreteStrategyAngle.ValueChanged += ParameterChanged;
        //
        // labelDiscreteStrategyAngle
        //
        labelDiscreteStrategyAngle.AutoSize = true;
        labelDiscreteStrategyAngle.Location = new Point(305, 230);
        labelDiscreteStrategyAngle.Name = "labelDiscreteStrategyAngle";
        labelDiscreteStrategyAngle.Size = new Size(76, 24);
        labelDiscreteStrategyAngle.TabIndex = 10;
        labelDiscreteStrategyAngle.Text = "策略角°";
        //
        // numericDiscretePartitionWidth
        //
        numericDiscretePartitionWidth.Location = new Point(160, 226);
        numericDiscretePartitionWidth.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
        numericDiscretePartitionWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericDiscretePartitionWidth.Name = "numericDiscretePartitionWidth";
        numericDiscretePartitionWidth.Size = new Size(135, 30);
        numericDiscretePartitionWidth.TabIndex = 9;
        numericDiscretePartitionWidth.Value = new decimal(new int[] { 5760, 0, 0, 0 });
        numericDiscretePartitionWidth.ValueChanged += ParameterChanged;
        //
        // labelDiscretePartitionWidth
        //
        labelDiscretePartitionWidth.AutoSize = true;
        labelDiscretePartitionWidth.Location = new Point(25, 230);
        labelDiscretePartitionWidth.Name = "labelDiscretePartitionWidth";
        labelDiscretePartitionWidth.Size = new Size(118, 24);
        labelDiscretePartitionWidth.TabIndex = 8;
        labelDiscretePartitionWidth.Text = "分区宽度(sp)";
        //
        // numericDiscreteGroup
        //
        numericDiscreteGroup.Location = new Point(390, 166);
        numericDiscreteGroup.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        numericDiscreteGroup.Minimum = new decimal(new int[] { 1, 0, 0, int.MinValue });
        numericDiscreteGroup.Name = "numericDiscreteGroup";
        numericDiscreteGroup.Size = new Size(160, 30);
        numericDiscreteGroup.TabIndex = 7;
        numericDiscreteGroup.ValueChanged += ParameterChanged;
        //
        // labelDiscreteGroup
        //
        labelDiscreteGroup.AutoSize = true;
        labelDiscreteGroup.Location = new Point(305, 170);
        labelDiscreteGroup.Name = "labelDiscreteGroup";
        labelDiscreteGroup.Size = new Size(68, 24);
        labelDiscreteGroup.TabIndex = 6;
        labelDiscreteGroup.Text = "组(-1全)";
        //
        // numericDiscreteLitCount
        //
        numericDiscreteLitCount.Location = new Point(160, 166);
        numericDiscreteLitCount.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
        numericDiscreteLitCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericDiscreteLitCount.Name = "numericDiscreteLitCount";
        numericDiscreteLitCount.Size = new Size(135, 30);
        numericDiscreteLitCount.TabIndex = 5;
        numericDiscreteLitCount.Value = new decimal(new int[] { 4, 0, 0, 0 });
        numericDiscreteLitCount.ValueChanged += ParameterChanged;
        //
        // labelDiscreteLitCount
        //
        labelDiscreteLitCount.AutoSize = true;
        labelDiscreteLitCount.Location = new Point(25, 170);
        labelDiscreteLitCount.Name = "labelDiscreteLitCount";
        labelDiscreteLitCount.Size = new Size(82, 24);
        labelDiscreteLitCount.TabIndex = 4;
        labelDiscreteLitCount.Text = "点亮颗数";
        //
        // numericDiscretePeriod
        //
        numericDiscretePeriod.Location = new Point(390, 106);
        numericDiscretePeriod.Maximum = new decimal(new int[] { 4096, 0, 0, 0 });
        numericDiscretePeriod.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericDiscretePeriod.Name = "numericDiscretePeriod";
        numericDiscretePeriod.Size = new Size(160, 30);
        numericDiscretePeriod.TabIndex = 3;
        numericDiscretePeriod.Value = new decimal(new int[] { 8, 0, 0, 0 });
        numericDiscretePeriod.ValueChanged += ParameterChanged;
        //
        // labelDiscretePeriod
        //
        labelDiscretePeriod.AutoSize = true;
        labelDiscretePeriod.Location = new Point(305, 110);
        labelDiscretePeriod.Name = "labelDiscretePeriod";
        labelDiscretePeriod.Size = new Size(46, 24);
        labelDiscretePeriod.TabIndex = 2;
        labelDiscretePeriod.Text = "周期";
        //
        // numericDiscreteAngle
        //
        numericDiscreteAngle.DecimalPlaces = 4;
        numericDiscreteAngle.Location = new Point(160, 106);
        numericDiscreteAngle.Maximum = new decimal(new int[] { 89, 0, 0, 0 });
        numericDiscreteAngle.Minimum = new decimal(new int[] { 89, 0, 0, int.MinValue });
        numericDiscreteAngle.Name = "numericDiscreteAngle";
        numericDiscreteAngle.Size = new Size(135, 30);
        numericDiscreteAngle.TabIndex = 1;
        numericDiscreteAngle.Value = new decimal(new int[] { 18435, 0, 0, 196608 });
        numericDiscreteAngle.ValueChanged += ParameterChanged;
        //
        // labelDiscreteAngle
        //
        labelDiscreteAngle.AutoSize = true;
        labelDiscreteAngle.Location = new Point(25, 110);
        labelDiscreteAngle.Name = "labelDiscreteAngle";
        labelDiscreteAngle.Size = new Size(100, 24);
        labelDiscreteAngle.TabIndex = 0;
        labelDiscreteAngle.Text = "倾斜角(°)";
        //
        // groupDiscreteMetadata
        //
        groupDiscreteMetadata.Controls.Add(numericDiscreteScreenSize);
        groupDiscreteMetadata.Controls.Add(labelDiscreteScreenSize);
        groupDiscreteMetadata.Controls.Add(numericDiscretePixelY);
        groupDiscreteMetadata.Controls.Add(labelDiscretePixelY);
        groupDiscreteMetadata.Controls.Add(numericDiscretePixelX);
        groupDiscreteMetadata.Controls.Add(labelDiscretePixelX);
        groupDiscreteMetadata.Location = new Point(13, 485);
        groupDiscreteMetadata.Name = "groupDiscreteMetadata";
        groupDiscreteMetadata.Size = new Size(590, 175);
        groupDiscreteMetadata.TabIndex = 2;
        groupDiscreteMetadata.TabStop = false;
        groupDiscreteMetadata.Text = "屏幕与物理尺寸";
        //
        // numericDiscreteScreenSize
        //
        numericDiscreteScreenSize.DecimalPlaces = 2;
        numericDiscreteScreenSize.Location = new Point(160, 111);
        numericDiscreteScreenSize.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        numericDiscreteScreenSize.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
        numericDiscreteScreenSize.Name = "numericDiscreteScreenSize";
        numericDiscreteScreenSize.Size = new Size(135, 30);
        numericDiscreteScreenSize.TabIndex = 5;
        numericDiscreteScreenSize.Value = new decimal(new int[] { 5, 0, 0, 0 });
        numericDiscreteScreenSize.ValueChanged += ExportOptionChanged;
        //
        // labelDiscreteScreenSize
        //
        labelDiscreteScreenSize.AutoSize = true;
        labelDiscreteScreenSize.Location = new Point(25, 115);
        labelDiscreteScreenSize.Name = "labelDiscreteScreenSize";
        labelDiscreteScreenSize.Size = new Size(112, 24);
        labelDiscreteScreenSize.TabIndex = 4;
        labelDiscreteScreenSize.Text = "屏幕尺寸(in)";
        //
        // numericDiscretePixelY
        //
        numericDiscretePixelY.DecimalPlaces = 4;
        numericDiscretePixelY.Location = new Point(390, 51);
        numericDiscretePixelY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        numericDiscretePixelY.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
        numericDiscretePixelY.Name = "numericDiscretePixelY";
        numericDiscretePixelY.Size = new Size(160, 30);
        numericDiscretePixelY.TabIndex = 3;
        numericDiscretePixelY.Value = new decimal(new int[] { 576, 0, 0, 65536 });
        numericDiscretePixelY.ValueChanged += ExportOptionChanged;
        //
        // labelDiscretePixelY
        //
        labelDiscretePixelY.AutoSize = true;
        labelDiscretePixelY.Location = new Point(305, 55);
        labelDiscretePixelY.Name = "labelDiscretePixelY";
        labelDiscretePixelY.Size = new Size(64, 24);
        labelDiscretePixelY.TabIndex = 2;
        labelDiscretePixelY.Text = "Y (μm)";
        //
        // numericDiscretePixelX
        //
        numericDiscretePixelX.DecimalPlaces = 4;
        numericDiscretePixelX.Location = new Point(105, 51);
        numericDiscretePixelX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        numericDiscretePixelX.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
        numericDiscretePixelX.Name = "numericDiscretePixelX";
        numericDiscretePixelX.Size = new Size(160, 30);
        numericDiscretePixelX.TabIndex = 1;
        numericDiscretePixelX.Value = new decimal(new int[] { 576, 0, 0, 65536 });
        numericDiscretePixelX.ValueChanged += ExportOptionChanged;
        //
        // labelDiscretePixelX
        //
        labelDiscretePixelX.AutoSize = true;
        labelDiscretePixelX.Location = new Point(25, 55);
        labelDiscretePixelX.Name = "labelDiscretePixelX";
        labelDiscretePixelX.Size = new Size(65, 24);
        labelDiscretePixelX.TabIndex = 0;
        labelDiscretePixelX.Text = "X (μm)";
        //
        // groupDiscreteSources
        //
        groupDiscreteSources.Controls.Add(buttonBrowseDiscreteB);
        groupDiscreteSources.Controls.Add(textDiscreteSourceB);
        groupDiscreteSources.Controls.Add(labelDiscreteSourceB);
        groupDiscreteSources.Controls.Add(buttonBrowseDiscreteA);
        groupDiscreteSources.Controls.Add(textDiscreteSourceA);
        groupDiscreteSources.Controls.Add(labelDiscreteSourceA);
        groupDiscreteSources.Controls.Add(comboDiscreteResultType);
        groupDiscreteSources.Controls.Add(labelDiscreteResultType);
        groupDiscreteSources.Location = new Point(13, 666);
        groupDiscreteSources.Name = "groupDiscreteSources";
        groupDiscreteSources.Size = new Size(590, 225);
        groupDiscreteSources.TabIndex = 3;
        groupDiscreteSources.TabStop = false;
        groupDiscreteSources.Text = "结果类型与自定义图源";
        //
        // buttonBrowseDiscreteB
        //
        buttonBrowseDiscreteB.Location = new Point(488, 164);
        buttonBrowseDiscreteB.Name = "buttonBrowseDiscreteB";
        buttonBrowseDiscreteB.Size = new Size(62, 36);
        buttonBrowseDiscreteB.TabIndex = 7;
        buttonBrowseDiscreteB.Text = "...";
        buttonBrowseDiscreteB.UseVisualStyleBackColor = true;
        buttonBrowseDiscreteB.Click += buttonBrowseDiscreteB_Click;
        //
        // textDiscreteSourceB
        //
        textDiscreteSourceB.Location = new Point(160, 166);
        textDiscreteSourceB.Name = "textDiscreteSourceB";
        textDiscreteSourceB.Size = new Size(315, 30);
        textDiscreteSourceB.TabIndex = 6;
        textDiscreteSourceB.TextChanged += ParameterChanged;
        //
        // labelDiscreteSourceB
        //
        labelDiscreteSourceB.AutoSize = true;
        labelDiscreteSourceB.Location = new Point(25, 170);
        labelDiscreteSourceB.Name = "labelDiscreteSourceB";
        labelDiscreteSourceB.Size = new Size(62, 24);
        labelDiscreteSourceB.TabIndex = 5;
        labelDiscreteSourceB.Text = "图源 B";
        //
        // buttonBrowseDiscreteA
        //
        buttonBrowseDiscreteA.Location = new Point(488, 107);
        buttonBrowseDiscreteA.Name = "buttonBrowseDiscreteA";
        buttonBrowseDiscreteA.Size = new Size(62, 36);
        buttonBrowseDiscreteA.TabIndex = 4;
        buttonBrowseDiscreteA.Text = "...";
        buttonBrowseDiscreteA.UseVisualStyleBackColor = true;
        buttonBrowseDiscreteA.Click += buttonBrowseDiscreteA_Click;
        //
        // textDiscreteSourceA
        //
        textDiscreteSourceA.Location = new Point(160, 109);
        textDiscreteSourceA.Name = "textDiscreteSourceA";
        textDiscreteSourceA.Size = new Size(315, 30);
        textDiscreteSourceA.TabIndex = 3;
        textDiscreteSourceA.TextChanged += ParameterChanged;
        //
        // labelDiscreteSourceA
        //
        labelDiscreteSourceA.AutoSize = true;
        labelDiscreteSourceA.Location = new Point(25, 113);
        labelDiscreteSourceA.Name = "labelDiscreteSourceA";
        labelDiscreteSourceA.Size = new Size(63, 24);
        labelDiscreteSourceA.TabIndex = 2;
        labelDiscreteSourceA.Text = "图源 A";
        //
        // comboDiscreteResultType
        //
        comboDiscreteResultType.DropDownStyle = ComboBoxStyle.DropDownList;
        comboDiscreteResultType.FormattingEnabled = true;
        comboDiscreteResultType.Items.AddRange(new object[] { "白黑", "黑白", "红蓝", "自定义" });
        comboDiscreteResultType.Location = new Point(160, 49);
        comboDiscreteResultType.Name = "comboDiscreteResultType";
        comboDiscreteResultType.Size = new Size(390, 32);
        comboDiscreteResultType.TabIndex = 1;
        comboDiscreteResultType.SelectedIndexChanged += ParameterChanged;
        //
        // labelDiscreteResultType
        //
        labelDiscreteResultType.AutoSize = true;
        labelDiscreteResultType.Location = new Point(25, 54);
        labelDiscreteResultType.Name = "labelDiscreteResultType";
        labelDiscreteResultType.Size = new Size(82, 24);
        labelDiscreteResultType.TabIndex = 0;
        labelDiscreteResultType.Text = "结果类型";
        //
        // groupDiscreteExport
        //
        groupDiscreteExport.Controls.Add(checkDiscreteSingleSource);
        groupDiscreteExport.Controls.Add(numericDiscreteQuality);
        groupDiscreteExport.Controls.Add(labelDiscreteQuality);
        groupDiscreteExport.Controls.Add(comboDiscreteFormat);
        groupDiscreteExport.Controls.Add(labelDiscreteFormat);
        groupDiscreteExport.Location = new Point(13, 897);
        groupDiscreteExport.Name = "groupDiscreteExport";
        groupDiscreteExport.Size = new Size(590, 150);
        groupDiscreteExport.TabIndex = 4;
        groupDiscreteExport.TabStop = false;
        groupDiscreteExport.Text = "图像和 LightTools 输出";
        //
        // checkDiscreteSingleSource
        //
        checkDiscreteSingleSource.AutoSize = true;
        checkDiscreteSingleSource.Checked = true;
        checkDiscreteSingleSource.CheckState = CheckState.Checked;
        checkDiscreteSingleSource.Location = new Point(25, 103);
        checkDiscreteSingleSource.Name = "checkDiscreteSingleSource";
        checkDiscreteSingleSource.Size = new Size(144, 28);
        checkDiscreteSingleSource.TabIndex = 4;
        checkDiscreteSingleSource.Text = "单光源 TXT";
        checkDiscreteSingleSource.UseVisualStyleBackColor = true;
        checkDiscreteSingleSource.CheckedChanged += ExportOptionChanged;
        //
        // numericDiscreteQuality
        //
        numericDiscreteQuality.Location = new Point(390, 49);
        numericDiscreteQuality.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericDiscreteQuality.Name = "numericDiscreteQuality";
        numericDiscreteQuality.Size = new Size(160, 30);
        numericDiscreteQuality.TabIndex = 3;
        numericDiscreteQuality.Value = new decimal(new int[] { 95, 0, 0, 0 });
        numericDiscreteQuality.ValueChanged += ExportOptionChanged;
        //
        // labelDiscreteQuality
        //
        labelDiscreteQuality.AutoSize = true;
        labelDiscreteQuality.Location = new Point(305, 53);
        labelDiscreteQuality.Name = "labelDiscreteQuality";
        labelDiscreteQuality.Size = new Size(46, 24);
        labelDiscreteQuality.TabIndex = 2;
        labelDiscreteQuality.Text = "质量";
        //
        // comboDiscreteFormat
        //
        comboDiscreteFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        comboDiscreteFormat.FormattingEnabled = true;
        comboDiscreteFormat.Items.AddRange(new object[] { "PNG", "JPEG", "BMP", "TIFF", "WebP" });
        comboDiscreteFormat.Location = new Point(160, 47);
        comboDiscreteFormat.Name = "comboDiscreteFormat";
        comboDiscreteFormat.Size = new Size(135, 32);
        comboDiscreteFormat.TabIndex = 1;
        comboDiscreteFormat.SelectedIndexChanged += ExportOptionChanged;
        //
        // labelDiscreteFormat
        //
        labelDiscreteFormat.AutoSize = true;
        labelDiscreteFormat.Location = new Point(25, 52);
        labelDiscreteFormat.Name = "labelDiscreteFormat";
        labelDiscreteFormat.Size = new Size(82, 24);
        labelDiscreteFormat.TabIndex = 0;
        labelDiscreteFormat.Text = "图像格式";
        //
        // groupDiscreteActions
        //
        groupDiscreteActions.Controls.Add(buttonDiscreteLightTools);
        groupDiscreteActions.Controls.Add(buttonDiscreteBatch);
        groupDiscreteActions.Controls.Add(buttonDiscreteSave);
        groupDiscreteActions.Controls.Add(buttonDiscretePreview);
        groupDiscreteActions.Location = new Point(13, 1053);
        groupDiscreteActions.Name = "groupDiscreteActions";
        groupDiscreteActions.Size = new Size(590, 210);
        groupDiscreteActions.TabIndex = 5;
        groupDiscreteActions.TabStop = false;
        groupDiscreteActions.Text = "预览与导出";
        //
        // buttonDiscreteLightTools
        //
        buttonDiscreteLightTools.Location = new Point(25, 150);
        buttonDiscreteLightTools.Name = "buttonDiscreteLightTools";
        buttonDiscreteLightTools.Size = new Size(525, 45);
        buttonDiscreteLightTools.TabIndex = 3;
        buttonDiscreteLightTools.Text = "导出 LightTools 光源 TXT...";
        buttonDiscreteLightTools.UseVisualStyleBackColor = true;
        buttonDiscreteLightTools.Click += buttonDiscreteLightTools_Click;
        //
        // buttonDiscreteBatch
        //
        buttonDiscreteBatch.Location = new Point(305, 94);
        buttonDiscreteBatch.Name = "buttonDiscreteBatch";
        buttonDiscreteBatch.Size = new Size(245, 45);
        buttonDiscreteBatch.TabIndex = 2;
        buttonDiscreteBatch.Text = "按组批量导出图像...";
        buttonDiscreteBatch.UseVisualStyleBackColor = true;
        buttonDiscreteBatch.Click += buttonDiscreteBatch_Click;
        //
        // buttonDiscreteSave
        //
        buttonDiscreteSave.Location = new Point(25, 94);
        buttonDiscreteSave.Name = "buttonDiscreteSave";
        buttonDiscreteSave.Size = new Size(245, 45);
        buttonDiscreteSave.TabIndex = 1;
        buttonDiscreteSave.Text = "保存当前组图像...";
        buttonDiscreteSave.UseVisualStyleBackColor = true;
        buttonDiscreteSave.Click += buttonDiscreteSave_Click;
        //
        // buttonDiscretePreview
        //
        buttonDiscretePreview.Location = new Point(25, 38);
        buttonDiscretePreview.Name = "buttonDiscretePreview";
        buttonDiscretePreview.Size = new Size(525, 45);
        buttonDiscretePreview.TabIndex = 0;
        buttonDiscretePreview.Text = "刷新离散光源预览";
        buttonDiscretePreview.UseVisualStyleBackColor = true;
        buttonDiscretePreview.Click += buttonDiscretePreview_Click;
        //
        // tabImageConverter
        //
        tabImageConverter.Controls.Add(converterFlowPanel);
        tabImageConverter.Location = new Point(4, 33);
        tabImageConverter.Name = "tabImageConverter";
        tabImageConverter.Padding = new Padding(3);
        tabImageConverter.Size = new Size(652, 923);
        tabImageConverter.TabIndex = 2;
        tabImageConverter.Text = "图片转 LightTools";
        tabImageConverter.UseVisualStyleBackColor = true;
        //
        // converterFlowPanel
        //
        converterFlowPanel.AutoScroll = true;
        converterFlowPanel.Controls.Add(groupConverterSource);
        converterFlowPanel.Controls.Add(groupConverterMetadata);
        converterFlowPanel.Controls.Add(groupConverterActions);
        converterFlowPanel.Dock = DockStyle.Fill;
        converterFlowPanel.FlowDirection = FlowDirection.TopDown;
        converterFlowPanel.Location = new Point(3, 3);
        converterFlowPanel.Name = "converterFlowPanel";
        converterFlowPanel.Padding = new Padding(10);
        converterFlowPanel.Size = new Size(646, 917);
        converterFlowPanel.TabIndex = 0;
        converterFlowPanel.WrapContents = false;
        //
        // groupConverterSource
        //
        groupConverterSource.Controls.Add(buttonBrowseConverterImage);
        groupConverterSource.Controls.Add(textConverterImagePath);
        groupConverterSource.Controls.Add(labelConverterImagePath);
        groupConverterSource.Location = new Point(13, 13);
        groupConverterSource.Name = "groupConverterSource";
        groupConverterSource.Size = new Size(590, 130);
        groupConverterSource.TabIndex = 0;
        groupConverterSource.TabStop = false;
        groupConverterSource.Text = "源图片（JPEG / BMP / PNG / TIFF）";
        //
        // buttonBrowseConverterImage
        //
        buttonBrowseConverterImage.Location = new Point(488, 55);
        buttonBrowseConverterImage.Name = "buttonBrowseConverterImage";
        buttonBrowseConverterImage.Size = new Size(62, 36);
        buttonBrowseConverterImage.TabIndex = 2;
        buttonBrowseConverterImage.Text = "...";
        buttonBrowseConverterImage.UseVisualStyleBackColor = true;
        buttonBrowseConverterImage.Click += buttonBrowseConverterImage_Click;
        //
        // textConverterImagePath
        //
        textConverterImagePath.Location = new Point(115, 57);
        textConverterImagePath.Name = "textConverterImagePath";
        textConverterImagePath.Size = new Size(360, 30);
        textConverterImagePath.TabIndex = 1;
        textConverterImagePath.TextChanged += ParameterChanged;
        //
        // labelConverterImagePath
        //
        labelConverterImagePath.AutoSize = true;
        labelConverterImagePath.Location = new Point(25, 61);
        labelConverterImagePath.Name = "labelConverterImagePath";
        labelConverterImagePath.Size = new Size(64, 24);
        labelConverterImagePath.TabIndex = 0;
        labelConverterImagePath.Text = "源图片";
        //
        // groupConverterMetadata
        //
        groupConverterMetadata.Controls.Add(numericConverterPixelY);
        groupConverterMetadata.Controls.Add(labelConverterPixelY);
        groupConverterMetadata.Controls.Add(numericConverterPixelX);
        groupConverterMetadata.Controls.Add(labelConverterPixelX);
        groupConverterMetadata.Controls.Add(checkConverterSingleSource);
        groupConverterMetadata.Location = new Point(13, 149);
        groupConverterMetadata.Name = "groupConverterMetadata";
        groupConverterMetadata.Size = new Size(590, 180);
        groupConverterMetadata.TabIndex = 1;
        groupConverterMetadata.TabStop = false;
        groupConverterMetadata.Text = "LightTools 参数";
        //
        // numericConverterPixelY
        //
        numericConverterPixelY.DecimalPlaces = 4;
        numericConverterPixelY.Location = new Point(390, 52);
        numericConverterPixelY.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        numericConverterPixelY.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
        numericConverterPixelY.Name = "numericConverterPixelY";
        numericConverterPixelY.Size = new Size(160, 30);
        numericConverterPixelY.TabIndex = 3;
        numericConverterPixelY.Value = new decimal(new int[] { 576, 0, 0, 65536 });
        numericConverterPixelY.ValueChanged += ExportOptionChanged;
        //
        // labelConverterPixelY
        //
        labelConverterPixelY.AutoSize = true;
        labelConverterPixelY.Location = new Point(305, 56);
        labelConverterPixelY.Name = "labelConverterPixelY";
        labelConverterPixelY.Size = new Size(64, 24);
        labelConverterPixelY.TabIndex = 2;
        labelConverterPixelY.Text = "Y (μm)";
        //
        // numericConverterPixelX
        //
        numericConverterPixelX.DecimalPlaces = 4;
        numericConverterPixelX.Location = new Point(105, 52);
        numericConverterPixelX.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        numericConverterPixelX.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
        numericConverterPixelX.Name = "numericConverterPixelX";
        numericConverterPixelX.Size = new Size(160, 30);
        numericConverterPixelX.TabIndex = 1;
        numericConverterPixelX.Value = new decimal(new int[] { 576, 0, 0, 65536 });
        numericConverterPixelX.ValueChanged += ExportOptionChanged;
        //
        // labelConverterPixelX
        //
        labelConverterPixelX.AutoSize = true;
        labelConverterPixelX.Location = new Point(25, 56);
        labelConverterPixelX.Name = "labelConverterPixelX";
        labelConverterPixelX.Size = new Size(65, 24);
        labelConverterPixelX.TabIndex = 0;
        labelConverterPixelX.Text = "X (μm)";
        //
        // checkConverterSingleSource
        //
        checkConverterSingleSource.AutoSize = true;
        checkConverterSingleSource.Checked = true;
        checkConverterSingleSource.CheckState = CheckState.Checked;
        checkConverterSingleSource.Location = new Point(25, 115);
        checkConverterSingleSource.Name = "checkConverterSingleSource";
        checkConverterSingleSource.Size = new Size(214, 28);
        checkConverterSingleSource.TabIndex = 4;
        checkConverterSingleSource.Text = "输出单个子像素光源 TXT";
        checkConverterSingleSource.UseVisualStyleBackColor = true;
        checkConverterSingleSource.CheckedChanged += ExportOptionChanged;
        //
        // groupConverterActions
        //
        groupConverterActions.Controls.Add(buttonConverterExport);
        groupConverterActions.Controls.Add(buttonConverterPreview);
        groupConverterActions.Location = new Point(13, 335);
        groupConverterActions.Name = "groupConverterActions";
        groupConverterActions.Size = new Size(590, 150);
        groupConverterActions.TabIndex = 2;
        groupConverterActions.TabStop = false;
        groupConverterActions.Text = "预览与转换";
        //
        // buttonConverterExport
        //
        buttonConverterExport.Location = new Point(305, 50);
        buttonConverterExport.Name = "buttonConverterExport";
        buttonConverterExport.Size = new Size(245, 55);
        buttonConverterExport.TabIndex = 1;
        buttonConverterExport.Text = "转换为 LightTools TXT...";
        buttonConverterExport.UseVisualStyleBackColor = true;
        buttonConverterExport.Click += buttonConverterExport_Click;
        //
        // buttonConverterPreview
        //
        buttonConverterPreview.Location = new Point(25, 50);
        buttonConverterPreview.Name = "buttonConverterPreview";
        buttonConverterPreview.Size = new Size(245, 55);
        buttonConverterPreview.TabIndex = 0;
        buttonConverterPreview.Text = "预览源图片";
        buttonConverterPreview.UseVisualStyleBackColor = true;
        buttonConverterPreview.Click += buttonConverterPreview_Click;
        //
        // borderOverlayEditor
        //
        borderOverlayEditor.CanvasSize = new Size(1920, 1080);
        borderOverlayEditor.Location = new Point(19, 986);
        borderOverlayEditor.Name = "borderOverlayEditor";
        borderOverlayEditor.Size = new Size(650, 305);
        borderOverlayEditor.TabIndex = 1;
        borderOverlayEditor.SettingsChanged += borderOverlayEditor_SettingsChanged;
        //
        // previewPanel
        //
        previewPanel.Controls.Add(previewControl);
        previewPanel.Controls.Add(previewHeaderPanel);
        previewPanel.Dock = DockStyle.Fill;
        previewPanel.Location = new Point(0, 0);
        previewPanel.Name = "previewPanel";
        previewPanel.Size = new Size(1524, 1349);
        previewPanel.TabIndex = 0;
        //
        // previewControl
        //
        previewControl.BackColor = SystemColors.Control;
        previewControl.Dock = DockStyle.Fill;
        previewControl.Location = new Point(0, 66);
        previewControl.Name = "previewControl";
        previewControl.ShowCenterCrosshair = true;
        previewControl.ShowPixelCoordinates = true;
        previewControl.Size = new Size(1524, 1283);
        previewControl.TabIndex = 1;
        //
        // previewHeaderPanel
        //
        previewHeaderPanel.Controls.Add(labelPreviewInfo);
        previewHeaderPanel.Dock = DockStyle.Top;
        previewHeaderPanel.Location = new Point(0, 0);
        previewHeaderPanel.Name = "previewHeaderPanel";
        previewHeaderPanel.Padding = new Padding(20, 18, 20, 12);
        previewHeaderPanel.Size = new Size(1524, 66);
        previewHeaderPanel.TabIndex = 0;
        //
        // labelPreviewInfo
        //
        labelPreviewInfo.AutoSize = true;
        labelPreviewInfo.Location = new Point(20, 21);
        labelPreviewInfo.Name = "labelPreviewInfo";
        labelPreviewInfo.Size = new Size(190, 24);
        labelPreviewInfo.TabIndex = 0;
        labelPreviewInfo.Text = "预览尚未生成";
        //
        // statusStrip
        //
        statusStrip.ImageScalingSize = new Size(24, 24);
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 1349);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(2250, 31);
        statusStrip.TabIndex = 1;
        //
        // statusLabel
        //
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(46, 24);
        statusLabel.Text = "就绪";
        //
        // previewTimer
        //
        previewTimer.Interval = 350;
        previewTimer.Tick += previewTimer_Tick;
        //
        // openImageDialog
        //
        openImageDialog.Filter = "图像文件|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff;*.webp|所有文件|*.*";
        openImageDialog.Title = "选择图像";
        //
        // folderBrowserDialog
        //
        folderBrowserDialog.Description = "选择输出目录";
        //
        // NonIntegerFusionForm
        //
        AutoScaleDimensions = new SizeF(144F, 144F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(2250, 1380);
        Controls.Add(splitContainerMain);
        Controls.Add(statusStrip);
        MinimumSize = new Size(1500, 1000);
        Name = "NonIntegerFusionForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "非整数融合与 LightTools 光源工具（C#）";
        FormClosing += NonIntegerFusionForm_FormClosing;
        FormClosed += NonIntegerFusionForm_FormClosed;
        splitContainerMain.Panel1.ResumeLayout(false);
        splitContainerMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
        splitContainerMain.ResumeLayout(false);
        settingsFlowPanel.ResumeLayout(false);
        tabModes.ResumeLayout(false);
        tabContinuous.ResumeLayout(false);
        continuousFlowPanel.ResumeLayout(false);
        groupContinuousCanvas.ResumeLayout(false);
        groupContinuousCanvas.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericContinuousHeight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousWidth).EndInit();
        groupContinuousFusion.ResumeLayout(false);
        groupContinuousFusion.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericContinuousDuty).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousOffset).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousMultiplier).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousPeriod).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousAngle).EndInit();
        groupContinuousSources.ResumeLayout(false);
        groupContinuousSources.PerformLayout();
        groupContinuousMetadata.ResumeLayout(false);
        groupContinuousMetadata.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericContinuousPixelHeight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericContinuousPixelWidth).EndInit();
        groupContinuousExport.ResumeLayout(false);
        groupContinuousExport.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericContinuousQuality).EndInit();
        groupContinuousActions.ResumeLayout(false);
        tabDiscrete.ResumeLayout(false);
        discreteFlowPanel.ResumeLayout(false);
        groupDiscreteCanvas.ResumeLayout(false);
        groupDiscreteCanvas.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteHeight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteWidth).EndInit();
        groupDiscretePattern.ResumeLayout(false);
        groupDiscretePattern.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteTranslation).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteStrategyAngle).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscretePartitionWidth).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteGroup).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteLitCount).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscretePeriod).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteAngle).EndInit();
        groupDiscreteMetadata.ResumeLayout(false);
        groupDiscreteMetadata.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteScreenSize).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscretePixelY).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericDiscretePixelX).EndInit();
        groupDiscreteSources.ResumeLayout(false);
        groupDiscreteSources.PerformLayout();
        groupDiscreteExport.ResumeLayout(false);
        groupDiscreteExport.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericDiscreteQuality).EndInit();
        groupDiscreteActions.ResumeLayout(false);
        tabImageConverter.ResumeLayout(false);
        converterFlowPanel.ResumeLayout(false);
        groupConverterSource.ResumeLayout(false);
        groupConverterSource.PerformLayout();
        groupConverterMetadata.ResumeLayout(false);
        groupConverterMetadata.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericConverterPixelY).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericConverterPixelX).EndInit();
        groupConverterActions.ResumeLayout(false);
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
    private TabControl tabModes;
    private TabPage tabContinuous;
    private FlowLayoutPanel continuousFlowPanel;
    private GroupBox groupContinuousCanvas;
    private NumericUpDown numericContinuousHeight;
    private Label labelContinuousHeight;
    private NumericUpDown numericContinuousWidth;
    private Label labelContinuousWidth;
    private GroupBox groupContinuousFusion;
    private CheckBox checkContinuousReverse;
    private ComboBox comboContinuousSourceMode;
    private Label labelContinuousSourceMode;
    private NumericUpDown numericContinuousDuty;
    private Label labelContinuousDuty;
    private NumericUpDown numericContinuousOffset;
    private Label labelContinuousOffset;
    private NumericUpDown numericContinuousMultiplier;
    private Label labelContinuousMultiplier;
    private NumericUpDown numericContinuousPeriod;
    private Label labelContinuousPeriod;
    private NumericUpDown numericContinuousAngle;
    private Label labelContinuousAngle;
    private GroupBox groupContinuousSources;
    private ComboBox comboContinuousInputOrder;
    private Label labelContinuousInputOrder;
    private Button buttonBrowseContinuousRight;
    private TextBox textContinuousRightPath;
    private Label labelContinuousRightPath;
    private Button buttonBrowseContinuousLeft;
    private TextBox textContinuousLeftPath;
    private Label labelContinuousLeftPath;
    private GroupBox groupContinuousMetadata;
    private NumericUpDown numericContinuousPixelHeight;
    private Label labelContinuousPixelHeight;
    private NumericUpDown numericContinuousPixelWidth;
    private Label labelContinuousPixelWidth;
    private GroupBox groupContinuousExport;
    private CheckBox checkContinuousWriteMesh;
    private CheckBox checkContinuousSaveSources;
    private TextBox textContinuousPrefix;
    private Label labelContinuousPrefix;
    private NumericUpDown numericContinuousQuality;
    private Label labelContinuousQuality;
    private ComboBox comboContinuousFormat;
    private Label labelContinuousFormat;
    private GroupBox groupContinuousActions;
    private Button buttonContinuousBatch;
    private Button buttonContinuousSave;
    private Button buttonContinuousPreview;
    private TabPage tabDiscrete;
    private FlowLayoutPanel discreteFlowPanel;
    private GroupBox groupDiscreteCanvas;
    private NumericUpDown numericDiscreteHeight;
    private Label labelDiscreteHeight;
    private NumericUpDown numericDiscreteWidth;
    private Label labelDiscreteWidth;
    private GroupBox groupDiscretePattern;
    private CheckBox checkDiscretePositiveDirection;
    private NumericUpDown numericDiscreteTranslation;
    private Label labelDiscreteTranslation;
    private NumericUpDown numericDiscreteStrategyAngle;
    private Label labelDiscreteStrategyAngle;
    private NumericUpDown numericDiscretePartitionWidth;
    private Label labelDiscretePartitionWidth;
    private NumericUpDown numericDiscreteGroup;
    private Label labelDiscreteGroup;
    private NumericUpDown numericDiscreteLitCount;
    private Label labelDiscreteLitCount;
    private NumericUpDown numericDiscretePeriod;
    private Label labelDiscretePeriod;
    private NumericUpDown numericDiscreteAngle;
    private Label labelDiscreteAngle;
    private GroupBox groupDiscreteMetadata;
    private NumericUpDown numericDiscreteScreenSize;
    private Label labelDiscreteScreenSize;
    private NumericUpDown numericDiscretePixelY;
    private Label labelDiscretePixelY;
    private NumericUpDown numericDiscretePixelX;
    private Label labelDiscretePixelX;
    private GroupBox groupDiscreteSources;
    private Button buttonBrowseDiscreteB;
    private TextBox textDiscreteSourceB;
    private Label labelDiscreteSourceB;
    private Button buttonBrowseDiscreteA;
    private TextBox textDiscreteSourceA;
    private Label labelDiscreteSourceA;
    private ComboBox comboDiscreteResultType;
    private Label labelDiscreteResultType;
    private GroupBox groupDiscreteExport;
    private CheckBox checkDiscreteSingleSource;
    private NumericUpDown numericDiscreteQuality;
    private Label labelDiscreteQuality;
    private ComboBox comboDiscreteFormat;
    private Label labelDiscreteFormat;
    private GroupBox groupDiscreteActions;
    private Button buttonDiscreteLightTools;
    private Button buttonDiscreteBatch;
    private Button buttonDiscreteSave;
    private Button buttonDiscretePreview;
    private TabPage tabImageConverter;
    private FlowLayoutPanel converterFlowPanel;
    private GroupBox groupConverterSource;
    private Button buttonBrowseConverterImage;
    private TextBox textConverterImagePath;
    private Label labelConverterImagePath;
    private GroupBox groupConverterMetadata;
    private NumericUpDown numericConverterPixelY;
    private Label labelConverterPixelY;
    private NumericUpDown numericConverterPixelX;
    private Label labelConverterPixelX;
    private CheckBox checkConverterSingleSource;
    private GroupBox groupConverterActions;
    private Button buttonConverterExport;
    private Button buttonConverterPreview;
    private BorderOverlayEditor borderOverlayEditor;
    private Panel previewPanel;
    private ImagePreviewControl previewControl;
    private Panel previewHeaderPanel;
    private Label labelPreviewInfo;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusLabel;
    private System.Windows.Forms.Timer previewTimer;
    private OpenFileDialog openImageDialog;
    private SaveFileDialog saveFileDialog;
    private FolderBrowserDialog folderBrowserDialog;
    private ToolTip toolTip;
}
