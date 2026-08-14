using EolTestPatternGenerator.Services;
using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Controls;

namespace EolTestPatternGenerator;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length >= 1 && args[0].Equals("--self-test", StringComparison.OrdinalIgnoreCase))
        {
            GeneratedPatternVerifier.RunAll();
            Console.WriteLine("图卡像素自检通过。");
            return 0;
        }

        if (args.Length >= 1 && args[0].Equals("--ui-smoke-test", StringComparison.OrdinalIgnoreCase))
        {
            ApplicationConfiguration.Initialize();
            using var mainForm = new MainForm();
            using var phaseStripeForm = new PhaseStripeForm();
            using var screenOneForm = new ScreenOneForm();
            using var nonIntegerFusionForm = new NonIntegerFusionForm();
            using var previewControl = new ImagePreviewControl();

            // 直接点击 Designer 创建的工具栏按钮，验证事件绑定和属性同步都有效。
            if (!previewControl.ShowCenterCrosshair || !previewControl.ShowPixelCoordinates)
            {
                throw new InvalidOperationException("预览辅助显示开关的默认状态不正确。");
            }

            ToolStrip toolStrip = previewControl.Controls
                .OfType<ToolStrip>()
                .Single();
            var centerButton = toolStrip.Items["buttonCenterCrosshair"] as ToolStripButton
                ?? throw new InvalidOperationException("未找到中心十字开关。");
            var coordinateButton = toolStrip.Items["buttonPixelCoordinates"] as ToolStripButton
                ?? throw new InvalidOperationException("未找到像素坐标开关。");

            centerButton.PerformClick();
            coordinateButton.PerformClick();
            if (previewControl.ShowCenterCrosshair || previewControl.ShowPixelCoordinates)
            {
                throw new InvalidOperationException("预览辅助显示开关无法关闭。");
            }

            centerButton.PerformClick();
            coordinateButton.PerformClick();
            if (!previewControl.ShowCenterCrosshair || !previewControl.ShowPixelCoordinates)
            {
                throw new InvalidOperationException("预览辅助显示开关无法重新开启。");
            }

            // Show 会触发与真实运行一致的 OnLoad；立即隐藏，自动验证三个窗体及 OpenCV 预览。
            ShowAndHide(mainForm);
            ShowAndHide(phaseStripeForm);
            ShowAndHide(screenOneForm);
            ShowAndHide(nonIntegerFusionForm);
            Console.WriteLine("WinForms 界面构造自检通过。");
            return 0;
        }

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

    private static void ShowAndHide(Form form)
    {
        form.ShowInTaskbar = false;
        form.Opacity = 0;
        form.Show();
        Application.DoEvents();
        form.Hide();
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
