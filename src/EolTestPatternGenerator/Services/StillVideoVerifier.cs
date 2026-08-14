using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>图片转视频的帧调度、适配、编码回读和取消安全回归验证。</summary>
public static class StillVideoVerifier
{
    /// <summary>不写视频的快速算法验证，可合并到常规 --self-test。</summary>
    public static void VerifyGeometryAndScheduling()
    {
        if (StillVideoEncoder.CalculateFrameCount(0.01d, 30) != 1 ||
            StillVideoEncoder.CalculateFrameCount(0.05d, 30) != 2 ||
            StillVideoEncoder.CalculateFrameCount(10d, 30) != 300)
        {
            throw new InvalidOperationException("静止图片秒数到帧数的换算不正确。");
        }

        AssertFrameSchedule(
            [0.05d, 0.05d, 0.05d],
            framesPerSecond: 30,
            expected: [2L, 1L, 2L],
            "累计半帧边界");
        AssertFrameSchedule(
            [0.02d, 0.02d, 1d],
            framesPerSecond: 30,
            expected: [1L, 1L, 29L],
            "极短段由后续长段补偿");
        AssertFrameSchedule(
            [0.01d, 0.01d, 0.01d, 0.01d],
            framesPerSecond: 30,
            expected: [1L, 1L, 1L, 1L],
            "每个非零项目至少一帧");

        System.Drawing.Size mp4Size = StillVideoEncoder.ResolveEncodedSize(
            7,
            5,
            StillVideoFormat.Mp4Mpeg4);
        if (mp4Size.Width != 8 || mp4Size.Height != 6)
        {
            throw new InvalidOperationException("MP4 奇数尺寸没有正确补到偶数尺寸。");
        }

        System.Drawing.Size losslessSize = StillVideoEncoder.ResolveEncodedSize(
            7,
            5,
            StillVideoFormat.MkvFfv1);
        if (losslessSize.Width != 8 || losslessSize.Height != 6)
        {
            throw new InvalidOperationException("FFV1 奇数尺寸没有正确补到偶数尺寸。");
        }

        using var source = new Mat(2, 4, MatType.CV_8UC3, new Scalar(17, 91, 203));
        using Mat fitted = StillVideoEncoder.FitImageToCanvas(
            source,
            requestedWidth: 8,
            requestedHeight: 8,
            encodedWidth: 8,
            encodedHeight: 8);
        AssertPixel(fitted, 0, 0, Scalar.Black, "顶部黑边");
        AssertPixel(fitted, 7, 7, Scalar.Black, "底部黑边");
        AssertPixel(fitted, 0, 2, new Scalar(17, 91, 203), "等比图片左上角");
        AssertPixel(fitted, 7, 5, new Scalar(17, 91, 203), "等比图片右下角");
        VerifyPreferencesPersistence();
    }

    private static void AssertFrameSchedule(
        IReadOnlyList<double> durations,
        int framesPerSecond,
        IReadOnlyList<long> expected,
        string description)
    {
        IReadOnlyList<long> actual = StillVideoEncoder.CalculateFrameSchedule(durations, framesPerSecond);
        if (!actual.SequenceEqual(expected))
        {
            throw new InvalidOperationException(
                $"{description}的帧调度为 [{string.Join(",", actual)}]，" +
                $"期望 [{string.Join(",", expected)}]。");
        }
    }

