using System.ComponentModel;
using System.Globalization;
using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;
using OpenCvSharp;

namespace EolTestPatternGenerator.Controls;

/// <summary>
/// 图片静止序列转视频页面。所有可见控件均由 StillVideoPage.Designer.cs 创建，
/// 页面可以直接嵌入统一工作台，也可以在 Visual Studio 设计器中单独编辑。
/// </summary>
public partial class StillVideoPage : UserControl
{
    private readonly List<StillVideoItemSettings> _items = new();
    private CancellationTokenSource? _encodingCancellation;
    private bool _userInterfaceInitialized;
    private bool _updatingControls;
    private string _lastInputDirectory = string.Empty;
    private string _lastOutputDirectory = string.Empty;
    private string? _lastSettingsSaveErrorMessage;

    public StillVideoPage()
    {
        InitializeComponent();
    }

    /// <summary>页面当前是否在后台写入视频。</summary>
    public bool IsEncoding => _encodingCancellation is not null;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (_userInterfaceInitialized || IsInDesignMode())
        {
            return;
        }

        _userInterfaceInitialized = true;
        InitializeUserInterface();
    }

    /// <summary>
    /// 统一工作台关闭前调用。若正在编码，用户可以请求取消；在后台任务真正结束前返回 false，
    /// 防止已释放的页面接收进度回调，也保证已有目标文件不被破坏。
    /// </summary>
    public bool PrepareToClose()
    {
        // DataGridView 只有在验证成功后才把编辑值写回 _items；直接点击窗口关闭
        // 也必须提交最后一次时长输入，不能悄悄保存旧值。
        if (!TryCommitPendingDurationEdit())
        {
            return false;
        }

        if (_encodingCancellation is null)
        {
            SavePreferencesNow();
            return true;
        }

        if (_encodingCancellation.IsCancellationRequested)
        {
            MessageBox.Show(
                FindForm(),
                "正在等待视频编码器安全停止，请稍候再关闭窗口。",
                "正在取消",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return false;
        }

        DialogResult choice = MessageBox.Show(
            FindForm(),
            "视频仍在生成。是否取消本次生成？\n取消完成后可以再次关闭窗口，已有输出文件不会被修改。",
            "取消视频生成",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);
        if (choice == DialogResult.Yes)
        {
            _encodingCancellation.Cancel();
            labelProgress.Text = "正在安全取消，请稍候...";
        }

        return false;
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (!RecreatingHandle && _userInterfaceInitialized)
        {
            saveTimer.Stop();
            // FormClosing 阶段已经负责提示错误；销毁句柄时不再弹出对话框。
            SavePreferencesNow(showError: false);
        }

        base.OnHandleDestroyed(e);
    }

    private static bool IsInDesignMode()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
    }

    private void InitializeUserInterface()
    {
        _updatingControls = true;
        try
        {
            LoadPreferences();
            RefreshImageGrid();
        }
        finally
        {
            _updatingControls = false;
        }

        toolTip.SetToolTip(
            numericOutputWidth,
            "首批添加图片时自动采用第一张图片的尺寸，之后可手动修改。奇数宽度会向右补 1 像素黑边以防编码器截断。");
        toolTip.SetToolTip(
            numericOutputHeight,
            "不同尺寸图片会等比缩放、居中，剩余区域填黑；不会拉伸或裁切。奇数高度会向下补 1 像素黑边以防编码器截断。");
        toolTip.SetToolTip(
            comboFormat,
            "FFV1/MKV 与 HuffYUV/AVI 均经过当前 OpenCV 运行时的写入、回读和像素无损验证。");

        UpdateFormatDescription();
        UpdateSummary();
        UpdateSelectionButtons();
    }

    private void LoadPreferences()
    {
        StillVideoPreferences preferences = UserSettingsStore.Shared.Load().StillVideo;
        _items.Clear();
        _items.AddRange((preferences.Items ?? new List<StillVideoItemSettings>())
            .Where(item => item is not null)
            .Select(item => item.Clone()));

        SetNumericValue(numericOutputWidth, preferences.OutputWidth);
        SetNumericValue(numericOutputHeight, preferences.OutputHeight);
        SetNumericValue(numericFramesPerSecond, preferences.FramesPerSecond);
        int formatIndex = StillVideoFormatCatalog.All
            .Select((profile, index) => (profile, index))
            .Where(entry => entry.profile.Format == preferences.OutputFormat)
            .Select(entry => entry.index)
            .DefaultIfEmpty(0)
            .First();
        comboFormat.SelectedIndex = formatIndex;
        textOutputPath.Text = preferences.OutputPath ?? string.Empty;
        _lastInputDirectory = preferences.LastInputDirectory ?? string.Empty;
        _lastOutputDirectory = preferences.LastOutputDirectory ?? string.Empty;
    }

    private static void SetNumericValue(NumericUpDown control, decimal value)
    {
        control.Value = Math.Clamp(value, control.Minimum, control.Maximum);
    }

    private void RefreshImageGrid(IEnumerable<StillVideoItemSettings>? selectedItems = null)
    {
        var selected = selectedItems is null
            ? new HashSet<StillVideoItemSettings>()
            : new HashSet<StillVideoItemSettings>(selectedItems);

        _updatingControls = true;
        try
        {
            gridImages.Rows.Clear();
            for (int index = 0; index < _items.Count; index++)
            {
                StillVideoItemSettings item = _items[index];
                int rowIndex = gridImages.Rows.Add(
                    index + 1,
                    Path.GetFileName(item.ImagePath),
                    item.ImagePath,
                    item.DurationSeconds);
                gridImages.Rows[rowIndex].Selected = selected.Contains(item);
            }

            if (selected.Count == 0)
            {
                gridImages.ClearSelection();
            }
        }
        finally
        {
            _updatingControls = false;
        }

        UpdateSummary();
        UpdateSelectionButtons();
    }

    private void buttonAddImages_Click(object? sender, EventArgs e)
    {
        string? fallbackImagePath = _items.FirstOrDefault()?.ImagePath;
        _lastInputDirectory = DialogDirectoryResolver.ResolveExistingDirectory(
            _lastInputDirectory,
            fallbackImagePath);
        openImagesDialog.InitialDirectory = _lastInputDirectory;
        openImagesDialog.FileName = string.Empty;

        if (openImagesDialog.ShowDialog(FindForm()) != DialogResult.OK || openImagesDialog.FileNames.Length == 0)
        {
            return;
        }

        string[] selectedPaths = openImagesDialog.FileNames.Select(Path.GetFullPath).ToArray();
        bool useFirstImageSize = _items.Count == 0;
        if (useFirstImageSize)
        {
            try
            {
                using Mat firstImage = UnicodeImageLoader.LoadColor(selectedPaths[0]);
                SetNumericValue(numericOutputWidth, firstImage.Cols);
                SetNumericValue(numericOutputHeight, firstImage.Rows);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidDataException)
            {
                MessageBox.Show(
                    FindForm(),
                    $"无法读取第一张图片，因此没有加入列表：\n{exception.Message}",
                    "添加图片失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
        }

        var added = new List<StillVideoItemSettings>();
        foreach (string path in selectedPaths)
        {
            var item = new StillVideoItemSettings
            {
                ImagePath = path,
                DurationSeconds = 10d
            };
            _items.Add(item);
            added.Add(item);
        }

        _lastInputDirectory = DialogDirectoryResolver.RememberFileDirectory(
            selectedPaths[0],
            _lastInputDirectory);
        if (string.IsNullOrWhiteSpace(textOutputPath.Text))
        {
            string outputDirectory = Directory.Exists(_lastOutputDirectory)
                ? _lastOutputDirectory
                : _lastInputDirectory;
            textOutputPath.Text = Path.Combine(outputDirectory, "静止图片序列" + CurrentProfile.Extension);
        }

        RefreshImageGrid(added);
        QueuePreferenceSave();
    }

    private void buttonRemoveImages_Click(object? sender, EventArgs e)
    {
        int[] selectedIndices = GetSelectedIndices().OrderByDescending(index => index).ToArray();
        foreach (int index in selectedIndices)
        {
            _items.RemoveAt(index);
        }

        RefreshImageGrid();
        QueuePreferenceSave();
    }

    private void buttonMoveUp_Click(object? sender, EventArgs e)
    {
        HashSet<StillVideoItemSettings> selected = GetSelectedItems();
        for (int index = 1; index < _items.Count; index++)
        {
            if (selected.Contains(_items[index]) && !selected.Contains(_items[index - 1]))
            {
                (_items[index - 1], _items[index]) = (_items[index], _items[index - 1]);
            }
        }

        RefreshImageGrid(selected);
        QueuePreferenceSave();
    }

    private void buttonMoveDown_Click(object? sender, EventArgs e)
    {
        HashSet<StillVideoItemSettings> selected = GetSelectedItems();
        for (int index = _items.Count - 2; index >= 0; index--)
        {
            if (selected.Contains(_items[index]) && !selected.Contains(_items[index + 1]))
            {
                (_items[index], _items[index + 1]) = (_items[index + 1], _items[index]);
            }
        }

        RefreshImageGrid(selected);
        QueuePreferenceSave();
    }

    private void buttonClearImages_Click(object? sender, EventArgs e)
    {
        if (_items.Count == 0)
        {
            return;
        }

        if (MessageBox.Show(
                FindForm(),
                "确定清空视频图片列表吗？",
                "清空列表",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }

        _items.Clear();
        RefreshImageGrid();
        QueuePreferenceSave();
    }

    private void gridImages_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (_updatingControls || e.RowIndex < 0 || e.ColumnIndex != columnDuration.Index)
        {
            return;
        }

        if (!TryParsePositiveDuration(e.FormattedValue?.ToString(), out double duration))
        {
            gridImages.Rows[e.RowIndex].ErrorText = "静止时间必须是大于 0 且不超过 86400 的秒数。";
            e.Cancel = true;
            return;
        }

        _items[e.RowIndex].DurationSeconds = duration;
        gridImages.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = duration;
        gridImages.Rows[e.RowIndex].ErrorText = string.Empty;
        UpdateSummary();
        QueuePreferenceSave();
    }

    private void gridImages_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            gridImages.Rows[e.RowIndex].ErrorText = string.Empty;
        }
    }

    private bool TryCommitPendingDurationEdit()
    {
        if (gridImages.IsCurrentCellInEditMode && !gridImages.EndEdit())
        {
            ShowInvalidDurationMessage();
            return false;
        }

        if (!ValidateChildren(ValidationConstraints.Enabled))
        {
            ShowInvalidDurationMessage();
            return false;
        }

        return true;
    }

    private void ShowInvalidDurationMessage()
    {
        MessageBox.Show(
            FindForm(),
            "请先修正图片列表中无效的静止时间，再关闭窗口。",
            "静止时间无效",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    private static bool TryParsePositiveDuration(string? value, out double duration)
    {
        bool parsed = double.TryParse(
            value,
            NumberStyles.Float,
            CultureInfo.CurrentCulture,
            out duration);
        if (!parsed)
        {
            parsed = double.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out duration);
        }

        return parsed && double.IsFinite(duration) && duration > 0d && duration <= 86400d;
    }

    private void gridImages_SelectionChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            UpdateSelectionButtons();
        }
    }

    private int[] GetSelectedIndices()
    {
        return gridImages.SelectedRows
            .Cast<DataGridViewRow>()
            .Select(row => row.Index)
            .Where(index => index >= 0 && index < _items.Count)
            .Distinct()
            .ToArray();
    }

    private HashSet<StillVideoItemSettings> GetSelectedItems()
    {
        return GetSelectedIndices().Select(index => _items[index]).ToHashSet();
    }

    private void UpdateSelectionButtons()
    {
        int[] selected = GetSelectedIndices();
        bool idle = _encodingCancellation is null;
        buttonRemoveImages.Enabled = idle && selected.Length > 0;
        buttonMoveUp.Enabled = idle && selected.Any(index => index > 0);
        buttonMoveDown.Enabled = idle && selected.Any(index => index < _items.Count - 1);
        buttonClearImages.Enabled = idle && _items.Count > 0;
    }

    private void outputSetting_ValueChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        UpdateFormatDescription();
        UpdateSummary();
        QueuePreferenceSave();
    }

    private void comboFormat_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_updatingControls || comboFormat.SelectedIndex < 0)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(textOutputPath.Text))
        {
            try
            {
                textOutputPath.Text = Path.ChangeExtension(textOutputPath.Text.Trim(), CurrentProfile.Extension);
            }
            catch (ArgumentException)
            {
                // 用户仍在输入路径时先保留原文，生成前会显示明确的路径错误。
            }
        }

        UpdateFormatDescription();
        QueuePreferenceSave();
    }

    private void UpdateFormatDescription()
    {
        if (comboFormat.SelectedIndex < 0)
        {
            return;
        }

        StillVideoFormatProfile profile = CurrentProfile;
        System.Drawing.Size encodedSize = StillVideoEncoder.ResolveEncodedSize(
            decimal.ToInt32(numericOutputWidth.Value),
            decimal.ToInt32(numericOutputHeight.Value),
            profile.Format);
        labelActualSize.Text =
            $"实际编码：{encodedSize.Width} × {encodedSize.Height} · {numericFramesPerSecond.Value:0} FPS";
        if (profile.IsLossless)
        {
            labelFormatNotice.ForeColor = Color.FromArgb(16, 116, 61);
            labelFormatNotice.Text = "无损编码：图片不缩放时可保持解码后的像素值；文件会明显大于 MP4。";
        }
        else
        {
            labelFormatNotice.ForeColor = Color.FromArgb(176, 91, 0);
            labelFormatNotice.Text = "MP4 为有损编码，视频回读后的像素值不能保证严格保持 0 或 255。";
        }
    }

    private StillVideoFormatProfile CurrentProfile
    {
        get
        {
            int index = Math.Clamp(comboFormat.SelectedIndex, 0, StillVideoFormatCatalog.All.Count - 1);
            return StillVideoFormatCatalog.All[index];
        }
    }

    private void textOutputPath_TextChanged(object? sender, EventArgs e)
    {
        if (!_updatingControls)
        {
            QueuePreferenceSave();
        }
    }

    private void buttonBrowseOutput_Click(object? sender, EventArgs e)
    {
        StillVideoFormatProfile profile = CurrentProfile;
        saveVideoDialog.FilterIndex = comboFormat.SelectedIndex + 1;
        saveVideoDialog.DefaultExt = profile.Extension.TrimStart('.');
        _lastOutputDirectory = DialogDirectoryResolver.ResolveExistingDirectory(
            _lastOutputDirectory,
            textOutputPath.Text);
        saveVideoDialog.InitialDirectory = _lastOutputDirectory;

        if (!string.IsNullOrWhiteSpace(textOutputPath.Text))
        {
            try
            {
                saveVideoDialog.FileName = Path.GetFileName(
                    Path.ChangeExtension(textOutputPath.Text.Trim(), profile.Extension));
            }
            catch (ArgumentException)
            {
                saveVideoDialog.FileName = "静止图片序列" + profile.Extension;
            }
        }
        else
        {
            saveVideoDialog.FileName = "静止图片序列" + profile.Extension;
        }

        if (saveVideoDialog.ShowDialog(FindForm()) != DialogResult.OK)
        {
            return;
        }

        // 保存对话框也允许切换格式；同步回页面下拉框，避免所见扩展名与实际编码器不一致。
        int selectedFormatIndex = Math.Clamp(
            saveVideoDialog.FilterIndex - 1,
            0,
            StillVideoFormatCatalog.All.Count - 1);
        comboFormat.SelectedIndex = selectedFormatIndex;
        profile = CurrentProfile;
        string selectedPath = Path.ChangeExtension(Path.GetFullPath(saveVideoDialog.FileName), profile.Extension);
        textOutputPath.Text = selectedPath;
        _lastOutputDirectory = Path.GetDirectoryName(selectedPath) ?? _lastOutputDirectory;
        QueuePreferenceSave();
    }

    private async void buttonGenerate_Click(object? sender, EventArgs e)
    {
        if (_encodingCancellation is not null)
        {
            return;
        }

        if (!ValidateChildren(ValidationConstraints.Enabled))
        {
            return;
        }

        StillVideoEncodingOptions options;
        try
        {
            options = CreateEncodingOptions();
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException or IOException or NotSupportedException)
        {
            MessageBox.Show(
                FindForm(),
                exception.Message,
                "视频参数不完整",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        textOutputPath.Text = options.OutputPath;
        _lastOutputDirectory = Path.GetDirectoryName(options.OutputPath) ?? _lastOutputDirectory;
        SavePreferencesNow();

        _encodingCancellation = new CancellationTokenSource();
        SetEncodingState(isEncoding: true);
        progressEncoding.Value = 0;
        labelProgress.Text = "正在准备编码器...";

        var progress = new Progress<StillVideoEncodingProgress>(UpdateEncodingProgress);
        try
        {
            StillVideoEncodingResult result = await StillVideoEncoder.EncodeAsync(
                options,
                progress,
                _encodingCancellation.Token);
            progressEncoding.Value = progressEncoding.Maximum;
            labelProgress.Text = $"生成完成：{result.FrameCount:N0} 帧 · {FormatDuration(result.Duration.TotalSeconds)}";
            MessageBox.Show(
                FindForm(),
                $"视频已生成并通过回读验证：\n{result.OutputPath}\n\n" +
                $"{result.Width} × {result.Height}，{result.FramesPerSecond} FPS，{result.FrameCount:N0} 帧",
                "生成完成",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (OperationCanceledException)
        {
            progressEncoding.Value = 0;
            labelProgress.Text = "已取消；旧输出文件保持不变。";
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentException or
            InvalidOperationException or NotSupportedException or OpenCvSharpException)
        {
            progressEncoding.Value = 0;
            labelProgress.Text = "生成失败；旧输出文件保持不变。";
            MessageBox.Show(
                FindForm(),
                exception.Message,
                "视频生成失败",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _encodingCancellation.Dispose();
            _encodingCancellation = null;
            SetEncodingState(isEncoding: false);
        }
    }

    private StillVideoEncodingOptions CreateEncodingOptions()
    {
        if (_items.Count == 0)
        {
            throw new InvalidOperationException("请先添加至少一张图片。");
        }

        if (string.IsNullOrWhiteSpace(textOutputPath.Text))
        {
            throw new InvalidOperationException("请选择输出视频文件。");
        }

        StillVideoFormatProfile profile = CurrentProfile;
        string outputPath = Path.ChangeExtension(
            Path.GetFullPath(textOutputPath.Text.Trim()),
            profile.Extension);
        return new StillVideoEncodingOptions
        {
            Items = _items.Select(item => item.Clone()).ToArray(),
            OutputPath = outputPath,
            OutputWidth = decimal.ToInt32(numericOutputWidth.Value),
            OutputHeight = decimal.ToInt32(numericOutputHeight.Value),
            FramesPerSecond = decimal.ToInt32(numericFramesPerSecond.Value),
            OutputFormat = profile.Format
        };
    }

    private void UpdateEncodingProgress(StillVideoEncodingProgress progress)
    {
        int value = progress.TotalFrames == 0
            ? 0
            : (int)Math.Clamp(
                progress.CompletedFrames * (long)progressEncoding.Maximum / progress.TotalFrames,
                0L,
                progressEncoding.Maximum);
        progressEncoding.Value = value;
        labelProgress.Text =
            $"第 {progress.ItemIndex + 1}/{progress.ItemCount} 张：{Path.GetFileName(progress.ImagePath)} · " +
            $"{progress.CompletedFrames:N0}/{progress.TotalFrames:N0} 帧";
    }

    private void buttonCancel_Click(object? sender, EventArgs e)
    {
        if (_encodingCancellation is null || _encodingCancellation.IsCancellationRequested)
        {
            return;
        }

        _encodingCancellation.Cancel();
        buttonCancel.Enabled = false;
        labelProgress.Text = "正在安全取消，请稍候...";
    }

    private void SetEncodingState(bool isEncoding)
    {
        groupOutput.Enabled = !isEncoding;
        gridImages.ReadOnly = isEncoding;
        buttonAddImages.Enabled = !isEncoding;
        buttonGenerate.Enabled = !isEncoding;
        buttonCancel.Enabled = isEncoding;
        UpdateSelectionButtons();
    }

    private void UpdateSummary()
    {
        double totalSeconds = _items.Sum(item =>
            double.IsFinite(item.DurationSeconds) && item.DurationSeconds > 0d
                ? item.DurationSeconds
                : 0d);
        long totalFrames = 0;
        int framesPerSecond = decimal.ToInt32(numericFramesPerSecond.Value);
        try
        {
            IReadOnlyList<long> frameSchedule = StillVideoEncoder.CalculateFrameSchedule(
                _items.Select(item => item.DurationSeconds).ToArray(),
                framesPerSecond);
            foreach (long frameCount in frameSchedule)
            {
                totalFrames = checked(totalFrames + frameCount);
            }
        }
        catch (Exception exception) when (exception is ArgumentOutOfRangeException or OverflowException)
        {
            // 单元格校验会阻止无效时长离开编辑状态；损坏的旧配置先以 0 帧提示。
            totalFrames = 0;
        }

        labelSummary.Text =
            $"{_items.Count} 张图片 · 总时长 {FormatDuration(totalSeconds)} · {totalFrames:N0} 帧";
    }

    private static string FormatDuration(double totalSeconds)
    {
        if (!double.IsFinite(totalSeconds) || totalSeconds < 0d)
        {
            return "—";
        }

        TimeSpan value = TimeSpan.FromSeconds(totalSeconds);
        int totalHours = checked((int)Math.Floor(value.TotalHours));
        double secondsWithinMinute = value.Seconds + (value.Milliseconds / 1000d);
        return $"{totalHours:00}:{value.Minutes:00}:{secondsWithinMinute:00.###}";
    }

    private void QueuePreferenceSave()
    {
        if (_updatingControls || !_userInterfaceInitialized)
        {
            return;
        }

        saveTimer.Stop();
        saveTimer.Start();
    }

    private void saveTimer_Tick(object? sender, EventArgs e)
    {
        saveTimer.Stop();
        SavePreferencesNow();
    }

    private bool SavePreferencesNow(bool showError = true)
    {
        if (!_userInterfaceInitialized || IsInDesignMode())
        {
            return true;
        }

        saveTimer.Stop();
        StillVideoPreferences snapshot = CreatePreferencesSnapshot();
        if (UserSettingsStore.Shared.TryUpdateAndSave(
                root => root.StillVideo = snapshot.Clone(),
                out Exception? error))
        {
            _lastSettingsSaveErrorMessage = null;
            return true;
        }

        string errorMessage = error?.Message ?? "未知错误";
        if (showError && !string.Equals(
                _lastSettingsSaveErrorMessage,
                errorMessage,
                StringComparison.Ordinal))
        {
            MessageBox.Show(
                FindForm(),
                $"无法保存图片转视频页面的上次输入值：{errorMessage}",
                "保存设置失败",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        // 自动保存失败时同一原因只提示一次；一旦保存成功便允许后续新错误再次提示。
        _lastSettingsSaveErrorMessage = errorMessage;
        return false;
    }

    private StillVideoPreferences CreatePreferencesSnapshot()
    {
        string outputPath = textOutputPath.Text ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(outputPath))
        {
            try
            {
                string? directory = Path.GetDirectoryName(Path.GetFullPath(outputPath.Trim()));
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    _lastOutputDirectory = directory;
                }
            }
            catch (ArgumentException)
            {
                // 未完成的路径文本也要原样保存；目录仍使用最后一次有效值。
            }
        }

        return new StillVideoPreferences
        {
            Items = _items.Select(item => item.Clone()).ToList(),
            OutputWidth = decimal.ToInt32(numericOutputWidth.Value),
            OutputHeight = decimal.ToInt32(numericOutputHeight.Value),
            FramesPerSecond = decimal.ToInt32(numericFramesPerSecond.Value),
            OutputFormat = CurrentProfile.Format,
            OutputPath = outputPath,
            LastInputDirectory = _lastInputDirectory,
            LastOutputDirectory = _lastOutputDirectory
        };
    }
}
