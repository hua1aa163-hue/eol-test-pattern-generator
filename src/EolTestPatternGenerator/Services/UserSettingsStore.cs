using System.Globalization;
using System.Security;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 线程安全的用户配置仓库。共享实例固定写入 LocalApplicationData，配置文件不会进入项目目录。
/// </summary>
public sealed class UserSettingsStore
{
    private const string ApplicationDirectoryName = "EolTestPatternGenerator";
    private const string SettingsFileName = "settings.json";

    private static readonly Lazy<UserSettingsStore> SharedStore =
        new(() => new UserSettingsStore(GetDefaultSettingsFilePath()), LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly JsonSerializerOptions SerializerOptions = CreateSerializerOptions();

    private readonly object _gate = new();
    private ApplicationPreferences? _current;
    private Exception? _lastLoadError;
    private string? _lastRecoveryBackupPath;
    private bool _requiresRecoveryBackup;
    private bool _loaded;

    /// <summary>进程内共享的配置仓库，供主窗体和所有子窗体共同使用。</summary>
    public static UserSettingsStore Shared => SharedStore.Value;

    /// <summary>实际用户配置文件的绝对路径。</summary>
    public string SettingsFilePath { get; }

    /// <summary>
    /// 最近一次加载失败的原因；文件不存在属于正常首次运行，不会记录为错误。
    /// </summary>
    public Exception? LastLoadError
    {
        get
        {
            lock (_gate)
            {
                return _lastLoadError;
            }
        }
    }

    /// <summary>
    /// 最近一次在覆盖无法读取或更高版本配置前保留的原文备份路径。
    /// </summary>
    public string? LastRecoveryBackupPath
    {
        get
        {
            lock (_gate)
            {
                return _lastRecoveryBackupPath;
            }
        }
    }

    /// <summary>
    /// 使用指定文件的实例供同程序集自检使用；产品界面应始终使用 Shared。
    /// </summary>
    internal UserSettingsStore(string settingsFilePath)
    {
        SettingsFilePath = Path.GetFullPath(settingsFilePath);
    }

    /// <summary>
    /// 返回当前配置的深副本。首次调用时从磁盘读取；文件缺失或损坏时返回默认配置。
    /// </summary>
    public ApplicationPreferences Load()
    {
        lock (_gate)
        {
            EnsureLoadedCore();
            return _current!.Clone();
        }
    }

    /// <summary>忽略内存缓存并重新读取磁盘，主要供诊断和测试使用。</summary>
    public ApplicationPreferences Reload()
    {
        lock (_gate)
        {
            _loaded = false;
            _current = null;
            _lastLoadError = null;
            _requiresRecoveryBackup = false;
            EnsureLoadedCore();
            return _current!.Clone();
        }
    }

    /// <summary>
    /// 用一份完整配置替换当前配置并立即原子保存。
    /// 保存成功前不会替换进程内的最后有效快照。
    /// </summary>
    public void Save(ApplicationPreferences preferences)
    {
        ArgumentNullException.ThrowIfNull(preferences);

        lock (_gate)
        {
            EnsureLoadedCore();
            ApplicationPreferences candidate = PrepareForSave(preferences);
            EnsureRecoveryBackupCore();
            WriteAtomicallyCore(candidate);
            _current = candidate;
            _loaded = true;
        }
    }

    /// <summary>
    /// 在共享配置副本上执行更新并立即保存。整个读取、修改和写入过程由同一把锁保护，
    /// 可避免主窗体与子窗体在同一进程中互相覆盖配置节。
    /// </summary>
    public void UpdateAndSave(Action<ApplicationPreferences> update)
    {
        ArgumentNullException.ThrowIfNull(update);

        lock (_gate)
        {
            EnsureLoadedCore();
            ApplicationPreferences candidate = _current!.Clone();
            update(candidate);
            candidate = PrepareForSave(candidate);
            EnsureRecoveryBackupCore();
            WriteAtomicallyCore(candidate);
            _current = candidate;
        }
    }

    /// <summary>
    /// 不向界面线程抛出保存异常的便捷入口。返回 false 时 error 包含失败原因。
    /// </summary>
    public bool TryUpdateAndSave(Action<ApplicationPreferences> update, out Exception? error)
    {
        try
        {
            UpdateAndSave(update);
            error = null;
            return true;
        }
        catch (Exception exception) when (IsRecoverableSettingsException(exception))
        {
            error = exception;
            return false;
        }
    }

    /// <summary>计算规范要求的用户配置路径。</summary>
    public static string GetDefaultSettingsFilePath()
    {
        string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(localApplicationData))
        {
            throw new InvalidOperationException("无法取得 LocalApplicationData 用户目录。");
        }

        return Path.Combine(localApplicationData, ApplicationDirectoryName, SettingsFileName);
    }

