using EolTestPatternGenerator.Controls;

namespace EolTestPatternGenerator;

#nullable disable

partial class CrosstalkGridForm
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
        regionMarginsEditor = new RegionMarginsEditor();
        groupPhase = new GroupBox();
        cycleEditor = new CrosstalkPixelGridEditor();
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
        checkTwoInOne = new CheckBox();
        buttonBatchTwoInOneFolder = new Button();
        buttonResetDefaults = new Button();
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
        splitContainerMain.Name = "splitContainerMain";
        splitContainerMain.Panel1.Controls.Add(settingsFlowPanel);
        splitContainerMain.Panel1MinSize = 405;
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
        groupCanvas.Name = "groupCanvas";
        groupCanvas.Size = new Size(550, 159);
        groupCanvas.TabIndex = 0;
        groupCanvas.TabStop = false;
        groupCanvas.Text = "画布尺寸";
        //
        // numericCanvasHeight
        //
        numericCanvasHeight.Location = new Point(168, 99);
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
        labelCanvasHeight.Name = "labelCanvasHeight";
        labelCanvasHeight.Size = new Size(82, 24);
        labelCanvasHeight.TabIndex = 2;
        labelCanvasHeight.Text = "画布高度";
        //
        // numericCanvasWidth
        //
        numericCanvasWidth.Location = new Point(168, 42);
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
        labelCanvasWidth.Name = "labelCanvasWidth";
        labelCanvasWidth.Size = new Size(82, 24);
        labelCanvasWidth.TabIndex = 0;
        labelCanvasWidth.Text = "画布宽度";
        //
        // groupRegion
        //
        groupRegion.Controls.Add(regionMarginsEditor);
        groupRegion.Location = new Point(19, 185);
        groupRegion.Name = "groupRegion";
        groupRegion.Size = new Size(550, 320);
        groupRegion.TabIndex = 1;
        groupRegion.TabStop = false;
        groupRegion.Text = "图案外缘四边距";
        //
        // regionMarginsEditor
        //
        regionMarginsEditor.CanvasSize = new Size(1920, 1080);
        regionMarginsEditor.Location = new Point(27, 38);
        regionMarginsEditor.Name = "regionMarginsEditor";
        regionMarginsEditor.Size = new Size(496, 256);
        regionMarginsEditor.TabIndex = 0;
        regionMarginsEditor.MarginsChanged += regionMarginsEditor_MarginsChanged;
        //
        // groupPhase
        //
        groupPhase.Controls.Add(cycleEditor);
        groupPhase.Controls.Add(numericPhase);
        groupPhase.Controls.Add(labelPhase);
        groupPhase.Location = new Point(19, 512);
        groupPhase.Name = "groupPhase";
        groupPhase.Size = new Size(550, 790);
        groupPhase.TabIndex = 2;
        groupPhase.TabStop = false;
        groupPhase.Text = "相位与水平周期像素";
        //
        // cycleEditor
        //
        cycleEditor.Location = new Point(27, 94);
        cycleEditor.Name = "cycleEditor";
        cycleEditor.Size = new Size(496, 657);
        cycleEditor.TabIndex = 2;
        cycleEditor.SettingsChanged += cycleEditor_SettingsChanged;
        //
        // numericPhase
        //
        numericPhase.Location = new Point(168, 42);
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
        labelPhase.Name = "labelPhase";
        labelPhase.Size = new Size(88, 24);
        labelPhase.TabIndex = 0;
        labelPhase.Text = "当前相位";
        //
        // borderOverlayEditor
        //
        borderOverlayEditor.CanvasSize = new Size(1920, 1080);
        borderOverlayEditor.Location = new Point(21, 1309);
        borderOverlayEditor.Margin = new Padding(6);
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
        groupExport.Location = new Point(19, 1664);
        groupExport.Name = "groupExport";
        groupExport.Size = new Size(550, 208);
        groupExport.TabIndex = 4;
        groupExport.TabStop = false;
        groupExport.Text = "导出设置";
        //
        // labelExportHelp
        //
        labelExportHelp.ForeColor = Color.DimGray;
        labelExportHelp.Location = new Point(27, 153);
        labelExportHelp.Name = "labelExportHelp";
        labelExportHelp.Size = new Size(496, 45);
        labelExportHelp.TabIndex = 4;
        labelExportHelp.Text = "PNG、BMP、TIFF 可保持精确的 0/255 像素。";
        //
        // numericQuality
        //
        numericQuality.Location = new Point(168, 99);
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
        comboOutputFormat.Name = "comboOutputFormat";
        comboOutputFormat.Size = new Size(354, 32);
        comboOutputFormat.TabIndex = 1;
        comboOutputFormat.SelectedIndexChanged += comboOutputFormat_SelectedIndexChanged;
        //
        // labelOutputFormat
        //
        labelOutputFormat.AutoSize = true;
        labelOutputFormat.Location = new Point(27, 48);
        labelOutputFormat.Name = "labelOutputFormat";
        labelOutputFormat.Size = new Size(82, 24);
        labelOutputFormat.TabIndex = 0;
        labelOutputFormat.Text = "图片格式";
        //
        // groupActions
        //
        groupActions.Controls.Add(buttonBatchTwoInOneFolder);
        groupActions.Controls.Add(buttonResetDefaults);
        groupActions.Controls.Add(checkTwoInOne);
        groupActions.Controls.Add(buttonBatchExport);
        groupActions.Controls.Add(buttonSaveCurrent);
        groupActions.Controls.Add(buttonPreview);
        groupActions.Location = new Point(19, 1879);
        groupActions.Name = "groupActions";
        groupActions.Size = new Size(550, 283);
        groupActions.TabIndex = 5;
        groupActions.TabStop = false;
        groupActions.Text = "生成与保存";
        //
        // checkTwoInOne
        //
        checkTwoInOne.Location = new Point(27, 39);
        checkTwoInOne.Name = "checkTwoInOne";
        checkTwoInOne.Size = new Size(287, 29);
        checkTwoInOne.TabIndex = 0;
        checkTwoInOne.Text = "二合一（水平复制）";
        checkTwoInOne.UseVisualStyleBackColor = true;
        checkTwoInOne.CheckedChanged += checkTwoInOne_CheckedChanged;
        //
        // buttonBatchTwoInOneFolder
        //
        buttonBatchTwoInOneFolder.Location = new Point(320, 34);
        buttonBatchTwoInOneFolder.Name = "buttonBatchTwoInOneFolder";
        buttonBatchTwoInOneFolder.Size = new Size(203, 39);
        buttonBatchTwoInOneFolder.TabIndex = 1;
        buttonBatchTwoInOneFolder.Text = "文件夹图片批量二合一...";
        buttonBatchTwoInOneFolder.UseVisualStyleBackColor = true;
        buttonBatchTwoInOneFolder.Click += buttonBatchTwoInOneFolder_Click;
        //
        // buttonResetDefaults
        //
        buttonResetDefaults.Location = new Point(27, 209);
        buttonResetDefaults.Name = "buttonResetDefaults";
        buttonResetDefaults.Size = new Size(496, 51);
        buttonResetDefaults.TabIndex = 5;
        buttonResetDefaults.Text = "恢复默认参数";
        buttonResetDefaults.UseVisualStyleBackColor = true;
        buttonResetDefaults.Click += buttonResetDefaults_Click;
        //
        // buttonBatchExport
        //
        buttonBatchExport.Location = new Point(288, 144);
        buttonBatchExport.Name = "buttonBatchExport";
        buttonBatchExport.Size = new Size(236, 54);
        buttonBatchExport.TabIndex = 4;
        buttonBatchExport.Text = "批量导出完整周期";
        buttonBatchExport.UseVisualStyleBackColor = true;
        buttonBatchExport.Click += buttonBatchExport_Click;
        //
        // buttonSaveCurrent
        //
        buttonSaveCurrent.Location = new Point(27, 144);
        buttonSaveCurrent.Name = "buttonSaveCurrent";
        buttonSaveCurrent.Size = new Size(236, 54);
        buttonSaveCurrent.TabIndex = 3;
        buttonSaveCurrent.Text = "保存当前相位...";
        buttonSaveCurrent.UseVisualStyleBackColor = true;
        buttonSaveCurrent.Click += buttonSaveCurrent_Click;
        //
        // buttonPreview
        //
        buttonPreview.Location = new Point(27, 80);
        buttonPreview.Name = "buttonPreview";
        buttonPreview.Size = new Size(496, 51);
        buttonPreview.TabIndex = 2;
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
        previewPanel.Name = "previewPanel";
        previewPanel.Size = new Size(1516, 1319);
        previewPanel.TabIndex = 0;
        //
        // previewControl
        //
        previewControl.BackColor = SystemColors.Control;
        previewControl.Dock = DockStyle.Fill;
        previewControl.Location = new Point(0, 66);
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
        previewHeaderPanel.Name = "previewHeaderPanel";
        previewHeaderPanel.Size = new Size(1516, 66);
        previewHeaderPanel.TabIndex = 0;
        //
        // labelPreviewInfo
        //
        labelPreviewInfo.AutoSize = true;
        labelPreviewInfo.ForeColor = Color.WhiteSmoke;
        labelPreviewInfo.Location = new Point(24, 18);
        labelPreviewInfo.Name = "labelPreviewInfo";
        labelPreviewInfo.Size = new Size(308, 24);
        labelPreviewInfo.TabIndex = 0;
        labelPreviewInfo.Text = "串扰像素排列2预览尚未生成";
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
        saveFileDialog.Title = "保存串扰像素排列2图卡";
        //
        // folderBrowserDialog
        //
        folderBrowserDialog.Description = "选择串扰像素排列2图卡的导出目录";
        folderBrowserDialog.UseDescriptionForTitle = true;
        //
        // CrosstalkGridForm
        //
        AutoScaleDimensions = new SizeF(144F, 144F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(2130, 1350);
        Controls.Add(splitContainerMain);
        Controls.Add(statusStrip);
        Font = new Font("Microsoft YaHei UI", 9F);
        MinimumSize = new Size(1669, 1052);
        Name = "CrosstalkGridForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "串扰像素排列2";
        FormClosing += CrosstalkGridForm_FormClosing;
        FormClosed += CrosstalkGridForm_FormClosed;
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
    private RegionMarginsEditor regionMarginsEditor;
    private GroupBox groupPhase;
    private CrosstalkPixelGridEditor cycleEditor;
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
    private CheckBox checkTwoInOne;
    private Button buttonBatchTwoInOneFolder;
    private Button buttonResetDefaults;
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
