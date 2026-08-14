using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;
using System.ComponentModel;

namespace EolTestPatternGenerator;

/// <summary>
/// 编辑、预览并导出“1号屏”三张二值图卡的独立窗口。
/// 可见控件均在 ScreenOneForm.Designer.cs 中声明，便于使用 WinForms 设计器调整。
/// </summary>
public partial class ScreenOneForm : Form
{
    private bool _updatingControls;
    private bool _isRendering;
    private bool _isExporting;
    private bool _userInterfaceInitialized;

    public ScreenOneForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // 设计器实例化窗体时跳过 OpenCV 预览，防止原生 DLL/大图分配阻断设计器。
        if (_userInterfaceInitialized ||
            DesignMode ||
            LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            return;
        }

        _userInterfaceInitialized = true;
        InitializeUserInterface();
    }

    /// <summary>
    /// 写入参考图默认参数，并生成窗口打开后的第一张预览。
    /// </summary>
    private void InitializeUserInterface()
    {
        ScreenOnePreferences preferences = UserSettingsStore.Shared.Load().ScreenOne;
        _updatingControls = true;
        try
        {
            comboCardKind.SelectedIndex = Enum.IsDefined(preferences.CardKind)
                ? (int)preferences.CardKind
                : (int)ScreenOneCardKind.BlackLeftWhiteRight;
            comboOutputFormat.SelectedIndex = Enum.IsDefined(preferences.OutputFormat)
                ? (int)preferences.OutputFormat
                : (int)ImageFormatKind.Png;
            SetNumericValue(numericQuality, preferences.Quality);
            WriteSettings(preferences.Settings);
            previewControl.ShowCenterCrosshair = preferences.PreviewOverlay.ShowCenterCrosshair;
            previewControl.ShowPixelCoordinates = preferences.PreviewOverlay.ShowPixelCoordinates;
        }
        finally
        {
            _updatingControls = false;
        }

        UpdateExportControls();
        UpdatePreview(resetView: true);
    }

    /// <summary>
    /// 从设计器中的输入控件读取一份完整参数快照。
    /// </summary>
    private ScreenOneSettings ReadSettings()
    {
        return new ScreenOneSettings
        {
            CanvasWidth = (int)numericCanvasWidth.Value,
            CanvasHeight = (int)numericCanvasHeight.Value,
            LeftX = (int)numericLeftX.Value,
            LeftY = (int)numericLeftY.Value,
            LeftWidth = (int)numericLeftWidth.Value,
            LeftHeight = (int)numericLeftHeight.Value,
            RightX = (int)numericRightX.Value,
            RightY = (int)numericRightY.Value,
            RightWidth = (int)numericRightWidth.Value,
            RightHeight = (int)numericRightHeight.Value
        };
    }

    /// <summary>
    /// 将参数写回设计器控件。该方法同时负责在控件范围内安全截断数值。
    /// </summary>
    private void WriteSettings(ScreenOneSettings settings)
    {
        bool previousUpdatingState = _updatingControls;
        _updatingControls = true;
        try
        {
            SetNumericValue(numericCanvasWidth, settings.CanvasWidth);
            SetNumericValue(numericCanvasHeight, settings.CanvasHeight);
            SetNumericValue(numericLeftX, settings.LeftX);
            SetNumericValue(numericLeftY, settings.LeftY);
            SetNumericValue(numericLeftWidth, settings.LeftWidth);
            SetNumericValue(numericLeftHeight, settings.LeftHeight);
            SetNumericValue(numericRightX, settings.RightX);
            SetNumericValue(numericRightY, settings.RightY);
            SetNumericValue(numericRightWidth, settings.RightWidth);
            SetNumericValue(numericRightHeight, settings.RightHeight);
        }
        finally
        {
            _updatingControls = previousUpdatingState;
        }
    }

    /// <summary>
    /// 避免将默认值或未来配置文件中的值写出 NumericUpDown 的有效范围。
    /// </summary>
    private static void SetNumericValue(NumericUpDown control, int value)
    {
        control.Value = Math.Min(control.Maximum, Math.Max(control.Minimum, value));
    }

    private ScreenOneCardKind SelectedCardKind
    {
        get
        {
            int selectedIndex = comboCardKind.SelectedIndex;
            return Enum.IsDefined(typeof(ScreenOneCardKind), selectedIndex)
                ? (ScreenOneCardKind)selectedIndex
                : ScreenOneCardKind.BlackLeftWhiteRight;
        }
    }

    private ImageFormatKind SelectedFormat
    {
        get
        {
            int selectedIndex = comboOutputFormat.SelectedIndex;
            return Enum.IsDefined(typeof(ImageFormatKind), selectedIndex)
                ? (ImageFormatKind)selectedIndex
                : ImageFormatKind.Png;
        }
    }

    /// <summary>
    /// 构造文件写入服务所需的导出参数。
    /// </summary>
    private ImageExportOptions ReadExportOptions()
    {
        return new ImageExportOptions
        {
            Format = SelectedFormat,
            Quality = (int)numericQuality.Value
        };
    }

    private void Parameter_ValueChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            SchedulePreview();
        }
    }

    private void comboCardKind_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            SchedulePreview();
        }
    }

    private void comboOutputFormat_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateExportControls();
    }

    /// <summary>
    /// JPEG 和 WebP 显示质量参数，但不会直接允许输出，避免引入 0/255 以外的颜色值。
    /// </summary>
    private void UpdateExportControls()
    {
        bool isLossy = SelectedFormat is ImageFormatKind.Jpeg or ImageFormatKind.WebP;
        numericQuality.Enabled = isLossy;
        labelQuality.Enabled = isLossy;
        labelExportHelp.Text = isLossy
            ? "JPEG/WebP 会产生中间颜色值；保存时必须切换为无损 PNG。"
            : "PNG、BMP、TIFF 可保持 RGB 通道严格只有 0 和 255。";
    }

    /// <summary>
    /// 合并频繁的数值更改，避免拖动输入框时重复生成大尺寸图像。
    /// </summary>
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

    private void buttonRestoreDefaults_Click(object? sender, EventArgs e)
    {
        WriteSettings(ScreenOneSettings.CreateReferenceDefault());
        comboCardKind.SelectedIndex = (int)ScreenOneCardKind.BlackLeftWhiteRight;
        statusLabel.Text = "已恢复三张参考图的默认画布和矩形参数。";
        UpdatePreview(resetView: true);
    }

    /// <summary>
    /// 按当前选择生成完整分辨率预览；预览控件负责滚轮缩放和鼠标拖动。
    /// </summary>
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
            ScreenOneSettings settings = ReadSettings();
            ScreenOneCardKind cardKind = SelectedCardKind;
            using var image = ScreenOnePatternGenerator.Generate(settings, cardKind);
            Bitmap bitmap = MatBitmapConverter.ToBitmap(image);
            previewControl.SetImage(bitmap, preserveView: !resetView);

            string fileBaseName = ScreenOnePatternGenerator.GetFileBaseName(cardKind);
            labelPreviewInfo.Text =
                $"预览：{settings.CanvasWidth:N0} × {settings.CanvasHeight:N0} | {fileBaseName}";
            statusLabel.Text =
                $"就绪 | 左区域 {settings.LeftWidth:N0}×{settings.LeftHeight:N0} @ ({settings.LeftX},{settings.LeftY}) | " +
                $"右区域 {settings.RightWidth:N0}×{settings.RightHeight:N0} @ ({settings.RightX},{settings.RightY})";
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

    /// <summary>
    /// 二值图卡禁止直接使用有损格式；用户同意后自动改为 PNG 再继续。
    /// </summary>
    private bool EnsureLosslessExport()
    {
        if (SelectedFormat is not (ImageFormatKind.Jpeg or ImageFormatKind.WebP))
        {
            return true;
        }

        DialogResult result = MessageBox.Show(
            this,
            "1号屏图卡要求 RGB 通道严格只有 0 和 255。JPEG/WebP 会产生中间颜色值。\n\n是否切换为 PNG 后继续？",
            "需要无损格式",
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

        ScreenOneSettings settings = ReadSettings();
        ScreenOneCardKind cardKind = SelectedCardKind;
        ImageExportOptions exportOptions = ReadExportOptions();
        string extension = ImageFileWriter.GetExtension(exportOptions.Format);

        saveFileDialog.Filter = ImageFileWriter.GetDialogFilter(exportOptions.Format);
        saveFileDialog.DefaultExt = extension.TrimStart('.');
        saveFileDialog.FileName = ScreenOnePatternGenerator.GetFileBaseName(cardKind) + extension;

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            string targetPath = ImageFileWriter.NormalizePath(saveFileDialog.FileName, exportOptions.Format);
            // 扩展名被格式规则改写时，SaveFileDialog不会替真正的目标文件做覆盖确认。
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

            using var image = ScreenOnePatternGenerator.Generate(settings, cardKind);
            string actualPath = ImageFileWriter.Write(targetPath, image, exportOptions);
            statusLabel.Text = $"已保存：{actualPath}";
        }
        catch (Exception exception)
        {
            statusLabel.Text = $"保存失败：{exception.Message}";
            MessageBox.Show(this, exception.Message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // 后台任务只使用快照，防止导出过程中参数被界面修改。
        ScreenOneSettings settings = ReadSettings().Clone();
        ImageExportOptions exportOptions = ReadExportOptions();
        string outputDirectory = folderBrowserDialog.SelectedPath;
        string extension = ImageFileWriter.GetExtension(exportOptions.Format);
        string[] targetPaths = Enum.GetValues<ScreenOneCardKind>()
            .Select(cardKind => Path.Combine(
                outputDirectory,
                ScreenOnePatternGenerator.GetFileBaseName(cardKind) + extension))
            .ToArray();
        int existingFileCount = targetPaths.Count(File.Exists);

        if (existingFileCount > 0)
        {
            DialogResult overwrite = MessageBox.Show(
                this,
                $"目标目录中已有 {existingFileCount} 个同名图卡，将被覆盖。是否继续？",
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
        statusLabel.Text = "正在导出 1.B_W、2.W_B、3.B...";

        try
        {
            IReadOnlyList<string> paths = await Task.Run(() =>
                ScreenOneBatchExporter.ExportReferenceThree(
                    outputDirectory,
                    settings,
                    exportOptions));

            statusLabel.Text = $"批量导出完成：{paths.Count} 张 | {outputDirectory}";
            MessageBox.Show(
                this,
                $"已导出 1.B_W、2.W_B、3.B，共 {paths.Count} 张。\n" +
                $"尺寸：{settings.CanvasWidth} × {settings.CanvasHeight}\n目录：{outputDirectory}",
                "导出完成",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            statusLabel.Text = $"批量导出失败：{exception.Message}";
            MessageBox.Show(this, exception.Message, "批量导出失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _isExporting = false;
            settingsFlowPanel.Enabled = true;
            groupActions.Enabled = true;
            UseWaitCursor = false;
        }
    }

    private void ScreenOneForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_isExporting)
        {
            e.Cancel = true;
            MessageBox.Show(
                this,
                "1号屏图卡正在导出，请等待导出完成后再关闭窗口。",
                "正在导出",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        SaveUserPreferences();
    }

    private void SaveUserPreferences()
    {
        var preferences = new ScreenOnePreferences
        {
            Settings = ReadSettings(),
            CardKind = SelectedCardKind,
            OutputFormat = SelectedFormat,
            Quality = (int)numericQuality.Value,
            PreviewOverlay = new PreviewOverlayPreferences
            {
                ShowCenterCrosshair = previewControl.ShowCenterCrosshair,
                ShowPixelCoordinates = previewControl.ShowPixelCoordinates
            }
        };

        if (!UserSettingsStore.Shared.TryUpdateAndSave(
                root => root.ScreenOne = preferences,
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

    private void ScreenOneForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        previewTimer.Stop();
    }
}