    private void EnsureLoadedCore()
    {
        if (_loaded)
        {
            return;
        }

        _loaded = true;
        _lastLoadError = null;

        if (!File.Exists(SettingsFilePath))
        {
            _requiresRecoveryBackup = false;
            _current = CreateDefaultPreferences();
            return;
        }

        try
        {
            using var stream = new FileStream(
                SettingsFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            JsonNode? rootNode = JsonNode.Parse(
                stream,
                nodeOptions: null,
                new JsonDocumentOptions
                {
                    AllowTrailingCommas = true,
                    CommentHandling = JsonCommentHandling.Skip
                });
            if (rootNode is not JsonObject rootObject)
            {
                throw new JsonException("用户配置文件没有有效的根对象。");
            }

            int sourceVersion = GetInt32(rootObject, "Version", 1);
            if (sourceVersion > ApplicationPreferences.CurrentVersion)
            {
                throw new NotSupportedException(
                    $"用户配置版本 {sourceVersion} 高于当前支持版本 {ApplicationPreferences.CurrentVersion}。");
            }

            if (sourceVersion < 2)
            {
                MigrateVersion1To2(rootObject);
            }

            SetProperty(rootObject, "Version", ApplicationPreferences.CurrentVersion);
            ApplicationPreferences? loaded = rootObject.Deserialize<ApplicationPreferences>(SerializerOptions);
            if (loaded is null)
            {
                throw new JsonException("用户配置文件没有有效的根对象。");
            }

            loaded.Version = ApplicationPreferences.CurrentVersion;
            loaded.RestoreMissingSections();
            _requiresRecoveryBackup = false;
            _current = loaded;
        }
        catch (Exception exception) when (IsRecoverableSettingsException(exception))
        {
            // 损坏文件保持原样便于排查；程序继续使用内存默认值，直到下次主动保存。
            _lastLoadError = exception;
            _requiresRecoveryBackup = true;
            _current = CreateDefaultPreferences();
        }
    }

    /// <summary>
    /// v1 以 X/Y/Width/Height 保存区域；v2 改为最外缘四边距。
    /// 迁移只重写几何字段，其余用户输入和未知扩展字段均原样保留。
    /// </summary>
    private static void MigrateVersion1To2(JsonObject root)
    {
        JsonObject? main = GetObject(root, "Main");
        JsonObject? profiles = main is null ? null : GetObject(main, "PatternProfiles");
        if (profiles is not null)
        {
            foreach ((string _, JsonNode? node) in profiles.ToList())
            {
                if (node is JsonObject profile)
                {
                    MigratePatternSettings(profile);
                }
            }
        }

        JsonObject? phaseSettings = GetObject(GetObject(root, "PhaseStripe"), "Settings");
        if (phaseSettings is not null)
        {
            MigratePatternSettings(phaseSettings, PatternType.PhaseStripes);
        }

        JsonObject? screenSettings = GetObject(GetObject(root, "ScreenOne"), "Settings");
        if (screenSettings is not null)
        {
            MigrateScreenOneSettings(screenSettings);
        }

        JsonObject? blend = GetObject(root, "NonIntegerBlend");
        JsonObject? matlab = GetObject(blend, "Matlab");
        if (matlab is not null)
        {
            MigrateBorderOverlay(
                GetObject(matlab, "BorderOverlay"),
                GetInt32(matlab, "CanvasWidth", 1920),
                GetInt32(matlab, "CanvasHeight", 1080));
        }

        JsonObject? discrete = GetObject(blend, "Discrete");
        if (discrete is not null)
        {
            MigrateBorderOverlay(
                GetObject(discrete, "BorderOverlay"),
                GetInt32(discrete, "Width", 1920),
                GetInt32(discrete, "Height", 1080));
        }

        SetProperty(root, "Version", 2);
    }

    private static void MigratePatternSettings(JsonObject settings, PatternType? forcedType = null)
    {
        int canvasWidth = GetInt32(settings, "CanvasWidth", 1920);
        int canvasHeight = GetInt32(settings, "CanvasHeight", 1080);
        PatternType patternType = forcedType ?? GetPatternType(settings, PatternType.Border);

        // 已含 v2 字段时不覆盖；这样也兼容用户在版本号更新前手动编辑的新字段。
        if (!HasProperty(settings, "LeftMargin"))
        {
            PatternSettings defaults = PatternPresets.Create(patternType);

            int x = GetInt32(settings, "PatternX", defaults.PatternX);
            int y = GetInt32(settings, "PatternY", defaults.PatternY);
            int width = GetInt32(settings, "PatternWidth", defaults.PatternWidth);
            int height = GetInt32(settings, "PatternHeight", defaults.PatternHeight);
            int dotRadius = GetInt32(settings, "DotRadius", defaults.DotRadius);
            int rows = GetInt32(settings, "Rows", defaults.Rows);
            int columns = GetInt32(settings, "Columns", defaults.Columns);

            RegionMargins margins;
            if (patternType is PatternType.Black or PatternType.FullWhite or PatternType.FullRed or
                PatternType.FullGreen or PatternType.FullBlue or PatternType.ImportedImage)
            {
                margins = new RegionMargins(0, 0, 0, 0);
            }
            else if (patternType is PatternType.NinePointGrid or PatternType.DistortionGrid)
            {
                // 旧生成器在单列/单行时完全忽略对应的首末圆心跨度。
                // 将该轴归零不改变任何输出像素，但能让 v2 外缘边距精确对应真实圆点 bbox。
                int effectiveWidth = columns == 1 ? 0 : width;
                int effectiveHeight = rows == 1 ? 0 : height;
                long outerX = (long)x - dotRadius;
                long outerY = (long)y - dotRadius;
                long outerWidth = (long)effectiveWidth + (2L * dotRadius) + 1L;
                long outerHeight = (long)effectiveHeight + (2L * dotRadius) + 1L;
                margins = ConvertLegacyLongBounds(
                    canvasWidth,
                    canvasHeight,
                    outerX,
                    outerY,
                    outerWidth,
                    outerHeight,
                    "旧点阵配置");
            }
            else
            {
                margins = MarginGeometry.FromLegacyBounds(
                    canvasWidth,
                    canvasHeight,
                    x,
                    y,
                    width,
                    height,
                    "旧图案配置");
            }

            WriteMargins(settings, margins);
        }

        RemoveProperty(settings, "PatternX");
        RemoveProperty(settings, "PatternY");
        RemoveProperty(settings, "PatternWidth");
        RemoveProperty(settings, "PatternHeight");
        MigrateBorderOverlay(GetObject(settings, "BorderOverlay"), canvasWidth, canvasHeight);
    }

    private static void MigrateBorderOverlay(JsonObject? border, int canvasWidth, int canvasHeight)
    {
        if (border is null)
        {
            return;
        }

        if (!HasProperty(border, "LeftMargin"))
        {
            int x = GetInt32(border, "X", 71);
            int y = GetInt32(border, "Y", 226);
            int width = GetInt32(border, "Width", 1777);
            int height = GetInt32(border, "Height", 627);
            WriteMargins(
                border,
                MarginGeometry.FromLegacyBounds(
                    canvasWidth,
                    canvasHeight,
                    x,
                    y,
                    width,
                    height,
                    "旧白框配置"));
        }

        RemoveProperty(border, "X");
        RemoveProperty(border, "Y");
        RemoveProperty(border, "Width");
        RemoveProperty(border, "Height");
    }

    private static void MigrateScreenOneSettings(JsonObject settings)
    {
        int canvasWidth = GetInt32(settings, "CanvasWidth", 3200);
        int canvasHeight = GetInt32(settings, "CanvasHeight", 2000);

        if (!HasProperty(settings, "LeftRegionLeftMargin"))
        {
            RegionMargins left = MarginGeometry.FromLegacyBounds(
                canvasWidth,
                canvasHeight,
                GetInt32(settings, "LeftX", 50),
                GetInt32(settings, "LeftY", 50),
                GetInt32(settings, "LeftWidth", 1500),
                GetInt32(settings, "LeftHeight", 1900),
                "旧显示器左区域");
            WritePrefixedMargins(settings, "LeftRegion", left);
        }

        if (!HasProperty(settings, "RightRegionLeftMargin"))
        {
            RegionMargins right = MarginGeometry.FromLegacyBounds(
                canvasWidth,
                canvasHeight,
                GetInt32(settings, "RightX", 1650),
                GetInt32(settings, "RightY", 50),
                GetInt32(settings, "RightWidth", 1500),
                GetInt32(settings, "RightHeight", 1900),
                "旧显示器右区域");
            WritePrefixedMargins(settings, "RightRegion", right);
        }

        foreach (string propertyName in new[]
                 {
                     "LeftX", "LeftY", "LeftWidth", "LeftHeight",
                     "RightX", "RightY", "RightWidth", "RightHeight"
                 })
        {
            RemoveProperty(settings, propertyName);
        }
    }

    private static RegionMargins ConvertLegacyLongBounds(
        int canvasWidth,
        int canvasHeight,
        long x,
        long y,
        long width,
        long height,
        string description)
    {
        if (x is < int.MinValue or > int.MaxValue ||
            y is < int.MinValue or > int.MaxValue ||
            width is < 1 or > int.MaxValue ||
            height is < 1 or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(width), $"{description}超出支持的整数范围。");
        }

        return MarginGeometry.FromLegacyBounds(
            canvasWidth,
            canvasHeight,
            (int)x,
            (int)y,
            (int)width,
            (int)height,
            description);
    }

