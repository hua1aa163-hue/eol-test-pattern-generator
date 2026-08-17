using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 以 Windows 文件系统实体身份比较目录，而不是只比较路径字符串。
/// 这样 junction、符号链接、映射盘和 UNC 别名不能绕过源目录保护。
/// </summary>
internal static class WindowsFileSystemIdentity
{
    private const uint FileFlagBackupSemantics = 0x02000000;
    private const uint FileFlagOpenReparsePoint = 0x00200000;
    private const uint FileListDirectory = 0x0001;
    private const uint OpenExisting = 3;

    public static bool AreSameDirectory(string firstPath, string secondPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(secondPath);

        using SafeFileHandle first = OpenDirectory(firstPath, shareDelete: true);
        using SafeFileHandle second = OpenDirectory(secondPath, shareDelete: true);
        FileIdentity firstIdentity = ReadIdentity(first, firstPath);
        FileIdentity secondIdentity = ReadIdentity(second, secondPath);
        return firstIdentity == secondIdentity;
    }

    /// <summary>
    /// 验证两个现有目录不是同一文件系统实体，并在返回对象生存期内同时锁定目录目标和
    /// 可能存在的 junction/符号链接入口。句柄不共享 Delete，因此批处理期间不能删除、
    /// 重命名或替换这两个目录入口。
    /// </summary>
    public static DirectoryIdentityLease AcquireIndependentDirectoryLease(
        string sourceDirectory,
        string outputDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

        string source = Path.GetFullPath(sourceDirectory);
        string output = Path.GetFullPath(outputDirectory);
        SafeFileHandle? sourceTarget = null;
        SafeFileHandle? outputTarget = null;
        SafeFileHandle? sourceEntry = null;
        SafeFileHandle? outputEntry = null;
        try
        {
            // 先不跟随重解析点锁定路径入口，再打开其解析后目标。
            // 此顺序避免 junction/符号链接在“已打开旧目标、尚未锁定入口”
            // 的短暂窗口中被替换。普通目录则会得到同一实体的两个句柄。
            sourceEntry = OpenDirectory(source, shareDelete: false, openReparsePoint: true);
            outputEntry = OpenDirectory(output, shareDelete: false, openReparsePoint: true);
            sourceTarget = OpenDirectory(source, shareDelete: false);
            outputTarget = OpenDirectory(output, shareDelete: false);
            FileIdentity sourceIdentity = ReadIdentity(sourceTarget, source);
            FileIdentity outputIdentity = ReadIdentity(outputTarget, output);
            if (sourceIdentity == outputIdentity)
            {
                throw new InvalidOperationException(
                    "源图片文件夹与输出文件夹不能相同，否则会覆盖原图。请选择另一个输出文件夹。");
            }

            var lease = new DirectoryIdentityLease(
                sourceTarget,
                outputTarget,
                sourceEntry,
                outputEntry);
            sourceTarget = null;
            outputTarget = null;
            sourceEntry = null;
            outputEntry = null;
            return lease;
        }
        finally
        {
            sourceTarget?.Dispose();
            outputTarget?.Dispose();
            sourceEntry?.Dispose();
            outputEntry?.Dispose();
        }
    }

    private static SafeFileHandle OpenDirectory(
        string path,
        bool shareDelete,
        bool openReparsePoint = false)
    {
        FileShare share = FileShare.ReadWrite;
        if (shareDelete)
        {
            share |= FileShare.Delete;
        }

        uint flags = FileFlagBackupSemantics;
        if (openReparsePoint)
        {
            flags |= FileFlagOpenReparsePoint;
        }

        SafeFileHandle handle = CreateFile(
            Path.GetFullPath(path),
            // 请求真实的目录枚举权限，使不共享 Delete 的句柄
            // 参与 Windows 的删除/重命名共享检查；只读物理标识时仍用
            // 零权限句柄，以避免对普通目录比较增加不必要的权限要求。
            desiredAccess: shareDelete ? 0 : FileListDirectory,
            share,
            securityAttributes: IntPtr.Zero,
            creationDisposition: OpenExisting,
            flagsAndAttributes: flags,
            templateFile: IntPtr.Zero);
        if (handle.IsInvalid)
        {
            int error = Marshal.GetLastWin32Error();
            handle.Dispose();
            throw new IOException(
                $"无法验证图片文件夹的物理位置：{Path.GetFullPath(path)}",
                new Win32Exception(error));
        }

        return handle;
    }

    private static FileIdentity ReadIdentity(SafeFileHandle handle, string path)
    {
        if (!GetFileInformationByHandle(handle, out ByHandleFileInformation information))
        {
            throw new IOException(
                $"无法读取图片文件夹的物理标识：{Path.GetFullPath(path)}",
                new Win32Exception(Marshal.GetLastWin32Error()));
        }

        return new FileIdentity(
            information.VolumeSerialNumber,
            ((ulong)information.FileIndexHigh << 32) | information.FileIndexLow);
    }

    private readonly record struct FileIdentity(uint VolumeSerialNumber, ulong FileIndex);

    /// <summary>持有两个已验证目录的目标与路径入口句柄，Dispose 后才允许替换目录。</summary>
    public sealed class DirectoryIdentityLease : IDisposable
    {
        private SafeFileHandle? _sourceTarget;
        private SafeFileHandle? _outputTarget;
        private SafeFileHandle? _sourceEntry;
        private SafeFileHandle? _outputEntry;

        internal DirectoryIdentityLease(
            SafeFileHandle sourceTarget,
            SafeFileHandle outputTarget,
            SafeFileHandle sourceEntry,
            SafeFileHandle outputEntry)
        {
            _sourceTarget = sourceTarget;
            _outputTarget = outputTarget;
            _sourceEntry = sourceEntry;
            _outputEntry = outputEntry;
        }

        public void Dispose()
        {
            // 先释放路径入口，再释放解析后的目标；重复 Dispose 也是安全的。
            Interlocked.Exchange(ref _outputEntry, null)?.Dispose();
            Interlocked.Exchange(ref _sourceEntry, null)?.Dispose();
            Interlocked.Exchange(ref _outputTarget, null)?.Dispose();
            Interlocked.Exchange(ref _sourceTarget, null)?.Dispose();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ByHandleFileInformation
    {
        public uint FileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
        public uint VolumeSerialNumber;
        public uint FileSizeHigh;
        public uint FileSizeLow;
        public uint NumberOfLinks;
        public uint FileIndexHigh;
        public uint FileIndexLow;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFile(
        string fileName,
        uint desiredAccess,
        FileShare shareMode,
        IntPtr securityAttributes,
        uint creationDisposition,
        uint flagsAndAttributes,
        IntPtr templateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetFileInformationByHandle(
        SafeFileHandle file,
        out ByHandleFileInformation fileInformation);
}
