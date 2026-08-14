using System.ComponentModel;
using EolTestPatternGenerator.Controls;
using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;

namespace EolTestPatternGenerator;

/// <summary>
/// 应用程序的统一工作台。各业务界面仍然是独立的 Designer 窗体，
/// 运行时以非顶级页面嵌入这里，因此原有控件可以继续在设计器中编辑。
/// </summary>
public partial class WorkspaceForm : Form
{
    private readonly Dictionary<int, Control> _pages = new();
    private bool _closingChildren;
    private bool _settingsLoadWarningShown;

    public WorkspaceForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (IsInDesignMode())
        {
            return;
        }

        UserSettingsStore settingsStore = UserSettingsStore.Shared;
        int savedIndex = settingsStore.Load().Workspace.SelectedNavigationIndex;
        if (!_settingsLoadWarningShown && settingsStore.LastLoadError is Exception loadError)
        {
            _settingsLoadWarningShown = true;
            MessageBox.Show(
                this,
                "上次输入配置无法读取，本次已暂时使用默认值。\n\n" +
                $"原文件：{settingsStore.SettingsFilePath}\n" +
                $"原因：{loadError.Message}\n\n" +
                "首次保存前程序会先在同目录创建唯一恢复备份；" +
                "如果备份失败，将拒绝覆盖原文件。",
                "配置已保护",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        listNavigation.SelectedIndex = listNavigation.Items.Count == 0
            ? -1
            : Math.Clamp(savedIndex, 0, listNavigation.Items.Count - 1);
    }

    private static bool IsInDesignMode() =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    private void listNavigation_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (listNavigation.SelectedIndex < 0 || IsInDesignMode())
        {
            return;
        }

        try
        {
            ShowPage(listNavigation.SelectedIndex);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "无法打开功能页", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowPage(int pageIndex)
    {
        Control page = GetOrCreatePage(pageIndex);

        foreach (Control existingPage in pageHost.Controls)
        {
            existingPage.Visible = ReferenceEquals(existingPage, page);
        }

        page.Visible = true;
        page.BringToFront();
        labelPageTitle.Text = listNavigation.Items[pageIndex]?.ToString() ?? "图卡工具";
        labelPageDescription.Text = GetPageDescription(pageIndex);

        // 三个 LightTools 页面共用一个 Designer 窗体，通过主导航直接定位到对应内容。
        // 嵌入模式会隐藏内部重复页签头，但不删除任何功能或 Designer 控件。
        if (page is NonIntegerFusionForm nonIntegerPage)
        {
            nonIntegerPage.SelectWorkspaceSection(pageIndex - 3);
        }
    }

    private Control GetOrCreatePage(int pageIndex)
    {
        if (_pages.TryGetValue(pageIndex, out Control? cachedPage))
        {
            return cachedPage;
        }

        Control page = pageIndex switch
        {
            0 => CreateEmbeddedForm(new MainForm(), form => form.ConfigureAsWorkspacePage()),
            1 => CreateEmbeddedForm(new PhaseStripeForm(), form => form.ConfigureAsWorkspacePage()),
            2 => CreateEmbeddedForm(new ScreenOneForm(), form => form.ConfigureAsWorkspacePage()),
            >= 3 and <= 5 => GetOrCreateNonIntegerPage(),
            6 => new StillVideoPage { Dock = DockStyle.Fill },
            _ => throw new ArgumentOutOfRangeException(nameof(pageIndex))
        };

        // 非整数的三个导航项故意指向同一个实例。
        if (pageIndex is >= 3 and <= 5)
        {
            _pages[3] = page;
            _pages[4] = page;
            _pages[5] = page;
        }
        else
        {
            _pages[pageIndex] = page;
        }

        if (!pageHost.Controls.Contains(page))
        {
            pageHost.Controls.Add(page);
            if (page is Form embeddedForm)
            {
                embeddedForm.Show();
            }
        }

        return page;
    }

    private NonIntegerFusionForm GetOrCreateNonIntegerPage()
    {
        if (_pages.TryGetValue(3, out Control? cached) && cached is NonIntegerFusionForm existing)
        {
            return existing;
        }

        return CreateEmbeddedForm(new NonIntegerFusionForm(), form => form.ConfigureAsWorkspacePage());
    }

    private static TForm CreateEmbeddedForm<TForm>(TForm form, Action<TForm> configure)
        where TForm : Form
    {
        configure(form);
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        form.MinimumSize = Size.Empty;
        form.ShowInTaskbar = false;
        return form;
    }

    private static string GetPageDescription(int pageIndex) => pageIndex switch
    {
        0 => "基础图卡、纯色图、导入图片及白框叠加",
        1 => "按周期配置每个像素点亮的 R/G/B 通道",
        2 => "3D显示器图卡左右区域及三张参考图卡",
        3 => "MATLAB 兼容的非整数覆盖率融合",
        4 => "离散光源图、分组图片及 LightTools 光源文件",
        5 => "将任意图片转换为单文件或 R/G/B 光源文件",
        6 => "按指定顺序和静止时长生成 MP4 或无损视频",
        _ => string.Empty
    };

    private void WorkspaceForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_closingChildren)
        {
            return;
        }

        _closingChildren = true;
        try
        {
            // 第一阶段只读检查所有传统页面；任一后台导出仍在运行时，不能先释放其他页面。
            IReadOnlyList<string> busyPages = GetBusyPageNames();
            if (busyPages.Count > 0)
            {
                MessageBox.Show(
                    this,
                    $"以下功能仍在导出文件：{string.Join("、", busyPages)}。\n请等待导出完成后再关闭工作台。",
                    "正在导出",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                e.Cancel = true;
                return;
            }

            // 视频页可以因仍在编码或当前时长无效而拒绝关闭。必须先取得许可，
            // 否则取消关闭后，导航缓存会留下已经 Dispose 的其他页面实例。
            foreach (StillVideoPage videoPage in _pages.Values.OfType<StillVideoPage>().Distinct())
            {
                if (!videoPage.PrepareToClose())
                {
                    e.Cancel = true;
                    return;
                }
            }

            foreach (Form child in _pages.Values.OfType<Form>().Distinct().ToArray())
            {
                child.Close();
                if (!child.IsDisposed)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }
        finally
        {
            _closingChildren = false;
        }

        if (!e.Cancel)
        {
            SaveWorkspacePreferences();
        }
    }

    private IReadOnlyList<string> GetBusyPageNames()
    {
        var names = new List<string>();
        foreach (Control page in _pages.Values.Distinct())
        {
            switch (page)
            {
                case MainForm { IsExporting: true }:
                    names.Add("基础图卡");
                    break;
                case PhaseStripeForm { IsExporting: true }:
                    names.Add("串扰像素排列");
                    break;
                case ScreenOneForm { IsExporting: true }:
                    names.Add("3D显示器图卡");
                    break;
                case NonIntegerFusionForm { IsExporting: true }:
                    names.Add("非整数融合 / 离散光源 / 图片转 LightTools");
                    break;
            }
        }

        return names;
    }

    private void SaveWorkspacePreferences()
    {
        int selectedIndex = listNavigation.SelectedIndex < 0 ? 0 : listNavigation.SelectedIndex;
        if (!UserSettingsStore.Shared.TryUpdateAndSave(
                preferences =>
                {
                    preferences.Workspace ??= new WorkspacePreferences();
                    preferences.Workspace.SelectedNavigationIndex = selectedIndex;
                },
                out Exception? error))
        {
            MessageBox.Show(
                this,
                $"无法保存工作台上次打开页面：{error?.Message}",
                "保存设置失败",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
