using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace EolTestPatternGenerator.Projection;

/// <summary>
/// Windows 桌面壁纸和 Win+P 拓扑的轻量封装。
/// PNG/JPEG 等非 BMP 输入会先转换成 24 位 BMP，再交给 Windows。
/// </summary>
public sealed class DesktopDisplayService : IDesktopDisplayService
{
    private const uint SpiGetDesktopWallpaper = 0x0073;
    private const uint SpiSetDesktopWallpaper = 0x0014;
    private const uint SpifUpdateIniFile = 0x0001;
    private const uint SpifSendWinIniChange = 0x0002;

    private const uint SdcApply = 0x00000080;
    private const uint SdcTopologyInternal = 0x00000001;
    private const uint SdcTopologyClone = 0x00000002;
    private const uint SdcTopologyExtend = 0x00000004;
    private const uint SdcTopologyExternal = 0x00000008;

    private static readonly object CacheSync = new();
    private static readonly object SystemStateSync = new();

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(
        uint uiAction,
        uint uiParam,
        string pvParam,
        uint fWinIni);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(
        uint uiAction,
        uint uiParam,
        StringBuilder pvParam,
        uint fWinIni);

    [DllImport("user32.dll")]
    private static extern int SetDisplayConfig(
        uint numPathArrayElements,
        IntPtr pathArray,
        uint numModeArrayElements,
        IntPtr modeArray,
        uint flags);

    /// <inheritdoc />
    public string? GetCurrentWallpaper()
    {
        EnsureWindows();

        // 使用足够容纳 Windows 长路径的缓冲区；失败时保持参考实现的 null 语义。
        var path = new StringBuilder(32768);
        lock (SystemStateSync)
        {
            return SystemParametersInfo(SpiGetDesktopWallpaper, (uint)path.Capacity, path, 0)
                ? path.ToString()
                : null;
        }
    }

    /// <inheritdoc />
    public void ApplyTopology(DisplayTopology topology)
    {
        if (topology == DisplayTopology.None)
        {
            return;
        }

        EnsureWindows();

        uint topologyFlag = topology switch
        {
            DisplayTopology.Internal => SdcTopologyInternal,
            DisplayTopology.Clone => SdcTopologyClone,
            DisplayTopology.External => SdcTopologyExternal,
            DisplayTopology.Extend => SdcTopologyExtend,
            _ => throw new ArgumentOutOfRangeException(nameof(topology), topology, null)
        };

        // 预定义拓扑不需要调用方构造 DISPLAYCONFIG_PATH_INFO/MODE_INFO 数组。
        int result;
        lock (SystemStateSync)
        {
            // 与壁纸设置共用系统状态锁，避免实时帧和 Win+P 拓扑同时进入 user32。
            result = SetDisplayConfig(
                0,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                SdcApply | topologyFlag);
        }
        if (result != 0)
        {
            throw new Win32Exception(result, "切换 Windows 投影模式失败。");
        }
    }

    /// <inheritdoc />
    public void SetWallpaper(string imagePath)
    {
        EnsureWindows();
        ArgumentNullException.ThrowIfNull(imagePath);

        string wallpaperPath;
        if (imagePath.Length == 0)
        {
            // SPI_GETDESKWALLPAPER 在当前为纯色桌面时会成功返回空字符串；把同一空值
            // 传回 SPI_SETDESKWALLPAPER 才能恢复“没有壁纸”的原始状态。
            wallpaperPath = string.Empty;
        }
        else
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(imagePath);
            string fullPath = Path.GetFullPath(imagePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("投图文件不存在。", fullPath);
            }

            wallpaperPath = ConvertToBmpIfNeeded(fullPath);
        }

        bool success;
        lock (SystemStateSync)
        {
            success = SystemParametersInfo(
                SpiSetDesktopWallpaper,
                0,
                wallpaperPath,
                SpifUpdateIniFile | SpifSendWinIniChange);
        }
        if (!success)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "切换桌面图片失败。");
        }
    }

    /// <summary>
    /// BMP 直接使用原路径；其他格式按“完整路径、长度、最后修改时间”生成缓存键，
    /// 转换为 24 位 BMP。正式缓存使用内容稳定的文件名，可被后续相同请求复用。
    /// </summary>
    internal static string ConvertToBmpIfNeeded(string imagePath)
    {
        string fullPath = Path.GetFullPath(imagePath);
        if (string.Equals(Path.GetExtension(fullPath), ".bmp", StringComparison.OrdinalIgnoreCase))
        {
            return fullPath;
        }

        var sourceInfo = new FileInfo(fullPath);
        if (!sourceInfo.Exists)
        {
            throw new FileNotFoundException("投图文件不存在。", fullPath);
        }

        string cacheKey = $"{sourceInfo.FullName}|{sourceInfo.Length}|{sourceInfo.LastWriteTimeUtc.Ticks}";
        string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cacheKey)))[..20];
        string cacheDirectory = Path.Combine(
            Path.GetTempPath(),
            "EolTestPatternGenerator",
            "WallpaperCache");
        string bmpPath = Path.Combine(cacheDirectory, $"wallpaper-{hash}.bmp");

        lock (CacheSync)
        {
            Directory.CreateDirectory(cacheDirectory);
            if (File.Exists(bmpPath))
            {
                return bmpPath;
            }

            string temporaryPath = Path.Combine(
                cacheDirectory,
                $".{Path.GetFileName(bmpPath)}.{Guid.NewGuid():N}.tmp");
            try
            {
                using Image source = Image.FromFile(fullPath);
                using var bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.Black);
                    // 使用默认 SourceOver，使透明 PNG 与黑底正确合成。
                    graphics.DrawImage(source, 0, 0, source.Width, source.Height);
                }

                // 显式指定 BMP 编码，因此临时文件无需使用 .bmp 扩展名。
                bitmap.Save(temporaryPath, ImageFormat.Bmp);
                File.Move(temporaryPath, bmpPath, overwrite: true);
                return bmpPath;
            }
            finally
            {
                TryDeleteFile(temporaryPath);
            }
        }
    }

    private static void EnsureWindows()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("桌面壁纸投图仅支持 Windows。");
        }
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
            // 转换异常不能被临时文件清理异常遮蔽。
        }
        catch (UnauthorizedAccessException)
        {
            // 缓存目录不可写时，原始转换/移动异常更有诊断价值。
        }
    }
}
