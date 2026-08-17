using System.Drawing.Imaging;

namespace EolTestPatternGenerator.Projection;

/// <summary>
/// 每个实时控制器独占的 BMP 帧目录。成功切换后只保留 Windows 当前引用的帧；
/// 恢复原壁纸后即可清空整个目录。
/// </summary>
internal sealed class OwnedWallpaperFrameCache
{
    private readonly object _sync = new();
    private readonly HashSet<string> _ownedPaths = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _directory;

    public OwnedWallpaperFrameCache(string? cacheRoot = null)
    {
        string root = string.IsNullOrWhiteSpace(cacheRoot)
            ? Path.Combine(Path.GetTempPath(), "EolTestPatternGenerator", "RealtimeProjection")
            : Path.GetFullPath(cacheRoot);
        _directory = Path.Combine(root, $"session-{Guid.NewGuid():N}");
    }

    /// <summary>把调用方位图复制成 24 位 BMP，并通过同目录原子移动提交。</summary>
    public string Write(Bitmap source, long sequence)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.Width <= 0 || source.Height <= 0)
        {
            throw new ArgumentException("投图位图的宽高必须大于 0。", nameof(source));
        }

        Directory.CreateDirectory(_directory);
        string targetPath = Path.Combine(_directory, $"frame-{sequence:D20}.bmp");
        string temporaryPath = Path.Combine(
            _directory,
            $".frame-{sequence:D20}.{Guid.NewGuid():N}.tmp");

        try
        {
            using var bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Black);
                // 默认 SourceOver 会把带透明通道的输入正确合成到黑底。
                graphics.DrawImageUnscaled(source, 0, 0);
            }

            bitmap.Save(temporaryPath, ImageFormat.Bmp);
            File.Move(temporaryPath, targetPath, overwrite: true);
            lock (_sync)
            {
                _ownedPaths.Add(targetPath);
            }

            return targetPath;
        }
        finally
        {
            TryDeleteFile(temporaryPath);
        }
    }

    /// <summary>删除未被 Windows 当前壁纸引用的历史帧。</summary>
    public void KeepOnly(string? retainedPath)
    {
        string? retainedFullPath = string.IsNullOrWhiteSpace(retainedPath)
            ? null
            : Path.GetFullPath(retainedPath);
        string[] removable;
        lock (_sync)
        {
            removable = _ownedPaths
                .Where(path => !string.Equals(path, retainedFullPath, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        foreach (string path in removable)
        {
            if (TryDeleteFile(path))
            {
                lock (_sync)
                {
                    _ownedPaths.Remove(path);
                }
            }
        }

        TryDeleteDirectoryIfEmpty();
    }

    /// <summary>在原壁纸成功恢复后删除本控制器生成的全部文件。</summary>
    public void DeleteAll()
    {
        string[] paths;
        lock (_sync)
        {
            paths = _ownedPaths.ToArray();
        }

        foreach (string path in paths)
        {
            DeleteTrackedFile(path);
        }

        if (!Directory.Exists(_directory))
        {
            return;
        }

        try
        {
            // 目录由本实例用随机 GUID 创建；枚举受控文件名可兜底清理一次异常中断后
            // 尚未来得及登记，或先前版本已从内存追踪中丢失的帧。
            foreach (string framePath in Directory.EnumerateFiles(_directory, "frame-*.bmp"))
            {
                DeleteTrackedFile(framePath, requireTracked: false);
            }

            foreach (string temporaryPath in Directory.EnumerateFiles(_directory, ".frame-*.tmp"))
            {
                TryDeleteFile(temporaryPath);
            }

            Directory.Delete(_directory, recursive: false);
        }
        catch (IOException)
        {
            // Windows 或杀毒软件短暂占用时保留缓存，不影响壁纸恢复结果。
        }
        catch (UnauthorizedAccessException)
        {
            // 清理失败不应令已完成的停止流程失败。
        }
    }

    public void Delete(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        string fullPath = Path.GetFullPath(path);
        DeleteTrackedFile(fullPath);
        TryDeleteDirectoryIfEmpty();
    }

    private void DeleteTrackedFile(string fullPath, bool requireTracked = true)
    {
        bool isTracked;
        lock (_sync)
        {
            isTracked = _ownedPaths.Contains(fullPath);
        }

        if (requireTracked && !isTracked)
        {
            return;
        }

        if (!IsInOwnedDirectory(fullPath) || !TryDeleteFile(fullPath))
        {
            return;
        }

        lock (_sync)
        {
            _ownedPaths.Remove(fullPath);
        }
    }

    private bool IsInOwnedDirectory(string path)
    {
        string fullPath = Path.GetFullPath(path);
        string directoryPrefix = Path.TrimEndingDirectorySeparator(Path.GetFullPath(_directory))
            + Path.DirectorySeparatorChar;
        return fullPath.StartsWith(directoryPrefix, StringComparison.OrdinalIgnoreCase);
    }

    private void TryDeleteDirectoryIfEmpty()
    {
        try
        {
            if (Directory.Exists(_directory) && !Directory.EnumerateFileSystemEntries(_directory).Any())
            {
                Directory.Delete(_directory, recursive: false);
            }
        }
        catch (IOException)
        {
            // 并发文件扫描或短暂占用时下次清理会再次尝试。
        }
        catch (UnauthorizedAccessException)
        {
            // 缓存清理不影响投图主流程。
        }
    }

    /// <summary>文件已不存在或已成功删除时返回 true；短暂失败时保留追踪以便下次重试。</summary>
    private static bool TryDeleteFile(string path)
    {
        try
        {
            // File.Delete 对不存在的文件本身就是成功 no-op；直接删除可避免 File.Exists
            // 在权限/IO 错误时静默返回 false，继而误把仍存在的文件从追踪集合移除。
            File.Delete(path);
            return true;
        }
        catch (IOException)
        {
            // 系统读取壁纸文件期间可能短暂占用，下次切图/停止时会再次清理。
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            // 清理失败不遮蔽投图或恢复结果。
            return false;
        }
    }
}
