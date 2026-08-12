using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;

namespace EolTestPatternGenerator;

public partial class MainForm : Form
{
    private readonly Dictionary<PatternType, PatternSettings> _patternProfiles = new();
    private PatternType _activePatternType = PatternType.Border;
    private bool _updatingControls;
    private bool _isRendering;

    public MainForm()
    {
        InitializeComponent();
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
        toolTip.SetToolTip(buttonBatchExport, "按各图卡已保存的参数、当前画布和当前导出格式生成全部 14 张图卡。");

        ResetProfilesToDefaults();

        _updatingControls = true;
        comboOutputFormat.SelectedIndex = 0;
        comboPattern.SelectedIndex = 0;
        _updatingControls = false;

        _activePatternType = PatternType.Border;
        LoadActiveProfile();
        UpdatePreview();
    }

    private PatternType SelectedPatternType =>
        comboPattern.SelectedIndex >= 0 ? (PatternType)comboPattern.SelectedIndex : PatternType.Border;

    private void ResetProfilesToDefaults()
    {
        _patternProfiles.Clear();
        foreach (PatternType type in Enum.GetValues<PatternType>())
        {
            _patternProfiles[type] = PatternPresets.Create(type);
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
        return new PatternSettings
        {
            PatternType = patternType ?? SelectedPatternType,
            CanvasWidth = (int)numericCanvasWidth.Value,
            CanvasHeight = (int)numericCanvasHeight.Value,
            PatternX = (int)numericPatternX.Value,
            PatternY = (int)numericPatternY.Value,
            PatternWidth = (int)numericPatternWidth.Value,
            PatternHeight = (int)numericPatternHeight.Value,
            Phase = (int)numericPhase.Value,
            DotRadius = (int)numericDotRadius.Value,
            Rows = (int)numericRows.Value,
            Columns = (int)numericColumns.Value,
            LineWidth = (int)numericLineWidth.Value
        };
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
        bool isBlack = type == PatternType.Black;
        bool usesLineWidth = type is PatternType.Border or PatternType.CorrectionCross;
        bool usesQuality = comboOutputFormat.SelectedIndex is (int)ImageFormatKind.Jpeg or (int)ImageFormatKind.WebP;

        numericPhase.Enabled = type == PatternType.PhaseStripes;
        labelPhase.Enabled = numericPhase.Enabled;

        numericPatternX.Enabled = !isBlack;
        numericPatternY.Enabled = !isBlack;
        numericPatternWidth.Enabled = !isBlack;
        numericPatternHeight.Enabled = !isBlack;
        buttonCenter.Enabled = !isBlack;

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
            : "矩形类图案使用左上角 X/Y 和区域宽高；所有参数均为整数像素。";
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
            Bitmap bitmap = MatBitmapConverter.ToPreviewBitmap(image);
            Image? previous = previewPictureBox.Image;
            previewPictureBox.Image = bitmap;
            previous?.Dispose();

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
        if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        StoreControlsInProfiles(_activePatternType);
        PatternSettings current = _patternProfiles[_activePatternType].Clone();
        ImageExportOptions exportOptions = ReadExportOptions();
        var profileSnapshot = _patternProfiles.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.Clone());

        groupActions.Enabled = false;
        UseWaitCursor = true;
        statusLabel.Text = "正在批量导出 14 张图卡...";

        try
        {
            IReadOnlyList<string> paths = await Task.Run(() => DefaultBatchExporter.ExportAll(
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
            groupActions.Enabled = true;
            UseWaitCursor = false;
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

    private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        previewTimer.Stop();
    }
}
