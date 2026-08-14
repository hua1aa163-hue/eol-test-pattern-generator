using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;
using System.ComponentModel;

namespace EolTestPatternGenerator;

public partial class MainForm : Form
{
    // 与 MainForm.Designer.cs 中 comboPattern.Items 的顺序一一对应。
    private static readonly PatternType[] MainPatternTypes =
    [
        PatternType.Border,
        PatternType.NinePointGrid,
        PatternType.DistortionGrid,
        PatternType.CorrectionCross,
        PatternType.WhiteRectangle,
        PatternType.Black,
        PatternType.FullWhite,
        PatternType.FullRed,
        PatternType.FullGreen,
        PatternType.FullBlue,
        PatternType.ImportedImage
    ];

    // 每种图卡保留独立参数快照；切换下拉项时不会丢失用户刚才的调整。
    private readonly Dictionary<PatternType, PatternSettings> _patternProfiles = new();
    private PatternType _activePatternType = PatternType.Border;
    private bool _updatingControls;
    private bool _isRendering;
    private bool _userInterfaceInitialized;
    private int _activeExports;

    public MainForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // WinForms 设计器也会调用无参构造函数。等 Site 建立后的 OnLoad 再判断，
        // 避免设计时加载 OpenCV 原生库并生成整张预览图。
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
        toolTip.SetToolTip(numericCanvasWidth, "最终导出图片的像素宽度；界面上限 8192，画布总像素上限 4000 万。");
        toolTip.SetToolTip(numericCanvasHeight, "最终导出图片的像素高度；界面上限 8192，画布总像素上限 4000 万。");
        toolTip.SetToolTip(numericPatternX, "矩形图为左上角 X；点阵图为第一个圆心 X。允许负数。");
        toolTip.SetToolTip(numericPatternY, "矩形图为左上角 Y；点阵图为第一个圆心 Y。允许负数。");
        toolTip.SetToolTip(numericPatternWidth, "矩形图为区域宽度；点阵图为第一个到最后一个圆心的 X 距离。");
        toolTip.SetToolTip(numericPatternHeight, "矩形图为区域高度；点阵图为第一个到最后一个圆心的 Y 距离。");
        toolTip.SetToolTip(numericDotRadius, "半径 4 对应 9 像素直径；半径 0 对应单像素点。");
        toolTip.SetToolTip(buttonBatchExport, "生成主窗口的 10 张图卡；RGB 相移 8 张请在独立工具中导出。");
        toolTip.SetToolTip(buttonExportScreen1, "打开1号屏独立工具，可调整画布分辨率及左右矩形的位置和大小。");

        ResetProfilesToDefaults();
        LoadUserPreferences();

