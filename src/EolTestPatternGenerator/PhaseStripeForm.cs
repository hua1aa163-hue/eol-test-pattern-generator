using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;
using System.ComponentModel;

namespace EolTestPatternGenerator;

/// <summary>
/// 周期像素数可调、可选择六种 RGB 排列的“串扰像素排列”编辑和导出窗口。
/// </summary>
public partial class PhaseStripeForm : Form
{
    private bool _updatingControls;
    private bool _isRendering;
    private bool _isExporting;
    private bool _userInterfaceInitialized;
    private string _lastExportDirectory = string.Empty;

    public PhaseStripeForm()
    {
        InitializeComponent();
    }

    /// <summary>统一工作台在两阶段关闭前只读检查后台导出状态。</summary>
    public bool IsExporting => _isExporting;

    /// <summary>
    /// 统一工作台已经提供非整数功能的直接入口，因此嵌入时移除旧跳转按钮。
    /// </summary>
    public void ConfigureAsWorkspacePage()
    {
        buttonNonIntegerFusion.Visible = false;
        groupActions.Height = 243;
        Text = "串扰像素排列";
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // 设计器只需要 InitializeComponent 创建控件，不能在设计时生成 OpenCV 图像。
        if (_userInterfaceInitialized ||
            DesignMode ||
            LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            return;
        }

        _userInterfaceInitialized = true;
        InitializeUserInterface();
    }

    private void InitializeUserInterface()
    {
        PhaseStripePreferences preferences = UserSettingsStore.Shared.Load().PhaseStripe;
        _updatingControls = true;
        try
        {
            WriteSettings(preferences.Settings);
            comboOutputFormat.SelectedIndex = Enum.IsDefined(preferences.OutputFormat)
                ? (int)preferences.OutputFormat
                : (int)ImageFormatKind.Png;
            SetNumericValue(numericQuality, preferences.Quality);
            previewControl.ShowCenterCrosshair = preferences.PreviewOverlay.ShowCenterCrosshair;
            previewControl.ShowPixelCoordinates = preferences.PreviewOverlay.ShowPixelCoordinates;
            UpdateBorderCanvasSize();
        }
        finally
        {
            _updatingControls = false;
        }

        _lastExportDirectory = DialogDirectoryResolver.ResolveExistingDirectory(
            preferences.LastExportDirectory);
        ApplyExportDialogInitialDirectory();

        toolTip.SetToolTip(numericCanvasWidth, "最终导出图像的像素宽度。");
        toolTip.SetToolTip(numericCanvasHeight, "最终导出图像的像素高度。");
        toolTip.SetToolTip(
            cycleEditor,
            "设置周期像素数、RGB 排列和倾斜角；每行位移为 3 × tan(倾斜角)。");
        toolTip.SetToolTip(buttonBatchExport, "依次生成当前周期的全部相位，文件名为相位序号。");
        toolTip.SetToolTip(previewControl, "鼠标滚轮缩放；按住鼠标左键拖动图像。");

        UpdateControlAvailability();
        UpdatePreview(resetView: true);
    }

    private void ApplyExportDialogInitialDirectory()
    {
        _lastExportDirectory = DialogDirectoryResolver.ResolveExistingDirectory(_lastExportDirectory);
        saveFileDialog.InitialDirectory = _lastExportDirectory;
        folderBrowserDialog.InitialDirectory = _lastExportDirectory;
    }

    /// <summary>将保存的相移参数安全回写到设计器输入控件。</summary>
    private void WriteSettings(PatternSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        SetNumericValue(numericCanvasWidth, settings.CanvasWidth);
        SetNumericValue(numericCanvasHeight, settings.CanvasHeight);
        regionMarginsEditor.CanvasSize = new Size(settings.CanvasWidth, settings.CanvasHeight);
        regionMarginsEditor.SetMargins(settings.GetMargins());
        cycleEditor.SetCycle(CrosstalkPixelCyclePresets.Resolve(settings), settings.PixelOrder);
        numericPhase.Maximum = Math.Max(1, cycleEditor.PeriodLength);
        SetNumericValue(numericPhase, settings.Phase);
        borderOverlayEditor.SetSettings(settings.BorderOverlay ?? new BorderOverlaySettings());
    }

