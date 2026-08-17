using System.ComponentModel;
using EolTestPatternGenerator.Controls;
using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Projection;
using EolTestPatternGenerator.Services;

namespace EolTestPatternGenerator;

/// <summary>
/// 应用程序的统一工作台。各业务界面仍然是独立的 Designer 窗体，
/// 运行时以非顶级页面嵌入这里，因此原有控件可以继续在设计器中编辑。
/// </summary>
public partial class WorkspaceForm : Form
{
    private readonly Dictionary<int, Control> _pages = new();
    private readonly IDesktopDisplayService _desktopDisplayService;
    private readonly RealtimeProjectionController _projectionController;
    private readonly SemaphoreSlim _projectionLifecycleGate = new(1, 1);
    private ImagePreviewControl? _activePreviewControl;
    private long _projectionPreviewEpoch;
    private long _liveProjectionRequestEpoch;
    private bool _closePreparationRunning;
    private bool _closeApproved;
    private bool _settingsLoadWarningShown;
    private bool _suppressLiveProjectionEvent;
    private bool _projectionUiBusy;
    private bool _projectionClosing;
    private bool _projectionDisposed;

    public WorkspaceForm()
        : this(new DesktopDisplayService())
    {
    }

    internal WorkspaceForm(IDesktopDisplayService desktopDisplayService)
    {
        InitializeComponent();
        _desktopDisplayService = desktopDisplayService ?? throw new ArgumentNullException(nameof(desktopDisplayService));
        // 是否恢复壁纸由界面复选框决定，不能让 Dispose 在用户取消恢复后再次强制恢复。
        _projectionController = new RealtimeProjectionController(
            _desktopDisplayService,
            restoreOriginalOnDispose: false);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (IsInDesignMode())
        {
            return;
        }

        UserSettingsStore settingsStore = UserSettingsStore.Shared;
        WorkspacePreferences workspacePreferences = settingsStore.Load().Workspace;
        comboProjectionTopology.SelectedIndex = Math.Clamp(
            (int)workspacePreferences.ProjectionTopology,
            0,
            comboProjectionTopology.Items.Count - 1);
        checkRestoreWallpaper.Checked = workspacePreferences.RestoreWallpaperOnStop;
        // 投图会修改 Windows 桌面状态，因此每次启动都保持关闭，由用户本次明确开启。
        SetLiveProjectionChecked(false);
        int savedIndex = WorkspaceNavigationPages.ResolveSelectedIndex(
            workspacePreferences,
            listNavigation.Items.Count);
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

        listNavigation.SelectedIndex = savedIndex;
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
            nonIntegerPage.SelectWorkspaceSection(pageIndex - 4);
        }

