#nullable disable

namespace EolTestPatternGenerator;

partial class WorkspaceForm
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
        navigationPanel = new Panel();
        listNavigation = new ListBox();
        labelNavigation = new Label();
        labelApplication = new Label();
        contentPanel = new Panel();
        pageHost = new Panel();
        pageHeaderPanel = new Panel();
        projectionPanel = new Panel();
        labelProjection = new Label();
        comboProjectionTopology = new ComboBox();
        buttonApplyTopology = new Button();
        buttonProjectCurrent = new Button();
        checkLiveProjection = new CheckBox();
        buttonStopProjection = new Button();
        checkRestoreWallpaper = new CheckBox();
        labelProjectionStatus = new Label();
        labelPageDescription = new Label();
        labelPageTitle = new Label();
        navigationPanel.SuspendLayout();
        contentPanel.SuspendLayout();
        pageHeaderPanel.SuspendLayout();
        projectionPanel.SuspendLayout();
        SuspendLayout();
        //
        // navigationPanel
        //
        navigationPanel.BackColor = Color.FromArgb(35, 43, 58);
        navigationPanel.Controls.Add(listNavigation);
        navigationPanel.Controls.Add(labelNavigation);
        navigationPanel.Controls.Add(labelApplication);
        navigationPanel.Dock = DockStyle.Left;
        navigationPanel.Location = new Point(0, 0);
        navigationPanel.Name = "navigationPanel";
        navigationPanel.Padding = new Padding(18, 24, 18, 24);
        navigationPanel.Size = new Size(285, 1120);
        navigationPanel.TabIndex = 0;
        //
        // listNavigation
        //
        listNavigation.BackColor = Color.FromArgb(35, 43, 58);
        listNavigation.BorderStyle = BorderStyle.None;
        listNavigation.Dock = DockStyle.Fill;
        listNavigation.Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        listNavigation.ForeColor = Color.White;
        listNavigation.FormattingEnabled = true;
        listNavigation.IntegralHeight = false;
        listNavigation.ItemHeight = 30;
        listNavigation.Items.AddRange(new object[] { "基础图卡", "3D显示器图卡", "串扰像素排列", "非整数连续融合", "离散光源", "图片转 LightTools", "图片转视频" });
        listNavigation.Location = new Point(18, 135);
        listNavigation.Name = "listNavigation";
        listNavigation.Size = new Size(249, 961);
        listNavigation.TabIndex = 2;
        listNavigation.SelectedIndexChanged += listNavigation_SelectedIndexChanged;
        //
        // labelNavigation
        //
        labelNavigation.Dock = DockStyle.Top;
        labelNavigation.ForeColor = Color.FromArgb(174, 185, 204);
        labelNavigation.Location = new Point(18, 86);
        labelNavigation.Name = "labelNavigation";
        labelNavigation.Padding = new Padding(4, 8, 0, 0);
        labelNavigation.Size = new Size(249, 49);
        labelNavigation.TabIndex = 1;
        labelNavigation.Text = "功能导航";
        //
        // labelApplication
        //
        labelApplication.Dock = DockStyle.Top;
        labelApplication.Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        labelApplication.ForeColor = Color.White;
        labelApplication.Location = new Point(18, 24);
        labelApplication.Name = "labelApplication";
        labelApplication.Size = new Size(249, 62);
        labelApplication.TabIndex = 0;
        labelApplication.Text = "EOL 图卡工作台";
        labelApplication.TextAlign = ContentAlignment.MiddleLeft;
        //
        // contentPanel
        //
        contentPanel.Controls.Add(pageHost);
        contentPanel.Controls.Add(pageHeaderPanel);
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.Location = new Point(285, 0);
        contentPanel.Name = "contentPanel";
        contentPanel.Size = new Size(1635, 1120);
        contentPanel.TabIndex = 1;
        //
        // pageHost
        //
        pageHost.BackColor = Color.FromArgb(239, 242, 246);
        pageHost.Dock = DockStyle.Fill;
        pageHost.Location = new Point(0, 86);
        pageHost.Name = "pageHost";
        pageHost.Size = new Size(1635, 1034);
        pageHost.TabIndex = 1;
        //
        // pageHeaderPanel
        //
        pageHeaderPanel.BackColor = Color.White;
        pageHeaderPanel.Controls.Add(projectionPanel);
        pageHeaderPanel.Controls.Add(labelPageDescription);
        pageHeaderPanel.Controls.Add(labelPageTitle);
        pageHeaderPanel.Dock = DockStyle.Top;
        pageHeaderPanel.Location = new Point(0, 0);
        pageHeaderPanel.Name = "pageHeaderPanel";
        pageHeaderPanel.Padding = new Padding(24, 10, 24, 8);
        pageHeaderPanel.Size = new Size(1635, 86);
        pageHeaderPanel.TabIndex = 0;
        //
        // projectionPanel
        //
        projectionPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        projectionPanel.Controls.Add(labelProjectionStatus);
        projectionPanel.Controls.Add(checkRestoreWallpaper);
        projectionPanel.Controls.Add(buttonStopProjection);
        projectionPanel.Controls.Add(checkLiveProjection);
        projectionPanel.Controls.Add(buttonProjectCurrent);
        projectionPanel.Controls.Add(buttonApplyTopology);
        projectionPanel.Controls.Add(comboProjectionTopology);
        projectionPanel.Controls.Add(labelProjection);
        projectionPanel.Location = new Point(755, 8);
        projectionPanel.Name = "projectionPanel";
        projectionPanel.Size = new Size(856, 70);
        projectionPanel.TabIndex = 2;
        //
        // labelProjection
        //
        labelProjection.AutoSize = true;
        labelProjection.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        labelProjection.Location = new Point(0, 8);
        labelProjection.Name = "labelProjection";
        labelProjection.Size = new Size(110, 24);
        labelProjection.TabIndex = 0;
        labelProjection.Text = "Windows 投图";
        //
        // comboProjectionTopology
        //
        comboProjectionTopology.DropDownStyle = ComboBoxStyle.DropDownList;
        comboProjectionTopology.FormattingEnabled = true;
        comboProjectionTopology.Items.AddRange(new object[] { "保持当前模式", "仅电脑屏幕", "复制屏幕", "仅第二屏幕", "扩展屏幕" });
        comboProjectionTopology.Location = new Point(116, 4);
        comboProjectionTopology.Name = "comboProjectionTopology";
        comboProjectionTopology.Size = new Size(142, 32);
        comboProjectionTopology.TabIndex = 1;
        //
        // buttonApplyTopology
        //
        buttonApplyTopology.Location = new Point(266, 3);
        buttonApplyTopology.Name = "buttonApplyTopology";
        buttonApplyTopology.Size = new Size(86, 34);
        buttonApplyTopology.TabIndex = 2;
        buttonApplyTopology.Text = "应用模式";
        buttonApplyTopology.UseVisualStyleBackColor = true;
        buttonApplyTopology.Click += buttonApplyTopology_Click;
        //
        // buttonProjectCurrent
        //
        buttonProjectCurrent.Enabled = false;
        buttonProjectCurrent.Location = new Point(360, 3);
        buttonProjectCurrent.Name = "buttonProjectCurrent";
        buttonProjectCurrent.Size = new Size(122, 34);
        buttonProjectCurrent.TabIndex = 3;
        buttonProjectCurrent.Text = "投图当前预览";
        buttonProjectCurrent.UseVisualStyleBackColor = true;
        buttonProjectCurrent.Click += buttonProjectCurrent_Click;
        //
        // checkLiveProjection
        //
        checkLiveProjection.AutoSize = true;
        checkLiveProjection.Location = new Point(491, 8);
        checkLiveProjection.Name = "checkLiveProjection";
        checkLiveProjection.Size = new Size(90, 28);
        checkLiveProjection.TabIndex = 4;
        checkLiveProjection.Text = "实时投图";
        checkLiveProjection.UseVisualStyleBackColor = true;
        checkLiveProjection.CheckedChanged += checkLiveProjection_CheckedChanged;
        //
        // buttonStopProjection
        //
        buttonStopProjection.Enabled = false;
        buttonStopProjection.Location = new Point(589, 3);
        buttonStopProjection.Name = "buttonStopProjection";
        buttonStopProjection.Size = new Size(88, 34);
        buttonStopProjection.TabIndex = 5;
        buttonStopProjection.Text = "停止投图";
        buttonStopProjection.UseVisualStyleBackColor = true;
        buttonStopProjection.Click += buttonStopProjection_Click;
        //
        // checkRestoreWallpaper
        //
        checkRestoreWallpaper.AutoSize = true;
        checkRestoreWallpaper.Checked = true;
        checkRestoreWallpaper.CheckState = CheckState.Checked;
        checkRestoreWallpaper.Location = new Point(687, 8);
        checkRestoreWallpaper.Name = "checkRestoreWallpaper";
        checkRestoreWallpaper.Size = new Size(162, 28);
        checkRestoreWallpaper.TabIndex = 6;
        checkRestoreWallpaper.Text = "停止时恢复原壁纸";
        checkRestoreWallpaper.UseVisualStyleBackColor = true;
        //
        // labelProjectionStatus
        //
        labelProjectionStatus.ForeColor = Color.DimGray;
        labelProjectionStatus.Location = new Point(116, 42);
        labelProjectionStatus.Name = "labelProjectionStatus";
        labelProjectionStatus.Size = new Size(733, 24);
        labelProjectionStatus.TabIndex = 7;
        labelProjectionStatus.Text = "投图已关闭；投出原始图卡，不包含预览十字线和坐标。";
        //
        // labelPageDescription
        //
        labelPageDescription.Dock = DockStyle.Fill;
        labelPageDescription.ForeColor = Color.DimGray;
        labelPageDescription.Location = new Point(24, 49);
        labelPageDescription.Name = "labelPageDescription";
        labelPageDescription.Size = new Size(1587, 29);
        labelPageDescription.TabIndex = 1;
        labelPageDescription.Text = "选择左侧功能开始";
        //
        // labelPageTitle
        //
        labelPageTitle.Dock = DockStyle.Top;
        labelPageTitle.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold, GraphicsUnit.Point);
        labelPageTitle.Location = new Point(24, 10);
        labelPageTitle.Name = "labelPageTitle";
        labelPageTitle.Size = new Size(1587, 39);
        labelPageTitle.TabIndex = 0;
        labelPageTitle.Text = "基础图卡";
        //
        // WorkspaceForm
        //
        AutoScaleDimensions = new SizeF(144F, 144F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1920, 1120);
        Controls.Add(contentPanel);
        Controls.Add(navigationPanel);
        Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        MinimumSize = new Size(1500, 900);
        Name = "WorkspaceForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "EOL 图卡工作台 - OpenCvSharp";
        FormClosing += WorkspaceForm_FormClosing;
        FormClosed += WorkspaceForm_FormClosed;
        navigationPanel.ResumeLayout(false);
        contentPanel.ResumeLayout(false);
        pageHeaderPanel.ResumeLayout(false);
        projectionPanel.ResumeLayout(false);
        projectionPanel.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private Panel navigationPanel;
    private ListBox listNavigation;
    private Label labelNavigation;
    private Label labelApplication;
    private Panel contentPanel;
    private Panel pageHost;
    private Panel pageHeaderPanel;
    private Panel projectionPanel;
    private Label labelProjection;
    private ComboBox comboProjectionTopology;
    private Button buttonApplyTopology;
    private Button buttonProjectCurrent;
    private CheckBox checkLiveProjection;
    private Button buttonStopProjection;
    private CheckBox checkRestoreWallpaper;
    private Label labelProjectionStatus;
    private Label labelPageDescription;
    private Label labelPageTitle;
}
