using EolTestPatternGenerator.Controls;

namespace EolTestPatternGenerator;

#nullable disable

partial class ScreenOneForm
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
        groupCard = new GroupBox();
        labelCardHelp = new Label();
        comboCardKind = new ComboBox();
        labelCardKind = new Label();
        groupLeftRegion = new GroupBox();
        leftRegionMarginsEditor = new RegionMarginsEditor();
        groupRightRegion = new GroupBox();
        rightRegionMarginsEditor = new RegionMarginsEditor();
        groupExport = new GroupBox();
        labelExportHelp = new Label();
        numericQuality = new NumericUpDown();
        labelQuality = new Label();
        comboOutputFormat = new ComboBox();
        labelOutputFormat = new Label();
        groupActions = new GroupBox();
        buttonRestoreDefaults = new Button();
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
        groupCard.SuspendLayout();
        groupLeftRegion.SuspendLayout();
        groupRightRegion.SuspendLayout();
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
        splitContainerMain.Panel1MinSize = 430;
        // 
        // splitContainerMain.Panel2
        // 
        splitContainerMain.Panel2.Controls.Add(previewPanel);
        splitContainerMain.Size = new Size(2160, 1319);
        splitContainerMain.SplitterDistance = 645;
        splitContainerMain.SplitterWidth = 6;
        splitContainerMain.TabIndex = 0;
        // 
        // settingsFlowPanel
        // 
        settingsFlowPanel.AutoScroll = true;
        settingsFlowPanel.Controls.Add(groupCanvas);
        settingsFlowPanel.Controls.Add(groupCard);
        settingsFlowPanel.Controls.Add(groupLeftRegion);
        settingsFlowPanel.Controls.Add(groupRightRegion);
        settingsFlowPanel.Controls.Add(groupExport);
        settingsFlowPanel.Controls.Add(groupActions);
        settingsFlowPanel.Dock = DockStyle.Fill;
        settingsFlowPanel.FlowDirection = FlowDirection.TopDown;
        settingsFlowPanel.Location = new Point(0, 0);
        settingsFlowPanel.Margin = new Padding(4, 4, 4, 4);
        settingsFlowPanel.Name = "settingsFlowPanel";
        settingsFlowPanel.Padding = new Padding(15, 15, 12, 24);
        settingsFlowPanel.Size = new Size(645, 1319);
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
        groupCanvas.Size = new Size(591, 123);
        groupCanvas.TabIndex = 0;
        groupCanvas.TabStop = false;
        groupCanvas.Text = "画布尺寸";
        // 
        // numericCanvasHeight
        // 
        numericCanvasHeight.Location = new Point(412, 50);
        numericCanvasHeight.Margin = new Padding(4, 4, 4, 4);
        numericCanvasHeight.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericCanvasHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericCanvasHeight.Name = "numericCanvasHeight";
        numericCanvasHeight.Size = new Size(158, 30);
        numericCanvasHeight.TabIndex = 3;
        numericCanvasHeight.ThousandsSeparator = true;
        toolTip.SetToolTip(numericCanvasHeight, "最终导出图像的像素高度。");
        numericCanvasHeight.Value = new decimal(new int[] { 2000, 0, 0, 0 });
        numericCanvasHeight.ValueChanged += Parameter_ValueChanged;
        // 
        // labelCanvasHeight
        // 
        labelCanvasHeight.AutoSize = true;
        labelCanvasHeight.Location = new Point(328, 52);
        labelCanvasHeight.Margin = new Padding(4, 0, 4, 0);
        labelCanvasHeight.Name = "labelCanvasHeight";
        labelCanvasHeight.Size = new Size(46, 24);
        labelCanvasHeight.TabIndex = 2;
        labelCanvasHeight.Text = "高度";
        // 
        // numericCanvasWidth
        // 
        numericCanvasWidth.Location = new Point(111, 50);
        numericCanvasWidth.Margin = new Padding(4, 4, 4, 4);
        numericCanvasWidth.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericCanvasWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericCanvasWidth.Name = "numericCanvasWidth";
        numericCanvasWidth.Size = new Size(176, 30);
        numericCanvasWidth.TabIndex = 1;
        numericCanvasWidth.ThousandsSeparator = true;
        toolTip.SetToolTip(numericCanvasWidth, "最终导出图像的像素宽度。");
        numericCanvasWidth.Value = new decimal(new int[] { 3200, 0, 0, 0 });
        numericCanvasWidth.ValueChanged += Parameter_ValueChanged;
        // 
        // labelCanvasWidth
        // 
        labelCanvasWidth.AutoSize = true;
        labelCanvasWidth.Location = new Point(27, 52);
        labelCanvasWidth.Margin = new Padding(4, 0, 4, 0);
        labelCanvasWidth.Name = "labelCanvasWidth";
        labelCanvasWidth.Size = new Size(46, 24);
        labelCanvasWidth.TabIndex = 0;
        labelCanvasWidth.Text = "宽度";
        // 
        // groupCard
        // 
        groupCard.Controls.Add(labelCardHelp);
        groupCard.Controls.Add(comboCardKind);
        groupCard.Controls.Add(labelCardKind);
        groupCard.Location = new Point(19, 150);
        groupCard.Margin = new Padding(4, 4, 4, 4);
        groupCard.Name = "groupCard";
        groupCard.Padding = new Padding(4, 4, 4, 4);
        groupCard.Size = new Size(591, 150);
        groupCard.TabIndex = 1;
        groupCard.TabStop = false;
        groupCard.Text = "图卡选择";
        // 
        // labelCardHelp
        // 
        labelCardHelp.AutoSize = true;
        labelCardHelp.ForeColor = Color.DimGray;
        labelCardHelp.Location = new Point(111, 99);
        labelCardHelp.Margin = new Padding(4, 0, 4, 0);
        labelCardHelp.Name = "labelCardHelp";
        labelCardHelp.Size = new Size(424, 24);
        labelCardHelp.TabIndex = 2;
        labelCardHelp.Text = "三张图共用下方左右区域参数；区域外背景为纯白。";
        // 
        // comboCardKind
        // 
        comboCardKind.DropDownStyle = ComboBoxStyle.DropDownList;
        comboCardKind.FormattingEnabled = true;
        comboCardKind.Items.AddRange(new object[] { "1.B_W — 左黑 / 右白", "2.W_B — 左白 / 右黑", "3.B — 左黑 / 右黑" });
        comboCardKind.Location = new Point(111, 44);
        comboCardKind.Margin = new Padding(4, 4, 4, 4);
        comboCardKind.Name = "comboCardKind";
        comboCardKind.Size = new Size(457, 32);
        comboCardKind.TabIndex = 1;
        comboCardKind.SelectedIndexChanged += comboCardKind_SelectedIndexChanged;
        // 
        // labelCardKind
        // 
        labelCardKind.AutoSize = true;
        labelCardKind.Location = new Point(27, 48);
        labelCardKind.Margin = new Padding(4, 0, 4, 0);
        labelCardKind.Name = "labelCardKind";
        labelCardKind.Size = new Size(46, 24);
        labelCardKind.TabIndex = 0;
        labelCardKind.Text = "类型";
        // 
        // groupLeftRegion
        // 
        groupLeftRegion.Controls.Add(leftRegionMarginsEditor);
        groupLeftRegion.Location = new Point(19, 308);
        groupLeftRegion.Margin = new Padding(4, 4, 4, 4);
        groupLeftRegion.Name = "groupLeftRegion";
        groupLeftRegion.Padding = new Padding(4, 4, 4, 4);
        groupLeftRegion.Size = new Size(591, 320);
        groupLeftRegion.TabIndex = 2;
        groupLeftRegion.TabStop = false;
        groupLeftRegion.Text = "左区域外缘四边距";
        //
        // leftRegionMarginsEditor
        //
        leftRegionMarginsEditor.CanvasSize = new Size(3200, 2000);
        leftRegionMarginsEditor.Location = new Point(27, 38);
        leftRegionMarginsEditor.Margin = new Padding(4);
        leftRegionMarginsEditor.Name = "leftRegionMarginsEditor";
        leftRegionMarginsEditor.Size = new Size(540, 256);
        leftRegionMarginsEditor.TabIndex = 0;
        leftRegionMarginsEditor.MarginsChanged += regionMarginsEditor_MarginsChanged;
        // 
        // groupRightRegion
        // 
        groupRightRegion.Controls.Add(rightRegionMarginsEditor);
        groupRightRegion.Location = new Point(19, 487);
        groupRightRegion.Margin = new Padding(4, 4, 4, 4);
        groupRightRegion.Name = "groupRightRegion";
        groupRightRegion.Padding = new Padding(4, 4, 4, 4);
        groupRightRegion.Size = new Size(591, 320);
        groupRightRegion.TabIndex = 3;
        groupRightRegion.TabStop = false;
        groupRightRegion.Text = "右区域外缘四边距";
        //
        // rightRegionMarginsEditor
        //
        rightRegionMarginsEditor.CanvasSize = new Size(3200, 2000);
        rightRegionMarginsEditor.Location = new Point(27, 38);
        rightRegionMarginsEditor.Margin = new Padding(4);
        rightRegionMarginsEditor.Name = "rightRegionMarginsEditor";
        rightRegionMarginsEditor.Size = new Size(540, 256);
        rightRegionMarginsEditor.TabIndex = 0;
        rightRegionMarginsEditor.MarginsChanged += regionMarginsEditor_MarginsChanged;
        // 
        // groupExport
        // 
        groupExport.Controls.Add(labelExportHelp);
        groupExport.Controls.Add(numericQuality);
        groupExport.Controls.Add(labelQuality);
        groupExport.Controls.Add(comboOutputFormat);
        groupExport.Controls.Add(labelOutputFormat);
        groupExport.Location = new Point(19, 666);
        groupExport.Margin = new Padding(4, 4, 4, 4);
        groupExport.Name = "groupExport";
        groupExport.Padding = new Padding(4, 4, 4, 4);
        groupExport.Size = new Size(591, 183);
        groupExport.TabIndex = 4;
        groupExport.TabStop = false;
        groupExport.Text = "输出设置";
        // 
        // labelExportHelp
        // 
        labelExportHelp.ForeColor = Color.DimGray;
        labelExportHelp.Location = new Point(27, 117);
        labelExportHelp.Margin = new Padding(4, 0, 4, 0);
        labelExportHelp.Name = "labelExportHelp";
        labelExportHelp.Size = new Size(543, 52);
        labelExportHelp.TabIndex = 4;
        labelExportHelp.Text = "PNG、BMP、TIFF 可保持 RGB 通道严格只有 0 和 255。";
        // 
        // numericQuality
        // 
        numericQuality.Location = new Point(412, 51);
        numericQuality.Margin = new Padding(4, 4, 4, 4);
        numericQuality.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericQuality.Name = "numericQuality";
        numericQuality.Size = new Size(158, 30);
        numericQuality.TabIndex = 3;
        numericQuality.Value = new decimal(new int[] { 95, 0, 0, 0 });
        // 
        // labelQuality
        // 
        labelQuality.AutoSize = true;
        labelQuality.Location = new Point(328, 54);
        labelQuality.Margin = new Padding(4, 0, 4, 0);
        labelQuality.Name = "labelQuality";
        labelQuality.Size = new Size(46, 24);
        labelQuality.TabIndex = 2;
        labelQuality.Text = "质量";
        // 
        // comboOutputFormat
        // 
        comboOutputFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        comboOutputFormat.FormattingEnabled = true;
        comboOutputFormat.Items.AddRange(new object[] { "PNG", "JPEG", "BMP", "TIFF", "WebP" });
        comboOutputFormat.Location = new Point(111, 48);
        comboOutputFormat.Margin = new Padding(4, 4, 4, 4);
        comboOutputFormat.Name = "comboOutputFormat";
        comboOutputFormat.Size = new Size(174, 32);
        comboOutputFormat.TabIndex = 1;
        comboOutputFormat.SelectedIndexChanged += comboOutputFormat_SelectedIndexChanged;
        // 
        // labelOutputFormat
        // 
        labelOutputFormat.AutoSize = true;
        labelOutputFormat.Location = new Point(27, 52);
        labelOutputFormat.Margin = new Padding(4, 0, 4, 0);
        labelOutputFormat.Name = "labelOutputFormat";
        labelOutputFormat.Size = new Size(46, 24);
        labelOutputFormat.TabIndex = 0;
        labelOutputFormat.Text = "格式";
        // 
        // groupActions
        // 
        groupActions.Controls.Add(buttonRestoreDefaults);
        groupActions.Controls.Add(buttonBatchExport);
        groupActions.Controls.Add(buttonSaveCurrent);
        groupActions.Controls.Add(buttonPreview);
        groupActions.Location = new Point(19, 857);
        groupActions.Margin = new Padding(4, 4, 4, 4);
        groupActions.Name = "groupActions";
        groupActions.Padding = new Padding(4, 4, 4, 4);
        groupActions.Size = new Size(591, 168);
        groupActions.TabIndex = 5;
        groupActions.TabStop = false;
        groupActions.Text = "操作";
        // 
        // buttonRestoreDefaults
        // 
        buttonRestoreDefaults.Location = new Point(309, 100);
        buttonRestoreDefaults.Margin = new Padding(4, 4, 4, 4);
        buttonRestoreDefaults.Name = "buttonRestoreDefaults";
        buttonRestoreDefaults.Size = new Size(261, 45);
        buttonRestoreDefaults.TabIndex = 3;
        buttonRestoreDefaults.Text = "恢复参考图默认参数";
        buttonRestoreDefaults.UseVisualStyleBackColor = true;
        buttonRestoreDefaults.Click += buttonRestoreDefaults_Click;
        // 
        // buttonBatchExport
        // 
        buttonBatchExport.Location = new Point(27, 100);
        buttonBatchExport.Margin = new Padding(4, 4, 4, 4);
        buttonBatchExport.Name = "buttonBatchExport";
        buttonBatchExport.Size = new Size(261, 45);
        buttonBatchExport.TabIndex = 2;
        buttonBatchExport.Text = "批量导出三张";
        buttonBatchExport.UseVisualStyleBackColor = true;
        buttonBatchExport.Click += buttonBatchExport_Click;
        // 
        // buttonSaveCurrent
        // 
        buttonSaveCurrent.Location = new Point(309, 40);
        buttonSaveCurrent.Margin = new Padding(4, 4, 4, 4);
        buttonSaveCurrent.Name = "buttonSaveCurrent";
        buttonSaveCurrent.Size = new Size(261, 45);
        buttonSaveCurrent.TabIndex = 1;
        buttonSaveCurrent.Text = "保存当前图卡";
        buttonSaveCurrent.UseVisualStyleBackColor = true;
        buttonSaveCurrent.Click += buttonSaveCurrent_Click;
        // 
        // buttonPreview
        // 
        buttonPreview.Location = new Point(27, 40);
        buttonPreview.Margin = new Padding(4, 4, 4, 4);
        buttonPreview.Name = "buttonPreview";
        buttonPreview.Size = new Size(261, 45);
        buttonPreview.TabIndex = 0;
        buttonPreview.Text = "立即刷新预览";
        buttonPreview.UseVisualStyleBackColor = true;
        buttonPreview.Click += buttonPreview_Click;
        // 
        // previewPanel
        // 
        previewPanel.Controls.Add(previewControl);
        previewPanel.Controls.Add(previewHeaderPanel);
        previewPanel.Dock = DockStyle.Fill;
        previewPanel.Location = new Point(0, 0);
        previewPanel.Margin = new Padding(4, 4, 4, 4);
        previewPanel.Name = "previewPanel";
        previewPanel.Size = new Size(1509, 1319);
        previewPanel.TabIndex = 0;
        // 
        // previewControl
        // 
        previewControl.BackColor = Color.FromArgb(224, 228, 234);
        previewControl.Dock = DockStyle.Fill;
        previewControl.Location = new Point(0, 66);
        previewControl.Margin = new Padding(6, 6, 6, 6);
        previewControl.Name = "previewControl";
        previewControl.Size = new Size(1509, 1253);
        previewControl.TabIndex = 1;
        toolTip.SetToolTip(previewControl, "单击预览后使用滚轮缩放；按住鼠标左键拖动图像。");
        // 
        // previewHeaderPanel
        // 
        previewHeaderPanel.BackColor = Color.FromArgb(52, 58, 64);
        previewHeaderPanel.Controls.Add(labelPreviewInfo);
        previewHeaderPanel.Dock = DockStyle.Top;
        previewHeaderPanel.Location = new Point(0, 0);
        previewHeaderPanel.Margin = new Padding(4, 4, 4, 4);
        previewHeaderPanel.Name = "previewHeaderPanel";
        previewHeaderPanel.Size = new Size(1509, 66);
        previewHeaderPanel.TabIndex = 0;
        // 
        // labelPreviewInfo
        // 
        labelPreviewInfo.AutoSize = true;
        labelPreviewInfo.ForeColor = Color.WhiteSmoke;
        labelPreviewInfo.Location = new Point(24, 20);
        labelPreviewInfo.Margin = new Padding(4, 0, 4, 0);
        labelPreviewInfo.Name = "labelPreviewInfo";
        labelPreviewInfo.Size = new Size(242, 24);
        labelPreviewInfo.TabIndex = 0;
        labelPreviewInfo.Text = "预览：3200 × 2000 | 1.B_W";
        // 
        // statusStrip
        // 
        statusStrip.ImageScalingSize = new Size(24, 24);
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 1319);
        statusStrip.Name = "statusStrip";
        statusStrip.Padding = new Padding(2, 0, 21, 0);
        statusStrip.Size = new Size(2160, 31);
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
        saveFileDialog.Title = "保存显示器图卡";
        // 
        // folderBrowserDialog
        // 
        folderBrowserDialog.Description = "选择显示器三张图卡的导出目录";
        folderBrowserDialog.UseDescriptionForTitle = true;
        // 
        // ScreenOneForm
        // 
        AutoScaleDimensions = new SizeF(144F, 144F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(2160, 1350);
        Controls.Add(splitContainerMain);
        Controls.Add(statusStrip);
        Font = new Font("Microsoft YaHei UI", 9F);
        Margin = new Padding(4, 4, 4, 4);
        MinimumSize = new Size(1729, 1112);
        Name = "ScreenOneForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "显示器";
        FormClosing += ScreenOneForm_FormClosing;
        FormClosed += ScreenOneForm_FormClosed;
        splitContainerMain.Panel1.ResumeLayout(false);
        splitContainerMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
        splitContainerMain.ResumeLayout(false);
        settingsFlowPanel.ResumeLayout(false);
        groupCanvas.ResumeLayout(false);
        groupCanvas.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericCanvasHeight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericCanvasWidth).EndInit();
        groupCard.ResumeLayout(false);
        groupCard.PerformLayout();
        groupLeftRegion.ResumeLayout(false);
        groupLeftRegion.PerformLayout();
        groupRightRegion.ResumeLayout(false);
        groupRightRegion.PerformLayout();
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
    private GroupBox groupCard;
    private Label labelCardHelp;
    private ComboBox comboCardKind;
    private Label labelCardKind;
    private GroupBox groupLeftRegion;
    private RegionMarginsEditor leftRegionMarginsEditor;
    private GroupBox groupRightRegion;
    private RegionMarginsEditor rightRegionMarginsEditor;
    private GroupBox groupExport;
    private Label labelExportHelp;
    private NumericUpDown numericQuality;
    private Label labelQuality;
    private ComboBox comboOutputFormat;
    private Label labelOutputFormat;
    private GroupBox groupActions;
    private Button buttonRestoreDefaults;
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