    private static void WriteMargins(JsonObject target, RegionMargins margins)
    {
        SetProperty(target, "LeftMargin", margins.Left);
        SetProperty(target, "TopMargin", margins.Top);
        SetProperty(target, "RightMargin", margins.Right);
        SetProperty(target, "BottomMargin", margins.Bottom);
    }

    private static void WritePrefixedMargins(JsonObject target, string prefix, RegionMargins margins)
    {
        SetProperty(target, prefix + "LeftMargin", margins.Left);
        SetProperty(target, prefix + "TopMargin", margins.Top);
        SetProperty(target, prefix + "RightMargin", margins.Right);
        SetProperty(target, prefix + "BottomMargin", margins.Bottom);
    }

    private static PatternType GetPatternType(JsonObject target, PatternType fallback)
    {
        if (!TryGetProperty(target, "PatternType", out _, out JsonNode? node) || node is not JsonValue value)
        {
            return fallback;
        }

        if (value.TryGetValue<string>(out string? name) &&
            Enum.TryParse(name, ignoreCase: true, out PatternType namedType))
        {
            return namedType;
        }

        if (value.TryGetValue<int>(out int numericType) && Enum.IsDefined(typeof(PatternType), numericType))
        {
            return (PatternType)numericType;
        }

        return fallback;
    }

