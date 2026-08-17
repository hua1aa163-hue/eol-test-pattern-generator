using EolTestPatternGenerator.Models;
using OpenCvSharp;
using System.Drawing.Imaging;

namespace EolTestPatternGenerator.Services;

/// <summary>文件夹批量二合一服务的无界面回归验证，供统一自检入口调用。</summary>
internal static class BatchTwoInOneImageExporterVerifier
{
    public static void RunAll()
    {
        string root = Path.Combine(
            Path.GetTempPath(),
            $"EolBatchTwoInOneVerifier-{Environment.ProcessId}-{Guid.NewGuid():N}");
        string source = Path.Combine(root, "源图片");
        string output = Path.Combine(root, "输出图片");

        try
        {
            Directory.CreateDirectory(source);
            Directory.CreateDirectory(output);

            var sourceTypes = new Dictionary<string, MatType>(StringComparer.OrdinalIgnoreCase)
            {
                [CreateImage(source, "灰度.png", MatType.CV_8UC1, ImageFormatKind.Png)] = MatType.CV_8UC1,
                // 使用真实的奇数宽有损源图，确保服务是按格式拒绝，
                // 而不是偶然依赖 JPEG/WebP 块边界维持左右相似。
                [CreateImage(source, "彩色.jpeg", MatType.CV_8UC3, ImageFormatKind.Jpeg, width: 17, height: 13)] = MatType.CV_8UC3,
                [CreateImage(source, "位图.bmp", MatType.CV_8UC3, ImageFormatKind.Bmp)] = MatType.CV_8UC3,
                [CreateImage(source, "扫描.tif", MatType.CV_8UC1, ImageFormatKind.Tiff)] = MatType.CV_8UC1,
                [CreateImage(source, "网页.webp", MatType.CV_8UC3, ImageFormatKind.WebP, width: 17, height: 13)] = MatType.CV_8UC3,
                [CreateImage(source, "透明.png", MatType.CV_8UC4, ImageFormatKind.Png)] = MatType.CV_8UC4,
                [CreateHighDepthImage(source, "高位深.png")] = MatType.CV_16UC1,
                [VerifierTiffImageFactory.CreateMultiPage(source, "多页.tiff")] = MatType.CV_8UC3
            };
            Directory.CreateDirectory(Path.Combine(source, "子文件夹"));
            CreateImage(
                Path.Combine(source, "子文件夹"),
                "不应扫描.png",
                MatType.CV_8UC3,
                ImageFormatKind.Png);
            File.WriteAllText(Path.Combine(source, "忽略.txt"), "not an image");

            Dictionary<string, byte[]> originalBytes = sourceTypes.Keys.ToDictionary(
                path => path,
                File.ReadAllBytes,
                StringComparer.OrdinalIgnoreCase);

            // 计划阶段必须冻结当前层八张受支持扩展名图片，并集中报告同名目标。
            string collisionPath = Path.Combine(output, "灰度.png");
            File.Copy(Path.Combine(source, "灰度.png"), collisionPath);
            byte[] collisionBytes = File.ReadAllBytes(collisionPath);
            BatchTwoInOneExportPlan collisionPlan = BatchTwoInOneImageExporter.CreatePlan(source, output);
            if (collisionPlan.Items.Count != 8 || collisionPlan.ExistingOutputPaths.Count != 1)
            {
                throw new InvalidOperationException("批量二合一没有冻结正确的当前层图片或集中预检同名输出。");
            }

            try
            {
                BatchTwoInOneImageExporter.Export(
                    collisionPlan,
                    overwriteExisting: false);
                throw new InvalidOperationException("批量二合一未经授权便覆盖了预检到的同名文件。");
            }
            catch (IOException)
            {
                // 预期：任何图片落盘前即拒绝整个未授权批次。
            }

            if (!File.ReadAllBytes(collisionPath).SequenceEqual(collisionBytes) ||
                Directory.EnumerateFiles(output).Count() != 1)
            {
                throw new InvalidOperationException("批量二合一的集中预检发生了部分写入。");
            }

            Directory.Delete(output, recursive: true);
            Directory.CreateDirectory(output);
            BatchTwoInOneExportPlan plan = BatchTwoInOneImageExporter.CreatePlan(source, output);
            var progressItems = new List<BatchTwoInOneExportProgress>();
            BatchTwoInOneExportResult result = BatchTwoInOneImageExporter.Export(
                plan,
                overwriteExisting: false,
                new SynchronousProgress<BatchTwoInOneExportProgress>(progressItems.Add));

            if (result.OutputPaths.Count != 4 || result.Failures.Count != 4 ||
                progressItems.Count != plan.Items.Count ||
                progressItems[^1].CompletedCount != plan.Items.Count ||
                progressItems.Any(item => item.TotalCount != plan.Items.Count))
            {
                throw new InvalidOperationException("批量二合一的成功、失败或进度汇总不正确。");
            }

            VerifyRejected(result, "高位深.png", "高位深");
            VerifyRejected(result, "彩色.jpeg", "有损", "PNG");
            VerifyRejected(result, "网页.webp", "有损", "PNG");
            VerifyRejected(result, "多页.tiff", "多页 TIFF");

            string[] exportedNames = ["灰度.png", "位图.bmp", "扫描.tif", "透明.png"];
            foreach ((string sourcePath, MatType expectedType) in sourceTypes.Where(pair =>
                         exportedNames.Contains(Path.GetFileName(pair.Key), StringComparer.Ordinal)))
            {
                string outputPath = Path.Combine(output, Path.GetFileName(sourcePath));
                using Mat sourceImage = UnicodeImageLoader.LoadUnchanged(sourcePath);
                using Mat outputImage = UnicodeImageLoader.LoadUnchanged(outputPath);
                if (outputImage.Rows != sourceImage.Rows || outputImage.Cols != sourceImage.Cols * 2 ||
                    outputImage.Type() != expectedType)
                {
                    throw new InvalidOperationException(
                        $"批量二合一没有保留尺寸或通道类型：{Path.GetFileName(sourcePath)}");
                }

                VerifyHalvesMatch(outputImage, Path.GetFileName(sourcePath));
            }

            if (originalBytes.Any(pair => !File.ReadAllBytes(pair.Key).SequenceEqual(pair.Value)))
            {
                throw new InvalidOperationException("批量二合一修改了源图片文件。");
            }

            VerifyLateCollisionDoesNotOverwrite(root);
            VerifySameDirectoryRejected(source, output);
            VerifyDirectoryLeasePreventsReplacement(root);
        }
        finally
        {
            try
            {
                if (Directory.Exists(root))
                {
                    Directory.Delete(root, recursive: true);
                }
            }
            catch
            {
                // 临时目录清理失败不能覆盖真实的自检异常。
            }
        }
    }

