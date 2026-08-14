using System.ComponentModel;
using System.Globalization;
using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;
using OpenCvSharp;

namespace EolTestPatternGenerator;

/// <summary>
/// 纯 C# 的非整数连续融合、离散光源图及 LightTools 文本转换窗口。
/// 所有可见控件均在 NonIntegerFusionForm.Designer.cs 中创建，便于设计器编辑。
/// </summary>
public partial class NonIntegerFusionForm : Form
{
    private BorderOverlaySettings _continuousBorder = new();
    private BorderOverlaySettings _discreteBorder = new();
    private bool _updatingControls;
    private bool _isRendering;
    private bool _userInterfaceInitialized;
    private int _activeTabIndex;
    private int _activeExports;
    private string _continuousSourceFolder = string.Empty;
    private string _continuousOutputDirectory = string.Empty;
    private string _discreteOutputDirectory = string.Empty;
    private string _converterOutputDirectory = string.Empty;

    public NonIntegerFusionForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // WinForms Designer 也会实例化无参构造函数；设计时不能加载 OpenCV 或用户配置。
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
        _updatingControls = true;
        try
        {
            comboContinuousSourceMode.SelectedIndex = (int)NonIntegerFusionSourceMode.DutyWhiteBlack;
            comboContinuousInputOrder.SelectedIndex = 0;
            comboContinuousFormat.SelectedIndex = (int)ImageFormatKind.Png;
            comboDiscreteResultType.SelectedIndex = (int)DiscreteCrosstalkResultType.WhiteBlack;
            comboDiscreteFormat.SelectedIndex = (int)ImageFormatKind.Bmp;
            LoadPreferences();
        }
        finally
        {
            _updatingControls = false;
        }

        toolTip.SetToolTip(numericContinuousMultiplier, "有效周期 T = 子像素周期 × m；m 不缩放图像。");
        toolTip.SetToolTip(numericContinuousDuty, "占左眼半周期的比例；0.25 对应整个周期的 12.5%。");
        toolTip.SetToolTip(numericContinuousPixelWidth, "MATLAB 的 pixL：单个 RGB 子像素的横向宽度，单位毫米。");
        toolTip.SetToolTip(numericContinuousPixelHeight, "MATLAB 的 pixH：一行像素的纵向高度，单位毫米。");
        toolTip.SetToolTip(comboContinuousInputOrder, "兼容模式会复现旧 MATLAB 文件夹分支的红蓝通道交换。");
        toolTip.SetToolTip(numericDiscreteGroup, "-1 表示批量生成 0 到周期减一；预览时使用第 0 组。");
        toolTip.SetToolTip(previewControl, "滚轮缩放、单击拖动；工具栏可关闭中心十字和像素坐标。");

