using System.Text.Json;
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
            ApplicationPreferences candidate = PrepareForSave(preferences);
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

            ApplicationPreferences? loaded = JsonSerializer.Deserialize<ApplicationPreferences>(stream, SerializerOptions);
            if (loaded is null)
            {
                throw new JsonException("用户配置文件没有有效的根对象。");
            }

            if (loaded.Version > ApplicationPreferences.CurrentVersion)
            {
                throw new NotSupportedException(
                    $"用户配置版本 {loaded.Version} 高于当前支持版本 {ApplicationPreferences.CurrentVersion}。");
            }

            // 当前只有版本1。以后增加迁移时，应在这里按旧版本逐级升级。
            loaded.Version = ApplicationPreferences.CurrentVersion;
            loaded.RestoreMissingSections();
            _current = loaded;
        }
        catch (Exception exception) when (IsRecoverableSettingsException(exception))
        {
            // 损坏文件保持原样便于排查；程序继续使用内存默认值，直到下次主动保存。
            _lastLoadError = exception;
            _current = CreateDefaultPreferences();
        }
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
            or JsonException
            or NotSupportedException
            or ArgumentException;
    }
}