    private static int GetInt32(JsonObject target, string propertyName, int fallback)
    {
        if (TryGetProperty(target, propertyName, out _, out JsonNode? node) &&
            node is JsonValue value &&
            value.TryGetValue<int>(out int result))
        {
            return result;
        }

        return fallback;
    }

    private static JsonObject? GetObject(JsonObject? target, string propertyName)
    {
        if (target is not null &&
            TryGetProperty(target, propertyName, out _, out JsonNode? node) &&
            node is JsonObject result)
        {
            return result;
        }

        return null;
    }

    private static bool HasProperty(JsonObject target, string propertyName)
    {
        return TryGetProperty(target, propertyName, out _, out _);
    }

    private static void SetProperty(JsonObject target, string propertyName, int value)
    {
        if (TryGetProperty(target, propertyName, out string? actualName, out _))
        {
            target[actualName!] = value;
        }
        else
        {
            target[propertyName] = value;
        }
    }

    private static void RemoveProperty(JsonObject target, string propertyName)
    {
        if (TryGetProperty(target, propertyName, out string? actualName, out _))
        {
            target.Remove(actualName!);
        }
    }

    private static bool TryGetProperty(
        JsonObject target,
        string propertyName,
        out string? actualName,
        out JsonNode? value)
    {
        foreach ((string name, JsonNode? node) in target)
        {
            if (string.Equals(name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                actualName = name;
                value = node;
                return true;
            }
        }

        actualName = null;
        value = null;
        return false;
    }

    private static ApplicationPreferences PrepareForSave(ApplicationPreferences preferences)
    {
        ApplicationPreferences candidate = preferences.Clone();
        candidate.Version = ApplicationPreferences.CurrentVersion;
        candidate.RestoreMissingSections();
        return candidate;
    }

    private static ApplicationPreferences CreateDefaultPreferences()
    {
        var preferences = new ApplicationPreferences();
        preferences.RestoreMissingSections();
        return preferences;
    }

    /// <summary>
    /// 加载失败后首次写入前，先在同目录以“临时文件 + 原子改名”保留原文。
    /// 备份失败会中止保存，原配置绝不会被静默覆盖。
    /// </summary>
    private void EnsureRecoveryBackupCore()
    {
        if (!_requiresRecoveryBackup)
        {
            return;
        }

        if (!File.Exists(SettingsFilePath))
        {
            // 用户在加载后主动删除了无效文件，已没有可恢复的原文。
            _requiresRecoveryBackup = false;
            return;
        }

        string? directory = Path.GetDirectoryName(SettingsFilePath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException("用户配置文件路径没有有效目录。");
        }

        string timestamp = DateTime.UtcNow.ToString(
            "yyyyMMdd'T'HHmmss.fffffff'Z'",
            CultureInfo.InvariantCulture);
        string backupPath = Path.Combine(
            directory,
            $"{Path.GetFileName(SettingsFilePath)}.recovery-{timestamp}-{Guid.NewGuid():N}.bak");
        string temporaryBackupPath = backupPath + ".tmp";

        try
        {
            using (var source = new FileStream(
                       SettingsFilePath,
                       FileMode.Open,
                       FileAccess.Read,
                       FileShare.Read))
            using (var destination = new FileStream(
                       temporaryBackupPath,
                       FileMode.CreateNew,
                       FileAccess.Write,
                       FileShare.None,
                       bufferSize: 16 * 1024,
                       FileOptions.WriteThrough))
            {
                source.CopyTo(destination);
                destination.Flush(flushToDisk: true);
            }

            // 临时文件和备份在同一目录，改名时不会暴露半写入的备份。
            File.Move(temporaryBackupPath, backupPath);
            _lastRecoveryBackupPath = backupPath;
            _requiresRecoveryBackup = false;
        }
        finally
        {
            try
            {
                if (File.Exists(temporaryBackupPath))
                {
                    File.Delete(temporaryBackupPath);
                }
            }
            catch
            {
                // 清理失败不覆盖原始异常，也不会触碰源配置。
            }
        }
    }

    private void WriteAtomicallyCore(ApplicationPreferences preferences)
    {
        string? directory = Path.GetDirectoryName(SettingsFilePath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException("用户配置文件路径没有有效目录。");
        }

        Directory.CreateDirectory(directory);

        // 临时文件必须和目标文件位于同一目录，最终替换才能保持同卷原子性。
        string temporaryPath = Path.Combine(
            directory,
            $".{SettingsFileName}.{Environment.ProcessId}.{Guid.NewGuid():N}.tmp");

        try
        {
            using (var stream = new FileStream(
                       temporaryPath,
                       FileMode.CreateNew,
                       FileAccess.Write,
                       FileShare.None,
                       bufferSize: 16 * 1024,
                       FileOptions.WriteThrough))
            {
                JsonSerializer.Serialize(stream, preferences, SerializerOptions);
                stream.Flush(flushToDisk: true);
            }

            if (File.Exists(SettingsFilePath))
            {
                try
                {
                    File.Replace(temporaryPath, SettingsFilePath, destinationBackupFileName: null, ignoreMetadataErrors: true);
                }
                catch (Exception exception) when (exception is IOException or PlatformNotSupportedException)
                {
                    // 部分文件系统不支持 Replace；同目录 Move(overwrite) 仍避免暴露半写入文件。
                    File.Move(temporaryPath, SettingsFilePath, overwrite: true);
                }
            }
            else
            {
                File.Move(temporaryPath, SettingsFilePath);
            }
        }
        finally
        {
            // 替换成功后临时文件已不存在；失败时尽力清理，不覆盖原始异常。
            try
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
            catch
            {
                // 清理失败不会影响已经存在的旧配置文件。
            }
        }
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        // 枚举写成人类可读名称，后续调整枚举数值也不会破坏已有配置。
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    private static bool IsRecoverableSettingsException(Exception exception)
    {
        return exception is IOException
            or UnauthorizedAccessException
            or SecurityException
            or JsonException
            or NotSupportedException
            or ArgumentException;
    }
}
