namespace EolTestPatternGenerator.Models;

/// <summary>
/// 应用程序的用户级配置根对象。
/// Version 用于以后新增字段或迁移旧配置；缺失的节会由持久化服务补成默认值。
/// </summary>
public sealed class ApplicationPreferences
{
    public const int CurrentVersion = 1;

    public int Version { get; set; } = CurrentVersion;

    public MainPreferences Main { get; set; } = new();

    public PhaseStripePreferences PhaseStripe { get; set; } = new();

    public ScreenOnePreferences ScreenOne { get; set; } = new();

    /// <summary>
    /// 为后续接入“非整数融合图”预留独立配置节，避免与现有八步相移参数混用。
    /// </summary>
    public NonIntegerBlendPreferences NonIntegerBlend { get; set; } = new();

    /// <summary>
    /// 返回不共享可变引用的副本，防止界面线程在持久化过程中修改同一对象。
    /// </summary>
    public ApplicationPreferences Clone()
    {
        return new ApplicationPreferences
        {
            Version = Version,
            Main = (Main ?? new MainPreferences()).Clone(),
            PhaseStripe = (PhaseStripe ?? new PhaseStripePreferences()).Clone(),
            ScreenOne = (ScreenOne ?? new ScreenOnePreferences()).Clone(),
            NonIntegerBlend = (NonIntegerBlend ?? new NonIntegerBlendPreferences()).Clone()
        };
    }

    /// <summary>
    /// JSON 中缺失或显式写为 null 的节恢复为默认对象。
    /// 数值有效范围由各窗体写入 NumericUpDown 时负责截断，此处不改变用户数值。
    /// </summary>
    internal void RestoreMissingSections()
    {
        Main ??= new MainPreferences();
        PhaseStripe ??= new PhaseStripePreferences();
        ScreenOne ??= new ScreenOnePreferences();
        NonIntegerBlend ??= new NonIntegerBlendPreferences();

        Main.RestoreMissingSections();
        PhaseStripe.RestoreMissingSections();
        ScreenOne.RestoreMissingSections();
        NonIntegerBlend.RestoreMissingSections();
    }
}

/// <summary>所有预览窗口共用的辅助显示开关；不保存缩放比例与平移位置。</summary>
public sealed class PreviewOverlayPreferences
{
    public bool ShowCenterCrosshair { get; set; } = true;

    public bool ShowPixelCoordinates { get; set; } = true;

    public PreviewOverlayPreferences Clone()
    {
        return new PreviewOverlayPreferences
        {
            ShowCenterCrosshair = ShowCenterCrosshair,
            ShowPixelCoordinates = ShowPixelCoordinates
        };
    }
}

/// <summary>主窗口中每种图卡的独立参数以及当前导出选项。</summary>
public sealed class MainPreferences
{
    public PatternType SelectedPatternType { get; set; } = PatternType.Border;

    public ImageFormatKind OutputFormat { get; set; } = ImageFormatKind.Png;

    public int Quality { get; set; } = 95;

    /// <summary>
    /// 主窗口按图卡类型保存参数快照；首次运行时为空，由窗体以 PatternPresets 补齐。
    /// 其中也包含导入图片路径和每种图卡自己的白框参数。
    /// </summary>
    public Dictionary<PatternType, PatternSettings> PatternProfiles { get; set; } = new();

    public PreviewOverlayPreferences PreviewOverlay { get; set; } = new();

    public MainPreferences Clone()
    {
        var clone = new MainPreferences
        {
            SelectedPatternType = SelectedPatternType,
            OutputFormat = OutputFormat,
            Quality = Quality,
            PreviewOverlay = (PreviewOverlay ?? new PreviewOverlayPreferences()).Clone(),
            PatternProfiles = new Dictionary<PatternType, PatternSettings>()
        };

        if (PatternProfiles is not null)
        {
            foreach ((PatternType type, PatternSettings settings) in PatternProfiles)
            {
                if (settings is not null)
                {
                    clone.PatternProfiles[type] = settings.Clone();
                }
            }
        }

        return clone;
    }