        AttachProjectionPreview(page);
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
            1 => CreateEmbeddedForm(new ScreenOneForm(), form => form.ConfigureAsWorkspacePage()),
            2 => CreateEmbeddedForm(new PhaseStripeForm(), form => form.ConfigureAsWorkspacePage()),
            3 => CreateEmbeddedForm(new CrosstalkGridForm(), form => form.ConfigureAsWorkspacePage()),
            >= 4 and <= 6 => GetOrCreateNonIntegerPage(),
            7 => new StillVideoPage { Dock = DockStyle.Fill },
            _ => throw new ArgumentOutOfRangeException(nameof(pageIndex))
        };

        // 非整数的三个导航项故意指向同一个实例。
        if (pageIndex is >= 4 and <= 6)
        {
            _pages[4] = page;
            _pages[5] = page;
            _pages[6] = page;
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
        if (_pages.TryGetValue(4, out Control? cached) && cached is NonIntegerFusionForm existing)
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
        1 => "3D显示器图卡左右区域及三张参考图卡",
        2 => "周期可调的六种 RGB 排列、相位与倾斜角",
        3 => "按水平方向周期位置逐项选择 R/G/B 通道、横向步进、相位与倾斜角",
        4 => "MATLAB 兼容的非整数覆盖率融合",
        5 => "离散光源图、分组图片及 LightTools 光源文件",
        6 => "将任意图片转换为单文件或 R/G/B 光源文件",
        7 => "按指定顺序和静止时长生成 MP4 或无损视频",
        _ => string.Empty
    };

    private void AttachProjectionPreview(Control page)
    {
        ImagePreviewControl? nextPreview = FindPreviewControl(page);
        // 每次导航都进入新投图世代，即使三个 LightTools 页面复用同一个预览控件，
        // 上一功能区尚未排队的旧图也不能在切页后覆盖当前图。
        _projectionPreviewEpoch++;
        if (ReferenceEquals(_activePreviewControl, nextPreview))
        {
            UpdateProjectionControls();
            if (checkLiveProjection.Checked && nextPreview?.HasImage == true)
            {
                _ = ProjectActivePreviewAsync(showErrorDialog: false, requireLive: true);
            }

            return;
        }

        if (_activePreviewControl is not null)
        {
            _activePreviewControl.ImageChanged -= activePreviewControl_ImageChanged;
        }

        _activePreviewControl = nextPreview;
        if (_activePreviewControl is not null)
        {
            _activePreviewControl.ImageChanged += activePreviewControl_ImageChanged;
        }

        UpdateProjectionControls();
        if (checkLiveProjection.Checked && _activePreviewControl?.HasImage == true)
        {
            _ = ProjectActivePreviewAsync(showErrorDialog: false, requireLive: true);
        }
    }

    private static ImagePreviewControl? FindPreviewControl(Control root)
    {
        if (root is ImagePreviewControl preview)
        {
            return preview;
        }

        foreach (Control child in root.Controls)
        {
            ImagePreviewControl? found = FindPreviewControl(child);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private async void activePreviewControl_ImageChanged(object? sender, EventArgs e)
    {
        // 同一页面快速生成多张预览时，只有最后一次变化保留当前世代。
        long imageEpoch = ++_projectionPreviewEpoch;
        UpdateProjectionControls();
        if (!checkLiveProjection.Checked)
        {
            return;
        }

        if (sender is ImagePreviewControl source)
        {
            await ProjectPreviewAsync(
                source,
                imageEpoch,
                showErrorDialog: false,
                requireLive: true);
        }
    }

    private async void buttonApplyTopology_Click(object? sender, EventArgs e)
    {
        DisplayTopology topology = GetSelectedTopology();
        SetProjectionUiBusy(true);
        await _projectionLifecycleGate.WaitAsync();
        try
        {
            if (_projectionClosing)
            {
                return;
            }

            await Task.Run(() => _desktopDisplayService.ApplyTopology(topology));
            labelProjectionStatus.Text = topology == DisplayTopology.None
                ? "显示模式保持不变。"
                : $"已应用显示模式：{comboProjectionTopology.SelectedItem}。";
        }
        catch (Exception exception)
        {
            ShowProjectionError("无法切换显示模式", exception);
        }
        finally
        {
            _projectionLifecycleGate.Release();
            SetProjectionUiBusy(false);
        }
    }

    private async void buttonProjectCurrent_Click(object? sender, EventArgs e)
    {
        await ProjectActivePreviewAsync(showErrorDialog: true);
    }

    private async void checkLiveProjection_CheckedChanged(object? sender, EventArgs e)
    {
        if (_suppressLiveProjectionEvent || _projectionClosing)
        {
            return;
        }

        long liveRequestEpoch = ++_liveProjectionRequestEpoch;
        if (!checkLiveProjection.Checked)
        {
            await StopProjectionAsync(checkRestoreWallpaper.Checked, showErrorDialog: true);
            return;
        }

        if (_activePreviewControl?.HasImage != true)
        {
            SetLiveProjectionChecked(false);
            MessageBox.Show(
                this,
                "当前功能页还没有可投出的预览图片。请先生成预览，再开启实时投图。",
                "没有预览图片",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        ImagePreviewControl? source = _activePreviewControl;
        long epoch = _projectionPreviewEpoch;
        bool projected = await ProjectPreviewAsync(
            source,
            epoch,
            showErrorDialog: true,
            requireLive: true);
        // 等待期间可能关闭实时模式、切换页面或生成了更新的预览；旧请求无权改动当前开关。
        if (_projectionClosing ||
            liveRequestEpoch != _liveProjectionRequestEpoch ||
            !checkLiveProjection.Checked ||
            !ReferenceEquals(source, _activePreviewControl) ||
            epoch != _projectionPreviewEpoch)
        {
            return;
        }

        if (!projected)
        {
            SetLiveProjectionChecked(false);
            await StopProjectionAsync(checkRestoreWallpaper.Checked, showErrorDialog: true);
        }
        else
        {
            labelProjectionStatus.Text = "实时投图已开启；参数变化后会自动更新 Windows 壁纸。";
        }
    }

    private async void buttonStopProjection_Click(object? sender, EventArgs e)
    {
        _liveProjectionRequestEpoch++;
        SetLiveProjectionChecked(false);
        await StopProjectionAsync(checkRestoreWallpaper.Checked, showErrorDialog: true);
    }

    private async Task<bool> ProjectActivePreviewAsync(bool showErrorDialog, bool requireLive = false)
    {
        ImagePreviewControl? source = _activePreviewControl;
        long epoch = _projectionPreviewEpoch;
        return await ProjectPreviewAsync(source, epoch, showErrorDialog, requireLive);
    }

    private async Task<bool> ProjectPreviewAsync(
        ImagePreviewControl? source,
        long epoch,
        bool showErrorDialog,
        bool requireLive)
    {
        try
        {
            if (source?.HasImage != true)
            {
                UpdateProjectionControls();
                if (showErrorDialog)
                {
                    MessageBox.Show(
                        this,
                        "当前功能页还没有可投出的预览图片。",
                        "没有预览图片",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                return false;
            }

            if (!await EnsureProjectionStartedAsync())
            {
                return false;
            }

            // 实时更新在等待启动期间可能已被用户关闭；这种旧请求不能重新投图。
            // 页面也可能已经切换，旧页面的延迟任务不能覆盖新页面。
            if (_projectionClosing ||
                !ReferenceEquals(source, _activePreviewControl) ||
                epoch != _projectionPreviewEpoch ||
                (requireLive && !checkLiveProjection.Checked))
            {
                return false;
            }

            // 等待生命周期锁和最新世代检查通过后才复制大图，避免慢速拓扑切换期间
            // 每次预览变化都提前持有一份完整位图。复制也放在 try 内，统一处理 GDI+/内存错误。
            using Bitmap? image = source.CloneCurrentImage();
            if (image is null)
            {
                UpdateProjectionControls();
                return false;
            }

            ProjectionFrameResult result = await _projectionController.ProjectLatest(image);
            if (!_projectionClosing &&
                ReferenceEquals(source, _activePreviewControl) &&
                epoch == _projectionPreviewEpoch &&
                result == ProjectionFrameResult.Applied)
            {
                labelProjectionStatus.Text = checkLiveProjection.Checked
                    ? "实时投图已更新。"
                    : "当前预览已投为 Windows 壁纸；点击“停止投图”可恢复。";
            }

            UpdateProjectionControls();
            return result != ProjectionFrameResult.Stopped;
        }
        catch (Exception exception)
        {
            if (!_projectionClosing)
            {
                labelProjectionStatus.Text = $"投图失败：{exception.Message}";
                if (showErrorDialog)
                {
                    ShowProjectionError("无法投图", exception);
                }
            }

            UpdateProjectionControls();
            return false;
        }
    }

    private async Task<bool> EnsureProjectionStartedAsync()
    {
        await _projectionLifecycleGate.WaitAsync();
        try
        {
            if (_projectionClosing)
            {
                return false;
            }

            if (!_projectionController.IsRunning)
            {
                await Task.Run(() => _projectionController.Start(captureOriginal: true));
            }

            return true;
        }
        finally
        {
            _projectionLifecycleGate.Release();
        }
    }

    private async Task StopProjectionAsync(bool restoreOriginal, bool showErrorDialog)
    {
        await _projectionLifecycleGate.WaitAsync();
        try
        {
            await _projectionController.StopAsync(restoreOriginal);
            labelProjectionStatus.Text = restoreOriginal
                ? (_projectionController.LastProjectedWallpaperPath is null
                    ? "投图已停止，并已恢复原壁纸。"
                    : "投图已停止；原壁纸路径不可用，当前壁纸未改动。")
                : "投图已停止；按当前选择保留最后一张投图壁纸。";
        }
        catch (Exception exception)
        {
            labelProjectionStatus.Text = $"停止投图失败：{exception.Message}";
            if (showErrorDialog)
            {
                ShowProjectionError("无法停止投图", exception);
            }
            else
            {
                throw;
            }
        }
        finally
        {
            _projectionLifecycleGate.Release();
            UpdateProjectionControls();
        }
    }

    private DisplayTopology GetSelectedTopology()
    {
        int selectedIndex = comboProjectionTopology.SelectedIndex;
        return Enum.IsDefined(typeof(DisplayTopology), selectedIndex)
            ? (DisplayTopology)selectedIndex
            : DisplayTopology.None;
    }

    private void SetLiveProjectionChecked(bool value)
    {
        _suppressLiveProjectionEvent = true;
        try
        {
            checkLiveProjection.Checked = value;
        }
        finally
        {
            _suppressLiveProjectionEvent = false;
        }
    }

    private void SetProjectionUiBusy(bool value)
    {
        _projectionUiBusy = value;
        UpdateProjectionControls();
    }

    private void UpdateProjectionControls()
    {
        if (IsDisposed || Disposing)
        {
            return;
        }

        bool hasPreview = _activePreviewControl?.HasImage == true;
        comboProjectionTopology.Enabled = !_projectionUiBusy;
        buttonApplyTopology.Enabled = !_projectionUiBusy;
        buttonProjectCurrent.Enabled = !_projectionUiBusy && hasPreview;
        checkLiveProjection.Enabled = !_projectionUiBusy && _activePreviewControl is not null;
        buttonStopProjection.Enabled = !_projectionUiBusy &&
                                       (_projectionController.IsRunning ||
                                        _projectionController.HasOriginalWallpaper ||
                                        _projectionController.LastProjectedWallpaperPath is not null);
    }

    private void ShowProjectionError(string title, Exception exception)
    {
        MessageBox.Show(
            this,
            exception.Message,
            title,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private async void WorkspaceForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_closeApproved)
        {
            return;
        }

        // 第一次关闭只启动异步准备；准备完成后再由 _closeApproved 的第二次 Close 真正关闭。
        e.Cancel = true;
        if (_closePreparationRunning)
        {
            return;
        }

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
            return;
        }

        // 视频页可以因仍在编码或当前时长无效而拒绝关闭。此时不能先停止投图或释放其他页。
        foreach (StillVideoPage videoPage in _pages.Values.OfType<StillVideoPage>().Distinct())
        {
            if (!videoPage.PrepareToClose())
            {
                return;
            }
        }

        _closePreparationRunning = true;
        _projectionClosing = true;
        _liveProjectionRequestEpoch++;
        Enabled = false;
        UseWaitCursor = true;
        try
        {
            if (_activePreviewControl is not null)
            {
                _activePreviewControl.ImageChanged -= activePreviewControl_ImageChanged;
                _activePreviewControl = null;
                _projectionPreviewEpoch++;
            }

            SetLiveProjectionChecked(false);
            try
            {
                await StopProjectionAsync(checkRestoreWallpaper.Checked, showErrorDialog: false);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    this,
                    $"程序将继续关闭，但停止投图时出现错误：{exception.Message}",
                    "投图清理失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            foreach (Form child in _pages.Values.OfType<Form>().Distinct().ToArray())
            {
                child.Close();
                if (!child.IsDisposed)
                {
                    throw new InvalidOperationException("有功能页拒绝关闭，工作台保持打开。", null);
                }
            }

            SaveWorkspacePreferences();
            _closeApproved = true;
            BeginInvoke(new Action(Close));
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "无法关闭工作台",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        finally
        {
            if (!_closeApproved)
            {
                _projectionClosing = false;
                Control? visiblePage = pageHost.Controls.Cast<Control>().FirstOrDefault(control => control.Visible);
                if (visiblePage is not null && !visiblePage.IsDisposed)
                {
                    AttachProjectionPreview(visiblePage);
                }

                Enabled = true;
                UseWaitCursor = false;
            }

            _closePreparationRunning = false;
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
                case CrosstalkGridForm { IsExporting: true }:
                    names.Add("串扰像素排列2");
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
                    preferences.Workspace.SelectedNavigationPageId =
                        WorkspaceNavigationPages.GetPageId(selectedIndex);
                    preferences.Workspace.ProjectionTopology = GetSelectedTopology();
                    preferences.Workspace.RestoreWallpaperOnStop = checkRestoreWallpaper.Checked;
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

    private void WorkspaceForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        if (_projectionDisposed)
        {
            return;
        }

        _projectionDisposed = true;
        if (_activePreviewControl is not null)
        {
            _activePreviewControl.ImageChanged -= activePreviewControl_ImageChanged;
            _activePreviewControl = null;
        }

        try
        {
            _projectionController.Dispose();
        }
        finally
        {
            _projectionLifecycleGate.Dispose();
        }
    }
}
