#nullable disable

namespace EolTestPatternGenerator.Controls;

partial class StillVideoPage
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            _encodingCancellation?.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Component Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        layoutRoot = new TableLayoutPanel();
        groupOutput = new GroupBox();
        outputLayout = new TableLayoutPanel();
        labelOutputWidth = new Label();
        numericOutputWidth = new NumericUpDown();
        labelOutputHeight = new Label();
        numericOutputHeight = new NumericUpDown();
        labelFramesPerSecond = new Label();
        numericFramesPerSecond = new NumericUpDown();
        labelActualSize = new Label();
        labelFormat = new Label();
        comboFormat = new ComboBox();
        labelFormatNotice = new Label();
        labelOutputPath = new Label();
        textOutputPath = new TextBox();
        buttonBrowseOutput = new Button();
        groupSequence = new GroupBox();
        gridImages = new DataGridView();
        columnOrder = new DataGridViewTextBoxColumn();
        columnFileName = new DataGridViewTextBoxColumn();
        columnFullPath = new DataGridViewTextBoxColumn();
        columnDuration = new DataGridViewTextBoxColumn();
        sequenceToolbar = new FlowLayoutPanel();
        buttonAddImages = new Button();
        buttonRemoveImages = new Button();
        buttonMoveUp = new Button();
        buttonMoveDown = new Button();
        buttonClearImages = new Button();
        labelSequenceHint = new Label();
        statusLayout = new TableLayoutPanel();
        labelSummary = new Label();
        labelProgress = new Label();
        progressEncoding = new ProgressBar();
        actionPanel = new FlowLayoutPanel();
        buttonCancel = new Button();
        buttonGenerate = new Button();
        openImagesDialog = new OpenFileDialog();
        saveVideoDialog = new SaveFileDialog();
        toolTip = new ToolTip(components);
        saveTimer = new System.Windows.Forms.Timer(components);
        layoutRoot.SuspendLayout();
        groupOutput.SuspendLayout();
        outputLayout.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericOutputWidth).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericOutputHeight).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericFramesPerSecond).BeginInit();
        groupSequence.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridImages).BeginInit();
        sequenceToolbar.SuspendLayout();
        statusLayout.SuspendLayout();
        actionPanel.SuspendLayout();
        SuspendLayout();
        //
        // layoutRoot
        //
        layoutRoot.BackColor = Color.FromArgb(239, 242, 246);
        layoutRoot.ColumnCount = 1;
        layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layoutRoot.Controls.Add(groupOutput, 0, 0);
        layoutRoot.Controls.Add(groupSequence, 0, 1);
        layoutRoot.Controls.Add(statusLayout, 0, 2);
        layoutRoot.Dock = DockStyle.Fill;
        layoutRoot.Location = new Point(0, 0);
        layoutRoot.Name = "layoutRoot";
        layoutRoot.Padding = new Padding(18);
        layoutRoot.RowCount = 3;
        layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 190F));
        layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
        layoutRoot.Size = new Size(1500, 920);
        layoutRoot.TabIndex = 0;
        //
        // groupOutput
        //
        groupOutput.BackColor = Color.White;
        groupOutput.Controls.Add(outputLayout);
        groupOutput.Dock = DockStyle.Fill;
        groupOutput.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        groupOutput.Location = new Point(21, 21);
        groupOutput.Name = "groupOutput";
        groupOutput.Padding = new Padding(14, 10, 14, 12);
        groupOutput.Size = new Size(1458, 184);
        groupOutput.TabIndex = 0;
        groupOutput.TabStop = false;
        groupOutput.Text = "视频输出";
        //
        // outputLayout
        //
        outputLayout.ColumnCount = 7;
        outputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92F));
        outputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
        outputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92F));
        outputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
        outputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72F));
        outputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
        outputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        outputLayout.Controls.Add(labelOutputWidth, 0, 0);
        outputLayout.Controls.Add(numericOutputWidth, 1, 0);
        outputLayout.Controls.Add(labelOutputHeight, 2, 0);
        outputLayout.Controls.Add(numericOutputHeight, 3, 0);
        outputLayout.Controls.Add(labelFramesPerSecond, 4, 0);
        outputLayout.Controls.Add(numericFramesPerSecond, 5, 0);
        outputLayout.Controls.Add(labelActualSize, 6, 0);
        outputLayout.Controls.Add(labelFormat, 0, 1);
        outputLayout.Controls.Add(comboFormat, 1, 1);
        outputLayout.SetColumnSpan(comboFormat, 3);
        outputLayout.Controls.Add(labelFormatNotice, 4, 1);
        outputLayout.SetColumnSpan(labelFormatNotice, 3);
        outputLayout.Controls.Add(labelOutputPath, 0, 2);
        outputLayout.Controls.Add(textOutputPath, 1, 2);
        outputLayout.SetColumnSpan(textOutputPath, 5);
        outputLayout.Controls.Add(buttonBrowseOutput, 6, 2);
        outputLayout.Dock = DockStyle.Fill;
        outputLayout.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        outputLayout.Location = new Point(14, 30);
        outputLayout.Name = "outputLayout";
        outputLayout.RowCount = 3;
        outputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        outputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        outputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        outputLayout.Size = new Size(1430, 142);
        outputLayout.TabIndex = 0;
        //
        // labelOutputWidth
        //
        labelOutputWidth.Dock = DockStyle.Fill;
        labelOutputWidth.Location = new Point(3, 0);
        labelOutputWidth.Name = "labelOutputWidth";
        labelOutputWidth.Size = new Size(86, 42);
        labelOutputWidth.TabIndex = 0;
        labelOutputWidth.Text = "输出宽度";
        labelOutputWidth.TextAlign = ContentAlignment.MiddleLeft;
        //
        // numericOutputWidth
        //
        numericOutputWidth.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        numericOutputWidth.Location = new Point(95, 7);
        numericOutputWidth.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericOutputWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericOutputWidth.Name = "numericOutputWidth";
        numericOutputWidth.Size = new Size(154, 28);
        numericOutputWidth.TabIndex = 1;
        numericOutputWidth.Value = new decimal(new int[] { 1920, 0, 0, 0 });
        numericOutputWidth.ValueChanged += outputSetting_ValueChanged;
        //
        // labelOutputHeight
        //
        labelOutputHeight.Dock = DockStyle.Fill;
        labelOutputHeight.Location = new Point(255, 0);
        labelOutputHeight.Name = "labelOutputHeight";
        labelOutputHeight.Size = new Size(86, 42);
        labelOutputHeight.TabIndex = 2;
        labelOutputHeight.Text = "输出高度";
        labelOutputHeight.TextAlign = ContentAlignment.MiddleLeft;
        //
        // numericOutputHeight
        //
        numericOutputHeight.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        numericOutputHeight.Location = new Point(347, 7);
        numericOutputHeight.Maximum = new decimal(new int[] { 16384, 0, 0, 0 });
        numericOutputHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericOutputHeight.Name = "numericOutputHeight";
        numericOutputHeight.Size = new Size(154, 28);
        numericOutputHeight.TabIndex = 3;
        numericOutputHeight.Value = new decimal(new int[] { 1080, 0, 0, 0 });
        numericOutputHeight.ValueChanged += outputSetting_ValueChanged;
        //
        // labelFramesPerSecond
        //
        labelFramesPerSecond.Dock = DockStyle.Fill;
        labelFramesPerSecond.Location = new Point(507, 0);
        labelFramesPerSecond.Name = "labelFramesPerSecond";
        labelFramesPerSecond.Size = new Size(66, 42);
        labelFramesPerSecond.TabIndex = 4;
        labelFramesPerSecond.Text = "帧率";
        labelFramesPerSecond.TextAlign = ContentAlignment.MiddleLeft;
        //
        // numericFramesPerSecond
        //
        numericFramesPerSecond.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        numericFramesPerSecond.Location = new Point(579, 7);
        numericFramesPerSecond.Maximum = new decimal(new int[] { 120, 0, 0, 0 });
        numericFramesPerSecond.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numericFramesPerSecond.Name = "numericFramesPerSecond";
        numericFramesPerSecond.Size = new Size(134, 28);
        numericFramesPerSecond.TabIndex = 5;
        numericFramesPerSecond.Value = new decimal(new int[] { 30, 0, 0, 0 });
        numericFramesPerSecond.ValueChanged += outputSetting_ValueChanged;
        //
        // labelActualSize
        //
        labelActualSize.Dock = DockStyle.Fill;
        labelActualSize.ForeColor = Color.DimGray;
        labelActualSize.Location = new Point(719, 0);
        labelActualSize.Name = "labelActualSize";
        labelActualSize.Size = new Size(708, 42);
        labelActualSize.TabIndex = 6;
        labelActualSize.Text = "实际编码：1920 × 1080 · 30 FPS";
        labelActualSize.TextAlign = ContentAlignment.MiddleLeft;
        //
        // labelFormat
        //
        labelFormat.Dock = DockStyle.Fill;
        labelFormat.Location = new Point(3, 42);
        labelFormat.Name = "labelFormat";
        labelFormat.Size = new Size(86, 42);
        labelFormat.TabIndex = 7;
        labelFormat.Text = "视频格式";
        labelFormat.TextAlign = ContentAlignment.MiddleLeft;
        //
        // comboFormat
        //
        comboFormat.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        comboFormat.DropDownStyle = ComboBoxStyle.DropDownList;
        comboFormat.FormattingEnabled = true;
        comboFormat.Items.AddRange(new object[] { "MP4 / MPEG-4（mp4v，有损）", "MKV / FFV1（无损，推荐）", "AVI / HuffYUV（无损）" });
        comboFormat.Location = new Point(95, 48);
        comboFormat.Name = "comboFormat";
        comboFormat.Size = new Size(406, 28);
        comboFormat.TabIndex = 8;
        comboFormat.SelectedIndexChanged += comboFormat_SelectedIndexChanged;
        //
        // labelFormatNotice
        //
        labelFormatNotice.Dock = DockStyle.Fill;
        labelFormatNotice.ForeColor = Color.FromArgb(176, 91, 0);
        labelFormatNotice.Location = new Point(507, 42);
        labelFormatNotice.Name = "labelFormatNotice";
        labelFormatNotice.Size = new Size(920, 42);
        labelFormatNotice.TabIndex = 9;
        labelFormatNotice.Text = "MP4 为有损编码，视频回读后的像素值不能保证严格保持 0 或 255。";
        labelFormatNotice.TextAlign = ContentAlignment.MiddleLeft;
        //
        // labelOutputPath
        //
        labelOutputPath.Dock = DockStyle.Fill;
        labelOutputPath.Location = new Point(3, 84);
        labelOutputPath.Name = "labelOutputPath";
        labelOutputPath.Size = new Size(86, 42);
        labelOutputPath.TabIndex = 10;
        labelOutputPath.Text = "输出文件";
        labelOutputPath.TextAlign = ContentAlignment.MiddleLeft;
        //
        // textOutputPath
        //
        textOutputPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        textOutputPath.Location = new Point(95, 91);
        textOutputPath.Name = "textOutputPath";
        textOutputPath.Size = new Size(618, 27);
        textOutputPath.TabIndex = 11;
        textOutputPath.TextChanged += textOutputPath_TextChanged;
        //
        // buttonBrowseOutput
        //
        buttonBrowseOutput.Anchor = AnchorStyles.Left;
        buttonBrowseOutput.Location = new Point(719, 89);
        buttonBrowseOutput.Name = "buttonBrowseOutput";
        buttonBrowseOutput.Size = new Size(110, 32);
        buttonBrowseOutput.TabIndex = 12;
        buttonBrowseOutput.Text = "浏览...";
        buttonBrowseOutput.UseVisualStyleBackColor = true;
        buttonBrowseOutput.Click += buttonBrowseOutput_Click;
        //
        // groupSequence
        //
        groupSequence.BackColor = Color.White;
        groupSequence.Controls.Add(gridImages);
        groupSequence.Controls.Add(sequenceToolbar);
        groupSequence.Dock = DockStyle.Fill;
        groupSequence.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        groupSequence.Location = new Point(21, 211);
        groupSequence.Name = "groupSequence";
        groupSequence.Padding = new Padding(12, 10, 12, 12);
        groupSequence.Size = new Size(1458, 592);
        groupSequence.TabIndex = 1;
        groupSequence.TabStop = false;
        groupSequence.Text = "图片顺序与静止时长";
        //
        // gridImages
        //
        gridImages.AllowUserToAddRows = false;
        gridImages.AllowUserToDeleteRows = false;
        gridImages.AllowUserToResizeRows = false;
        gridImages.BackgroundColor = Color.White;
        gridImages.BorderStyle = BorderStyle.Fixed3D;
        gridImages.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        gridImages.Columns.AddRange(new DataGridViewColumn[] { columnOrder, columnFileName, columnFullPath, columnDuration });
        gridImages.Dock = DockStyle.Fill;
        gridImages.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        gridImages.Location = new Point(12, 76);
        gridImages.MultiSelect = true;
        gridImages.Name = "gridImages";
        gridImages.RowHeadersVisible = false;
        gridImages.RowHeadersWidth = 51;
        gridImages.RowTemplate.Height = 30;
        gridImages.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        gridImages.Size = new Size(1434, 504);
        gridImages.TabIndex = 1;
        gridImages.CellEndEdit += gridImages_CellEndEdit;
        gridImages.CellValidating += gridImages_CellValidating;
        gridImages.SelectionChanged += gridImages_SelectionChanged;
        //
        // columnOrder
        //
        columnOrder.HeaderText = "顺序";
        columnOrder.MinimumWidth = 6;
        columnOrder.Name = "columnOrder";
        columnOrder.ReadOnly = true;
        columnOrder.SortMode = DataGridViewColumnSortMode.NotSortable;
        columnOrder.Width = 70;
        //
        // columnFileName
        //
        columnFileName.HeaderText = "文件名";
        columnFileName.MinimumWidth = 150;
        columnFileName.Name = "columnFileName";
        columnFileName.ReadOnly = true;
        columnFileName.SortMode = DataGridViewColumnSortMode.NotSortable;
        columnFileName.Width = 240;
        //
        // columnFullPath
        //
        columnFullPath.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        columnFullPath.HeaderText = "完整路径";
        columnFullPath.MinimumWidth = 200;
        columnFullPath.Name = "columnFullPath";
        columnFullPath.ReadOnly = true;
        columnFullPath.SortMode = DataGridViewColumnSortMode.NotSortable;
        //
        // columnDuration
        //
        columnDuration.HeaderText = "静止时间（秒）";
        columnDuration.MinimumWidth = 120;
        columnDuration.Name = "columnDuration";
        columnDuration.SortMode = DataGridViewColumnSortMode.NotSortable;
        columnDuration.Width = 150;
        //
        // sequenceToolbar
        //
        sequenceToolbar.Controls.Add(buttonAddImages);
        sequenceToolbar.Controls.Add(buttonRemoveImages);
        sequenceToolbar.Controls.Add(buttonMoveUp);
        sequenceToolbar.Controls.Add(buttonMoveDown);
        sequenceToolbar.Controls.Add(buttonClearImages);
        sequenceToolbar.Controls.Add(labelSequenceHint);
        sequenceToolbar.Dock = DockStyle.Top;
        sequenceToolbar.FlowDirection = FlowDirection.LeftToRight;
        sequenceToolbar.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        sequenceToolbar.Location = new Point(12, 30);
        sequenceToolbar.Name = "sequenceToolbar";
        sequenceToolbar.Padding = new Padding(0, 3, 0, 4);
        sequenceToolbar.Size = new Size(1434, 46);
        sequenceToolbar.TabIndex = 0;
        sequenceToolbar.WrapContents = false;
        //
        // buttonAddImages
        //
        buttonAddImages.Location = new Point(3, 6);
        buttonAddImages.Name = "buttonAddImages";
        buttonAddImages.Size = new Size(110, 34);
        buttonAddImages.TabIndex = 0;
        buttonAddImages.Text = "添加图片...";
        buttonAddImages.UseVisualStyleBackColor = true;
        buttonAddImages.Click += buttonAddImages_Click;
        //
        // buttonRemoveImages
        //
        buttonRemoveImages.Location = new Point(119, 6);
        buttonRemoveImages.Name = "buttonRemoveImages";
        buttonRemoveImages.Size = new Size(94, 34);
        buttonRemoveImages.TabIndex = 1;
        buttonRemoveImages.Text = "删除所选";
        buttonRemoveImages.UseVisualStyleBackColor = true;
        buttonRemoveImages.Click += buttonRemoveImages_Click;
        //
        // buttonMoveUp
        //
        buttonMoveUp.Location = new Point(219, 6);
        buttonMoveUp.Name = "buttonMoveUp";
        buttonMoveUp.Size = new Size(82, 34);
        buttonMoveUp.TabIndex = 2;
        buttonMoveUp.Text = "上移";
        buttonMoveUp.UseVisualStyleBackColor = true;
        buttonMoveUp.Click += buttonMoveUp_Click;
        //
        // buttonMoveDown
        //
        buttonMoveDown.Location = new Point(307, 6);
        buttonMoveDown.Name = "buttonMoveDown";
        buttonMoveDown.Size = new Size(82, 34);
        buttonMoveDown.TabIndex = 3;
        buttonMoveDown.Text = "下移";
        buttonMoveDown.UseVisualStyleBackColor = true;
        buttonMoveDown.Click += buttonMoveDown_Click;
        //
        // buttonClearImages
        //
        buttonClearImages.Location = new Point(395, 6);
        buttonClearImages.Name = "buttonClearImages";
        buttonClearImages.Size = new Size(82, 34);
        buttonClearImages.TabIndex = 4;
        buttonClearImages.Text = "清空";
        buttonClearImages.UseVisualStyleBackColor = true;
        buttonClearImages.Click += buttonClearImages_Click;
        //
        // labelSequenceHint
        //
        labelSequenceHint.AutoSize = true;
        labelSequenceHint.ForeColor = Color.DimGray;
        labelSequenceHint.Location = new Point(495, 12);
        labelSequenceHint.Margin = new Padding(15, 9, 3, 0);
        labelSequenceHint.Name = "labelSequenceHint";
        labelSequenceHint.Size = new Size(263, 20);
        labelSequenceHint.TabIndex = 5;
        labelSequenceHint.Text = "可多选图片；双击“静止时间”单元格修改";
        //
        // statusLayout
        //
        statusLayout.BackColor = Color.White;
        statusLayout.ColumnCount = 3;
        statusLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 290F));
        statusLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        statusLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250F));
        statusLayout.Controls.Add(labelSummary, 0, 0);
        statusLayout.Controls.Add(labelProgress, 1, 0);
        statusLayout.Controls.Add(progressEncoding, 0, 1);
        statusLayout.SetColumnSpan(progressEncoding, 2);
        statusLayout.Controls.Add(actionPanel, 2, 0);
        statusLayout.SetRowSpan(actionPanel, 2);
        statusLayout.Dock = DockStyle.Fill;
        statusLayout.Location = new Point(21, 809);
        statusLayout.Name = "statusLayout";
        statusLayout.Padding = new Padding(12, 8, 12, 8);
        statusLayout.RowCount = 2;
        statusLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
        statusLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        statusLayout.Size = new Size(1458, 90);
        statusLayout.TabIndex = 2;
        //
        // labelSummary
        //
        labelSummary.Dock = DockStyle.Fill;
        labelSummary.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        labelSummary.Location = new Point(15, 8);
        labelSummary.Name = "labelSummary";
        labelSummary.Size = new Size(284, 34);
        labelSummary.TabIndex = 0;
        labelSummary.Text = "0 张图片 · 总时长 00:00:00";
        labelSummary.TextAlign = ContentAlignment.MiddleLeft;
        //
        // labelProgress
        //
        labelProgress.Dock = DockStyle.Fill;
        labelProgress.ForeColor = Color.DimGray;
        labelProgress.Location = new Point(305, 8);
        labelProgress.Name = "labelProgress";
        labelProgress.Size = new Size(888, 34);
        labelProgress.TabIndex = 1;
        labelProgress.Text = "等待生成";
        labelProgress.TextAlign = ContentAlignment.MiddleLeft;
        //
        // progressEncoding
        //
        progressEncoding.Dock = DockStyle.Fill;
        progressEncoding.Location = new Point(15, 48);
        progressEncoding.Margin = new Padding(3, 6, 12, 6);
        progressEncoding.Maximum = 1000;
        progressEncoding.Name = "progressEncoding";
        progressEncoding.Size = new Size(1175, 28);
        progressEncoding.TabIndex = 2;
        //
        // actionPanel
        //
        actionPanel.Controls.Add(buttonCancel);
        actionPanel.Controls.Add(buttonGenerate);
        actionPanel.Dock = DockStyle.Fill;
        actionPanel.FlowDirection = FlowDirection.RightToLeft;
        actionPanel.Location = new Point(1199, 11);
        actionPanel.Name = "actionPanel";
        actionPanel.Padding = new Padding(0, 9, 0, 0);
        actionPanel.Size = new Size(244, 68);
        actionPanel.TabIndex = 3;
        //
        // buttonCancel
        //
        buttonCancel.Enabled = false;
        buttonCancel.Location = new Point(126, 12);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new Size(115, 44);
        buttonCancel.TabIndex = 1;
        buttonCancel.Text = "取消";
        buttonCancel.UseVisualStyleBackColor = true;
        buttonCancel.Click += buttonCancel_Click;
        //
        // buttonGenerate
        //
        buttonGenerate.BackColor = Color.FromArgb(26, 115, 232);
        buttonGenerate.FlatStyle = FlatStyle.Flat;
        buttonGenerate.ForeColor = Color.White;
        buttonGenerate.Location = new Point(5, 12);
        buttonGenerate.Name = "buttonGenerate";
        buttonGenerate.Size = new Size(115, 44);
        buttonGenerate.TabIndex = 0;
        buttonGenerate.Text = "生成视频";
        buttonGenerate.UseVisualStyleBackColor = false;
        buttonGenerate.Click += buttonGenerate_Click;
        //
        // openImagesDialog
        //
        openImagesDialog.Filter = "图像文件|*.png;*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.webp|PNG 图像|*.png|BMP 图像|*.bmp|JPEG 图像|*.jpg;*.jpeg|TIFF 图像|*.tif;*.tiff|WebP 图像|*.webp|所有文件|*.*";
        openImagesDialog.Multiselect = true;
        openImagesDialog.Title = "选择加入视频的图片（可多选）";
        //
        // saveVideoDialog
        //
        saveVideoDialog.AddExtension = true;
        saveVideoDialog.Filter = "MP4 视频|*.mp4|Matroska 无损视频|*.mkv|AVI 无损视频|*.avi";
        saveVideoDialog.Title = "保存视频";
        //
        // saveTimer
        //
        saveTimer.Interval = 500;
        saveTimer.Tick += saveTimer_Tick;
        //
        // StillVideoPage
        //
        AutoScaleDimensions = new SizeF(144F, 144F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(239, 242, 246);
        Controls.Add(layoutRoot);
        Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        MinimumSize = new Size(1000, 650);
        Name = "StillVideoPage";
        Size = new Size(1500, 920);
        layoutRoot.ResumeLayout(false);
        groupOutput.ResumeLayout(false);
        outputLayout.ResumeLayout(false);
        outputLayout.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericOutputWidth).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericOutputHeight).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericFramesPerSecond).EndInit();
        groupSequence.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)gridImages).EndInit();
        sequenceToolbar.ResumeLayout(false);
        sequenceToolbar.PerformLayout();
        statusLayout.ResumeLayout(false);
        actionPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel layoutRoot;
    private GroupBox groupOutput;
    private TableLayoutPanel outputLayout;
    private Label labelOutputWidth;
    private NumericUpDown numericOutputWidth;
    private Label labelOutputHeight;
    private NumericUpDown numericOutputHeight;
    private Label labelFramesPerSecond;
    private NumericUpDown numericFramesPerSecond;
    private Label labelActualSize;
    private Label labelFormat;
    private ComboBox comboFormat;
    private Label labelFormatNotice;
    private Label labelOutputPath;
    private TextBox textOutputPath;
    private Button buttonBrowseOutput;
    private GroupBox groupSequence;
    private DataGridView gridImages;
    private DataGridViewTextBoxColumn columnOrder;
    private DataGridViewTextBoxColumn columnFileName;
    private DataGridViewTextBoxColumn columnFullPath;
    private DataGridViewTextBoxColumn columnDuration;
    private FlowLayoutPanel sequenceToolbar;
    private Button buttonAddImages;
    private Button buttonRemoveImages;
    private Button buttonMoveUp;
    private Button buttonMoveDown;
    private Button buttonClearImages;
    private Label labelSequenceHint;
    private TableLayoutPanel statusLayout;
    private Label labelSummary;
    private Label labelProgress;
    private ProgressBar progressEncoding;
    private FlowLayoutPanel actionPanel;
    private Button buttonCancel;
    private Button buttonGenerate;
    private OpenFileDialog openImagesDialog;
    private SaveFileDialog saveVideoDialog;
    private ToolTip toolTip;
    private System.Windows.Forms.Timer saveTimer;
}
