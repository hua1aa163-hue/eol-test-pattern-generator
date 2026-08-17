using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;
using System.ComponentModel;

namespace EolTestPatternGenerator;

/// <summary>
/// 以水平方向周期位置表格编辑 R/G/B 通道的“串扰像素排列2”窗口。
/// </summary>
public partial class CrosstalkGridForm : Form
{
    private bool _updatingControls;
    private bool _isRendering;
    private bool _isExporting;
    private bool _userInterfaceInitialized;
    private string _lastExportDirectory = string.Empty;
    private string _lastTwoInOneSourceDirectory = string.Empty;
    private string _lastTwoInOneOutputDirectory = string.Empty;

    public CrosstalkGridForm()
    {
        InitializeComponent();
    }

    /// <summary>统一工作台关闭前只读检查后台导出状态。</summary>
    public bool IsExporting => _isExporting;

    /// <summary>是否把完整图卡在右侧再复制一份。</summary>
    public bool TwoInOneEnabled
    {
        get => checkTwoInOne.Checked;
        set => checkTwoInOne.Checked = value;
    }

    public void ConfigureAsWorkspacePage()
    {
        Text = "串扰像素排列2";
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
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
        CrosstalkGridPreferences preferences = UserSettingsStore.Shared.Load().CrosstalkGrid;
        _updatingControls = true;
        try
        {
            WriteSettings(preferences.Settings);
            TwoInOneEnabled = preferences.TwoInOne;
            comboOutputFormat.SelectedIndex = Enum.IsDefined(preferences.OutputFormat)
                ? (int)preferences.OutputFormat
                : (int)ImageFormatKind.Png;
            SetNumericValue(numericQuality, preferences.Quality);
            previewControl.ShowCenterCrosshair = preferences.PreviewOverlay.ShowCenterCrosshair;
            previewControl.ShowPixelCoordinates = preferences.PreviewOverlay.ShowPixelCoordinates;
            UpdateCanvasSize();
        }
        finally
        {
            _updatingControls = false;
        }

        _lastExportDirectory = DialogDirectoryResolver.ResolveExistingDirectory(
            preferences.LastExportDirectory);
        _lastTwoInOneSourceDirectory = DialogDirectoryResolver.ResolveExistingDirectory(
            preferences.LastTwoInOneSourceDirectory);
        _lastTwoInOneOutputDirectory = DialogDirectoryResolver.ResolveExistingDirectory(
            preferences.LastTwoInOneOutputDirectory);
        ApplyExportDialogInitialDirectory();

        toolTip.SetToolTip(numericCanvasWidth, "最终导出图像的像素宽度。");
        toolTip.SetToolTip(numericCanvasHeight, "最终导出图像的像素高度。");
        toolTip.SetToolTip(
            cycleEditor,
            "表格每行是水平方向周期内的一个像素位置；复选框决定该位置点亮哪些通道。");
        toolTip.SetToolTip(buttonBatchExport, "依次生成当前周期的全部相位，文件名为相位序号。");
        toolTip.SetToolTip(
            checkTwoInOne,
            "在图卡和白框全部生成后，将整张图复制到右侧；输出宽度加倍，高度不变。");
        toolTip.SetToolTip(
            buttonBatchTwoInOneFolder,
            "选择源图片文件夹和独立输出文件夹，把当前层的 PNG、BMP 和单页 TIFF 水平复制为二合一；JPEG/WebP 会明确列为失败。");
        toolTip.SetToolTip(previewControl, "鼠标滚轮缩放；按住鼠标左键拖动图像。");

        UpdateControlAvailability();
        UpdatePreview(resetView: true);
    }

    private void ApplyExportDialogInitialDirectory()
    {
        _lastExportDirectory = DialogDirectoryResolver.ResolveExistingDirectory(_lastExportDirectory);
        saveFileDialog.InitialDirectory = _lastExportDirectory;
        folderBrowserDialog.InitialDirectory = _lastExportDirectory;
        folderBrowserDialog.SelectedPath = _lastExportDirectory;
    }

