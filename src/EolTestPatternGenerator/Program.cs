using EolTestPatternGenerator.Services;
using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Controls;
using EolTestPatternGenerator.Projection;

namespace EolTestPatternGenerator;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length >= 1 && args[0].Equals("--self-test", StringComparison.OrdinalIgnoreCase))
        {
            GeneratedPatternVerifier.RunAll();
            ProjectionVerifier.RunAll();
            StillVideoVerifier.VerifyGeometryAndScheduling();
            Console.WriteLine("图卡像素自检通过。");
            return 0;
        }

        if (args.Length >= 1 && args[0].Equals("--video-self-test", StringComparison.OrdinalIgnoreCase))
        {
            StillVideoVerifier.VerifyAllCodecs();
            Console.WriteLine("视频帧调度、适配、编码回读及无损像素自检通过。");
            return 0;
        }

        if (args.Length >= 1 && args[0].Equals("--ui-smoke-test", StringComparison.OrdinalIgnoreCase))
        {
            ApplicationConfiguration.Initialize();
            Application.AddMessageFilter(new NumericMouseWheelFilter());
            using var workspaceForm = new WorkspaceForm();
            using var mainForm = new MainForm();
            using var phaseStripeForm = new PhaseStripeForm();
            using var screenOneForm = new ScreenOneForm();
            using var nonIntegerFusionForm = new NonIntegerFusionForm();
            using var previewControl = new ImagePreviewControl();
            using var cycleEditor = new CrosstalkCycleEditor();
            using var stillVideoPage = new StillVideoPage();
            using var stillVideoHost = new Form
            {
                ShowInTaskbar = false,
                Size = new Size(1200, 760)
            };
            stillVideoPage.Dock = DockStyle.Fill;
            stillVideoHost.Controls.Add(stillVideoPage);

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

            int previewImageChanges = 0;
            previewControl.ImageChanged += (_, _) => previewImageChanges++;
            previewControl.SetImage(new Bitmap(3, 2), preserveView: false);
            using (Bitmap? previewClone = previewControl.CloneCurrentImage())
            {
                if (!previewControl.HasImage || previewClone?.Size != new Size(3, 2))
                {
                    throw new InvalidOperationException("实时投图无法取得当前原始预览图副本。");
                }
            }

            previewControl.SetImage(null, preserveView: false);
            if (previewControl.HasImage || previewImageChanges != 2)
            {
                throw new InvalidOperationException("预览图片变化事件或清空状态不正确。");
            }

            cycleEditor.SetCycle(new CrosstalkPixelCycle
            {
                Pixels = [RgbChannelMask.Blue, RgbChannelMask.None],
                ColumnAdvance = int.MaxValue,
                RowAdvance = int.MinValue,
                TiltAngleDegrees = 10.125d
            });
            CrosstalkPixelCycle normalizedCycle = cycleEditor.GetCycle();
            ComboBox presetCombo = FindRequiredControl<ComboBox>(cycleEditor, "comboLegacyPreset");
            if (normalizedCycle.PeriodLength != 8 ||
                normalizedCycle.ColumnAdvance != -3 ||
                normalizedCycle.ResolveTiltAngleDegrees() != 10.125d ||
                presetCombo.Items.Count != 6 ||
                cycleEditor.Controls.Find("numericColumnAdvance", true).Length != 0 ||
                cycleEditor.Controls.Find("numericPeriodLength", true).Length != 0 ||
                cycleEditor.Controls.Find("gridCycle", true).Length != 0)
            {
                throw new InvalidOperationException(
                    "串扰编辑器未固定为六种历史 8 像素预设，或已删除控件仍然存在。");
            }

            // Show 会触发与真实运行一致的 OnLoad；立即隐藏，自动验证三个窗体及 OpenCV 预览。
            ShowAndHide(mainForm);
            VerifyMainDotSingleAxisSynchronization(mainForm);
            ShowAndHide(phaseStripeForm);
            VerifyPhaseStripeResetDefaults(phaseStripeForm);
            ShowAndHide(screenOneForm);
            ShowAndHide(nonIntegerFusionForm);
            ShowAndHide(stillVideoHost);
            if (!stillVideoPage.PrepareToClose())
            {
                throw new InvalidOperationException("图片转视频页面无法提交当前输入或完成关闭准备。");
            }

            if (mainForm.IsExporting ||
                phaseStripeForm.IsExporting ||
                screenOneForm.IsExporting ||
                nonIntegerFusionForm.IsExporting)
            {
                throw new InvalidOperationException("新建页面被错误识别为正在导出，工作台两阶段关闭检查无效。");
            }

            VerifyWorkspaceNavigationMapping(workspaceForm);
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
        Application.AddMessageFilter(new NumericMouseWheelFilter());
        Application.Run(new WorkspaceForm());
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

    /// <summary>实际点击 Designer 恢复按钮，确认它不依赖其他页面参数并会立即恢复完整默认值。</summary>
    private static void VerifyPhaseStripeResetDefaults(PhaseStripeForm form)
    {
        form.Show();
        Application.DoEvents();

        NumericUpDown canvasWidth = FindRequiredControl<NumericUpDown>(form, "numericCanvasWidth");
        NumericUpDown phase = FindRequiredControl<NumericUpDown>(form, "numericPhase");
        NumericUpDown quality = FindRequiredControl<NumericUpDown>(form, "numericQuality");
        ComboBox format = FindRequiredControl<ComboBox>(form, "comboOutputFormat");
        RegionMarginsEditor margins = FindRequiredControl<RegionMarginsEditor>(form, "regionMarginsEditor");
        CrosstalkCycleEditor cycle = FindRequiredControl<CrosstalkCycleEditor>(form, "cycleEditor");
        BorderOverlayEditor border = FindRequiredControl<BorderOverlayEditor>(form, "borderOverlayEditor");
        ImagePreviewControl preview = FindRequiredControl<ImagePreviewControl>(form, "previewControl");
        Button reset = FindRequiredControl<Button>(form, "buttonResetDefaults");

        canvasWidth.Value = 800;
        margins.CanvasSize = new Size(800, 1080);
        margins.SetMargins(new RegionMargins(2, 3, 4, 5));
        cycle.SetCycle(new CrosstalkPixelCycle
        {
            Pixels = [RgbChannelMask.Blue, RgbChannelMask.None],
            ColumnAdvance = 2,
            RowAdvance = 0,
            TiltAngleDegrees = 10.125d
        });
        phase.Maximum = 2;
        phase.Value = 2;
        border.SetSettings(new BorderOverlaySettings { Enabled = true, LineWidth = 9 });
        format.SelectedIndex = (int)ImageFormatKind.Bmp;
        quality.Value = 42;
        preview.ShowCenterCrosshair = false;
        preview.ShowPixelCoordinates = false;

        reset.PerformClick();
        Application.DoEvents();

        RegionMargins restoredMargins = margins.GetMargins();
        CrosstalkPixelCycle restoredCycle = cycle.GetCycle();
        BorderOverlaySettings restoredBorder = border.GetSettings();
        if (canvasWidth.Value != 1920 ||
            FindRequiredControl<NumericUpDown>(form, "numericCanvasHeight").Value != 1080 ||
            restoredMargins.Left != 71 || restoredMargins.Top != 226 ||
            restoredMargins.Right != 72 || restoredMargins.Bottom != 227 ||
            phase.Value != 1 || restoredCycle.PeriodLength != 8 ||
            restoredCycle.ResolveTiltAngleDegrees() != CrosstalkPixelCycle.DefaultTiltAngleDegrees ||
            !CrosstalkPixelCyclePresets.TryGetLegacyOrder(restoredCycle, out RgbPixelOrder order) ||
            order != RgbPixelOrder.RGB || restoredBorder.Enabled || restoredBorder.LineWidth != 5 ||
            format.SelectedIndex != (int)ImageFormatKind.Png || quality.Value != 95 ||
            !preview.ShowCenterCrosshair || !preview.ShowPixelCoordinates)
        {
            throw new InvalidOperationException("串扰页面的恢复默认按钮未恢复最初版完整参数。");
        }

        form.Hide();
    }

    /// <summary>
    /// 逐项切换工作台导航，验证调整显示顺序后每个项目仍指向正确的 Designer 页面，
    /// 并验证旧 v2 序号只在稳定页面标识缺失时迁移一次。
    /// </summary>
    private static void VerifyWorkspaceNavigationMapping(WorkspaceForm form)
    {
        var expectedTitles = new[]
        {
            "基础图卡",
            "3D显示器图卡",
            "串扰像素排列",
            "非整数连续融合",
            "离散光源",
            "图片转 LightTools",
            "图片转视频"
        };
        Type[] expectedPageTypes =
        [
            typeof(MainForm),
            typeof(ScreenOneForm),
            typeof(PhaseStripeForm),
            typeof(NonIntegerFusionForm),
            typeof(NonIntegerFusionForm),
            typeof(NonIntegerFusionForm),
            typeof(StillVideoPage)
        ];
        string[] expectedPageIds =
        [
            WorkspaceNavigationPages.BasicPatterns,
            WorkspaceNavigationPages.DisplayCards,
            WorkspaceNavigationPages.CrosstalkPixels,
            WorkspaceNavigationPages.ContinuousFusion,
            WorkspaceNavigationPages.DiscreteLightSource,
            WorkspaceNavigationPages.ImageToLightTools,
            WorkspaceNavigationPages.ImageToVideo
        ];

        // 旧 v2 只保存序号，旧 1/2 分别是串扰/显示器；新配置用稳定标识消除二次交换。
        var legacyCrosstalk = new WorkspacePreferences { SelectedNavigationIndex = 1 };
        var legacyDisplay = new WorkspacePreferences { SelectedNavigationIndex = 2 };
        var migratedCrosstalk = new WorkspacePreferences
        {
            SelectedNavigationIndex = 2,
            SelectedNavigationPageId = WorkspaceNavigationPages.CrosstalkPixels
        };
        var migratedDisplay = new WorkspacePreferences
        {
            SelectedNavigationIndex = 1,
            SelectedNavigationPageId = WorkspaceNavigationPages.DisplayCards
        };
        if (WorkspaceNavigationPages.ResolveSelectedIndex(legacyCrosstalk, expectedTitles.Length) != 2 ||
            WorkspaceNavigationPages.ResolveSelectedIndex(legacyDisplay, expectedTitles.Length) != 1 ||
            WorkspaceNavigationPages.ResolveSelectedIndex(migratedCrosstalk, expectedTitles.Length) != 2 ||
            WorkspaceNavigationPages.ResolveSelectedIndex(migratedDisplay, expectedTitles.Length) != 1 ||
            Enumerable.Range(0, expectedPageIds.Length)
                .Any(index => WorkspaceNavigationPages.GetPageId(index) != expectedPageIds[index]))
        {
            throw new InvalidOperationException("工作台旧导航序号没有可靠迁移到稳定页面标识。");
        }

        form.ShowInTaskbar = false;
        form.Opacity = 0;
        form.Show();
        Application.DoEvents();

        try
        {
            ListBox navigation = FindRequiredControl<ListBox>(form, "listNavigation");
            Panel pageHost = FindRequiredControl<Panel>(form, "pageHost");
            Label pageTitle = FindRequiredControl<Label>(form, "labelPageTitle");
            ComboBox projectionTopology = FindRequiredControl<ComboBox>(form, "comboProjectionTopology");
            CheckBox liveProjection = FindRequiredControl<CheckBox>(form, "checkLiveProjection");
            CheckBox restoreWallpaper = FindRequiredControl<CheckBox>(form, "checkRestoreWallpaper");
            Button projectCurrent = FindRequiredControl<Button>(form, "buttonProjectCurrent");
            Button stopProjection = FindRequiredControl<Button>(form, "buttonStopProjection");
            int originalIndex = navigation.SelectedIndex;

            if (!navigation.Items.Cast<object>().Select(item => item?.ToString()).SequenceEqual(expectedTitles))
            {
                throw new InvalidOperationException("工作台左侧导航顺序与约定不一致。");
            }

            string[] expectedTopologies = ["保持当前模式", "仅电脑屏幕", "复制屏幕", "仅第二屏幕", "扩展屏幕"];
            if (!projectionTopology.Items.Cast<object>().Select(item => item?.ToString()).SequenceEqual(expectedTopologies) ||
                liveProjection.Checked ||
                projectCurrent.Text != "投图当前预览" ||
                stopProjection.Text != "停止投图")
            {
                throw new InvalidOperationException("工作台投图控件未按 Designer 配置，或实时投图错误地在启动时开启。");
            }

            var projectionPreferences = new WorkspacePreferences
            {
                ProjectionTopology = EolTestPatternGenerator.Projection.DisplayTopology.Extend,
                RestoreWallpaperOnStop = false
            };
            WorkspacePreferences projectionClone = projectionPreferences.Clone();
            if (projectionClone.ProjectionTopology != EolTestPatternGenerator.Projection.DisplayTopology.Extend ||
                projectionClone.RestoreWallpaperOnStop ||
                restoreWallpaper.Name != "checkRestoreWallpaper")
            {
                throw new InvalidOperationException("工作台投图设置没有可靠复制或加载。");
            }

            for (int index = 0; index < expectedTitles.Length; index++)
            {
                navigation.SelectedIndex = index;
                Application.DoEvents();

                Control activePage = pageHost.Controls.Cast<Control>().Single(control => control.Visible);
                if (activePage.GetType() != expectedPageTypes[index] ||
                    !string.Equals(pageTitle.Text, expectedTitles[index], StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"工作台导航“{expectedTitles[index]}”打开了错误的功能页。");
                }

                if (index is >= 3 and <= 5)
                {
                    TabControl sections = FindRequiredControl<TabControl>(activePage, "tabModes");
                    if (sections.SelectedIndex != index - 3)
                    {
                        throw new InvalidOperationException(
                            $"工作台导航“{expectedTitles[index]}”定位到了错误的内部功能区。");
                    }
                }
            }

            navigation.SelectedIndex = originalIndex;
            Application.DoEvents();
        }
        finally
        {
            form.Hide();
        }
    }

    /// <summary>
    /// 直接驱动 Designer 中的主界面输入，防止行列数变更时漏传给圆心/外缘同步路径。
    /// </summary>
    private static void VerifyMainDotSingleAxisSynchronization(MainForm form)
    {
        ComboBox pattern = FindRequiredControl<ComboBox>(form, "comboPattern");
        NumericUpDown canvasWidth = FindRequiredControl<NumericUpDown>(form, "numericCanvasWidth");
        NumericUpDown canvasHeight = FindRequiredControl<NumericUpDown>(form, "numericCanvasHeight");
        NumericUpDown rows = FindRequiredControl<NumericUpDown>(form, "numericRows");
        NumericUpDown columns = FindRequiredControl<NumericUpDown>(form, "numericColumns");
        NumericUpDown centerSpanWidth = FindRequiredControl<NumericUpDown>(form, "numericPatternWidth");
        NumericUpDown centerSpanHeight = FindRequiredControl<NumericUpDown>(form, "numericPatternHeight");
        NumericUpDown centerX = FindRequiredControl<NumericUpDown>(form, "numericPatternX");
        NumericUpDown centerY = FindRequiredControl<NumericUpDown>(form, "numericPatternY");
        NumericUpDown radius = FindRequiredControl<NumericUpDown>(form, "numericDotRadius");
        RegionMarginsEditor margins = FindRequiredControl<RegionMarginsEditor>(form, "regionMarginsEditor");
        BorderOverlayEditor borderOverlay = FindRequiredControl<BorderOverlayEditor>(form, "borderOverlayEditor");

        pattern.SelectedIndex = 1; // 九点阵
        rows.Value = 3;
        columns.Value = 3;
        centerSpanWidth.Value = 120;
        centerSpanHeight.Value = 80;
        columns.Value = 1;
        rows.Value = 1;

        (long outerWidth, long outerHeight) = margins.GetCalculatedSize();
        long expectedDiameter = (2L * decimal.ToInt32(radius.Value)) + 1L;
        if (centerSpanWidth.Value != 0 || centerSpanHeight.Value != 0 ||
            outerWidth != expectedDiameter || outerHeight != expectedDiameter)
        {
            throw new InvalidOperationException(
                "主界面将点阵行/列改为 1 后，未将对应圆心跨度和真实外缘同步为单圆直径。");
        }

        // 驱动 Designer 里的边距数字框，确认超过旧 ±32768 限制的值不会静默截断。
        rows.Value = 3;
        columns.Value = 3;
        var largeMargins = new RegionMargins(-100_000, -90_000, 50_000, 40_000);
        margins.SetMargins(largeMargins);
        NumericUpDown rightMargin = FindRequiredControl<NumericUpDown>(margins, "numericRight");
        rightMargin.Value += 1;
        rightMargin.Value -= 1;

        RegionMargins roundTrippedMargins = margins.GetMargins();
        long expectedCenterSpanWidth =
            (long)decimal.ToInt32(canvasWidth.Value) - largeMargins.Left - largeMargins.Right -
            (2L * decimal.ToInt32(radius.Value)) - 1L;
        long expectedCenterSpanHeight =
            (long)decimal.ToInt32(canvasHeight.Value) - largeMargins.Top - largeMargins.Bottom -
            (2L * decimal.ToInt32(radius.Value)) - 1L;
        if (roundTrippedMargins.Left != largeMargins.Left ||
            roundTrippedMargins.Top != largeMargins.Top ||
            roundTrippedMargins.Right != largeMargins.Right ||
            roundTrippedMargins.Bottom != largeMargins.Bottom ||
            centerX.Value != largeMargins.Left + radius.Value ||
            centerY.Value != largeMargins.Top + radius.Value ||
            centerSpanWidth.Value != expectedCenterSpanWidth ||
            centerSpanHeight.Value != expectedCenterSpanHeight)
        {
            throw new InvalidOperationException("主界面大负边距未能在外缘与圆心定义间无损往返。");
        }

        decimal preservedCenterX = centerX.Value;
        decimal preservedCenterY = centerY.Value;
        decimal preservedSpanWidth = centerSpanWidth.Value;
        decimal preservedSpanHeight = centerSpanHeight.Value;
        radius.Value += 1;
        RegionMargins radiusAdjustedMargins = margins.GetMargins();
        if (centerX.Value != preservedCenterX || centerY.Value != preservedCenterY ||
            centerSpanWidth.Value != preservedSpanWidth || centerSpanHeight.Value != preservedSpanHeight ||
            radiusAdjustedMargins.Left != decimal.ToInt32(preservedCenterX - radius.Value) ||
            radiusAdjustedMargins.Top != decimal.ToInt32(preservedCenterY - radius.Value) ||
            radiusAdjustedMargins.Right != checked(decimal.ToInt32(
                canvasWidth.Value - preservedCenterX - preservedSpanWidth - radius.Value - 1)) ||
            radiusAdjustedMargins.Bottom != checked(decimal.ToInt32(
                canvasHeight.Value - preservedCenterY - preservedSpanHeight - radius.Value - 1)))
        {
            throw new InvalidOperationException("大负边距下改变半径后未保持圆心和跨度。");
        }

        var overlaySettings = new BorderOverlaySettings
        {
            Enabled = true,
            LineWidth = 3
        };
        var overlayMargins = new RegionMargins(-120_000, -110_000, 70_000, 60_000);
        overlaySettings.SetMargins(overlayMargins);
        borderOverlay.SetSettings(overlaySettings);
        RegionMargins roundTrippedOverlay = borderOverlay.GetSettings().GetMargins();
        if (roundTrippedOverlay.Left != overlayMargins.Left ||
            roundTrippedOverlay.Top != overlayMargins.Top ||
            roundTrippedOverlay.Right != overlayMargins.Right ||
            roundTrippedOverlay.Bottom != overlayMargins.Bottom)
        {
            throw new InvalidOperationException("白框编辑器静默截断了大负边距。");
        }
    }

    private static TControl FindRequiredControl<TControl>(Control root, string name)
        where TControl : Control
    {
        return root.Controls.Find(name, searchAllChildren: true).OfType<TControl>().SingleOrDefault()
            ?? throw new InvalidOperationException($"未找到 Designer 控件 {name}。");
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