    private void Parameter_ValueChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        if (ReferenceEquals(sender, numericCanvasWidth) || ReferenceEquals(sender, numericCanvasHeight))
        {
            UpdateBorderCanvasSize();
        }

        SchedulePreview();
    }

    private void cycleEditor_SettingsChanged(object? sender, EventArgs e)
    {
        numericPhase.Maximum = Math.Max(1, cycleEditor.PeriodLength);
        if (numericPhase.Value > numericPhase.Maximum)
        {
            numericPhase.Value = numericPhase.Maximum;
        }

        if (!_updatingControls)
        {
            SchedulePreview();
        }
    }

    private void comboOutputFormat_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateControlAvailability();
    }

    private void borderOverlayEditor_SettingsChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            SchedulePreview();
        }
    }

    private void UpdateBorderCanvasSize()
    {
        borderOverlayEditor.CanvasSize = new Size(
            (int)numericCanvasWidth.Value,
            (int)numericCanvasHeight.Value);
        regionMarginsEditor.CanvasSize = new Size(
            (int)numericCanvasWidth.Value,
            (int)numericCanvasHeight.Value);
    }

    private void UpdateControlAvailability()
    {
        ImageFormatKind format = SelectedFormat;
        bool usesQuality = format is ImageFormatKind.Jpeg or ImageFormatKind.WebP;
        numericQuality.Enabled = usesQuality;
        labelQuality.Enabled = usesQuality;

        labelExportHelp.Text = usesQuality
            ? "JPEG/WebP 可能改变精确的 0/255 像素；保存时会提示改用 PNG。"
            : "PNG、BMP、TIFF 为无损格式，可保持图卡的精确 0/255 像素。";
    }

    private ImageFormatKind SelectedFormat =>
        comboOutputFormat.SelectedIndex >= 0
            ? (ImageFormatKind)comboOutputFormat.SelectedIndex
            : ImageFormatKind.Png;

    private PatternSettings ReadSettings(int? phase = null)
    {
        var settings = new PatternSettings
        {
            PatternType = PatternType.PhaseStripes,
            CanvasWidth = (int)numericCanvasWidth.Value,
            CanvasHeight = (int)numericCanvasHeight.Value,
            Phase = phase ?? (int)numericPhase.Value,
            PixelCycle = cycleEditor.GetCycle(),
            BorderOverlay = borderOverlayEditor.GetSettings()
        };
        settings.SetMargins(regionMarginsEditor.GetMargins());

        // 精简界面只会产生六种排列预设；始终同步旧枚举，使旧版本也能读到当前排列。
        settings.PixelOrder = cycleEditor.SelectedOrder;

        return settings;
    }

    private ImageExportOptions ReadExportOptions()
    {
        return new ImageExportOptions
        {
            Format = SelectedFormat,
            Quality = (int)numericQuality.Value
        };
    }

    private void SchedulePreview()
    {
        previewTimer.Stop();
        previewTimer.Start();
    }

    private void previewTimer_Tick(object? sender, EventArgs e)
    {
        previewTimer.Stop();
        UpdatePreview(resetView: false);
    }

    private void buttonPreview_Click(object? sender, EventArgs e)
    {
        UpdatePreview(resetView: false);
    }

    /// <summary>
    /// 只恢复串扰页面最初版的参数快照，不读取基础图卡或其他子页面的当前值。
    /// 上次导出目录仍然保留，避免“恢复参数”意外清除用户的文件夹记忆。
    /// </summary>
    private void buttonResetDefaults_Click(object? sender, EventArgs e)
    {
        PhaseStripePreferences defaults = PhaseStripePreferences.CreateReferenceDefault();

        _updatingControls = true;
        try
        {
            WriteSettings(defaults.Settings);
            comboOutputFormat.SelectedIndex = (int)defaults.OutputFormat;
            SetNumericValue(numericQuality, defaults.Quality);
            previewControl.ShowCenterCrosshair = defaults.PreviewOverlay.ShowCenterCrosshair;
            previewControl.ShowPixelCoordinates = defaults.PreviewOverlay.ShowPixelCoordinates;
            UpdateBorderCanvasSize();
        }
        finally
        {
            _updatingControls = false;
        }

        UpdateControlAvailability();
        if (UpdatePreview(resetView: true))
        {
            statusLabel.Text = "已恢复串扰默认参数（1920 × 1080、RGB 固定 8 像素周期、18.435°）。";
        }
    }

    private void regionMarginsEditor_MarginsChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            SchedulePreview();
        }
    }

    private static void SetNumericValue(NumericUpDown control, int value)
    {
        control.Value = Math.Min(control.Maximum, Math.Max(control.Minimum, value));
    }

    private bool UpdatePreview(bool resetView)
    {
        if (_isRendering || IsDisposed)
        {
            return false;
        }

        _isRendering = true;
        previewTimer.Stop();
        UseWaitCursor = true;

        try
        {
            PatternSettings settings = ReadSettings();
            double tiltAngle = settings.PixelCycle!.ResolveTiltAngleDegrees();
            using var image = PatternGenerator.Generate(settings);
            Bitmap bitmap = MatBitmapConverter.ToBitmap(image);
            previewControl.SetImage(bitmap, preserveView: !resetView);

            labelPreviewInfo.Text =
                $"预览：{settings.CanvasWidth:N0} × {settings.CanvasHeight:N0} | " +
                $"相位 {settings.Phase}/{cycleEditor.PeriodLength} | 倾斜角 {tiltAngle:0.######}°";
            statusLabel.Text =
                $"就绪 | 周期 {cycleEditor.PeriodLength} 像素 | 图案 {settings.CalculatedOuterWidth:N0} × {settings.CalculatedOuterHeight:N0}";
            return true;
        }
        catch (Exception exception)
        {
            statusLabel.Text = $"预览失败：{exception.Message}";
            return false;
        }
        finally
        {
            UseWaitCursor = false;
            _isRendering = false;
        }
    }

    private bool EnsureLosslessExport()
    {
        if (SelectedFormat is not (ImageFormatKind.Jpeg or ImageFormatKind.WebP))
        {
            return true;
        }

        DialogResult result = MessageBox.Show(
            this,
            "串扰像素排列图要求颜色通道只含 0 和 255。JPEG/WebP 的当前编码方式可能改变像素值。\n\n是否切换为无损 PNG 后继续？",
            "请选择无损格式",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button1);

        if (result != DialogResult.Yes)
        {
            return false;
        }

        comboOutputFormat.SelectedIndex = (int)ImageFormatKind.Png;
        return true;
    }

    private void buttonSaveCurrent_Click(object? sender, EventArgs e)
    {
        if (!EnsureLosslessExport())
        {
            return;
        }

        PatternSettings settings = ReadSettings();
        ImageExportOptions exportOptions = ReadExportOptions();
        string extension = ImageFileWriter.GetExtension(exportOptions.Format);

        saveFileDialog.Filter = ImageFileWriter.GetDialogFilter(exportOptions.Format);
        saveFileDialog.DefaultExt = extension.TrimStart('.');
        saveFileDialog.FileName = $"{settings.Phase}{extension}";
        ApplyExportDialogInitialDirectory();

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _lastExportDirectory = DialogDirectoryResolver.RememberFileDirectory(
            saveFileDialog.FileName,
            _lastExportDirectory);

        try
        {
            string targetPath = ImageFileWriter.NormalizePath(saveFileDialog.FileName, exportOptions.Format);
            bool targetWasChanged = !string.Equals(
                Path.GetFullPath(saveFileDialog.FileName),
                targetPath,
                StringComparison.OrdinalIgnoreCase);
            if (targetWasChanged && File.Exists(targetPath))
            {
                DialogResult overwrite = MessageBox.Show(
                    this,
                    $"所选格式会保存为：\n{targetPath}\n\n该文件已存在，是否覆盖？",
                    "确认覆盖",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (overwrite != DialogResult.Yes)
                {
                    return;
                }
            }

            using var image = PatternGenerator.Generate(settings);
            string actualPath = ImageFileWriter.Write(targetPath, image, exportOptions);
            statusLabel.Text = $"已保存：{actualPath}";
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            statusLabel.Text = $"保存失败：{exception.Message}";
        }
    }

    private async void buttonBatchExport_Click(object? sender, EventArgs e)
    {
        if (!EnsureLosslessExport())
        {
            return;
        }

        ApplyExportDialogInitialDirectory();
        if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _lastExportDirectory = DialogDirectoryResolver.RememberDirectory(
            folderBrowserDialog.SelectedPath,
            _lastExportDirectory);

        ImageExportOptions exportOptions = ReadExportOptions();
        string outputDirectory = folderBrowserDialog.SelectedPath;
        string extension = ImageFileWriter.GetExtension(exportOptions.Format);
        int phaseCount = cycleEditor.PeriodLength;
        // 开始后台任务前冻结完整周期的全部设置，避免用户操作改变正在导出的批次。
        PatternSettings[] settings = Enumerable.Range(1, phaseCount).Select(phase => ReadSettings(phase)).ToArray();
        string[] targetPaths = settings
            .Select(item => Path.Combine(outputDirectory, $"{item.Phase}{extension}"))
            .ToArray();
        string[] existingPaths = targetPaths.Where(File.Exists).ToArray();
        if (existingPaths.Length > 0)
        {
            DialogResult overwrite = MessageBox.Show(
                this,
                $"目标目录中已有 {existingPaths.Length} 个相位文件，将被覆盖。是否继续？",
                "确认批量覆盖",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (overwrite != DialogResult.Yes)
            {
                return;
            }
        }

        _isExporting = true;
        settingsFlowPanel.Enabled = false;
        groupActions.Enabled = false;
        UseWaitCursor = true;
        statusLabel.Text = $"正在批量导出相位 1–{phaseCount}...";

        try
        {
            IReadOnlyList<string> paths = await Task.Run(() =>
            {
                Directory.CreateDirectory(outputDirectory);
                var exported = new List<string>(phaseCount);

                foreach (PatternSettings item in settings)
                {
                    using var image = PatternGenerator.Generate(item);
                    string path = Path.Combine(outputDirectory, $"{item.Phase}{extension}");
                    exported.Add(ImageFileWriter.Write(path, image, exportOptions));
                }

                return exported;
            });

            statusLabel.Text = $"批量导出完成：{paths.Count} 张 | {outputDirectory}";
            MessageBox.Show(
                this,
                $"已导出相位 1–{phaseCount}，共 {paths.Count} 张图卡。\n目录：{outputDirectory}",
                "导出完成",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "批量导出失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            statusLabel.Text = $"批量导出失败：{exception.Message}";
        }
        finally
        {
            _isExporting = false;
            settingsFlowPanel.Enabled = true;
            groupActions.Enabled = true;
            UseWaitCursor = false;
        }
    }

    private void buttonNonIntegerFusion_Click(object? sender, EventArgs e)
    {
        using var form = new NonIntegerFusionForm();
        form.ShowDialog(this);
    }

    private void PhaseStripeForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_isExporting)
        {
            e.Cancel = true;
            MessageBox.Show(
                this,
                "相位图正在导出，请等待导出完成后再关闭窗口。",
                "正在导出",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        SaveUserPreferences();
    }

    private void SaveUserPreferences()
    {
        var preferences = new PhaseStripePreferences
        {
            Settings = ReadSettings(),
            OutputFormat = SelectedFormat,
            Quality = (int)numericQuality.Value,
            LastExportDirectory = _lastExportDirectory,
            PreviewOverlay = new PreviewOverlayPreferences
            {
                ShowCenterCrosshair = previewControl.ShowCenterCrosshair,
                ShowPixelCoordinates = previewControl.ShowPixelCoordinates
            }
        };

        if (!UserSettingsStore.Shared.TryUpdateAndSave(
                root => root.PhaseStripe = preferences,
                out Exception? error))
        {
            MessageBox.Show(
                this,
                $"无法保存上次输入值：{error?.Message}",
                "保存设置失败",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void PhaseStripeForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        previewTimer.Stop();
    }
}