    private static void VerifyLateCollisionDoesNotOverwrite(string root)
    {
        string source = Path.Combine(root, "竞态源");
        string output = Path.Combine(root, "竞态输出");
        Directory.CreateDirectory(source);
        Directory.CreateDirectory(output);
        CreateImage(source, "竞态.png", MatType.CV_8UC3, ImageFormatKind.Png);
        BatchTwoInOneExportPlan plan = BatchTwoInOneImageExporter.CreatePlan(source, output);

        string target = Path.Combine(output, "竞态.png");
        byte[] sentinel = [9, 8, 7, 6, 5];
        File.WriteAllBytes(target, sentinel);
        BatchTwoInOneExportResult result = BatchTwoInOneImageExporter.Export(
            plan,
            overwriteExisting: false);
        if (result.OutputPaths.Count != 0 || result.Failures.Count != 1 ||
            !File.ReadAllBytes(target).SequenceEqual(sentinel))
        {
            throw new InvalidOperationException("计划完成后出现的同名文件被未授权覆盖。");
        }
    }

    private static void VerifySameDirectoryRejected(string source, string differentDirectory)
    {
        if (!WindowsFileSystemIdentity.AreSameDirectory(source, source) ||
            WindowsFileSystemIdentity.AreSameDirectory(source, differentDirectory))
        {
            throw new InvalidOperationException("批量二合一无法可靠识别物理目录身份。");
        }

        try
        {
            BatchTwoInOneImageExporter.CreatePlan(source, source);
            throw new InvalidOperationException("批量二合一允许把输出写回源图片目录。");
        }
        catch (InvalidOperationException exception) when (
            exception.Message.Contains("不能相同", StringComparison.Ordinal))
        {
            // 预期：同一物理目录必须拒绝。
        }
    }

    private static string CreateImage(
        string directory,
        string fileName,
        MatType type,
        ImageFormatKind format,
        int width = 16,
        int height = 16)
    {
        Directory.CreateDirectory(directory);
        Scalar value = type switch
        {
            var item when item == MatType.CV_8UC1 => new Scalar(91),
            var item when item == MatType.CV_8UC4 => new Scalar(17, 83, 191, 77),
            _ => new Scalar(23, 101, 211)
        };
        using var image = new Mat(height, width, type, value);
        return ImageFileWriter.Write(
            Path.Combine(directory, fileName),
            image,
            new ImageExportOptions { Format = format, Quality = 95 });
    }