        _activeTabIndex = tabModes.SelectedIndex;
        LoadBorderForActiveTab();
        UpdateControlAvailability();
        UpdatePreview(resetView: true);
    }

    private void LoadPreferences()
    {
        ApplicationPreferences root = UserSettingsStore.Shared.Load();
        NonIntegerBlendPreferences preferences = root.NonIntegerBlend;
        WriteContinuousPreferences(preferences.Matlab);
        WriteDiscretePreferences(preferences.Discrete);
        WriteConverterPreferences(preferences.Converter);

        previewControl.ShowCenterCrosshair = preferences.PreviewOverlay.ShowCenterCrosshair;
        previewControl.ShowPixelCoordinates = preferences.PreviewOverlay.ShowPixelCoordinates;
        tabModes.SelectedIndex = Math.Clamp(preferences.ActiveTabIndex, 0, tabModes.TabCount - 1);
    }

    private void WriteContinuousPreferences(MatlabNonIntegerPreferences preferences)
    {
        SetNumericValue(numericContinuousWidth, preferences.CanvasWidth);
        SetNumericValue(numericContinuousHeight, preferences.CanvasHeight);
        SetNumericValue(numericContinuousAngle, preferences.Theta);
        SetNumericValue(numericContinuousPeriod, preferences.SubpixelPeriod);
        SetNumericValue(numericContinuousMultiplier, preferences.PeriodMultiplier);
        SetNumericValue(numericContinuousOffset, preferences.PhaseOffset);
        SetNumericValue(numericContinuousDuty, preferences.DutyCycle);
        comboContinuousSourceMode.SelectedIndex = ParseEnumIndex(
            preferences.SourceMode,
            NonIntegerFusionSourceMode.DutyWhiteBlack);
        checkContinuousReverse.Checked = preferences.Reverse;
        checkContinuousSaveSources.Checked = preferences.SaveSourceImages;
        checkContinuousWriteMesh.Checked = preferences.WriteMesh;
        textContinuousLeftPath.Text = preferences.LeftSourcePath ?? string.Empty;
        textContinuousRightPath.Text = preferences.RightSourcePath ?? string.Empty;
        comboContinuousInputOrder.SelectedIndex =
            string.Equals(preferences.InputChannelOrder, "LegacyBgr", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        // 空文本也是用户上次输入的一部分；真正导出时 GetSafePrefix 再使用 fused 兜底。
        textContinuousPrefix.Text = preferences.OutputPrefix ?? "fused";
        comboContinuousFormat.SelectedIndex = ValidFormatIndex(preferences.OutputFormat);
        SetNumericValue(numericContinuousQuality, preferences.Quality);
        SetNumericValue(numericContinuousPixelWidth, preferences.PixelWidthMillimeters);
        SetNumericValue(numericContinuousPixelHeight, preferences.PixelHeightMillimeters);
        _continuousSourceFolder = preferences.SourceFolderPath ?? string.Empty;
        _continuousOutputDirectory = preferences.OutputDirectory ?? string.Empty;
        _continuousBorder = (preferences.BorderOverlay ?? new BorderOverlaySettings()).Clone();
    }

    private void WriteDiscretePreferences(DiscreteNonIntegerPreferences preferences)
    {
        SetNumericValue(numericDiscreteWidth, preferences.Width);
        SetNumericValue(numericDiscreteHeight, preferences.Height);
        SetNumericValue(numericDiscreteAngle, preferences.AngleDegrees);
        SetNumericValue(numericDiscretePeriod, preferences.Period);
        UpdateDiscreteRanges();
        SetNumericValue(numericDiscreteLitCount, preferences.LitCount);
        SetNumericValue(numericDiscreteGroup, preferences.Group);
        SetNumericValue(numericDiscretePixelX, preferences.PixelXMicrometers);
        SetNumericValue(numericDiscretePixelY, preferences.PixelYMicrometers);
        SetNumericValue(numericDiscreteScreenSize, preferences.ScreenSizeInches);
        SetNumericValue(numericDiscretePartitionWidth, preferences.PartitionWidth);
        SetNumericValue(numericDiscreteStrategyAngle, preferences.StrategyAngleDegrees);
        SetNumericValue(numericDiscreteTranslation, preferences.Translation);
        checkDiscretePositiveDirection.Checked = !string.Equals(
            preferences.Direction,
            nameof(DiscreteCrosstalkDirection.RightPositive),
            StringComparison.OrdinalIgnoreCase);
        comboDiscreteResultType.SelectedIndex = ParseEnumIndex(
            preferences.ResultType,
            DiscreteCrosstalkResultType.WhiteBlack);
        checkDiscreteSingleSource.Checked = preferences.SingleSource;
        textDiscreteSourceA.Text = preferences.CustomSourceAPath ?? string.Empty;
        textDiscreteSourceB.Text = preferences.CustomSourceBPath ?? string.Empty;
        comboDiscreteFormat.SelectedIndex = ValidFormatIndex(preferences.OutputFormat);
        SetNumericValue(numericDiscreteQuality, preferences.Quality);
        _discreteOutputDirectory = preferences.OutputDirectory ?? string.Empty;
        _discreteBorder = (preferences.BorderOverlay ?? new BorderOverlaySettings()).Clone();
    }

    private void WriteConverterPreferences(ImageConverterPreferences preferences)
    {
        textConverterImagePath.Text = preferences.SourceImagePath ?? string.Empty;
        SetNumericValue(numericConverterPixelX, preferences.PixelXMicrometers);
        SetNumericValue(numericConverterPixelY, preferences.PixelYMicrometers);
        checkConverterSingleSource.Checked = preferences.SingleSource;
        _converterOutputDirectory = preferences.OutputDirectory ?? string.Empty;
    }

    private void ParameterChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        if (ReferenceEquals(sender, numericDiscretePeriod))
        {
            _updatingControls = true;
            try
            {
                UpdateDiscreteRanges();
            }
            finally
            {
                _updatingControls = false;
            }
        }

        UpdateBorderCanvasSize();
        UpdateControlAvailability();
        SchedulePreview();
    }

    /// <summary>
    /// 仅影响文件名、编码或 MESH 元数据的输入不需要重新生成整张预览图。
    /// </summary>
    private void ExportOptionChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        UpdateControlAvailability();
    }

    private void tabModes_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        StoreCurrentBorder();
        _activeTabIndex = tabModes.SelectedIndex;
        LoadBorderForActiveTab();
        UpdateControlAvailability();
        UpdatePreview(resetView: true);
    }

    private void borderOverlayEditor_SettingsChanged(object? sender, EventArgs e)
    {
        if (_updatingControls)
        {
            return;
        }

        StoreCurrentBorder();
        SchedulePreview();
    }

    private void previewTimer_Tick(object? sender, EventArgs e)
    {
        previewTimer.Stop();
        UpdatePreview(resetView: false);
    }

    private void SchedulePreview()
    {
        previewTimer.Stop();
        previewTimer.Start();
    }

    private void UpdateDiscreteRanges()
    {
        int period = Math.Max(1, (int)numericDiscretePeriod.Value);
        numericDiscreteLitCount.Maximum = period;
        numericDiscreteGroup.Maximum = period - 1;
        if (numericDiscreteLitCount.Value < 1)
        {
            numericDiscreteLitCount.Value = 1;
        }
    }

    private void UpdateControlAvailability()
    {
        bool continuousCustom = comboContinuousSourceMode.SelectedIndex ==
                                (int)NonIntegerFusionSourceMode.CustomImages;
        bool continuousDuty = comboContinuousSourceMode.SelectedIndex ==
                              (int)NonIntegerFusionSourceMode.DutyWhiteBlack;
        textContinuousLeftPath.Enabled = continuousCustom;
        textContinuousRightPath.Enabled = continuousCustom;
        buttonBrowseContinuousLeft.Enabled = continuousCustom;
        buttonBrowseContinuousRight.Enabled = continuousCustom;
        numericContinuousDuty.Enabled = continuousDuty;
        labelContinuousDuty.Enabled = continuousDuty;

        bool discreteCustom = comboDiscreteResultType.SelectedIndex ==
                              (int)DiscreteCrosstalkResultType.Custom;
        textDiscreteSourceA.Enabled = discreteCustom;
        textDiscreteSourceB.Enabled = discreteCustom;
        buttonBrowseDiscreteA.Enabled = discreteCustom;
        buttonBrowseDiscreteB.Enabled = discreteCustom;

        bool continuousLossy = SelectedContinuousFormat is ImageFormatKind.Jpeg or ImageFormatKind.WebP;
        numericContinuousQuality.Enabled = continuousLossy;
        labelContinuousQuality.Enabled = continuousLossy;
        bool discreteLossy = SelectedDiscreteFormat is ImageFormatKind.Jpeg or ImageFormatKind.WebP;
        numericDiscreteQuality.Enabled = discreteLossy;
        labelDiscreteQuality.Enabled = discreteLossy;

        borderOverlayEditor.Visible = tabModes.SelectedIndex != 2;
        UpdateBorderCanvasSize();
    }

    private void StoreCurrentBorder()
    {
        if (_activeTabIndex == 0)
        {
            _continuousBorder = borderOverlayEditor.GetSettings();
        }
        else if (_activeTabIndex == 1)
        {
            _discreteBorder = borderOverlayEditor.GetSettings();
        }
    }

    private void LoadBorderForActiveTab()
    {
        _updatingControls = true;
        try
        {
            BorderOverlaySettings settings = _activeTabIndex == 1
                ? _discreteBorder
                : _continuousBorder;
            borderOverlayEditor.SetSettings(settings);
            UpdateBorderCanvasSize();
        }
        finally
        {
            _updatingControls = false;
        }
    }

    private void UpdateBorderCanvasSize()
    {
        borderOverlayEditor.CanvasSize = tabModes.SelectedIndex == 1
            ? new System.Drawing.Size((int)numericDiscreteWidth.Value, (int)numericDiscreteHeight.Value)
            : new System.Drawing.Size((int)numericContinuousWidth.Value, (int)numericContinuousHeight.Value);
    }

    private ImageFormatKind SelectedContinuousFormat =>
        (ImageFormatKind)Math.Clamp(comboContinuousFormat.SelectedIndex, 0, 4);

    private ImageFormatKind SelectedDiscreteFormat =>
        (ImageFormatKind)Math.Clamp(comboDiscreteFormat.SelectedIndex, 0, 4);

    private NonIntegerFusionSettings ReadContinuousSettings(NonIntegerFusionSourceMode? sourceMode = null)
    {
        return new NonIntegerFusionSettings
        {
            CanvasWidth = (int)numericContinuousWidth.Value,
            CanvasHeight = (int)numericContinuousHeight.Value,
            ThetaDegrees = (double)numericContinuousAngle.Value,
            SubpixelPeriod = (double)numericContinuousPeriod.Value,
            ScaleFactor = (double)numericContinuousMultiplier.Value,
            PhaseShift = (double)numericContinuousOffset.Value,
            Duty = (double)numericContinuousDuty.Value,
            SourceMode = sourceMode ?? (NonIntegerFusionSourceMode)Math.Clamp(
                comboContinuousSourceMode.SelectedIndex,
                0,
                Enum.GetValues<NonIntegerFusionSourceMode>().Length - 1),
            ReverseEyes = checkContinuousReverse.Checked
        };
    }

    private DiscreteCrosstalkSettings ReadDiscreteSettings(int? group = null)
    {
        return new DiscreteCrosstalkSettings
        {
            CanvasWidth = (int)numericDiscreteWidth.Value,
            CanvasHeight = (int)numericDiscreteHeight.Value,
            TiltAngleDegrees = (double)numericDiscreteAngle.Value,
            SubpixelPeriod = (int)numericDiscretePeriod.Value,
            LitSubpixelCount = (int)numericDiscreteLitCount.Value,
            Group = group ?? (int)numericDiscreteGroup.Value,
            PixelSizeXMicrometers = (double)numericDiscretePixelX.Value,
            PixelSizeYMicrometers = (double)numericDiscretePixelY.Value,
            ScreenSizeInches = (double)numericDiscreteScreenSize.Value,
            PartitionWidthSubpixels = (double)numericDiscretePartitionWidth.Value,
            StrategyAngleDegrees = (double)numericDiscreteStrategyAngle.Value,
            // 原程序在“3×画布宽度 + 平移量”之后才截断；这里保留小数，
            // 尤其可避免负小数平移被提前截断成 0 后产生一个子像素的偏差。
            TranslationSubpixels = (double)numericDiscreteTranslation.Value,
            Direction = checkDiscretePositiveDirection.Checked
                ? DiscreteCrosstalkDirection.LeftPositive
                : DiscreteCrosstalkDirection.RightPositive,
            ResultType = (DiscreteCrosstalkResultType)Math.Clamp(
                comboDiscreteResultType.SelectedIndex,
                0,
                Enum.GetValues<DiscreteCrosstalkResultType>().Length - 1),
            SingleLightSourceFile = checkDiscreteSingleSource.Checked
        };
    }

    private ImageExportOptions ReadContinuousExportOptions() => new()
    {
        Format = SelectedContinuousFormat,
        Quality = (int)numericContinuousQuality.Value
    };

    private ImageExportOptions ReadDiscreteExportOptions() => new()
    {
        Format = SelectedDiscreteFormat,
        Quality = (int)numericDiscreteQuality.Value
    };

    private void UpdatePreview(bool resetView)
    {
        if (_isRendering || IsDisposed || !_userInterfaceInitialized)
        {
            return;
        }

        _isRendering = true;
        previewTimer.Stop();
        UseWaitCursor = true;
        try
        {
            using Mat image = tabModes.SelectedIndex switch
            {
                0 => GenerateContinuousImage(applyBorder: true),
                1 => GenerateDiscreteImageForPreview(),
                2 => LoadConverterImage(),
                _ => throw new InvalidOperationException("未知的生成模式。")
            };

            Bitmap bitmap = MatBitmapConverter.ToBitmap(image);
            previewControl.SetImage(bitmap, preserveView: !resetView);
            labelPreviewInfo.Text = tabModes.SelectedIndex switch
            {
                0 => $"连续融合：{image.Cols:N0} × {image.Rows:N0} | T={NonIntegerFusionGenerator.CalculateEffectivePeriod(ReadContinuousSettings()):0.######}",
                1 => $"离散光源：{image.Cols:N0} × {image.Rows:N0} | 组 {PreviewDiscreteGroup}",
                2 => $"源图片：{image.Cols:N0} × {image.Rows:N0} | {Path.GetFileName(textConverterImagePath.Text)}",
                _ => string.Empty
            };
            statusLabel.Text = "预览生成完成。";
        }
        catch (Exception exception)
        {
            // 参数或图源失效后不能继续显示上一模式的旧图，避免误判为当前结果。
            previewControl.SetImage(null, preserveView: false);
            labelPreviewInfo.Text = "当前参数没有可用预览";
            statusLabel.Text = $"预览失败：{exception.Message}";
        }
        finally
        {
            UseWaitCursor = false;
            _isRendering = false;
        }
    }

    private int PreviewDiscreteGroup => (int)numericDiscreteGroup.Value < 0
        ? 0
        : (int)numericDiscreteGroup.Value;

    private Mat GenerateContinuousImage(bool applyBorder)
    {
        NonIntegerFusionSettings settings = ReadContinuousSettings();
        Mat result;
        if (settings.SourceMode == NonIntegerFusionSourceMode.CustomImages)
        {
            using Mat left = UnicodeImageLoader.LoadColor(textContinuousLeftPath.Text);
            using Mat right = UnicodeImageLoader.LoadColor(textContinuousRightPath.Text);
            ApplyLegacyChannelOrder(left, right);
            EnsureConfiguredCanvasMatches(settings.CanvasWidth, settings.CanvasHeight, left, right, "自定义左右图源");
            result = NonIntegerFusionGenerator.Generate(settings, left, right);
        }
        else
        {
            result = NonIntegerFusionGenerator.Generate(settings);
        }

        try
        {
            if (applyBorder)
            {
                BorderOverlayRenderer.Apply(result, _continuousBorder);
            }

            return result;
        }
        catch
        {
            result.Dispose();
            throw;
        }
    }

    private Mat GenerateDiscreteImageForPreview()
    {
        DiscreteCrosstalkSettings settings = ReadDiscreteSettings(PreviewDiscreteGroup);
        return GenerateDiscreteImage(settings, applyBorder: true);
    }

    private Mat GenerateDiscreteImage(DiscreteCrosstalkSettings settings, bool applyBorder)
    {
        Mat? sourceA = null;
        Mat? sourceB = null;
        try
        {
            LoadDiscreteCustomSources(settings, out sourceA, out sourceB);
            Mat result = DiscreteCrosstalkGenerator.Generate(settings, sourceA, sourceB);
            try
            {
                if (applyBorder)
                {
                    BorderOverlayRenderer.Apply(result, _discreteBorder);
                }

                return result;
            }
            catch
            {
                result.Dispose();
                throw;
            }
        }
        finally
        {
            sourceA?.Dispose();
            sourceB?.Dispose();
        }
    }

    private Mat LoadConverterImage()
    {
        return UnicodeImageLoader.LoadColor(textConverterImagePath.Text);
    }

    private void ApplyLegacyChannelOrder(Mat left, Mat right)
    {
        if (comboContinuousInputOrder.SelectedIndex == 1)
        {
            UnicodeImageLoader.SwapRedBlueInPlace(left);
            UnicodeImageLoader.SwapRedBlueInPlace(right);
        }
    }

    private void LoadDiscreteCustomSources(
        DiscreteCrosstalkSettings settings,
        out Mat? sourceA,
        out Mat? sourceB)
    {
        sourceA = null;
        sourceB = null;
        if (settings.ResultType != DiscreteCrosstalkResultType.Custom)
        {
            return;
        }

        try
        {
            sourceA = UnicodeImageLoader.LoadColor(textDiscreteSourceA.Text);
            sourceB = UnicodeImageLoader.LoadColor(textDiscreteSourceB.Text);
            EnsureConfiguredCanvasMatches(
                settings.CanvasWidth,
                settings.CanvasHeight,
                sourceA,
                sourceB,
                "离散自定义图源");
        }
        catch
        {
            sourceA?.Dispose();
            sourceB?.Dispose();
            sourceA = null;
            sourceB = null;
            throw;
        }
    }

    private static void EnsureConfiguredCanvasMatches(
        int width,
        int height,
        Mat sourceA,
        Mat sourceB,
        string description)
    {
        if (sourceA.Cols != sourceB.Cols || sourceA.Rows != sourceB.Rows)
        {
            throw new InvalidDataException($"{description}的两张图片尺寸必须完全一致。");
        }

        if (sourceA.Cols != width || sourceA.Rows != height)
        {
            throw new InvalidDataException(
                $"{description}尺寸为 {sourceA.Cols}×{sourceA.Rows}，必须与画布 {width}×{height} 一致；不会自动拉伸或裁剪。");
        }
    }

    private void buttonContinuousPreview_Click(object? sender, EventArgs e) =>
        UpdatePreview(resetView: false);

    private void buttonDiscretePreview_Click(object? sender, EventArgs e) =>
        UpdatePreview(resetView: false);

    private void buttonConverterPreview_Click(object? sender, EventArgs e) =>
        UpdatePreview(resetView: false);

    private void buttonBrowseContinuousLeft_Click(object? sender, EventArgs e) =>
        BrowseImageInto(textContinuousLeftPath);

    private void buttonBrowseContinuousRight_Click(object? sender, EventArgs e) =>
        BrowseImageInto(textContinuousRightPath);

    private void buttonBrowseDiscreteA_Click(object? sender, EventArgs e) =>
        BrowseImageInto(textDiscreteSourceA);

    private void buttonBrowseDiscreteB_Click(object? sender, EventArgs e) =>
        BrowseImageInto(textDiscreteSourceB);

    private void buttonBrowseConverterImage_Click(object? sender, EventArgs e) =>
        BrowseImageInto(textConverterImagePath);

    private void BrowseImageInto(TextBox target)
    {
        string currentPath = target.Text.Trim();
        if (File.Exists(currentPath))
        {
            openImageDialog.InitialDirectory = Path.GetDirectoryName(Path.GetFullPath(currentPath));
            openImageDialog.FileName = Path.GetFileName(currentPath);
        }
        else
        {
            openImageDialog.FileName = string.Empty;
        }

        if (openImageDialog.ShowDialog(this) == DialogResult.OK)
        {
            target.Text = openImageDialog.FileName;
        }
    }

    private void buttonContinuousSave_Click(object? sender, EventArgs e)
    {
        ImageExportOptions options = ReadContinuousExportOptions();
        string extension = ImageFileWriter.GetExtension(options.Format);
        string defaultBaseName;
        try
        {
            defaultBaseName = GetContinuousFileBaseName();
        }
        catch (Exception exception)
        {
            ShowError("文件前缀无效", exception);
            return;
        }

        saveFileDialog.Filter = ImageFileWriter.GetDialogFilter(options.Format);
        saveFileDialog.DefaultExt = extension.TrimStart('.');
        saveFileDialog.FileName = defaultBaseName + extension;
        if (!string.IsNullOrWhiteSpace(_continuousOutputDirectory) &&
            Directory.Exists(_continuousOutputDirectory))
        {
            saveFileDialog.InitialDirectory = _continuousOutputDirectory;
        }

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            string targetPath = ImageFileWriter.NormalizePath(saveFileDialog.FileName, options.Format);
            string targetDirectory = Path.GetDirectoryName(targetPath)
                                     ?? throw new InvalidOperationException("所选文件没有有效目录。");
            string targetBaseName = Path.GetFileNameWithoutExtension(targetPath);
            var targets = new List<string> { targetPath };
            if (checkContinuousWriteMesh.Checked)
            {
                targets.Add(Path.Combine(targetDirectory, targetBaseName + ".txt"));
            }

            if (checkContinuousSaveSources.Checked)
            {
                targets.Add(Path.Combine(targetDirectory, targetBaseName + "_L.tiff"));
                targets.Add(Path.Combine(targetDirectory, targetBaseName + "_R.tiff"));
            }

            if (!ConfirmOverwriteTargets(targets, "连续融合输出"))
            {
                return;
            }

            using Mat image = GenerateContinuousImage(applyBorder: true);
            string actualPath = ImageFileWriter.Write(targetPath, image, options);
            _continuousOutputDirectory = Path.GetDirectoryName(actualPath) ?? string.Empty;
            int outputCount = 1;
            if (checkContinuousWriteMesh.Checked)
            {
                outputCount += LightToolsMeshWriter.WriteImageSourceFiles(
                    _continuousOutputDirectory,
                    Path.GetFileNameWithoutExtension(actualPath),
                    image,
                    // MATLAB 的 pixL 是子像素宽度；MESH 一行含 3×Width 个子像素，
                    // 通用写入器接收完整 RGB 像素宽度，因此这里乘以 3。
                    (double)numericContinuousPixelWidth.Value * 3000d,
                    (double)numericContinuousPixelHeight.Value * 1000d,
                    singleSourceFile: true).Count;
            }

            if (checkContinuousSaveSources.Checked)
            {
                outputCount += SaveContinuousSourceImages(
                    _continuousOutputDirectory,
                    Path.GetFileNameWithoutExtension(actualPath),
                    ReadContinuousSettings());
            }

            statusLabel.Text = $"已保存 {outputCount} 个文件：{_continuousOutputDirectory}";
        }
        catch (Exception exception)
        {
            ShowError("保存连续融合图失败", exception);
        }
    }

    private async void buttonContinuousBatch_Click(object? sender, EventArgs e)
    {
        string? inputDirectory = SelectFolder("选择包含 <名称>_L 与 <名称>_R 的输入目录", _continuousSourceFolder);
        if (inputDirectory is null)
        {
            return;
        }

        string? outputDirectory = SelectFolder("选择连续融合结果输出目录", _continuousOutputDirectory);
        if (outputDirectory is null)
        {
            return;
        }

        IReadOnlyList<FusionInputPair> pairs;
        try
        {
            pairs = FindFusionPairs(inputDirectory);
        }
        catch (Exception exception)
        {
            ShowError("读取左右图源失败", exception);
            return;
        }

        NonIntegerFusionSettings settings = ReadContinuousSettings(NonIntegerFusionSourceMode.CustomImages);
        BorderOverlaySettings border = _continuousBorder.Clone();
        ImageExportOptions options = ReadContinuousExportOptions();
        string prefix;
        try
        {
            prefix = GetSafePrefix();
        }
        catch (Exception exception)
        {
            ShowError("文件前缀无效", exception);
            return;
        }
        bool legacyOrder = comboContinuousInputOrder.SelectedIndex == 1;
        bool writeMesh = checkContinuousWriteMesh.Checked;
        bool saveSources = checkContinuousSaveSources.Checked;
        // 连续融合页 X 参数对应 MATLAB pixL（单个子像素宽度）。
        double pixelXMicrometers = (double)numericContinuousPixelWidth.Value * 3000d;
        double pixelYMicrometers = (double)numericContinuousPixelHeight.Value * 1000d;

        var targets = new List<string>();
        foreach (FusionInputPair pair in pairs)
        {
            string baseName = BuildContinuousBaseName(prefix, pair.BaseName, settings);
            targets.Add(Path.Combine(outputDirectory, baseName + ImageFileWriter.GetExtension(options.Format)));
            if (writeMesh)
            {
                targets.Add(Path.Combine(outputDirectory, baseName + ".txt"));
            }

            if (saveSources)
            {
                targets.Add(Path.Combine(outputDirectory, baseName + "_L.tiff"));
                targets.Add(Path.Combine(outputDirectory, baseName + "_R.tiff"));
            }
        }

        if (!ConfirmOverwriteTargets(targets, "批量连续融合"))
        {
            return;
        }

        _continuousSourceFolder = inputDirectory;
        _continuousOutputDirectory = outputDirectory;
        BeginExport("正在批量融合左右图源...");
        try
        {
            int fileCount = await Task.Run(() =>
            {
                int written = 0;
                foreach (FusionInputPair pair in pairs)
                {
                    using Mat left = UnicodeImageLoader.LoadColor(pair.LeftPath);
                    using Mat right = UnicodeImageLoader.LoadColor(pair.RightPath);
                    if (legacyOrder)
                    {
                        UnicodeImageLoader.SwapRedBlueInPlace(left);
                        UnicodeImageLoader.SwapRedBlueInPlace(right);
                    }

                    EnsureConfiguredCanvasMatches(
                        settings.CanvasWidth,
                        settings.CanvasHeight,
                        left,
                        right,
                        $"{pair.BaseName} 左右图源");
                    using Mat fused = NonIntegerFusionGenerator.Generate(settings, left, right);
                    BorderOverlayRenderer.Apply(fused, border);

                    string baseName = BuildContinuousBaseName(prefix, pair.BaseName, settings);
                    string imagePath = Path.Combine(
                        outputDirectory,
                        baseName + ImageFileWriter.GetExtension(options.Format));
                    ImageFileWriter.Write(imagePath, fused, options);
                    written++;

                    if (writeMesh)
                    {
                        written += LightToolsMeshWriter.WriteImageSourceFiles(
                            outputDirectory,
                            baseName,
                            fused,
                            pixelXMicrometers,
                            pixelYMicrometers,
                            singleSourceFile: true).Count;
                    }

                    if (saveSources)
                    {
                        var sourceOptions = new ImageExportOptions { Format = ImageFormatKind.Tiff, Quality = 100 };
                        Mat effectiveLeft = settings.ReverseEyes ? right : left;
                        Mat effectiveRight = settings.ReverseEyes ? left : right;
                        ImageFileWriter.Write(
                            Path.Combine(outputDirectory, baseName + "_L.tiff"),
                            effectiveLeft,
                            sourceOptions);
                        ImageFileWriter.Write(
                            Path.Combine(outputDirectory, baseName + "_R.tiff"),
                            effectiveRight,
                            sourceOptions);
                        written += 2;
                    }
                }

                return written;
            });

            statusLabel.Text = $"批量融合完成：{pairs.Count} 组，写入 {fileCount} 个文件。";
        }
        catch (Exception exception)
        {
            ShowError("批量融合失败", exception);
        }
        finally
        {
            EndExport();
        }
    }

    private void buttonDiscreteSave_Click(object? sender, EventArgs e)
    {
        if (!EnsureDiscreteLosslessFormat())
        {
            return;
        }

        DiscreteCrosstalkSettings settings = ReadDiscreteSettings(PreviewDiscreteGroup);
        ImageExportOptions options = ReadDiscreteExportOptions();
        string extension = ImageFileWriter.GetExtension(options.Format);
        saveFileDialog.Filter = ImageFileWriter.GetDialogFilter(options.Format);
        saveFileDialog.DefaultExt = extension.TrimStart('.');
        try
        {
            saveFileDialog.FileName =
                DiscreteCrosstalkGenerator.GetFileBaseName(settings, settings.Group) + extension;
        }
        catch (Exception exception)
        {
            ShowError("离散参数无效", exception);
            return;
        }

        if (!string.IsNullOrWhiteSpace(_discreteOutputDirectory) && Directory.Exists(_discreteOutputDirectory))
        {
            saveFileDialog.InitialDirectory = _discreteOutputDirectory;
        }

        if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            string targetPath = ImageFileWriter.NormalizePath(saveFileDialog.FileName, options.Format);
            if (!ConfirmOverwriteTargets([targetPath], "当前离散光源图"))
            {
                return;
            }

            using Mat image = GenerateDiscreteImage(settings, applyBorder: true);
            string path = ImageFileWriter.Write(targetPath, image, options);
            _discreteOutputDirectory = Path.GetDirectoryName(path) ?? string.Empty;
            statusLabel.Text = $"已保存：{path}";
        }
        catch (Exception exception)
        {
            ShowError("保存离散光源图失败", exception);
        }
    }

    private async void buttonDiscreteBatch_Click(object? sender, EventArgs e)
    {
        if (!EnsureDiscreteLosslessFormat())
        {
            return;
        }

        string? outputDirectory = SelectFolder("选择离散光源图输出目录", _discreteOutputDirectory);
        if (outputDirectory is null)
        {
            return;
        }

        DiscreteCrosstalkSettings settings = ReadDiscreteSettings();
        BorderOverlaySettings border = _discreteBorder.Clone();
        ImageExportOptions options = ReadDiscreteExportOptions();
        IReadOnlyList<int> groups;
        try
        {
            groups = DiscreteCrosstalkGenerator.ResolveGroups(settings);
        }
        catch (Exception exception)
        {
            ShowError("离散参数无效", exception);
            return;
        }

        if (!ConfirmLargeBatch(groups.Count, filesPerGroup: 1, "离散光源图"))
        {
            return;
        }

        string[] targetPaths = groups
            .Select(group => Path.Combine(
                outputDirectory,
                DiscreteCrosstalkGenerator.GetFileBaseName(settings, group) +
                ImageFileWriter.GetExtension(options.Format)))
            .ToArray();
        if (!ConfirmOverwriteTargets(targetPaths, "离散光源图"))
        {
            return;
        }

        Mat? sourceA = null;
        Mat? sourceB = null;
        try
        {
            LoadDiscreteCustomSources(settings, out sourceA, out sourceB);
            _discreteOutputDirectory = outputDirectory;
            BeginExport("正在按组导出离散光源图...");
            IReadOnlyList<string> paths = await Task.Run(() =>
            {
                var written = new List<string>();
                DiscreteCrosstalkGenerator.GenerateBatch(
                    settings,
                    (group, image) =>
                    {
                        BorderOverlayRenderer.Apply(image, border);
                        string path = Path.Combine(
                            outputDirectory,
                            DiscreteCrosstalkGenerator.GetFileBaseName(settings, group) +
                            ImageFileWriter.GetExtension(options.Format));
                        written.Add(ImageFileWriter.Write(path, image, options));
                    },
                    sourceA,
                    sourceB);
                return written;
            });
            statusLabel.Text = $"离散图导出完成：{paths.Count} 张。";
        }
        catch (Exception exception)
        {
            ShowError("批量导出离散图失败", exception);
        }
        finally
        {
            sourceA?.Dispose();
            sourceB?.Dispose();
            EndExport();
        }
    }

    private async void buttonDiscreteLightTools_Click(object? sender, EventArgs e)
    {
        string? outputDirectory = SelectFolder("选择 LightTools 光源 TXT 输出目录", _discreteOutputDirectory);
        if (outputDirectory is null)
        {
            return;
        }

        DiscreteCrosstalkSettings settings = ReadDiscreteSettings();
        BorderOverlaySettings border = _discreteBorder.Clone();
        IReadOnlyList<int> groups;
        try
        {
            groups = DiscreteCrosstalkGenerator.ResolveGroups(settings);
        }
        catch (Exception exception)
        {
            ShowError("离散参数无效", exception);
            return;
        }

        int meshFilesPerGroup = settings.SingleLightSourceFile ? 1 : 3;
        if (!ConfirmLargeBatch(groups.Count, meshFilesPerGroup, "LightTools 光源"))
        {
            return;
        }

        string[] targetPaths = groups
            .SelectMany(group => GetDiscreteMeshTargetPaths(outputDirectory, settings, group))
            .ToArray();
        if (!ConfirmOverwriteTargets(targetPaths, "LightTools 光源"))
        {
            return;
        }

        Mat? sourceA = null;
        Mat? sourceB = null;
        try
        {
            LoadDiscreteCustomSources(settings, out sourceA, out sourceB);
            _discreteOutputDirectory = outputDirectory;
            BeginExport("正在输出 LightTools 光源 TXT...");
            IReadOnlyList<string> paths = await Task.Run(() =>
            {
                var written = new List<string>();
                DiscreteCrosstalkGenerator.GenerateBatch(
                    settings,
                    (group, image) =>
                    {
                        BorderOverlayRenderer.Apply(image, border);
                        written.AddRange(LightToolsMeshWriter.WriteForGroup(
                            outputDirectory,
                            settings,
                            group,
                            image));
                    },
                    sourceA,
                    sourceB);
                return written;
            });
            statusLabel.Text = $"LightTools 导出完成：{paths.Count} 个文件。";
        }
        catch (Exception exception)
        {
            ShowError("LightTools 导出失败", exception);
        }
        finally
        {
            sourceA?.Dispose();
            sourceB?.Dispose();
            EndExport();
        }
    }

    private async void buttonConverterExport_Click(object? sender, EventArgs e)
    {
        string sourcePath = textConverterImagePath.Text.Trim();
        if (!File.Exists(sourcePath))
        {
            ShowError("图片转换失败", new FileNotFoundException("请选择存在的源图片。", sourcePath));
            return;
        }

        string? outputDirectory = SelectFolder("选择转换后的 LightTools TXT 输出目录", _converterOutputDirectory);
        if (outputDirectory is null)
        {
            return;
        }

        double pixelX = (double)numericConverterPixelX.Value;
        double pixelY = (double)numericConverterPixelY.Value;
        bool single = checkConverterSingleSource.Checked;
        string converterBaseName = Path.GetFileNameWithoutExtension(sourcePath);
        string[] targetPaths = single
            ? [Path.Combine(outputDirectory, converterBaseName + ".txt")]
            :
            [
                Path.Combine(outputDirectory, converterBaseName + "_R.txt"),
                Path.Combine(outputDirectory, converterBaseName + "_G.txt"),
                Path.Combine(outputDirectory, converterBaseName + "_B.txt")
            ];
        if (!ConfirmOverwriteTargets(targetPaths, "图片转换输出"))
        {
            return;
        }

        _converterOutputDirectory = outputDirectory;
        BeginExport("正在将图片转换为 LightTools 光源 TXT...");
        try
        {
            IReadOnlyList<string> paths = await Task.Run(() =>
            {
                using Mat image = UnicodeImageLoader.LoadColor(sourcePath);
                return LightToolsMeshWriter.WriteImageSourceFiles(
                    outputDirectory,
                    converterBaseName,
                    image,
                    pixelX,
                    pixelY,
                    single);
            });
            statusLabel.Text = $"图片转换完成：{paths.Count} 个文件。";
        }
        catch (Exception exception)
        {
            ShowError("图片转换失败", exception);
        }
        finally
        {
            EndExport();
        }
    }

    private int SaveContinuousSourceImages(
        string outputDirectory,
        string fileBaseName,
        NonIntegerFusionSettings settings)
    {
        Mat? left = null;
        Mat? right = null;
        try
        {
            if (settings.SourceMode == NonIntegerFusionSourceMode.CustomImages)
            {
                left = UnicodeImageLoader.LoadColor(textContinuousLeftPath.Text);
                right = UnicodeImageLoader.LoadColor(textContinuousRightPath.Text);
                ApplyLegacyChannelOrder(left, right);
                EnsureConfiguredCanvasMatches(
                    settings.CanvasWidth,
                    settings.CanvasHeight,
                    left,
                    right,
                    "自定义左右图源");
            }
            else
            {
                (left, right) = NonIntegerFusionGenerator.CreateEffectiveBuiltInSources(settings);
                // 内置图源服务已经按 ReverseEyes 交换；避免下面再次交换。
                settings = settings.Clone();
                settings.ReverseEyes = false;
            }

            Mat effectiveLeft = settings.ReverseEyes ? right : left;
            Mat effectiveRight = settings.ReverseEyes ? left : right;
            var options = new ImageExportOptions { Format = ImageFormatKind.Tiff, Quality = 100 };
            ImageFileWriter.Write(
                Path.Combine(outputDirectory, fileBaseName + "_L.tiff"),
                effectiveLeft,
                options);
            ImageFileWriter.Write(
                Path.Combine(outputDirectory, fileBaseName + "_R.tiff"),
                effectiveRight,
                options);
            return 2;
        }
        finally
        {
            left?.Dispose();
            right?.Dispose();
        }
    }

    private static IReadOnlyList<string> GetDiscreteMeshTargetPaths(
        string outputDirectory,
        DiscreteCrosstalkSettings settings,
        int group)
    {
        string baseName = DiscreteCrosstalkGenerator.GetFileBaseName(settings, group);
        return settings.SingleLightSourceFile
            ? [Path.Combine(outputDirectory, baseName + ".txt")]
            :
            [
                Path.Combine(outputDirectory, baseName + "_R.txt"),
                Path.Combine(outputDirectory, baseName + "_G.txt"),
                Path.Combine(outputDirectory, baseName + "_B.txt")
            ];
    }

    private bool ConfirmOverwriteTargets(IEnumerable<string> paths, string outputName)
    {
        string[] existing = paths
            .Select(Path.GetFullPath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(File.Exists)
            .ToArray();
        if (existing.Length == 0)
        {
            return true;
        }

        DialogResult result = MessageBox.Show(
            this,
            $"{outputName}有 {existing.Length} 个目标文件已存在，将被覆盖。是否继续？",
            "确认覆盖",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);
        return result == DialogResult.Yes;
    }

    private bool ConfirmLargeBatch(int groupCount, int filesPerGroup, string outputName)
    {
        int estimatedFileCount = checked(groupCount * filesPerGroup);
        if (estimatedFileCount <= 64)
        {
            return true;
        }

        DialogResult result = MessageBox.Show(
            this,
            $"{outputName}将生成 {groupCount} 组、约 {estimatedFileCount} 个文件。" +
            "大批量任务可能耗时很久，运行期间窗口不能关闭。是否继续？",
            "确认大批量导出",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);
        return result == DialogResult.Yes;
    }

    private bool EnsureDiscreteLosslessFormat()
    {
        if (SelectedDiscreteFormat is ImageFormatKind.Png or ImageFormatKind.Bmp or ImageFormatKind.Tiff)
        {
            return true;
        }

        DialogResult result = MessageBox.Show(
            this,
            "离散光源图需要保留精确像素。是否切换为 PNG 后继续？",
            "需要无损格式",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button1);
        if (result != DialogResult.Yes)
        {
            return false;
        }

        comboDiscreteFormat.SelectedIndex = (int)ImageFormatKind.Png;
        return true;
    }

    private string? SelectFolder(string description, string previousDirectory)
    {
        folderBrowserDialog.Description = description;
        if (!string.IsNullOrWhiteSpace(previousDirectory) && Directory.Exists(previousDirectory))
        {
            folderBrowserDialog.SelectedPath = previousDirectory;
        }

        return folderBrowserDialog.ShowDialog(this) == DialogResult.OK
            ? folderBrowserDialog.SelectedPath
            : null;
    }

    private static IReadOnlyList<FusionInputPair> FindFusionPairs(string directory)
    {
        string fullDirectory = Path.GetFullPath(directory);
        var pairs = new Dictionary<string, PairBuilder>(StringComparer.OrdinalIgnoreCase);
        string[] allowedExtensions = [".png", ".tif", ".tiff"];

        foreach (string path in Directory.EnumerateFiles(fullDirectory).OrderBy(item => item, StringComparer.OrdinalIgnoreCase))
        {
            if (!allowedExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            string stem = Path.GetFileNameWithoutExtension(path);
            if (stem.Length <= 2 || stem[^2] != '_')
            {
                continue;
            }

            char side = char.ToUpperInvariant(stem[^1]);
            if (side is not ('L' or 'R'))
            {
                continue;
            }

            string baseName = stem[..^2];
            if (!pairs.TryGetValue(baseName, out PairBuilder? builder))
            {
                builder = new PairBuilder(baseName);
                pairs.Add(baseName, builder);
            }

            if (side == 'L')
            {
                if (builder.LeftPath is not null)
                {
                    throw new InvalidDataException($"{baseName} 存在多个 _L 文件，请只保留一个。");
                }

                builder.LeftPath = path;
            }
            else
            {
                if (builder.RightPath is not null)
                {
                    throw new InvalidDataException($"{baseName} 存在多个 _R 文件，请只保留一个。");
                }

                builder.RightPath = path;
            }
        }

        string[] incompletePairs = pairs.Values
            .Where(item => item.LeftPath is null || item.RightPath is null)
            .Select(item => $"{item.BaseName}（缺少 {(item.LeftPath is null ? "_L" : "_R")}）")
            .OrderBy(item => item, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (incompletePairs.Length > 0)
        {
            string examples = string.Join("、", incompletePairs.Take(8));
            string suffix = incompletePairs.Length > 8 ? $" 等 {incompletePairs.Length} 组" : string.Empty;
            throw new InvalidDataException($"发现不完整的左右图对：{examples}{suffix}。请补齐或移出输入目录。");
        }

        FusionInputPair[] completed = pairs.Values
            .Where(item => item.LeftPath is not null && item.RightPath is not null)
            .Select(item => new FusionInputPair(item.BaseName, item.LeftPath!, item.RightPath!))
            .OrderBy(item => item.BaseName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (completed.Length == 0)
        {
            throw new InvalidDataException("目录中没有找到成对的 <名称>_L 与 <名称>_R PNG/TIFF 文件。");
        }

        return completed;
    }

    private string GetContinuousFileBaseName()
    {
        NonIntegerFusionSettings settings = ReadContinuousSettings();
        string sourceName = settings.SourceMode switch
        {
            NonIntegerFusionSourceMode.FullWhiteBlack or
            NonIntegerFusionSourceMode.FixedOneSubpixelWhiteBlack or
            NonIntegerFusionSourceMode.DutyWhiteBlack => settings.ReverseEyes ? "KW" : "WK",
            NonIntegerFusionSourceMode.RedBlue => settings.ReverseEyes ? "BR" : "RB",
            NonIntegerFusionSourceMode.CustomImages => "USER",
            _ => throw new ArgumentOutOfRangeException(nameof(settings.SourceMode))
        };
        return BuildContinuousBaseName(GetSafePrefix(), sourceName, settings);
    }

    private static string BuildContinuousBaseName(
        string prefix,
        string sourceName,
        NonIntegerFusionSettings settings)
    {
        string reverseSuffix = settings.ReverseEyes ? "_rev" : string.Empty;
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{prefix}_{sourceName}{reverseSuffix}_{settings.SubpixelPeriod:F3}_{settings.ThetaDegrees:F3}_{settings.PhaseShift:0.###}");
    }

    private string GetSafePrefix()
    {
        string prefix = textContinuousPrefix.Text.Trim();
        if (string.IsNullOrWhiteSpace(prefix))
        {
            prefix = "fused";
        }

        if (prefix.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new InvalidDataException("文件前缀包含无效字符。");
        }

        return prefix;
    }

    private void BeginExport(string status)
    {
        _activeExports++;
        settingsFlowPanel.Enabled = false;
        UseWaitCursor = true;
        statusLabel.Text = status;
    }

    private void EndExport()
    {
        _activeExports = Math.Max(0, _activeExports - 1);
        if (IsDisposed || Disposing)
        {
            return;
        }

        settingsFlowPanel.Enabled = _activeExports == 0;
        UseWaitCursor = _activeExports != 0;
    }

    private void SavePreferences()
    {
        StoreCurrentBorder();
        var matlab = new MatlabNonIntegerPreferences
        {
            CanvasWidth = (int)numericContinuousWidth.Value,
            CanvasHeight = (int)numericContinuousHeight.Value,
            Theta = (double)numericContinuousAngle.Value,
            SubpixelPeriod = (double)numericContinuousPeriod.Value,
            PeriodMultiplier = (double)numericContinuousMultiplier.Value,
            PhaseOffset = (double)numericContinuousOffset.Value,
            DutyCycle = (double)numericContinuousDuty.Value,
            SourceMode = ((NonIntegerFusionSourceMode)Math.Max(0, comboContinuousSourceMode.SelectedIndex)).ToString(),
            Reverse = checkContinuousReverse.Checked,
            SaveSourceImages = checkContinuousSaveSources.Checked,
            WriteMesh = checkContinuousWriteMesh.Checked,
            LeftSourcePath = textContinuousLeftPath.Text,
            RightSourcePath = textContinuousRightPath.Text,
            SourceFolderPath = _continuousSourceFolder,
            InputChannelOrder = comboContinuousInputOrder.SelectedIndex == 1 ? "LegacyBgr" : "RGB",
            OutputPrefix = textContinuousPrefix.Text,
            OutputDirectory = _continuousOutputDirectory,
            OutputFormat = SelectedContinuousFormat,
            Quality = (int)numericContinuousQuality.Value,
            PixelWidthMillimeters = (double)numericContinuousPixelWidth.Value,
            PixelHeightMillimeters = (double)numericContinuousPixelHeight.Value,
            BorderOverlay = _continuousBorder.Clone()
        };
        var discrete = new DiscreteNonIntegerPreferences
        {
            Width = (int)numericDiscreteWidth.Value,
            Height = (int)numericDiscreteHeight.Value,
            AngleDegrees = (double)numericDiscreteAngle.Value,
            Period = (int)numericDiscretePeriod.Value,
            LitCount = (int)numericDiscreteLitCount.Value,
            Group = (int)numericDiscreteGroup.Value,
            PixelXMicrometers = (double)numericDiscretePixelX.Value,
            PixelYMicrometers = (double)numericDiscretePixelY.Value,
            ScreenSizeInches = (double)numericDiscreteScreenSize.Value,
            PartitionWidth = (double)numericDiscretePartitionWidth.Value,
            StrategyAngleDegrees = (double)numericDiscreteStrategyAngle.Value,
            Translation = (double)numericDiscreteTranslation.Value,
            Direction = checkDiscretePositiveDirection.Checked
                ? nameof(DiscreteCrosstalkDirection.LeftPositive)
                : nameof(DiscreteCrosstalkDirection.RightPositive),
            ResultType = ((DiscreteCrosstalkResultType)Math.Max(0, comboDiscreteResultType.SelectedIndex)).ToString(),
            SingleSource = checkDiscreteSingleSource.Checked,
            CustomSourceAPath = textDiscreteSourceA.Text,
            CustomSourceBPath = textDiscreteSourceB.Text,
            OutputDirectory = _discreteOutputDirectory,
            OutputFormat = SelectedDiscreteFormat,
            Quality = (int)numericDiscreteQuality.Value,
            BorderOverlay = _discreteBorder.Clone()
        };
        var converter = new ImageConverterPreferences
        {
            SourceImagePath = textConverterImagePath.Text,
            PixelXMicrometers = (double)numericConverterPixelX.Value,
            PixelYMicrometers = (double)numericConverterPixelY.Value,
            SingleSource = checkConverterSingleSource.Checked,
            OutputDirectory = _converterOutputDirectory
        };
        var overlay = new PreviewOverlayPreferences
        {
            ShowCenterCrosshair = previewControl.ShowCenterCrosshair,
            ShowPixelCoordinates = previewControl.ShowPixelCoordinates
        };

        if (!UserSettingsStore.Shared.TryUpdateAndSave(
                root =>
                {
                    root.NonIntegerBlend.ActiveTabIndex = tabModes.SelectedIndex;
                    root.NonIntegerBlend.ActiveMode = tabModes.SelectedIndex switch
                    {
                        0 => "Matlab",
                        1 => "Discrete",
                        _ => "ImageConverter"
                    };
                    root.NonIntegerBlend.Matlab = matlab;
                    root.NonIntegerBlend.Discrete = discrete;
                    root.NonIntegerBlend.Converter = converter;
                    root.NonIntegerBlend.PreviewOverlay = overlay;
                },
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

    private void NonIntegerFusionForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_activeExports > 0)
        {
            e.Cancel = true;
            MessageBox.Show(
                this,
                "正在导出文件，请等待完成后再关闭窗口。",
                "正在导出",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        SavePreferences();
    }

    private void NonIntegerFusionForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        previewTimer.Stop();
        previewControl.SetImage(null, preserveView: false);
    }

    private void ShowError(string title, Exception exception)
    {
        statusLabel.Text = $"{title}：{exception.Message}";
        MessageBox.Show(this, exception.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private static int ValidFormatIndex(ImageFormatKind format) =>
        Enum.IsDefined(format) ? (int)format : (int)ImageFormatKind.Png;

    private static int ParseEnumIndex<TEnum>(string? value, TEnum fallback)
        where TEnum : struct, Enum
    {
        return Enum.TryParse(value, ignoreCase: true, out TEnum parsed) && Enum.IsDefined(parsed)
            ? Convert.ToInt32(parsed, CultureInfo.InvariantCulture)
            : Convert.ToInt32(fallback, CultureInfo.InvariantCulture);
    }

    private static void SetNumericValue(NumericUpDown control, int value) =>
        SetNumericValue(control, (double)value);

    private static void SetNumericValue(NumericUpDown control, double value)
    {
        decimal converted;
        try
        {
            converted = double.IsFinite(value) ? (decimal)value : control.Value;
        }
        catch (OverflowException)
        {
            converted = value < 0 ? control.Minimum : control.Maximum;
        }

        control.Value = Math.Min(control.Maximum, Math.Max(control.Minimum, converted));
    }

    private sealed class PairBuilder(string baseName)
    {
        public string BaseName { get; } = baseName;

        public string? LeftPath { get; set; }

        public string? RightPath { get; set; }
    }

    private sealed record FusionInputPair(string BaseName, string LeftPath, string RightPath);
}