    private static void VerifyPreferencesPersistence()
    {
        string uniqueFileName = $"EolVideoSettings_{Guid.NewGuid():N}.json";
        string temporaryRoot = Path.GetFullPath(Path.GetTempPath());
        string settingsPath = Path.GetFullPath(Path.Combine(temporaryRoot, uniqueFileName));
        if (!string.Equals(
                Path.GetDirectoryName(settingsPath),
                temporaryRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(Path.GetFileName(settingsPath), uniqueFileName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("无法确认视频设置自检临时文件的安全范围。");
        }

        try
        {
            var writer = new UserSettingsStore(settingsPath);
            writer.UpdateAndSave(preferences =>
            {
                preferences.StillVideo = new StillVideoPreferences
                {
                    Items =
                    [
                        new StillVideoItemSettings { ImagePath = @"C:\图片\甲.png", DurationSeconds = 3.25d },
                        new StillVideoItemSettings { ImagePath = @"D:\图卡\乙.bmp", DurationSeconds = 10d }
                    ],
                    OutputWidth = 321,
                    OutputHeight = 123,
                    FramesPerSecond = 24,
                    OutputFormat = StillVideoFormat.MkvFfv1,
                    OutputPath = @"D:\视频\结果.mkv",
                    LastInputDirectory = @"C:\图片",
                    LastOutputDirectory = @"D:\视频"
                };
            });

            var reader = new UserSettingsStore(settingsPath);
            StillVideoPreferences loaded = reader.Load().StillVideo;
            if (loaded.Items.Count != 2 ||
                !string.Equals(loaded.Items[0].ImagePath, @"C:\图片\甲.png", StringComparison.Ordinal) ||
                loaded.Items[0].DurationSeconds != 3.25d ||
                !string.Equals(loaded.Items[1].ImagePath, @"D:\图卡\乙.bmp", StringComparison.Ordinal) ||
                loaded.OutputWidth != 321 ||
                loaded.OutputHeight != 123 ||
                loaded.FramesPerSecond != 24 ||
                loaded.OutputFormat != StillVideoFormat.MkvFfv1 ||
                !string.Equals(loaded.OutputPath, @"D:\视频\结果.mkv", StringComparison.Ordinal) ||
                !string.Equals(loaded.LastInputDirectory, @"C:\图片", StringComparison.Ordinal) ||
                !string.Equals(loaded.LastOutputDirectory, @"D:\视频", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("视频列表、顺序、时长或路径没有从设置文件完整恢复。");
            }

            loaded.Items[0].DurationSeconds = 99d;
            if (reader.Load().StillVideo.Items[0].DurationSeconds != 3.25d)
            {
                throw new InvalidOperationException("视频设置 Load 返回了共享的可变列表，而不是深副本。");
            }
        }
        finally
        {
            if (File.Exists(settingsPath))
            {
                File.Delete(settingsPath);
            }
        }
    }

    /// <summary>
    /// 使用当前随程序发布的 OpenCV/FFmpeg 真正编码目录中的全部格式并回读。
    /// 无损格式逐帧与期望画布做像素级比较。
    /// </summary>
    public static void VerifyAllCodecs()
    {
        VerifyGeometryAndScheduling();
        string uniqueDirectoryName = $"EolVideoVerifier_中文_{Guid.NewGuid():N}";
        string temporaryRoot = Path.GetFullPath(Path.GetTempPath());
        string temporaryDirectory = Path.GetFullPath(Path.Combine(temporaryRoot, uniqueDirectoryName));
        ValidateTemporaryDirectory(temporaryRoot, temporaryDirectory, uniqueDirectoryName);

        Directory.CreateDirectory(temporaryDirectory);
        try
        {
            string redPath = Path.Combine(temporaryDirectory, "第一张_红.png");
            string bluePath = Path.Combine(temporaryDirectory, "第二张_蓝.png");
            WriteSolidPng(redPath, width: 7, height: 5, new Scalar(0, 0, 255));
            WriteSolidPng(bluePath, width: 7, height: 5, new Scalar(255, 0, 0));

            var items = new[]
            {
                new StillVideoItemSettings { ImagePath = redPath, DurationSeconds = 0.2d },
                new StillVideoItemSettings { ImagePath = bluePath, DurationSeconds = 0.2d }
            };

            foreach (StillVideoFormatProfile profile in StillVideoFormatCatalog.All)
            {
                string outputPath = Path.Combine(
                    temporaryDirectory,
                    $"回读验证_{profile.Format}{profile.Extension}");
                var options = new StillVideoEncodingOptions
                {
                    Items = items,
                    OutputPath = outputPath,
                    OutputWidth = 7,
                    OutputHeight = 5,
                    FramesPerSecond = 10,
                    OutputFormat = profile.Format
                };

                StillVideoEncodingResult result = StillVideoEncoder.EncodeAsync(options)
                    .GetAwaiter()
                    .GetResult();
                if (result.FrameCount != 4 || !File.Exists(outputPath))
                {
                    throw new InvalidOperationException($"{profile.DisplayName} 没有生成预期的 4 帧视频。");
                }

                VerifyDecodedFrames(outputPath, result, profile.IsLossless);
            }

            VerifyPreCanceledWritePreservesOldFile(temporaryDirectory, redPath);
        }
        finally
        {
            ValidateTemporaryDirectory(temporaryRoot, temporaryDirectory, uniqueDirectoryName);
            if (Directory.Exists(temporaryDirectory))
            {
                Directory.Delete(temporaryDirectory, recursive: true);
            }
        }
    }

    private static void VerifyDecodedFrames(
        string videoPath,
        StillVideoEncodingResult result,
        bool requireExactPixels)
    {
        using var capture = new VideoCapture(videoPath, VideoCaptureAPIs.FFMPEG);
        if (!capture.IsOpened())
        {
            capture.Open(videoPath);
        }

        if (!capture.IsOpened())
        {
            throw new InvalidOperationException($"回归验证无法打开视频：{videoPath}");
        }

        using var expectedRed = new Mat(
            result.Height,
            result.Width,
            MatType.CV_8UC3,
            Scalar.Black);
        using var expectedBlue = new Mat(
            result.Height,
            result.Width,
            MatType.CV_8UC3,
            Scalar.Black);
        FillRequestedArea(expectedRed, 7, 5, new Scalar(0, 0, 255));
        FillRequestedArea(expectedBlue, 7, 5, new Scalar(255, 0, 0));

        for (int frameIndex = 0; frameIndex < result.FrameCount; frameIndex++)
        {
            using var actual = new Mat();
            if (!capture.Read(actual) || actual.Empty())
            {
                throw new InvalidOperationException(
                    $"{result.Format.DisplayName} 无法读取第 {frameIndex + 1} 帧。");
            }

            if (actual.Cols != result.Width || actual.Rows != result.Height)
            {
                throw new InvalidOperationException(
                    $"{result.Format.DisplayName} 第 {frameIndex + 1} 帧尺寸不正确。");
            }

            if (requireExactPixels)
            {
                Mat expected = frameIndex < 2 ? expectedRed : expectedBlue;
                double maximumDifference = Cv2.Norm(actual, expected, NormTypes.INF);
                if (maximumDifference != 0d)
                {
                    throw new InvalidOperationException(
                        $"{result.Format.DisplayName} 第 {frameIndex + 1} 帧不是像素无损，" +
                        $"最大通道差值为 {maximumDifference}。");
                }
            }
        }
    }

    private static void FillRequestedArea(Mat target, int requestedWidth, int requestedHeight, Scalar color)
    {
        using var requestedArea = new Mat(target, new Rect(0, 0, requestedWidth, requestedHeight));
        requestedArea.SetTo(color);
    }

    private static void VerifyPreCanceledWritePreservesOldFile(string directory, string imagePath)
    {
        string outputPath = Path.Combine(directory, "取消保护.mp4");
        byte[] oldContent = [17, 29, 43, 71, 113];
        File.WriteAllBytes(outputPath, oldContent);

        var options = new StillVideoEncodingOptions
        {
            Items = [new StillVideoItemSettings { ImagePath = imagePath, DurationSeconds = 1d }],
            OutputPath = outputPath,
            OutputWidth = 8,
            OutputHeight = 6,
            FramesPerSecond = 10,
            OutputFormat = StillVideoFormat.Mp4Mpeg4
        };
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        try
        {
            StillVideoEncoder.EncodeAsync(options, cancellationToken: cancellation.Token)
                .GetAwaiter()
                .GetResult();
            throw new InvalidOperationException("预先取消的编码任务没有报告取消。");
        }
        catch (OperationCanceledException)
        {
            // 预期路径。
        }

        if (!File.ReadAllBytes(outputPath).SequenceEqual(oldContent))
        {
            throw new InvalidOperationException("取消编码修改了原有输出文件。");
        }
    }

    private static void WriteSolidPng(string path, int width, int height, Scalar color)
    {
        using var image = new Mat(height, width, MatType.CV_8UC3, color);
        if (!Cv2.ImEncode(".png", image, out byte[] bytes) || bytes.Length == 0)
        {
            throw new InvalidOperationException("无法创建视频回归验证用的 PNG 图片。");
        }

        // .NET 负责 Unicode 路径，避免原生 imwrite 的系统代码页差异。
        File.WriteAllBytes(path, bytes);
    }

    private static void AssertPixel(Mat image, int x, int y, Scalar expected, string description)
    {
        Vec3b actual = image.At<Vec3b>(y, x);
        if (actual.Item0 != (byte)expected.Val0 ||
            actual.Item1 != (byte)expected.Val1 ||
            actual.Item2 != (byte)expected.Val2)
        {
            throw new InvalidOperationException(
                $"{description}像素为 BGR({actual.Item0},{actual.Item1},{actual.Item2})，" +
                $"期望 BGR({expected.Val0},{expected.Val1},{expected.Val2})。");
        }
    }

    private static void ValidateTemporaryDirectory(
        string temporaryRoot,
        string temporaryDirectory,
        string uniqueDirectoryName)
    {
        string expectedParent = temporaryRoot.TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar);
        if (!string.Equals(Path.GetDirectoryName(temporaryDirectory), expectedParent, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(Path.GetFileName(temporaryDirectory), uniqueDirectoryName, StringComparison.Ordinal) ||
            !uniqueDirectoryName.StartsWith("EolVideoVerifier_中文_", StringComparison.Ordinal) ||
            uniqueDirectoryName.Length != "EolVideoVerifier_中文_".Length + 32)
        {
            throw new InvalidOperationException("无法确认视频回归验证临时目录的安全范围。");
        }
    }
}
