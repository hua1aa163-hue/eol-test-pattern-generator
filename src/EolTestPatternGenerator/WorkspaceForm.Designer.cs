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
        labelPageDescription = new Label();
        labelPageTitle = new Label();
        navigationPanel.SuspendLayout();
        contentPanel.SuspendLayout();
        pageHeaderPanel.SuspendLayout();
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
        pageHeaderPanel.Controls.Add(labelPageDescription);
        pageHeaderPanel.Controls.Add(labelPageTitle);
        pageHeaderPanel.Dock = DockStyle.Top;
        pageHeaderPanel.Location = new Point(0, 0);
        pageHeaderPanel.Name = "pageHeaderPanel";
        pageHeaderPanel.Padding = new Padding(24, 10, 24, 8);
        pageHeaderPanel.Size = new Size(1635, 86);
        pageHeaderPanel.TabIndex = 0;
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
        navigationPanel.ResumeLayout(false);
        contentPanel.ResumeLayout(false);
        pageHeaderPanel.ResumeLayout(false);
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
    private Label labelPageDescription;
    private Label labelPageTitle;
}