    internal void RestoreMissingSections()
    {
        PatternProfiles ??= new Dictionary<PatternType, PatternSettings>();
        PreviewOverlay ??= new PreviewOverlayPreferences();

        // 单个旧配置项也可能缺少白框对象；补齐引用但不校正其数值。
        foreach (PatternSettings settings in PatternProfiles.Values)
        {
            if (settings is not null)
            {
                settings.BorderOverlay ??= new BorderOverlaySettings();
                settings.SourceImagePath ??= string.Empty;
            }
        }
    }
}

/// <summary>现有“串扰图卡”（RGB 八步二值相移）窗口的用户输入。</summary>
public sealed class PhaseStripePreferences
{
    public PatternSettings Settings { get; set; } = CreateDefaultSettings();

    public ImageFormatKind OutputFormat { get; set; } = ImageFormatKind.Png;

    public int Quality { get; set; } = 95;

    public PreviewOverlayPreferences PreviewOverlay { get; set; } = new();

    public PhaseStripePreferences Clone()
    {
        return new PhaseStripePreferences
        {
            Settings = (Settings ?? CreateDefaultSettings()).Clone(),
            OutputFormat = OutputFormat,
            Quality = Quality,
            PreviewOverlay = (PreviewOverlay ?? new PreviewOverlayPreferences()).Clone()
        };
    }

    internal void RestoreMissingSections()
    {
        Settings ??= CreateDefaultSettings();
        Settings.PatternType = PatternType.PhaseStripes;
        Settings.BorderOverlay ??= new BorderOverlaySettings();
        Settings.SourceImagePath ??= string.Empty;
        PreviewOverlay ??= new PreviewOverlayPreferences();
    }

    private static PatternSettings CreateDefaultSettings()
    {
        return new PatternSettings
        {
            PatternType = PatternType.PhaseStripes
        };
    }
}

/// <summary>“显示器3D图/1号屏”窗口的画布、左右矩形和导出选项。</summary>
public sealed class ScreenOnePreferences
{
    public ScreenOneSettings Settings { get; set; } = ScreenOneSettings.CreateReferenceDefault();

    public ScreenOneCardKind CardKind { get; set; } = ScreenOneCardKind.BlackLeftWhiteRight;

    public ImageFormatKind OutputFormat { get; set; } = ImageFormatKind.Png;

    public int Quality { get; set; } = 95;

    public PreviewOverlayPreferences PreviewOverlay { get; set; } = new();

    public ScreenOnePreferences Clone()
    {
        return new ScreenOnePreferences
        {
            Settings = (Settings ?? ScreenOneSettings.CreateReferenceDefault()).Clone(),
            CardKind = CardKind,
            OutputFormat = OutputFormat,
            Quality = Quality,
            PreviewOverlay = (PreviewOverlay ?? new PreviewOverlayPreferences()).Clone()
        };
    }

    internal void RestoreMissingSections()
    {
        Settings ??= ScreenOneSettings.CreateReferenceDefault();
        PreviewOverlay ??= new PreviewOverlayPreferences();
    }
}

/// <summary>
/// 非整数融合功能的配置容器。算法模式用字符串保存，避免配置层依赖尚未确定的界面枚举。
/// </summary>
public sealed class NonIntegerBlendPreferences
{
    public int ActiveTabIndex { get; set; }

    public string ActiveMode { get; set; } = "Matlab";

    public MatlabNonIntegerPreferences Matlab { get; set; } = new();

    public DiscreteNonIntegerPreferences Discrete { get; set; } = new();

    public ImageConverterPreferences Converter { get; set; } = new();

    public PreviewOverlayPreferences PreviewOverlay { get; set; } = new();

    public NonIntegerBlendPreferences Clone()
    {
        return new NonIntegerBlendPreferences
        {
            ActiveTabIndex = ActiveTabIndex,
            ActiveMode = ActiveMode ?? "Matlab",
            Matlab = (Matlab ?? new MatlabNonIntegerPreferences()).Clone(),
            Discrete = (Discrete ?? new DiscreteNonIntegerPreferences()).Clone(),
            Converter = (Converter ?? new ImageConverterPreferences()).Clone(),
            PreviewOverlay = (PreviewOverlay ?? new PreviewOverlayPreferences()).Clone()
        };
    }

