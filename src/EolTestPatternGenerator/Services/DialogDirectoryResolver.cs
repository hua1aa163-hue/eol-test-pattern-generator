using System.Security;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 将持久化的目录安全转换为文件对话框可用的绝对初始目录。
/// 设置文件可能来自其他电脑，因此不存在、无权限或格式损坏的路径均回退为空。
/// </summary>
public static class DialogDirectoryResolver
{
    /// <summary>
    /// 优先返回仍存在的保存目录；若无效，可回退到一个文件路径所在的现存目录。
    /// 返回空字符串时让 Windows 对话框自行选择默认位置。
    /// </summary>
    public static string ResolveExistingDirectory(
        string? preferredDirectory,
        string? fallbackFilePath = null)
    {
        string preferred = NormalizeExistingDirectory(preferredDirectory);
        if (preferred.Length > 0)
        {
            return preferred;
        }

        return GetExistingParentDirectory(fallbackFilePath);
    }

    /// <summary>取得文件路径所在的现存绝对目录；输入无效时返回空字符串。</summary>
    public static string GetExistingParentDirectory(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return string.Empty;
        }

        try
        {
            string fullPath = Path.GetFullPath(filePath.Trim());
            return NormalizeExistingDirectory(Path.GetDirectoryName(fullPath));
        }
        catch (Exception exception) when (IsInvalidPathException(exception))
        {
            return string.Empty;
        }
    }

    /// <summary>规范化一个已选择目录；目录后来被删除时安全保留原回退值。</summary>
    public static string RememberDirectory(string? selectedDirectory, string? fallback = null)
    {
        string resolved = NormalizeExistingDirectory(selectedDirectory);
        return resolved.Length > 0
            ? resolved
            : NormalizeExistingDirectory(fallback);
    }

    /// <summary>规范化一个已选择文件的父目录；无法解析时安全保留原回退值。</summary>
    public static string RememberFileDirectory(string? selectedFilePath, string? fallback = null)
    {
        string resolved = GetExistingParentDirectory(selectedFilePath);
        return resolved.Length > 0
            ? resolved
            : NormalizeExistingDirectory(fallback);
    }

    private static string NormalizeExistingDirectory(string? directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
        {
            return string.Empty;
        }

        try
        {
            string fullPath = Path.GetFullPath(directory.Trim());
            return Directory.Exists(fullPath) ? fullPath : string.Empty;
        }
        catch (Exception exception) when (IsInvalidPathException(exception))
        {
            return string.Empty;
        }
    }

    private static bool IsInvalidPathException(Exception exception)
    {
        return exception is IOException
            or UnauthorizedAccessException
            or ArgumentException
            or NotSupportedException
            or SecurityException;
    }
}
