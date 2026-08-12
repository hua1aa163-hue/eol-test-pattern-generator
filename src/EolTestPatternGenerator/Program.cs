using EolTestPatternGenerator.Services;
using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length >= 2 && args[0].Equals("--export-defaults", StringComparison.OrdinalIgnoreCase))
        {
            ImageFormatKind format = args.Length >= 3 ? ParseFormat(args[2]) : ImageFormatKind.Png;
            int width = args.Length >= 4 ? int.Parse(args[3]) : 1920;
            int height = args.Length >= 5 ? int.Parse(args[4]) : 1080;
            int quality = args.Length >= 6 ? int.Parse(args[5]) : 95;
            var options = new ImageExportOptions { Format = format, Quality = quality };
            DefaultBatchExporter.ExportAll(args[1], width, height, 4, 5, options);
            Console.WriteLine($"已导出默认图卡到：{Path.GetFullPath(args[1])}");
            return 0;
        }

        if (args.Length >= 2 && args[0].Equals("--verify", StringComparison.OrdinalIgnoreCase))
        {
            return ReferenceVerifier.Run(args[1]);
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
        return 0;
    }

    private static ImageFormatKind ParseFormat(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "png" => ImageFormatKind.Png,
            "jpg" or "jpeg" => ImageFormatKind.Jpeg,
            "bmp" => ImageFormatKind.Bmp,
            "tif" or "tiff" => ImageFormatKind.Tiff,
            "webp" => ImageFormatKind.WebP,
            _ => throw new ArgumentException($"不支持的格式：{value}")
        };
    }
}