    internal void RestoreMissingSections()
    {
        ActiveMode ??= "Matlab";
        Matlab ??= new MatlabNonIntegerPreferences();
        Discrete ??= new DiscreteNonIntegerPreferences();
        Converter ??= new ImageConverterPreferences();
        PreviewOverlay ??= new PreviewOverlayPreferences();
        Matlab.RestoreMissingSections();
        Discrete.RestoreMissingSections();
        Converter.RestoreMissingSections();
    }
}

/// <summary>根据 MATLAB 脚本梳理出的非整数融合参数。</summary>
public sealed class MatlabNonIntegerPreferences
{
    public int CanvasWidth { get; set; } = 1920;

    public int CanvasHeight { get; set; } = 1080;

    /// <summary>MATLAB 参数 theta。</summary>
    public double Theta { get; set; } = 18.435d;

    /// <summary>子像素周期，对应脚本中的 spixtol。</summary>
    public double SubpixelPeriod { get; set; } = 10d;

    /// <summary>周期倍率，对应脚本中的 m。</summary>
    public double PeriodMultiplier { get; set; } = 1d;

    /// <summary>相位偏移，对应脚本中的 picnum。</summary>
    public double PhaseOffset { get; set; }

    public double DutyCycle { get; set; } = 0.25d;

    /// <summary>白黑、固定源、可调源、红蓝、自定义或文件夹等来源模式。</summary>
    public string SourceMode { get; set; } = "DutyWhiteBlack";

    public bool Reverse { get; set; }

    public bool SaveSourceImages { get; set; }

    public bool WriteMesh { get; set; } = true;

    public string LeftSourcePath { get; set; } = string.Empty;

    public string RightSourcePath { get; set; } = string.Empty;

    public string SourceFolderPath { get; set; } = string.Empty;

    public string InputChannelOrder { get; set; } = "RGB";

    public string OutputPrefix { get; set; } = "fused";

    public string OutputDirectory { get; set; } = string.Empty;

    public ImageFormatKind OutputFormat { get; set; } = ImageFormatKind.Png;

    public int Quality { get; set; } = 95;

    public double PixelWidthMillimeters { get; set; } = 0.0192d;

    public double PixelHeightMillimeters { get; set; } = 0.0576d;

    public BorderOverlaySettings BorderOverlay { get; set; } = new();

    public MatlabNonIntegerPreferences Clone()
    {
        return new MatlabNonIntegerPreferences
        {
            CanvasWidth = CanvasWidth,
            CanvasHeight = CanvasHeight,
            Theta = Theta,
            SubpixelPeriod = SubpixelPeriod,
            PeriodMultiplier = PeriodMultiplier,
            PhaseOffset = PhaseOffset,
            DutyCycle = DutyCycle,
            SourceMode = SourceMode ?? "DutyWhiteBlack",
            Reverse = Reverse,
            SaveSourceImages = SaveSourceImages,
            WriteMesh = WriteMesh,
            LeftSourcePath = LeftSourcePath ?? string.Empty,
            RightSourcePath = RightSourcePath ?? string.Empty,
            SourceFolderPath = SourceFolderPath ?? string.Empty,
            InputChannelOrder = InputChannelOrder ?? "RGB",
            OutputPrefix = OutputPrefix ?? "fused",
            OutputDirectory = OutputDirectory ?? string.Empty,
            OutputFormat = OutputFormat,
            Quality = Quality,
            PixelWidthMillimeters = PixelWidthMillimeters,
            PixelHeightMillimeters = PixelHeightMillimeters,
            BorderOverlay = (BorderOverlay ?? new BorderOverlaySettings()).Clone()
        };
    }

    internal void RestoreMissingSections()
    {
        SourceMode ??= "DutyWhiteBlack";
        LeftSourcePath ??= string.Empty;
        RightSourcePath ??= string.Empty;
        SourceFolderPath ??= string.Empty;
        InputChannelOrder ??= "RGB";
        OutputPrefix ??= "fused";
        OutputDirectory ??= string.Empty;
        BorderOverlay ??= new BorderOverlaySettings();
    }
}

