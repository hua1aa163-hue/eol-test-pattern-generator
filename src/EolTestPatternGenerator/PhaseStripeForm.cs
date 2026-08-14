using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;
using System.ComponentModel;

namespace EolTestPatternGenerator;

/// <summary>
/// RGB 八步二值相移条纹的独立编辑和导出窗口。
/// </summary>
public partial class PhaseStripeForm : Form
{
    private bool _updatingControls;
    private bool _isRendering;
    private bool _isExporting;
    private bool _userInterfaceInitialized;

    public PhaseStripeForm()
    {
        InitializeComponent();
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
            comboPixelOrder.SelectedItem = preferences.Settings.PixelOrder.ToString();
            if (comboPixelOrder.SelectedIndex < 0)
            {
                comboPixelOrder.SelectedIndex = 0;
            }

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

        toolTip.SetToolTip(numericCanvasWidth, "最终导出图像的像素宽度。");
        toolTip.SetToolTip(numericCanvasHeight, "最终导出图像的像素高度。");
        toolTip.SetToolTip(comboPixelOrder, "选择一个八像素周期内三个颜色通道的排列次序。");
        toolTip.SetToolTip(buttonBatchExport, "使用当前设置依次生成相位 1 到 8，文件名为 1 到 8。");
        toolTip.SetToolTip(previewControl, "鼠标滚轮缩放；按住鼠标左键拖动图像。");

        UpdateControlAvailability();
        UpdatePreview(resetView: true);
    }

    /// <summary>将保存的相移参数安全回写到设计器输入控件。</summary>
    private void WriteSettings(PatternSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        SetNumericValue(numericCanvasWidth, settings.CanvasWidth);
        SetNumericValue(numericCanvasHeight, settings.CanvasHeight);
        SetNumericValue(numericPatternX, settings.PatternX);
        SetNumericValue(numericPatternY, settings.PatternY);
        SetNumericValue(numericPatternWidth, settings.PatternWidth);
        SetNumericValue(numericPatternHeight, settings.PatternHeight);
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

    private void comboPixelOrder_SelectedIndexChanged(object? sender, EventArgs e)
    {
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

    private string SelectedPixelOrder =>
        comboPixelOrder.SelectedItem?.ToString() ?? "RGB";

    private PatternSettings ReadSettings(int? phase = null)
    {
        var settings = new PatternSettings
        {
            PatternType = PatternType.PhaseStripes,
            CanvasWidth = (int)numericCanvasWidth.Value,
            CanvasHeight = (int)numericCanvasHeight.Value,
            PatternX = (int)numericPatternX.Value,
            PatternY = (int)numericPatternY.Value,
            PatternWidth = (int)numericPatternWidth.Value,
            PatternHeight = (int)numericPatternHeight.Value,
            Phase = phase ?? (int)numericPhase.Value,
            BorderOverlay = borderOverlayEditor.GetSettings()
        };

        SetPixelOrder(settings, SelectedPixelOrder);
        return settings;
    }

    private static void SetPixelOrder(PatternSettings settings, string order)
    {
        // PixelOrder 的枚举由核心模型所有。这里按枚举成员名称赋值，使窗口不依赖
        // 枚举的具体类型名，同时仍能在模型扩展时保持二进制兼容。
        var property = typeof(PatternSettings).GetProperty(nameof(PatternSettings.PixelOrder))
            ?? throw new InvalidOperationException("PatternSettings 缺少 PixelOrder 属性。");

        if (!property.PropertyType.IsEnum)
        {
            throw new InvalidOperationException("PatternSettings.PixelOrder 必须是枚举类型。");
        }

        object value = Enum.Parse(property.PropertyType, order, ignoreCase: true);
        property.SetValue(settings, value);
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

    private void buttonCenter_Click(object? sender, EventArgs e)
    {
        int canvasWidth = (int)numericCanvasWidth.Value;
        int canvasHeight = (int)numericCanvasHeight.Value;
        int patternWidth = (int)numericPatternWidth.Value;
        int patternHeight = (int)numericPatternHeight.Value;

        _updatingControls = true;
        try
        {
            SetNumericValue(numericPatternX, (int)Math.Floor((canvasWidth - patternWidth) / 2.0));
            SetNumericValue(numericPatternY, (int)Math.Floor((canvasHeight - patternHeight) / 2.0));
        }
        finally
        {
            _updatingControls = false;
        }

        UpdatePreview(resetView: false);
    }

    private static void SetNumericValue(NumericUpDown control, int value)
    {
        control.Value = Math.Min(control.Maximum, Math.Max(control.Minimum, value));
    }

    private void UpdatePreview(bool resetView)
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
            PatternSettings settings = ReadSettings();
            using var image = PatternGenerator.Generate(settings);
            Bitmap bitmap = MatBitmapConverter.ToBitmap(image);
            previewControl.SetImage(bitmap, preserveView: !resetView);

            labelPreviewInfo.Text =
                $"预览：{settings.CanvasWidth:N0} × {settings.CanvasHeight:N0} | 相位 {settings.Phase} | {SelectedPixelOrder}";
            statusLabel.Text =
                $"就绪 | 区域 {settings.PatternWidth:N0} × {settings.PatternHeight:N0} | X={settings.PatternX}, Y={settings.PatternY}";
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

    private bool EnsureLosslessExport()
    {
        if (SelectedFormat is not (ImageFormatKind.Jpeg or ImageFormatKind.WebP))
        {
            return true;
        }

        DialogResult result = MessageBox.Show(
            this,
            "RGB 相移图卡要求颜色通道只含 0 和 255。JPEG/WebP 的当前编码方式可能改变像素值。\n\n是否切换为无损 PNG 后继续？",
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

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

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

        if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        ImageExportOptions exportOptions = ReadExportOptions();
        string outputDirectory = folderBrowserDialog.SelectedPath;
        string extension = ImageFileWriter.GetExtension(exportOptions.Format);
        // 开始后台任务前冻结八个相位的全部设置，避免用户操作改变正在导出的批次。
        PatternSettings[] settings = Enumerable.Range(1, 8).Select(phase => ReadSettings(phase)).ToArray();
        string exportedOrder = SelectedPixelOrder;
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
        statusLabel.Text = "正在批量导出相位 1–8...";

        try
        {
            IReadOnlyList<string> paths = await Task.Run(() =>
            {
                Directory.CreateDirectory(outputDirectory);
                var exported = new List<string>(8);

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
                $"已导出相位 1–8，共 {paths.Count} 张图卡。\n排列：{exportedOrder}\n目录：{outputDirectory}",
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
