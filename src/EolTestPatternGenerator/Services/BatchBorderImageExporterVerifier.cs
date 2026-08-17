using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>批量白框服务的无界面回归验证，供统一自检入口调用。</summary>
internal static class BatchBorderImageExporterVerifier
{
    public static void RunAll()
    {
        string root = Path.Combine(
            Path.GetTempPath(),
            $"EolBatchBorderVerifier-{Environment.ProcessId}-{Guid.NewGuid():N}");
        string source = Path.Combine(root, "源图片");
        string output = Path.Combine(root, "输出图片");

        try
        {
            Directory.CreateDirectory(source);
            Directory.CreateDirectory(output);
            string firstSource = CreateBlackImage(source, "一.png", 10, 8, ImageFormatKind.Png);
            string secondSource = CreateBlackImage(source, "二.bmp", 14, 11, ImageFormatKind.Bmp);
            string alphaSource = CreateTransparentImage(source, "透明.png", 12, 9);
            string multiPageSource = VerifierTiffImageFactory.CreateMultiPage(source, "多页.tiff");
            File.WriteAllText(Path.Combine(source, "忽略.txt"), "not an image");
            byte[] firstOriginalBytes = File.ReadAllBytes(firstSource);
            byte[] secondOriginalBytes = File.ReadAllBytes(secondSource);
            byte[] alphaOriginalBytes = File.ReadAllBytes(alphaSource);
            byte[] multiPageOriginalBytes = File.ReadAllBytes(multiPageSource);

            // 先制造一个同名目标，验证计划阶段会集中检出覆盖风险。
            File.Copy(firstSource, Path.Combine(output, Path.GetFileName(firstSource)));
            BatchBorderExportPlan collisionPlan = BatchBorderImageExporter.CreatePlan(source, output);
            if (collisionPlan.Items.Count != 4 || collisionPlan.ExistingOutputPaths.Count != 1)
            {
                throw new InvalidOperationException("批量白框没有冻结正确的输入列表或预检同名输出。");
            }

            var border = new BorderOverlaySettings { Enabled = false, LineWidth = 1 };
            border.SetMargins(new RegionMargins(1, 2, 1, 2));
            try
            {
                BatchBorderImageExporter.Export(collisionPlan, border, 95, overwriteExisting: false);
                throw new InvalidOperationException("批量白框在未经确认时覆盖了同名文件。");
            }
            catch (IOException)
            {
                // 预期：计划已发现同名文件且调用方没有授权覆盖。
            }

            Directory.Delete(output, recursive: true);
            Directory.CreateDirectory(output);
            BatchBorderExportPlan plan = BatchBorderImageExporter.CreatePlan(source, output);
            BatchBorderExportResult result = BatchBorderImageExporter.Export(
                plan,
                border,
                95,
                overwriteExisting: false);
            if (result.OutputPaths.Count != 3 || result.Failures.Count != 1)
            {
                throw new InvalidOperationException("批量白框的混合格式成功数或多页 TIFF 失败数不正确。");
            }

            BatchBorderExportFailure multiPageFailure = result.Failures.Single();
            if (!Path.GetFileName(multiPageFailure.SourcePath).Equals("多页.tiff", StringComparison.Ordinal) ||
                !multiPageFailure.ErrorMessage.Contains("多页 TIFF", StringComparison.Ordinal) ||
                File.Exists(multiPageFailure.OutputPath))
            {
                throw new InvalidOperationException("批量白框没有明确拒绝多页 TIFF。");
            }

            VerifyBorder(Path.Combine(output, "一.png"), 10, 8);
            VerifyBorder(Path.Combine(output, "二.bmp"), 14, 11);
            VerifyTransparentBorder(Path.Combine(output, "透明.png"), 12, 9);
            if (!File.ReadAllBytes(firstSource).SequenceEqual(firstOriginalBytes) ||
                !File.ReadAllBytes(secondSource).SequenceEqual(secondOriginalBytes) ||
                !File.ReadAllBytes(alphaSource).SequenceEqual(alphaOriginalBytes) ||
                !File.ReadAllBytes(multiPageSource).SequenceEqual(multiPageOriginalBytes))
            {
                throw new InvalidOperationException("批量白框修改了源图片文件。");
            }

            if (!WindowsFileSystemIdentity.AreSameDirectory(source, source) ||
                WindowsFileSystemIdentity.AreSameDirectory(source, output))
            {
                throw new InvalidOperationException("批量白框无法可靠区分同一物理目录和不同目录。");
            }

            try
            {
                BatchBorderImageExporter.CreatePlan(source, source);
                throw new InvalidOperationException("批量白框允许输出目录覆盖源图片目录。");
            }
            catch (InvalidOperationException exception) when (
                exception.Message.Contains("不能相同", StringComparison.Ordinal))
            {
                // 预期：保留原文件名时不能把输出写回源目录。
            }
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
                // 自检异常应保留原始失败信息，临时目录清理失败不覆盖它。
            }
        }
    }

    private static string CreateBlackImage(
        string directory,
        string fileName,
        int width,
        int height,
        ImageFormatKind format)
    {
        using var image = new Mat(height, width, MatType.CV_8UC3, Scalar.Black);
        return ImageFileWriter.Write(
            Path.Combine(directory, fileName),
            image,
            new ImageExportOptions { Format = format, Quality = 95 });
    }

    private static string CreateTransparentImage(string directory, string fileName, int width, int height)
    {
        using var image = new Mat(height, width, MatType.CV_8UC4, new Scalar(10, 20, 30, 77));
        return ImageFileWriter.Write(
            Path.Combine(directory, fileName),
            image,
            new ImageExportOptions { Format = ImageFormatKind.Png, Quality = 95 });
    }

    private static void VerifyBorder(string path, int expectedWidth, int expectedHeight)
    {
        using Mat image = UnicodeImageLoader.LoadColor(path);
        if (image.Cols != expectedWidth || image.Rows != expectedHeight)
        {
            throw new InvalidOperationException("批量白框改变了图片尺寸。");
        }

        Vec3b topLeftBorder = image.At<Vec3b>(2, 1);
        Vec3b bottomRightBorder = image.At<Vec3b>(expectedHeight - 3, expectedWidth - 2);
        Vec3b innerPixel = image.At<Vec3b>(3, 2);
        if (!IsWhite(topLeftBorder) || !IsWhite(bottomRightBorder) || IsWhite(innerPixel))
        {
            throw new InvalidOperationException("批量白框没有按每张图片自己的画布尺寸解析四边距。");
        }
    }

    private static bool IsWhite(Vec3b pixel)
    {
        return pixel.Item0 == 255 && pixel.Item1 == 255 && pixel.Item2 == 255;
    }

    private static void VerifyTransparentBorder(string path, int expectedWidth, int expectedHeight)
    {
        using Mat image = UnicodeImageLoader.LoadUnchanged(path);
        if (image.Type() != MatType.CV_8UC4 || image.Cols != expectedWidth || image.Rows != expectedHeight)
        {
            throw new InvalidOperationException("批量白框没有保留透明 PNG 的 BGRA 通道和尺寸。");
        }

        Vec4b border = image.At<Vec4b>(2, 1);
        Vec4b inside = image.At<Vec4b>(3, 2);
        if (border.Item0 != 255 || border.Item1 != 255 || border.Item2 != 255 || border.Item3 != 255 ||
            inside.Item0 != 10 || inside.Item1 != 20 || inside.Item2 != 30 || inside.Item3 != 77)
        {
            throw new InvalidOperationException("批量白框没有保留透明区域，或白框本身不是不透明纯白。");
        }
    }
}