/// <summary>Lt_Source_Creator 离散生成模式梳理出的输入和导出参数。</summary>
public sealed class DiscreteNonIntegerPreferences
{
    public int Width { get; set; } = 1920;

    public int Height { get; set; } = 1080;

    public double AngleDegrees { get; set; } = 18.43d;

    public int Period { get; set; } = 8;

    public int LitCount { get; set; } = 4;

    public int Group { get; set; }

    public double PixelXMicrometers { get; set; } = 57.6d;

    public double PixelYMicrometers { get; set; } = 57.6d;

    public double ScreenSizeInches { get; set; } = 5d;

    public double PartitionWidth { get; set; } = 5760d;

    public double StrategyAngleDegrees { get; set; }

    public double Translation { get; set; }

    public string Direction { get; set; } = "LeftPositive";

    public string ResultType { get; set; } = "WhiteBlack";

    public bool SingleSource { get; set; } = true;

    public string CustomSourceAPath { get; set; } = string.Empty;

    public string CustomSourceBPath { get; set; } = string.Empty;

    public string OutputPrefix { get; set; } = "Lt_Source";

    public string OutputDirectory { get; set; } = string.Empty;

    public ImageFormatKind OutputFormat { get; set; } = ImageFormatKind.Bmp;

    public int Quality { get; set; } = 95;

    public BorderOverlaySettings BorderOverlay { get; set; } = new();

    public DiscreteNonIntegerPreferences Clone()
    {
        return new DiscreteNonIntegerPreferences
        {
            Width = Width,
            Height = Height,
            AngleDegrees = AngleDegrees,
            Period = Period,
            LitCount = LitCount,
            Group = Group,
            PixelXMicrometers = PixelXMicrometers,
            PixelYMicrometers = PixelYMicrometers,
            ScreenSizeInches = ScreenSizeInches,
            PartitionWidth = PartitionWidth,
            StrategyAngleDegrees = StrategyAngleDegrees,
            Translation = Translation,
            Direction = Direction ?? "LeftPositive",
            ResultType = ResultType ?? "WhiteBlack",
            SingleSource = SingleSource,
            CustomSourceAPath = CustomSourceAPath ?? string.Empty,
            CustomSourceBPath = CustomSourceBPath ?? string.Empty,
            OutputPrefix = OutputPrefix ?? "Lt_Source",
            OutputDirectory = OutputDirectory ?? string.Empty,
            OutputFormat = OutputFormat,
            Quality = Quality,
            BorderOverlay = (BorderOverlay ?? new BorderOverlaySettings()).Clone()
        };
    }

    internal void RestoreMissingSections()
    {
        Direction ??= "LeftPositive";
        ResultType ??= "WhiteBlack";
        CustomSourceAPath ??= string.Empty;
        CustomSourceBPath ??= string.Empty;
        OutputPrefix ??= "Lt_Source";
        OutputDirectory ??= string.Empty;
        BorderOverlay ??= new BorderOverlaySettings();
    }
}

/// <summary>“图片转 LightTools”页的最后输入值。</summary>
public sealed class ImageConverterPreferences
{
    public string SourceImagePath { get; set; } = string.Empty;

    public double PixelXMicrometers { get; set; } = 57.6d;

    public double PixelYMicrometers { get; set; } = 57.6d;

    public bool SingleSource { get; set; } = true;

    public string OutputDirectory { get; set; } = string.Empty;

    public ImageConverterPreferences Clone()
    {
        return new ImageConverterPreferences
        {
            SourceImagePath = SourceImagePath ?? string.Empty,
            PixelXMicrometers = PixelXMicrometers,
            PixelYMicrometers = PixelYMicrometers,
            SingleSource = SingleSource,
            OutputDirectory = OutputDirectory ?? string.Empty
        };
    }

    internal void RestoreMissingSections()
    {
        SourceImagePath ??= string.Empty;
        OutputDirectory ??= string.Empty;
    }
}
