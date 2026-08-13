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
        splitContainerMain.Size = new Size(1420, 878);
        splitContainerMain.SplitterDistance = 405;
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
        settingsFlowPanel.Padding = new Padding(10, 10, 8, 16);
        settingsFlowPanel.Size = new Size(405, 878);
        settingsFlowPanel.TabIndex = 0;
        settingsFlowPanel.WrapContents = false;
        //
        // groupCanvas
        //
        groupCanvas.Controls.Add(numericCanvasHeight);
        groupCanvas.Controls.Add(labelCanvasHeight);
        groupCanvas.Controls.Add(numericCanvasWidth);
        groupCanvas.Controls.Add(labelCanvasWidth);
        groupCanvas.Location = new Point(13, 13);
        groupCanvas.Name = "groupCanvas";
        groupCanvas.Size = new Size(367, 106);
        groupCanvas.TabIndex = 0;
        groupCanvas.TabStop = false;
        groupCanvas.Text = "画布尺寸";
        //
        // numericCanvasHeight
        //
        numericCanvasHeight.Location = new Point(112, 66);
        numericCanvasHeight.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericCanvasHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericCanvasHeight.Name = "numericCanvasHeight";
        numericCanvasHeight.Size = new Size(150, 27);
        numericCanvasHeight.TabIndex = 3;
        numericCanvasHeight.Value = new decimal(new int[] { 1080, 0, 0, 0 });
        numericCanvasHeight.ValueChanged += Parameter_ValueChanged;
        //
        // labelCanvasHeight
        //
        labelCanvasHeight.AutoSize = true;
        labelCanvasHeight.Location = new Point(18, 70);
        labelCanvasHeight.Name = "labelCanvasHeight";
        labelCanvasHeight.Size = new Size(69, 20);
        labelCanvasHeight.TabIndex = 2;
        labelCanvasHeight.Text = "画布高度";
        //
        // numericCanvasWidth
        //
        numericCanvasWidth.Location = new Point(112, 28);
        numericCanvasWidth.Maximum = new decimal(new int[] { 8192, 0, 0, 0 });
        numericCanvasWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericCanvasWidth.Name = "numericCanvasWidth";
        numericCanvasWidth.Size = new Size(150, 27);
        numericCanvasWidth.TabIndex = 1;
        numericCanvasWidth.Value = new decimal(new int[] { 1920, 0, 0, 0 });
        numericCanvasWidth.ValueChanged += Parameter_ValueChanged;
        //
        // labelCanvasWidth
        //
        labelCanvasWidth.AutoSize = true;
        labelCanvasWidth.Location = new Point(18, 32);
        labelCanvasWidth.Name = "labelCanvasWidth";
        labelCanvasWidth.Size = new Size(69, 20);
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
        groupRegion.Location = new Point(13, 125);
        groupRegion.Name = "groupRegion";
        groupRegion.Size = new Size(367, 150);
        groupRegion.TabIndex = 1;
        groupRegion.TabStop = false;
        groupRegion.Text = "条纹区域";
        //
        // buttonCenter
        //
        buttonCenter.Location = new Point(250, 104);
        buttonCenter.Name = "buttonCenter";
        buttonCenter.Size = new Size(99, 30);
        buttonCenter.TabIndex = 8;
        buttonCenter.Text = "区域居中";
        buttonCenter.UseVisualStyleBackColor = true;
        buttonCenter.Click += buttonCenter_Click;
        //
        // numericPatternHeight
        //
        numericPatternHeight.Location = new Point(249, 66);
        numericPatternHeight.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericPatternHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPatternHeight.Name = "numericPatternHeight";
        numericPatternHeight.Size = new Size(100, 27);
        numericPatternHeight.TabIndex = 7;
        numericPatternHeight.Value = new decimal(new int[] { 627, 0, 0, 0 });
        numericPatternHeight.ValueChanged += Parameter_ValueChanged;
        //
        // labelPatternHeight
        //
        labelPatternHeight.AutoSize = true;
        labelPatternHeight.Location = new Point(193, 70);
        labelPatternHeight.Name = "labelPatternHeight";
        labelPatternHeight.Size = new Size(39, 20);
        labelPatternHeight.TabIndex = 6;
        labelPatternHeight.Text = "高度";
        //
        // numericPatternWidth
        //
        numericPatternWidth.Location = new Point(76, 66);
        numericPatternWidth.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericPatternWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPatternWidth.Name = "numericPatternWidth";
        numericPatternWidth.Size = new Size(100, 27);
        numericPatternWidth.TabIndex = 5;
        numericPatternWidth.Value = new decimal(new int[] { 1777, 0, 0, 0 });
        numericPatternWidth.ValueChanged += Parameter_ValueChanged;
        //
        // labelPatternWidth
        //
        labelPatternWidth.AutoSize = true;
        labelPatternWidth.Location = new Point(18, 70);
        labelPatternWidth.Name = "labelPatternWidth";
        labelPatternWidth.Size = new Size(39, 20);
        labelPatternWidth.TabIndex = 4;
        labelPatternWidth.Text = "宽度";
        //
        // numericPatternY
        //
        numericPatternY.Location = new Point(249, 28);
        numericPatternY.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericPatternY.Minimum = new decimal(new int[] { 16384, 0, 0, int.MinValue });
        numericPatternY.Name = "numericPatternY";
        numericPatternY.Size = new Size(100, 27);
        numericPatternY.TabIndex = 3;
        numericPatternY.Value = new decimal(new int[] { 226, 0, 0, 0 });
        numericPatternY.ValueChanged += Parameter_ValueChanged;
        //
        // labelPatternY
        //
        labelPatternY.AutoSize = true;
        labelPatternY.Location = new Point(193, 32);
        labelPatternY.Name = "labelPatternY";
        labelPatternY.Size = new Size(50, 20);
        labelPatternY.TabIndex = 2;
        labelPatternY.Text = "左上 Y";
        //
        // numericPatternX
        //
        numericPatternX.Location = new Point(76, 28);
        numericPatternX.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericPatternX.Minimum = new decimal(new int[] { 16384, 0, 0, int.MinValue });
        numericPatternX.Name = "numericPatternX";
        numericPatternX.Size = new Size(100, 27);
        numericPatternX.TabIndex = 1;
        numericPatternX.Value = new decimal(new int[] { 71, 0, 0, 0 });
        numericPatternX.ValueChanged += Parameter_ValueChanged;
        //
        // labelPatternX
        //
        labelPatternX.AutoSize = true;
        labelPatternX.Location = new Point(18, 32);
        labelPatternX.Name = "labelPatternX";
        labelPatternX.Size = new Size(50, 20);
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
        groupPhase.Location = new Point(13, 281);
        groupPhase.Name = "groupPhase";
        groupPhase.Size = new Size(367, 141);
        groupPhase.TabIndex = 2;
        groupPhase.TabStop = false;
        groupPhase.Text = "相位与颜色排列";
        //
        // labelOrderHelp
        //
        labelOrderHelp.ForeColor = Color.DimGray;
        labelOrderHelp.Location = new Point(18, 102);
        labelOrderHelp.Name = "labelOrderHelp";
        labelOrderHelp.Size = new Size(331, 30);
        labelOrderHelp.TabIndex = 4;
        labelOrderHelp.Text = "默认 RGB 与原始 1–8.png 保持一致。";
        //
        // comboPixelOrder
        //
        comboPixelOrder.DropDownStyle = ComboBoxStyle.DropDownList;
        comboPixelOrder.FormattingEnabled = true;
        comboPixelOrder.Items.AddRange(new object[] { "RGB", "RBG", "GRB", "GBR", "BRG", "BGR" });
        comboPixelOrder.Location = new Point(112, 66);
        comboPixelOrder.Name = "comboPixelOrder";
        comboPixelOrder.Size = new Size(237, 28);
        comboPixelOrder.TabIndex = 3;
        comboPixelOrder.SelectedIndexChanged += comboPixelOrder_SelectedIndexChanged;
        //
        // labelPixelOrder
        //
        labelPixelOrder.AutoSize = true;
        labelPixelOrder.Location = new Point(18, 70);
        labelPixelOrder.Name = "labelPixelOrder";
        labelPixelOrder.Size = new Size(69, 20);
        labelPixelOrder.TabIndex = 2;
        labelPixelOrder.Text = "像素排列";
        //
        // numericPhase
        //
        numericPhase.Location = new Point(112, 28);
        numericPhase.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
        numericPhase.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericPhase.Name = "numericPhase";
        numericPhase.Size = new Size(98, 27);
        numericPhase.TabIndex = 1;
        numericPhase.Value = new decimal(new int[] { 1, 0, 0, 0 });
        numericPhase.ValueChanged += Parameter_ValueChanged;
        //
        // labelPhase
        //
        labelPhase.AutoSize = true;
        labelPhase.Location = new Point(18, 32);
        labelPhase.Name = "labelPhase";
        labelPhase.Size = new Size(75, 20);
        labelPhase.TabIndex = 0;
        labelPhase.Text = "相位(1-8)";
        //
        // borderOverlayEditor
        //
        borderOverlayEditor.Location = new Point(13, 428);
        borderOverlayEditor.Name = "borderOverlayEditor";
        borderOverlayEditor.Size = new Size(367, 230);
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
        groupExport.Location = new Point(13, 664);
        groupExport.Name = "groupExport";
        groupExport.Size = new Size(367, 139);
        groupExport.TabIndex = 4;
        groupExport.TabStop = false;
        groupExport.Text = "导出设置";
        //
        // labelExportHelp
        //
        labelExportHelp.ForeColor = Color.DimGray;
        labelExportHelp.Location = new Point(18, 102);
        labelExportHelp.Name = "labelExportHelp";
        labelExportHelp.Size = new Size(331, 30);
        labelExportHelp.TabIndex = 4;
        labelExportHelp.Text = "PNG、BMP、TIFF 可保持精确的 0/255 像素。";
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
        groupActions.Controls.Add(buttonPreview);
        groupActions.Location = new Point(13, 809);
        groupActions.Name = "groupActions";
        groupActions.Size = new Size(367, 119);
        groupActions.TabIndex = 5;
        groupActions.TabStop = false;
        groupActions.Text = "生成与保存";
        //
        // buttonBatchExport
        //
        buttonBatchExport.Location = new Point(192, 69);
        buttonBatchExport.Name = "buttonBatchExport";
        buttonBatchExport.Size = new Size(157, 36);
        buttonBatchExport.TabIndex = 2;
        buttonBatchExport.Text = "批量导出相位 1–8";
        buttonBatchExport.UseVisualStyleBackColor = true;
        buttonBatchExport.Click += buttonBatchExport_Click;
        //
        // buttonSaveCurrent
        //
        buttonSaveCurrent.Location = new Point(18, 69);
        buttonSaveCurrent.Name = "buttonSaveCurrent";
        buttonSaveCurrent.Size = new Size(157, 36);
        buttonSaveCurrent.TabIndex = 1;
        buttonSaveCurrent.Text = "保存当前相位...";
        buttonSaveCurrent.UseVisualStyleBackColor = true;
        buttonSaveCurrent.Click += buttonSaveCurrent_Click;
        //
        // buttonPreview
        //
        buttonPreview.Location = new Point(18, 27);
        buttonPreview.Name = "buttonPreview";
        buttonPreview.Size = new Size(331, 34);
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
        previewPanel.Name = "previewPanel";
        previewPanel.Size = new Size(1011, 878);
        previewPanel.TabIndex = 0;
        //
        // previewControl
        //
        previewControl.BackColor = SystemColors.Control;
        previewControl.Dock = DockStyle.Fill;
        previewControl.Location = new Point(0, 44);
        previewControl.Name = "previewControl";
        previewControl.Size = new Size(1011, 834);
        previewControl.TabIndex = 1;
        //
        // previewHeaderPanel
        //
        previewHeaderPanel.BackColor = Color.FromArgb(42, 45, 50);
        previewHeaderPanel.Controls.Add(labelPreviewInfo);
        previewHeaderPanel.Dock = DockStyle.Top;
        previewHeaderPanel.Location = new Point(0, 0);
        previewHeaderPanel.Name = "previewHeaderPanel";
        previewHeaderPanel.Size = new Size(1011, 44);
        previewHeaderPanel.TabIndex = 0;
        //
        // labelPreviewInfo
        //
        labelPreviewInfo.AutoSize = true;
        labelPreviewInfo.ForeColor = Color.WhiteSmoke;
        labelPreviewInfo.Location = new Point(16, 12);
        labelPreviewInfo.Name = "labelPreviewInfo";
        labelPreviewInfo.Size = new Size(231, 20);
        labelPreviewInfo.TabIndex = 0;
        labelPreviewInfo.Text = "预览：1920 × 1080 | 相位 1 | RGB";
        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip.Location = new Point(0, 878);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1420, 22);
        statusStrip.TabIndex = 1;
        //
        // statusLabel
        //
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(104, 17);
        statusLabel.Text = "正在生成预览...";
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
        saveFileDialog.Title = "保存 RGB 相移图卡";
        //
        // folderBrowserDialog
        //
        folderBrowserDialog.Description = "选择 RGB 八步相移图卡的导出目录";
        folderBrowserDialog.UseDescriptionForTitle = true;
        //
        // PhaseStripeForm
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1420, 900);
        Controls.Add(splitContainerMain);
        Controls.Add(statusStrip);
        Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        MinimumSize = new Size(1120, 720);
        Name = "PhaseStripeForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "RGB 八步相移条纹";
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