        _activePatternType = SelectedPatternType;
        LoadActiveProfile();
        UpdatePreview();
    }

    private PatternType SelectedPatternType =>
        comboPattern.SelectedIndex >= 0 && comboPattern.SelectedIndex < MainPatternTypes.Length
            ? MainPatternTypes[comboPattern.SelectedIndex]
            : PatternType.Border;

    private void ResetProfilesToDefaults()
    {
        _patternProfiles.Clear();
        foreach (PatternType type in Enum.GetValues<PatternType>())
        {
            _patternProfiles[type] = PatternPresets.Create(type);
        }
    }

    /// <summary>加载上次关闭程序时保存的全部主窗口输入。</summary>
    private void LoadUserPreferences()
    {
        MainPreferences preferences = UserSettingsStore.Shared.Load().Main;
        foreach ((PatternType type, PatternSettings settings) in preferences.PatternProfiles)
        {
            if (Enum.IsDefined(type) && settings is not null)
            {
                _patternProfiles[type] = settings.Clone();
            }
        }

        _updatingControls = true;
        try
        {
            int patternIndex = Array.IndexOf(MainPatternTypes, preferences.SelectedPatternType);
            comboPattern.SelectedIndex = patternIndex >= 0 ? patternIndex : 0;
            comboOutputFormat.SelectedIndex = Enum.IsDefined(preferences.OutputFormat)
                ? (int)preferences.OutputFormat
                : (int)ImageFormatKind.Png;
            SetNumericValue(numericQuality, preferences.Quality);
            imagePreviewControl.ShowCenterCrosshair = preferences.PreviewOverlay.ShowCenterCrosshair;
            imagePreviewControl.ShowPixelCoordinates = preferences.PreviewOverlay.ShowPixelCoordinates;
        }
        finally
        {
            _updatingControls = false;
        }
    }

    private void comboPattern_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        StoreControlsInProfiles(_activePatternType);
        _activePatternType = SelectedPatternType;
        LoadActiveProfile();
        UpdatePreview();
    }

    private void comboOutputFormat_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateControlAvailability();
    }

    private void Parameter_ValueChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        borderOverlayEditor.CanvasSize = new Size(
            (int)numericCanvasWidth.Value,
            (int)numericCanvasHeight.Value);
        StoreControlsInProfiles(_activePatternType);
        SchedulePreview();
    }

    private void LoadActiveProfile()
    {
        PatternSettings settings = _patternProfiles[_activePatternType].Clone();
        WriteSettings(settings);
    }

    private void StoreControlsInProfiles(PatternType type)
    {
        PatternSettings settings = ReadSettings(type);
        _patternProfiles[type] = settings.Clone();

        foreach (PatternSettings profile in _patternProfiles.Values)
        {
            profile.CanvasWidth = settings.CanvasWidth;
            profile.CanvasHeight = settings.CanvasHeight;
        }

        if (type is PatternType.Border or PatternType.PhaseStripes or PatternType.WhiteRectangle)
        {
            foreach (PatternType rectangleType in new[]
                     {
                         PatternType.Border,
                         PatternType.PhaseStripes,
                         PatternType.WhiteRectangle
                     })
            {
                PatternSettings profile = _patternProfiles[rectangleType];
                profile.PatternX = settings.PatternX;
                profile.PatternY = settings.PatternY;
                profile.PatternWidth = settings.PatternWidth;
                profile.PatternHeight = settings.PatternHeight;
            }
        }

        if (type is PatternType.NinePointGrid or PatternType.DistortionGrid)
        {
            _patternProfiles[PatternType.NinePointGrid].DotRadius = settings.DotRadius;
            _patternProfiles[PatternType.DistortionGrid].DotRadius = settings.DotRadius;
        }

        if (type is PatternType.Border or PatternType.CorrectionCross)
        {
            _patternProfiles[PatternType.Border].LineWidth = settings.LineWidth;
            _patternProfiles[PatternType.CorrectionCross].LineWidth = settings.LineWidth;
        }
    }

    private PatternSettings ReadSettings(PatternType? patternType = null)
    {
        PatternType type = patternType ?? SelectedPatternType;
        PatternSettings settings = _patternProfiles.TryGetValue(type, out PatternSettings? profile)
            ? profile.Clone()
            : PatternPresets.Create(type);

        settings.PatternType = type;
        settings.CanvasWidth = (int)numericCanvasWidth.Value;
        settings.CanvasHeight = (int)numericCanvasHeight.Value;
        settings.PatternX = (int)numericPatternX.Value;
        settings.PatternY = (int)numericPatternY.Value;
        settings.PatternWidth = (int)numericPatternWidth.Value;
        settings.PatternHeight = (int)numericPatternHeight.Value;
        settings.Phase = (int)numericPhase.Value;
        settings.DotRadius = (int)numericDotRadius.Value;
        settings.Rows = (int)numericRows.Value;
        settings.Columns = (int)numericColumns.Value;
        settings.LineWidth = (int)numericLineWidth.Value;
        settings.BorderOverlay = borderOverlayEditor.GetSettings();
        return settings;
    }

    private void WriteSettings(PatternSettings settings)
    {
        _updatingControls = true;
        try
        {
            SetNumericValue(numericCanvasWidth, settings.CanvasWidth);
            SetNumericValue(numericCanvasHeight, settings.CanvasHeight);
            SetNumericValue(numericPatternX, settings.PatternX);
            SetNumericValue(numericPatternY, settings.PatternY);
            SetNumericValue(numericPatternWidth, settings.PatternWidth);
            SetNumericValue(numericPatternHeight, settings.PatternHeight);
            SetNumericValue(numericPhase, settings.Phase);
            SetNumericValue(numericDotRadius, settings.DotRadius);
            SetNumericValue(numericRows, settings.Rows);
            SetNumericValue(numericColumns, settings.Columns);
            SetNumericValue(numericLineWidth, settings.LineWidth);
            borderOverlayEditor.CanvasSize = new Size(settings.CanvasWidth, settings.CanvasHeight);
            borderOverlayEditor.SetSettings(settings.BorderOverlay);
        }
        finally
        {
            _updatingControls = false;
        }

        UpdateControlAvailability();
    }

    private static void SetNumericValue(NumericUpDown control, int value)
    {
        decimal clamped = Math.Min(control.Maximum, Math.Max(control.Minimum, value));
        control.Value = clamped;
    }

    private void UpdateControlAvailability()
    {
        PatternType type = SelectedPatternType;
        bool isGrid = type is PatternType.NinePointGrid or PatternType.DistortionGrid;
        bool isSolid = IsExactSolidPattern(type);
        bool isImported = type == PatternType.ImportedImage;
        bool usesLineWidth = type is PatternType.Border or PatternType.CorrectionCross;
        bool usesQuality = comboOutputFormat.SelectedIndex is (int)ImageFormatKind.Jpeg or (int)ImageFormatKind.WebP;

        numericPhase.Enabled = false;
        labelPhase.Enabled = false;

        numericPatternX.Enabled = !isSolid && !isImported;
        numericPatternY.Enabled = !isSolid && !isImported;
        numericPatternWidth.Enabled = !isSolid && !isImported;
        numericPatternHeight.Enabled = !isSolid && !isImported;
        buttonCenter.Enabled = !isSolid && !isImported;
        buttonBrowseSourceImage.Visible = isImported;
        labelPatternHelp.Width = isImported ? 190 : 331;

        numericDotRadius.Enabled = isGrid;
        numericRows.Enabled = isGrid;
        numericColumns.Enabled = isGrid;
        labelDotRadius.Enabled = isGrid;
        labelRows.Enabled = isGrid;
        labelColumns.Enabled = isGrid;
        numericLineWidth.Enabled = usesLineWidth;
        labelLineWidth.Enabled = usesLineWidth;

        numericQuality.Enabled = usesQuality;
        labelQuality.Enabled = usesQuality;

        labelPatternX.Text = isGrid ? "首点 X" : "左上 X";
        labelPatternY.Text = isGrid ? "首点 Y" : "左上 Y";
        labelPatternWidth.Text = isGrid ? "X 跨度" : "宽度";
        labelPatternHeight.Text = isGrid ? "Y 跨度" : "高度";
        labelPatternHelp.Text = isGrid
            ? "点阵 X/Y 是左上第一个圆心；跨度是首末圆心距离，圆点半径单独设置。"
            : isImported
                ? string.IsNullOrWhiteSpace(_patternProfiles[type].SourceImagePath)
                    ? "请选择外部图片；图片会按画布尺寸最近邻缩放，再应用可调白框叠加层。"
                    : $"底图：{Path.GetFileName(_patternProfiles[type].SourceImagePath)}"
            : isSolid
                ? "纯色图整张画布仅使用 0/255；保存时只允许 PNG、BMP 或 TIFF。"
                : "矩形类图案使用左上角 X/Y 和区域宽高；白框叠加层可覆盖任意底图。";

        borderOverlayEditor.CanvasSize = new Size(
            (int)numericCanvasWidth.Value,
            (int)numericCanvasHeight.Value);
    }

    private void SchedulePreview()
    {
        previewTimer.Stop();
        previewTimer.Start();
    }

    private void previewTimer_Tick(object? sender, EventArgs e)
    {
        previewTimer.Stop();
        UpdatePreview();
    }

    private void buttonRefresh_Click(object? sender, EventArgs e)
    {
        StoreControlsInProfiles(_activePatternType);
        UpdatePreview();
    }

    private void buttonReset_Click(object? sender, EventArgs e)
    {
        ResetProfilesToDefaults();
        _activePatternType = SelectedPatternType;
        LoadActiveProfile();
        statusLabel.Text = "已恢复全部图卡的样图默认参数。";
        UpdatePreview();
    }

    private void buttonCenter_Click(object? sender, EventArgs e)
    {
        PatternSettings settings = ReadSettings(_activePatternType);
        PatternLayout.Center(settings);

        _updatingControls = true;
        try
        {
            SetNumericValue(numericPatternX, settings.PatternX);
            SetNumericValue(numericPatternY, settings.PatternY);
        }
        finally
        {
            _updatingControls = false;
        }

        StoreControlsInProfiles(_activePatternType);
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        if (_isRendering || IsDisposed)
        {
            return;
        }

        _isRendering = true;
        previewTimer.Stop();
        UseWaitCursor = true;

        try
        {
            PatternSettings settings = ReadSettings(_activePatternType);
            using var image = PatternGenerator.Generate(settings);
            Bitmap bitmap = MatBitmapConverter.ToBitmap(image);
            imagePreviewControl.SetImage(bitmap, preserveView: true);

            labelPreviewInfo.Text = $"预览：{settings.CanvasWidth:N0} × {settings.CanvasHeight:N0} | {comboPattern.Text}";
            statusLabel.Text = $"就绪 | 区域/跨度 {settings.PatternWidth:N0} × {settings.PatternHeight:N0} | X={settings.PatternX}, Y={settings.PatternY}";
        }
        catch (Exception exception)
        {
            statusLabel.Text = $"预览失败：{exception.Message}";
        }
        finally
        {
            UseWaitCursor = false;
            _isRendering = false;
        }
    }

    private void buttonSaveCurrent_Click(object? sender, EventArgs e)
    {
        StoreControlsInProfiles(_activePatternType);
        PatternSettings settings = ReadSettings(_activePatternType);
        ImageExportOptions exportOptions = ReadExportOptions();
        if (IsExactSolidPattern(settings.PatternType) && !EnsureLosslessFormat(exportOptions, "纯色图"))
        {
            return;
        }

        string extension = ImageFileWriter.GetExtension(exportOptions.Format);

        saveFileDialog.Filter = ImageFileWriter.GetDialogFilter(exportOptions.Format);
        saveFileDialog.DefaultExt = extension.TrimStart('.');
        saveFileDialog.FileName = PatternFileNames.Get(settings, exportOptions.Format);

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

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

        try
        {
            using var image = PatternGenerator.Generate(settings);
            string actualPath = ImageFileWriter.Write(targetPath, image, exportOptions);
            statusLabel.Text = $"已保存：{actualPath}";
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void buttonBatchExport_Click(object? sender, EventArgs e)
    {
        ImageExportOptions exportOptions = ReadExportOptions();
        if (!EnsureLosslessFormat(exportOptions, "包含纯色图的批量导出"))
        {
            return;
        }

        if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        StoreControlsInProfiles(_activePatternType);
        PatternSettings current = _patternProfiles[_activePatternType].Clone();
        var profileSnapshot = _patternProfiles.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.Clone());

        BeginExport();
        UseWaitCursor = true;
        statusLabel.Text = "正在批量导出主窗口 10 张图卡...";

        try
        {
            IReadOnlyList<string> paths = await Task.Run(() => MainPatternBatchExporter.ExportAll(
                folderBrowserDialog.SelectedPath,
                current.CanvasWidth,
                current.CanvasHeight,
                current.DotRadius,
                current.LineWidth,
                exportOptions,
                profileSnapshot));

            statusLabel.Text = $"批量导出完成：{paths.Count} 张，目录 {folderBrowserDialog.SelectedPath}";
            MessageBox.Show(
                this,
                $"已导出 {paths.Count} 张图卡。\n格式：{exportOptions.Format}\n目录：{folderBrowserDialog.SelectedPath}",
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
            EndExport();
        }
    }

    private ImageExportOptions ReadExportOptions()
    {
        int formatIndex = Math.Max(0, comboOutputFormat.SelectedIndex);
        return new ImageExportOptions
        {
            Format = (ImageFormatKind)formatIndex,
            Quality = (int)numericQuality.Value
        };
    }

    private void borderOverlayEditor_SettingsChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        StoreControlsInProfiles(_activePatternType);
        SchedulePreview();
    }

    private void buttonPhaseTool_Click(object? sender, EventArgs e)
    {
        using var form = new PhaseStripeForm();
        form.ShowDialog(this);
    }

    private void buttonBrowseSourceImage_Click(object? sender, EventArgs e)
    {
        if (openImageDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        PatternSettings settings = ReadSettings(PatternType.ImportedImage);
        settings.SourceImagePath = openImageDialog.FileName;
        _patternProfiles[PatternType.ImportedImage] = settings;
        labelPatternHelp.Text = $"底图：{Path.GetFileName(settings.SourceImagePath)}";
        UpdatePreview();
    }

    private void buttonExportScreen1_Click(object? sender, EventArgs e)
    {
        using var form = new ScreenOneForm();
        form.ShowDialog(this);
    }

    private bool EnsureLosslessFormat(ImageExportOptions options, string outputName)
    {
        if (options.Format is ImageFormatKind.Png or ImageFormatKind.Bmp or ImageFormatKind.Tiff)
        {
            return true;
        }

        MessageBox.Show(
            this,
            $"{outputName}要求 RGB 通道值严格只有 0 和 255。JPEG/WebP 会产生中间值。\n\n请改选 PNG、BMP 或 TIFF。",
            "需要无损格式",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
        return false;
    }

    private static bool IsExactSolidPattern(PatternType type)
    {
        return type is PatternType.Black
            or PatternType.FullWhite
            or PatternType.FullRed
            or PatternType.FullGreen
            or PatternType.FullBlue;
    }

    private void BeginExport()
    {
        _activeExports++;
        settingsFlowPanel.Enabled = false;
        groupActions.Enabled = false;
        UseWaitCursor = true;
    }

    private void EndExport()
    {
        _activeExports = Math.Max(0, _activeExports - 1);
        if (IsDisposed || Disposing)
        {
            return;
        }

        bool enabled = _activeExports == 0;
        settingsFlowPanel.Enabled = enabled;
        groupActions.Enabled = enabled;
        UseWaitCursor = !enabled;
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_activeExports > 0)
        {
            e.Cancel = true;
            MessageBox.Show(
                this,
                "图卡正在导出，请等待导出完成后再关闭窗口。",
                "正在导出",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        SaveUserPreferences();
    }

    /// <summary>将每种图卡的独立参数、导出选项和预览开关原子保存到用户目录。</summary>
    private void SaveUserPreferences()
    {
        StoreControlsInProfiles(_activePatternType);
        var preferences = new MainPreferences
        {
            SelectedPatternType = _activePatternType,
            OutputFormat = ReadExportOptions().Format,
            Quality = (int)numericQuality.Value,
            PatternProfiles = _patternProfiles.ToDictionary(
                pair => pair.Key,
                pair => pair.Value.Clone()),
            PreviewOverlay = new PreviewOverlayPreferences
            {
                ShowCenterCrosshair = imagePreviewControl.ShowCenterCrosshair,
                ShowPixelCoordinates = imagePreviewControl.ShowPixelCoordinates
            }
        };

        if (!UserSettingsStore.Shared.TryUpdateAndSave(
                root => root.Main = preferences,
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

    private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        previewTimer.Stop();
    }

    private void buttonPhaseTool_Click_1(object sender, EventArgs e)
    {
        using var form = new PhaseStripeForm();
        form.ShowDialog(this);
    }

    private void buttonExportScreen1_Click_1(object sender, EventArgs e)
    {
        using var form = new ScreenOneForm();
        form.ShowDialog(this);
    }
}
