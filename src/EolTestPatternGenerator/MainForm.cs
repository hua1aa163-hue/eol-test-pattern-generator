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
    private string _lastImportedImageDirectory = string.Empty;
    private string _lastExportDirectory = string.Empty;

    public MainForm()
    {
        InitializeComponent();
    }

    /// <summary>统一工作台在两阶段关闭前只读检查后台导出状态。</summary>
    public bool IsExporting => _activeExports > 0;

    /// <summary>
    /// 作为统一工作台中的基础图卡页面时，隐藏旧的链式子窗口入口。
    /// 这些功能已由工作台左侧导航直接提供。
    /// </summary>
    public void ConfigureAsWorkspacePage()
    {
        buttonPhaseTool.Visible = false;
        buttonExportScreen1.Visible = false;
        groupPattern.Height = 164;
        Text = "基础图卡";
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
        toolTip.SetToolTip(regionMarginsEditor, "统一按图案真实最外缘到画布四边的距离定义区域。");
        toolTip.SetToolTip(numericPatternX, "点阵左上第一个圆点的圆心 X；与外缘四边距双向同步。");
        toolTip.SetToolTip(numericPatternY, "点阵左上第一个圆点的圆心 Y；与外缘四边距双向同步。");
        toolTip.SetToolTip(numericPatternWidth, "点阵第一个到最后一个圆心的水平距离。");
        toolTip.SetToolTip(numericPatternHeight, "点阵第一个到最后一个圆心的垂直距离。");
        toolTip.SetToolTip(numericDotRadius, "半径 4 对应 9 像素直径；半径 0 对应单像素点。");
        toolTip.SetToolTip(buttonBatchExport, "生成 10 张基础图卡；串扰像素排列与显示器图卡请使用对应功能页导出。");
        toolTip.SetToolTip(
            buttonBatchAddBorder,
            "选择源图片文件夹和独立输出文件夹，按当前白框四边距与线宽批量处理当前层图片；多页 TIFF 会明确列为失败。");
        toolTip.SetToolTip(buttonPhaseTool, "打开串扰像素排列，可调整周期像素数并选择六种 RGB 排列和倾斜角。");
        toolTip.SetToolTip(buttonExportScreen1, "打开显示器图卡编辑页，可调整画布及左右区域的外缘四边距。");

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

    /// <summary>加载上次关闭程序时保存的全部基础图卡页面输入。</summary>
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

        bool previousUpdatingState = _updatingControls;
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
            _updatingControls = previousUpdatingState;
        }

        string importedImagePath = _patternProfiles.TryGetValue(
            PatternType.ImportedImage,
            out PatternSettings? importedProfile)
            ? importedProfile.SourceImagePath
            : string.Empty;
        _lastImportedImageDirectory = DialogDirectoryResolver.ResolveExistingDirectory(
            preferences.LastImportedImageDirectory,
            importedImagePath);
        _lastExportDirectory = DialogDirectoryResolver.ResolveExistingDirectory(
            preferences.LastExportDirectory);
        ApplyDialogInitialDirectories();
    }

    /// <summary>只把仍存在的绝对目录交给文件对话框；失效路径回退到系统默认位置。</summary>
    private void ApplyDialogInitialDirectories()
    {
        string importedImagePath = _patternProfiles.TryGetValue(
            PatternType.ImportedImage,
            out PatternSettings? importedProfile)
            ? importedProfile.SourceImagePath
            : string.Empty;
        _lastImportedImageDirectory = DialogDirectoryResolver.ResolveExistingDirectory(
            _lastImportedImageDirectory,
            importedImagePath);
        _lastExportDirectory = DialogDirectoryResolver.ResolveExistingDirectory(_lastExportDirectory);
        openImageDialog.InitialDirectory = _lastImportedImageDirectory;
        saveFileDialog.InitialDirectory = _lastExportDirectory;
        folderBrowserDialog.InitialDirectory = _lastExportDirectory;
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

        bool canvasChanged = ReferenceEquals(sender, numericCanvasWidth) ||
            ReferenceEquals(sender, numericCanvasHeight);
        if (canvasChanged)
        {
            regionMarginsEditor.CanvasSize = new Size(
                (int)numericCanvasWidth.Value,
                (int)numericCanvasHeight.Value);
        }

        bool dotCenterGeometryChanged = ReferenceEquals(sender, numericPatternX) ||
            ReferenceEquals(sender, numericPatternY) ||
            ReferenceEquals(sender, numericPatternWidth) ||
            ReferenceEquals(sender, numericPatternHeight) ||
            ReferenceEquals(sender, numericDotRadius);
        bool dotGridCountChanged = ReferenceEquals(sender, numericRows) ||
            ReferenceEquals(sender, numericColumns);
        if (_activePatternType is PatternType.NinePointGrid or PatternType.DistortionGrid)
        {
            if (dotCenterGeometryChanged)
            {
                NormalizeSingleAxisDotCenterInputs();
                UpdateMarginsFromDotCenterControls();
            }
            else if (canvasChanged || dotGridCountChanged)
            {
                SynchronizeDotGeometryFromMargins();
            }
        }

        borderOverlayEditor.CanvasSize = new Size(
            (int)numericCanvasWidth.Value,
            (int)numericCanvasHeight.Value);
        StoreControlsInProfiles(_activePatternType);
        SchedulePreview();
    }

    private void regionMarginsEditor_MarginsChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        if (_activePatternType is PatternType.NinePointGrid or PatternType.DistortionGrid)
        {
            SynchronizeDotGeometryFromMargins();
        }

        StoreControlsInProfiles(_activePatternType);
        SchedulePreview();
    }

    private PatternSettings CreateSettingsForDotSynchronization()
    {
        PatternSettings settings = _patternProfiles[_activePatternType].Clone();
        settings.PatternType = _activePatternType;
        settings.CanvasWidth = (int)numericCanvasWidth.Value;
        settings.CanvasHeight = (int)numericCanvasHeight.Value;
        settings.DotRadius = (int)numericDotRadius.Value;
        settings.Rows = (int)numericRows.Value;
        settings.Columns = (int)numericColumns.Value;
        settings.SetMargins(regionMarginsEditor.GetMargins());
        return settings;
    }

    /// <summary>
    /// 单列/单行时首末圆心是同一点，对应跨度必须回到 0。
    /// 这里抑制 ValueChanged 递归，随后由统一换算保留首圆心。
    /// </summary>
    private void NormalizeSingleAxisDotCenterInputs()
    {
        bool previousUpdatingState = _updatingControls;
        _updatingControls = true;
        try
        {
            if (numericColumns.Value == 1)
            {
                numericPatternWidth.Value = 0;
            }

            if (numericRows.Value == 1)
            {
                numericPatternHeight.Value = 0;
            }
        }
        finally
        {
            _updatingControls = previousUpdatingState;
        }
    }

    /// <summary>
    /// 外缘边距、画布或行列数改变后同步圆心定义。
    /// 若某轴只有一个圆心，保留左/上外缘并反算末端边距，使四边距仍是真实 bbox。
    /// </summary>
    private void SynchronizeDotGeometryFromMargins()
    {
        PatternSettings settings = CreateSettingsForDotSynchronization();
        if (settings.NormalizeSingleAxisDotSpans())
        {
            regionMarginsEditor.SetMargins(settings.GetMargins());
        }

        SynchronizeDotCenterControls(settings);
    }

    private void UpdateMarginsFromDotCenterControls()
    {
        var settings = new PatternSettings
        {
            PatternType = _activePatternType,
            CanvasWidth = (int)numericCanvasWidth.Value,
            CanvasHeight = (int)numericCanvasHeight.Value,
            DotRadius = (int)numericDotRadius.Value
        };
        settings.SetLegacyBounds(
            (int)numericPatternX.Value,
            (int)numericPatternY.Value,
            (int)numericPatternWidth.Value,
            (int)numericPatternHeight.Value);
        regionMarginsEditor.SetMargins(settings.GetMargins());
    }

    private void SynchronizeDotCenterControls(PatternSettings settings)
    {
        if (!settings.IsDotGrid)
        {
            return;
        }

        bool previousUpdatingState = _updatingControls;
        _updatingControls = true;
        try
        {
            SetNumericValue(numericPatternX, settings.PatternX);
            SetNumericValue(numericPatternY, settings.PatternY);
            SetNumericValue(numericPatternWidth, settings.PatternWidth);
            SetNumericValue(numericPatternHeight, settings.PatternHeight);
        }
        finally
        {
            _updatingControls = previousUpdatingState;
        }
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
                profile.SetMargins(settings.GetMargins());
            }
        }

        if (type is PatternType.NinePointGrid or PatternType.DistortionGrid)
        {
            // 两种点阵共享半径，但各自保留独立的首圆心和圆心跨度。
            // 不能只改 DotRadius，否则未激活点阵的圆心会随半径漂移。
            _patternProfiles[PatternType.NinePointGrid]
                .SetDotRadiusPreservingCenterGeometry(settings.DotRadius);
            _patternProfiles[PatternType.DistortionGrid]
                .SetDotRadiusPreservingCenterGeometry(settings.DotRadius);
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
        settings.Phase = (int)numericPhase.Value;
        settings.DotRadius = (int)numericDotRadius.Value;
        settings.Rows = (int)numericRows.Value;
        settings.Columns = (int)numericColumns.Value;
        settings.LineWidth = (int)numericLineWidth.Value;
        settings.SetMargins(regionMarginsEditor.GetMargins());
        settings.NormalizeFullCanvasMargins();
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
            SetNumericValue(numericPhase, settings.Phase);
            SetNumericValue(numericDotRadius, settings.DotRadius);
            SetNumericValue(numericRows, settings.Rows);
            SetNumericValue(numericColumns, settings.Columns);
            SetNumericValue(numericLineWidth, settings.LineWidth);
            regionMarginsEditor.CanvasSize = new Size(settings.CanvasWidth, settings.CanvasHeight);
            regionMarginsEditor.SetMargins(settings.GetMargins());
            SynchronizeDotCenterControls(settings);
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

        regionMarginsEditor.InputsEnabled = !isSolid && !isImported;
        bool showCenterGeometry = isGrid;
        numericPatternX.Visible = showCenterGeometry;
        numericPatternY.Visible = showCenterGeometry;
        numericPatternWidth.Visible = showCenterGeometry;
        numericPatternHeight.Visible = showCenterGeometry;
        labelPatternX.Visible = showCenterGeometry;
        labelPatternY.Visible = showCenterGeometry;
        labelPatternWidth.Visible = showCenterGeometry;
        labelPatternHeight.Visible = showCenterGeometry;
        labelPlacementHelp.Visible = showCenterGeometry;
        buttonCenter.Visible = false;
        groupPlacement.Height = showCenterGeometry ? 500 : 330;
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

        labelPatternX.Text = "首圆心 X";
        labelPatternY.Text = "首圆心 Y";
        labelPatternWidth.Text = "圆心 X 跨度";
        labelPatternHeight.Text = "圆心 Y 跨度";
        labelPatternHelp.Text = isGrid
            ? "点阵可同时按圆心与真实外缘校准；两组参数会自动双向同步。"
            : isImported
                ? string.IsNullOrWhiteSpace(_patternProfiles[type].SourceImagePath)
                    ? "请选择外部图片；图片会按画布尺寸最近邻缩放，再应用可调白框叠加层。"
                    : $"底图：{Path.GetFileName(_patternProfiles[type].SourceImagePath)}"
            : isSolid
                ? "纯色图整张画布仅使用 0/255；保存时只允许 PNG、BMP 或 TIFF。"
                : "所有有限区域均使用图案真实最外缘到画布四边的距离。";

        regionMarginsEditor.CanvasSize = new Size(
            (int)numericCanvasWidth.Value,
            (int)numericCanvasHeight.Value);
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
        regionMarginsEditor.CenterPreservingSize();
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
            statusLabel.Text = settings.IsDotGrid
                ? $"就绪 | 外包 {settings.CalculatedOuterWidth:N0} × {settings.CalculatedOuterHeight:N0} | 圆心跨度 {settings.CalculatedCenterSpanWidth:N0} × {settings.CalculatedCenterSpanHeight:N0}"
                : $"就绪 | 图案 {settings.CalculatedOuterWidth:N0} × {settings.CalculatedOuterHeight:N0}";
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
        ApplyDialogInitialDirectories();

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _lastExportDirectory = DialogDirectoryResolver.RememberFileDirectory(
            saveFileDialog.FileName,
            _lastExportDirectory);

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

        ApplyDialogInitialDirectories();
        folderBrowserDialog.Description = "选择批量导出目录";
        if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _lastExportDirectory = DialogDirectoryResolver.RememberDirectory(
            folderBrowserDialog.SelectedPath,
            _lastExportDirectory);

        StoreControlsInProfiles(_activePatternType);
        PatternSettings current = _patternProfiles[_activePatternType].Clone();
        var profileSnapshot = _patternProfiles.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.Clone());

        BeginExport();
        UseWaitCursor = true;
        statusLabel.Text = "正在批量导出 10 张基础图卡...";

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

    /// <summary>
    /// 使用基础页当前白框四边距和线宽，批量处理源文件夹当前层的全部受支持图片。
    /// 源、输出目录分别选择并分别复用导入目录/导出目录的上次输入记录。
    /// </summary>
    private async void buttonBatchAddBorder_Click(object? sender, EventArgs e)
    {
        string? sourceDirectory = SelectBatchBorderDirectory(
            "选择要批量添加白框的源图片文件夹",
            _lastImportedImageDirectory);
        if (sourceDirectory is null)
        {
            return;
        }

        _lastImportedImageDirectory = DialogDirectoryResolver.RememberDirectory(
            sourceDirectory,
            _lastImportedImageDirectory);

        string? outputDirectory = SelectBatchBorderDirectory(
            "选择白框图片输出文件夹（不能与源文件夹相同）",
            _lastExportDirectory);
        if (outputDirectory is null)
        {
            return;
        }

        _lastExportDirectory = DialogDirectoryResolver.RememberDirectory(
            outputDirectory,
            _lastExportDirectory);
        ApplyDialogInitialDirectories();

        BatchBorderExportPlan plan;
        try
        {
            plan = BatchBorderImageExporter.CreatePlan(sourceDirectory, outputDirectory);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "无法开始批量加白框",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            statusLabel.Text = $"批量加白框未开始：{exception.Message}";
            return;
        }

        if (plan.Items.Count == 0)
        {
            MessageBox.Show(
                this,
                "源文件夹当前层没有可处理的 PNG、JPEG、BMP、TIFF 或 WebP 图片。",
                "没有可处理图片",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            statusLabel.Text = "批量加白框未开始：源文件夹当前层没有受支持的图片。";
            return;
        }

        bool overwriteExisting = false;
        if (plan.ExistingOutputPaths.Count > 0)
        {
            string existingNames = string.Join(
                "\n",
                plan.ExistingOutputPaths.Take(8).Select(Path.GetFileName));
            string remainingText = plan.ExistingOutputPaths.Count > 8
                ? $"\n……另有 {plan.ExistingOutputPaths.Count - 8} 个同名文件"
                : string.Empty;
            DialogResult overwriteResult = MessageBox.Show(
                this,
                $"输出文件夹已有 {plan.ExistingOutputPaths.Count} 个同名文件：\n\n" +
                $"{existingNames}{remainingText}\n\n是否在本批次中覆盖这些同名文件？",
                "确认批量覆盖",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (overwriteResult != DialogResult.Yes)
            {
                statusLabel.Text = "已取消批量加白框：没有覆盖现有文件。";
                return;
            }

            overwriteExisting = true;
        }

        BorderOverlaySettings border = borderOverlayEditor.GetSettings();
        border.Enabled = true;
        int lossyQuality = (int)numericQuality.Value;
        var progress = new Progress<BatchBorderExportProgress>(item =>
        {
            statusLabel.Text =
                $"正在批量加白框：{item.CompletedCount}/{item.TotalCount} | {item.FileName}";
        });

        BeginExport();
        statusLabel.Text = $"正在批量加白框：0/{plan.Items.Count}";
        try
        {
            BatchBorderExportResult result = await Task.Run(() => BatchBorderImageExporter.Export(
                plan,
                border,
                lossyQuality,
                overwriteExisting,
                progress));

            if (result.Failures.Count == 0)
            {
                statusLabel.Text =
                    $"批量加白框完成：{result.OutputPaths.Count} 张，目录 {plan.OutputDirectory}";
                MessageBox.Show(
                    this,
                    $"已为 {result.OutputPaths.Count} 张图片添加白框。\n" +
                    $"输出目录：{plan.OutputDirectory}\n\n" +
                    "每张图片均保留原文件名和原图片格式，源图片未修改；多页 TIFF 不会被静默截取。",
                    "批量加白框完成",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                string failureDetails = string.Join(
                    "\n",
                    result.Failures.Take(8).Select(failure =>
                        $"{Path.GetFileName(failure.SourcePath)}：{failure.ErrorMessage}"));
                string remainingText = result.Failures.Count > 8
                    ? $"\n……另有 {result.Failures.Count - 8} 张失败"
                    : string.Empty;
                statusLabel.Text =
                    $"批量加白框完成：成功 {result.OutputPaths.Count} 张，失败 {result.Failures.Count} 张。";
                MessageBox.Show(
                    this,
                    $"成功：{result.OutputPaths.Count} 张\n失败：{result.Failures.Count} 张\n\n" +
                    $"{failureDetails}{remainingText}\n\n输出目录：{plan.OutputDirectory}",
                    "批量加白框存在失败项",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "批量加白框失败",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            statusLabel.Text = $"批量加白框失败：{exception.Message}";
        }
        finally
        {
            EndExport();
        }
    }

    private string? SelectBatchBorderDirectory(string description, string rememberedDirectory)
    {
        string initialDirectory = DialogDirectoryResolver.ResolveExistingDirectory(rememberedDirectory);
        folderBrowserDialog.Description = description;
        folderBrowserDialog.InitialDirectory = initialDirectory;
        folderBrowserDialog.SelectedPath = initialDirectory;
        folderBrowserDialog.UseDescriptionForTitle = true;
        return folderBrowserDialog.ShowDialog(this) == DialogResult.OK
            ? folderBrowserDialog.SelectedPath
            : null;
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
        ApplyDialogInitialDirectories();
        if (openImageDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _lastImportedImageDirectory = DialogDirectoryResolver.RememberFileDirectory(
            openImageDialog.FileName,
            _lastImportedImageDirectory);

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
            LastImportedImageDirectory = _lastImportedImageDirectory,
            LastExportDirectory = _lastExportDirectory,
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