    private static string CreateHighDepthImage(string directory, string fileName)
    {
        using var image = new Mat(8, 8, MatType.CV_16UC1, new Scalar(2048));
        return ImageFileWriter.Write(
            Path.Combine(directory, fileName),
            image,
            new ImageExportOptions { Format = ImageFormatKind.Png, Quality = 95 });
    }

    private static void VerifyHalvesMatch(Mat image, string fileName)
    {
        int halfWidth = image.Cols / 2;
        using var left = new Mat(image, new Rect(0, 0, halfWidth, image.Rows));
        using var right = new Mat(image, new Rect(halfWidth, 0, halfWidth, image.Rows));
        using var difference = new Mat();
        Cv2.Absdiff(left, right, difference);
        using Mat flattenedDifference = difference.Reshape(1);
        if (Cv2.CountNonZero(flattenedDifference) != 0)
        {
            throw new InvalidOperationException($"批量二合一的左右图片不一致：{fileName}");
        }
    }

    private static void VerifyRejected(
        BatchTwoInOneExportResult result,
        string fileName,
        params string[] requiredMessages)
    {
        BatchTwoInOneExportFailure failure = result.Failures.Single(item =>
            Path.GetFileName(item.SourcePath).Equals(fileName, StringComparison.Ordinal));
        if (requiredMessages.Any(message =>
                !failure.ErrorMessage.Contains(message, StringComparison.Ordinal)) ||
            File.Exists(failure.OutputPath))
        {
            throw new InvalidOperationException($"批量二合一没有明确拒绝 {fileName}。");
        }
    }

    private static void VerifyDirectoryLeasePreventsReplacement(string root)
    {
        string source = Path.Combine(root, "租约源");
        string output = Path.Combine(root, "租约输出");
        string renamedSource = Path.Combine(root, "租约源-改名");
        string renamedOutput = Path.Combine(root, "租约输出-改名");
        Directory.CreateDirectory(source);
        Directory.CreateDirectory(output);

        using (WindowsFileSystemIdentity.DirectoryIdentityLease lease =
               WindowsFileSystemIdentity.AcquireIndependentDirectoryLease(source, output))
        {
            VerifyMoveIsBlocked(source, renamedSource);
            VerifyMoveIsBlocked(output, renamedOutput);
        }

        // 释放租约后改名必须立即恢复，以同时验证句柄没有泄漏。
        Directory.Move(source, renamedSource);
        Directory.Move(renamedSource, source);
        Directory.Move(output, renamedOutput);
        Directory.Move(renamedOutput, output);
    }

    private static void VerifyMoveIsBlocked(string source, string destination)
    {
        try
        {
            Directory.Move(source, destination);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return;
        }

        // 理论上不应该进入此分支；先尽量恢复测试目录，再报告租约失效。
        if (Directory.Exists(destination) && !Directory.Exists(source))
        {
            Directory.Move(destination, source);
        }

        throw new InvalidOperationException("目录租约期间仍可以替换源或输出目录。");
    }

    private sealed class SynchronousProgress<T>(Action<T> report) : IProgress<T>
    {
        public void Report(T value) => report(value);
    }
}

/// <summary>为无界面回归创建真实的两页 TIFF，避免使用伪造文件测试。</summary>
internal static class VerifierTiffImageFactory
{
    public static string CreateMultiPage(string directory, string fileName)
    {
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, fileName);
        ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().Single(item =>
            item.FormatID == ImageFormat.Tiff.Guid);
        using var first = new Bitmap(9, 7);
        using var second = new Bitmap(9, 7);
        using (Graphics graphics = Graphics.FromImage(first))
        {
            graphics.Clear(Color.Red);
        }

        using (Graphics graphics = Graphics.FromImage(second))
        {
            graphics.Clear(Color.Blue);
        }

        using (var start = CreateSaveFlag(EncoderValue.MultiFrame))
        {
            first.Save(path, codec, start);
        }

        using (var page = CreateSaveFlag(EncoderValue.FrameDimensionPage))
        {
            first.SaveAdd(second, page);
        }

        using (var flush = CreateSaveFlag(EncoderValue.Flush))
        {
            first.SaveAdd(flush);
        }

        return path;
    }

    private static EncoderParameters CreateSaveFlag(EncoderValue value)
    {
        var parameters = new EncoderParameters(1);
        parameters.Param[0] = new EncoderParameter(
            System.Drawing.Imaging.Encoder.SaveFlag,
            (long)value);
        return parameters;
    }
}