    private void WriteSettings(PatternSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        SetNumericValue(numericCanvasWidth, settings.CanvasWidth);
        SetNumericValue(numericCanvasHeight, settings.CanvasHeight);
        regionMarginsEditor.CanvasSize = new Size(settings.CanvasWidth, settings.CanvasHeight);
        regionMarginsEditor.SetMargins(settings.GetMargins());
        cycleEditor.SetCycle(CrosstalkPixelCyclePresets.Resolve(settings));
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
            UpdateCanvasSize();
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

    private void regionMarginsEditor_MarginsChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            SchedulePreview();
        }
    }

    private void UpdateCanvasSize()
    {
        var size = new Size((int)numericCanvasWidth.Value, (int)numericCanvasHeight.Value);
        borderOverlayEditor.CanvasSize = size;
        regionMarginsEditor.CanvasSize = size;
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
            PixelOrder = RgbPixelOrder.RGB,
            PixelCycle = cycleEditor.GetCycle(),
            BorderOverlay = borderOverlayEditor.GetSettings()
        };
        settings.SetMargins(regionMarginsEditor.GetMargins());
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

    private void checkTwoInOne_CheckedChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            SchedulePreview();
        }
    }

    private void buttonResetDefaults_Click(object? sender, EventArgs e)
    {
        CrosstalkGridPreferences defaults = CrosstalkGridPreferences.CreateReferenceDefault();
        _updatingControls = true;
        try
        {
            WriteSettings(defaults.Settings);
            TwoInOneEnabled = defaults.TwoInOne;
            comboOutputFormat.SelectedIndex = (int)defaults.OutputFormat;
            SetNumericValue(numericQuality, defaults.Quality);
            previewControl.ShowCenterCrosshair = defaults.PreviewOverlay.ShowCenterCrosshair;
            previewControl.ShowPixelCoordinates = defaults.PreviewOverlay.ShowPixelCoordinates;
            UpdateCanvasSize();
        }
        finally
        {
            _updatingControls = false;
        }

        UpdateControlAvailability();
        if (UpdatePreview(resetView: true))
        {
            statusLabel.Text = "已恢复默认参数（8 像素 RGB 掩码、横向步进 -3、倾斜角 18.435°）。";
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
            CrosstalkPixelCycle cycle = settings.PixelCycle!;
            using var image = GenerateOutputImage(settings, TwoInOneEnabled);
            Bitmap bitmap = MatBitmapConverter.ToBitmap(image);
            previewControl.SetImage(bitmap, preserveView: !resetView);
            labelPreviewInfo.Text =
                $"预览：{image.Cols:N0} × {image.Rows:N0} | " +
                $"相位 {settings.Phase}/{cycle.PeriodLength} | 横向步进 {cycle.ColumnAdvance} | " +
                $"倾斜角 {cycle.ResolveTiltAngleDegrees():0.######}°" +
                (TwoInOneEnabled ? " | 二合一" : string.Empty);
            statusLabel.Text =
                $"就绪 | 周期 {cycle.PeriodLength} 像素 | 输出 {image.Cols:N0} × {image.Rows:N0} | " +
                $"图案 {settings.CalculatedOuterWidth:N0} × {settings.CalculatedOuterHeight:N0}";
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
            "串扰像素排列图要求颜色通道只含 0 和 255。JPEG/WebP 的当前编码方式可能改变像素值。\n\n" +
            "是否切换为无损 PNG 后继续？",
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

            using var image = GenerateOutputImage(settings, TwoInOneEnabled);
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
        folderBrowserDialog.Description = "选择串扰像素排列2图卡的导出目录";
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
        bool twoInOne = TwoInOneEnabled;
        PatternSettings[] settings = Enumerable.Range(1, phaseCount)
            .Select(phase => ReadSettings(phase))
            .ToArray();
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
                    using var image = GenerateOutputImage(item, twoInOne);
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

    /// <summary>
    /// 将源文件夹当前层图片逐张水平复制到独立目录。该命令与当前图卡的二合一
    /// 复选框相互独立；为保证左右两半逐像素一致，只导出无损单页图片。
    /// </summary>
    private async void buttonBatchTwoInOneFolder_Click(object? sender, EventArgs e)
    {
        string? sourceDirectory = SelectBatchTwoInOneDirectory(
            "选择要批量二合一的源图片文件夹",
            _lastTwoInOneSourceDirectory);
        if (sourceDirectory is null)
        {
            return;
        }

        _lastTwoInOneSourceDirectory = DialogDirectoryResolver.RememberDirectory(
            sourceDirectory,
            _lastTwoInOneSourceDirectory);

        string? outputDirectory = SelectBatchTwoInOneDirectory(
            "选择二合一图片输出文件夹（不能与源文件夹相同）",
            _lastTwoInOneOutputDirectory);
        if (outputDirectory is null)
        {
            return;
        }

        _lastTwoInOneOutputDirectory = DialogDirectoryResolver.RememberDirectory(
            outputDirectory,
            _lastTwoInOneOutputDirectory);

        BatchTwoInOneExportPlan plan;
        try
        {
            plan = BatchTwoInOneImageExporter.CreatePlan(sourceDirectory, outputDirectory);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                exception.Message,
                "无法开始批量二合一",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            statusLabel.Text = $"批量二合一未开始：{exception.Message}";
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
            statusLabel.Text = "批量二合一未开始：源文件夹当前层没有受支持的图片。";
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
            DialogResult overwrite = MessageBox.Show(
                this,
                $"输出文件夹已有 {plan.ExistingOutputPaths.Count} 个同名文件：\n\n" +
                $"{existingNames}{remainingText}\n\n是否在本批次中覆盖这些同名文件？",
                "确认批量覆盖",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (overwrite != DialogResult.Yes)
            {
                statusLabel.Text = "已取消批量二合一：没有覆盖现有文件。";
                return;
            }

            overwriteExisting = true;
        }

        var progress = new Progress<BatchTwoInOneExportProgress>(item =>
        {
            statusLabel.Text =
                $"正在批量二合一：{item.CompletedCount}/{item.TotalCount} | {item.FileName}";
        });

        _isExporting = true;
        settingsFlowPanel.Enabled = false;
        groupActions.Enabled = false;
        UseWaitCursor = true;
        statusLabel.Text = $"正在批量二合一：0/{plan.Items.Count}";
        try
        {
            BatchTwoInOneExportResult result = await Task.Run(() =>
                BatchTwoInOneImageExporter.Export(
                    plan,
                    overwriteExisting,
                    progress));

            ShowBatchTwoInOneResult(plan, result);
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "批量二合一失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            statusLabel.Text = $"批量二合一失败：{exception.Message}";
        }
        finally
        {
            _isExporting = false;
            settingsFlowPanel.Enabled = true;
            groupActions.Enabled = true;
            UseWaitCursor = false;
        }
    }

    private string? SelectBatchTwoInOneDirectory(string description, string rememberedDirectory)
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

    private void ShowBatchTwoInOneResult(
        BatchTwoInOneExportPlan plan,
        BatchTwoInOneExportResult result)
    {
        if (result.Failures.Count == 0)
        {
            statusLabel.Text = $"批量二合一完成：{result.OutputPaths.Count} 张 | {plan.OutputDirectory}";
            MessageBox.Show(
                this,
                $"已完成 {result.OutputPaths.Count} 张文件夹图片二合一。\n" +
                $"输出目录：{plan.OutputDirectory}\n\n" +
                "保留原文件名和无损格式，左右两半逐像素一致，源图片未修改。",
                "批量二合一完成",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        string failureDetails = string.Join(
            "\n",
            result.Failures.Take(8).Select(failure =>
                $"{Path.GetFileName(failure.SourcePath)}：{failure.ErrorMessage}"));
        string remainingText = result.Failures.Count > 8
            ? $"\n……另有 {result.Failures.Count - 8} 张失败"
            : string.Empty;
        statusLabel.Text =
            $"批量二合一完成：成功 {result.OutputPaths.Count} 张，失败 {result.Failures.Count} 张。";
        MessageBox.Show(
            this,
            $"成功：{result.OutputPaths.Count} 张\n失败：{result.Failures.Count} 张\n\n" +
            $"{failureDetails}{remainingText}\n\n输出目录：{plan.OutputDirectory}",
            "批量二合一存在失败项",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    /// <summary>先生成包含白框的整张图，再按开关把它逐像素复制到右侧。</summary>
    private static OpenCvSharp.Mat GenerateOutputImage(PatternSettings settings, bool twoInOne)
    {
        OpenCvSharp.Mat image = PatternGenerator.Generate(settings);
        if (!twoInOne)
        {
            return image;
        }

        try
        {
            return HorizontalImageComposer.DuplicateToRight(image);
        }
        finally
        {
            image.Dispose();
        }
    }

    private void CrosstalkGridForm_FormClosing(object? sender, FormClosingEventArgs e)
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
        var preferences = new CrosstalkGridPreferences
        {
            Settings = ReadSettings(),
            TwoInOne = TwoInOneEnabled,
            OutputFormat = SelectedFormat,
            Quality = (int)numericQuality.Value,
            LastExportDirectory = _lastExportDirectory,
            LastTwoInOneSourceDirectory = _lastTwoInOneSourceDirectory,
            LastTwoInOneOutputDirectory = _lastTwoInOneOutputDirectory,
            PreviewOverlay = new PreviewOverlayPreferences
            {
                ShowCenterCrosshair = previewControl.ShowCenterCrosshair,
                ShowPixelCoordinates = previewControl.ShowPixelCoordinates
            }
        };

        if (!UserSettingsStore.Shared.TryUpdateAndSave(
                root => root.CrosstalkGrid = preferences,
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

    private void CrosstalkGridForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        previewTimer.Stop();
    }
}
