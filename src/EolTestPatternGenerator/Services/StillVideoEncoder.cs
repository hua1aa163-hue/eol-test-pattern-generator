using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>界面显示和编码器打开所需的视频格式元数据。</summary>
public sealed record StillVideoFormatProfile(
    StillVideoFormat Format,
    string DisplayName,
    string Extension,
    string FourCc,
    bool IsLossless,
    bool RequiresEvenDimensions);

/// <summary>
/// 当前 OpenCvSharp Windows 运行时已经过写入、回读和无损像素验证的格式目录。
/// </summary>
public static class StillVideoFormatCatalog
{
    private static readonly IReadOnlyList<StillVideoFormatProfile> Profiles =
    [
        new(
            StillVideoFormat.Mp4Mpeg4,
            "MP4 / MPEG-4（mp4v，有损）",
            ".mp4",
            "mp4v",
            IsLossless: false,
            RequiresEvenDimensions: true),
        new(
            StillVideoFormat.MkvFfv1,
            "MKV / FFV1（无损，推荐）",
            ".mkv",
            "FFV1",
            IsLossless: true,
            RequiresEvenDimensions: true),
        new(
            StillVideoFormat.AviHuffyuv,
            "AVI / HuffYUV（无损）",
            ".avi",
            "HFYU",
            IsLossless: true,
            RequiresEvenDimensions: true)
    ];

    public static IReadOnlyList<StillVideoFormatProfile> All => Profiles;

    public static StillVideoFormatProfile Get(StillVideoFormat format)
    {
        return Profiles.FirstOrDefault(profile => profile.Format == format)
            ?? throw new ArgumentOutOfRangeException(nameof(format), format, "未知的视频格式。");
    }
}

/// <summary>编码进度；CompletedFrames 与 TotalFrames 均按真正写入的视频帧计数。</summary>
public sealed record StillVideoEncodingProgress(
    int ItemIndex,
    int ItemCount,
    long CompletedFrames,
    long TotalFrames,
    string ImagePath);

/// <summary>成功编码并通过回读后的结果。</summary>
public sealed record StillVideoEncodingResult(
    string OutputPath,
    int Width,
    int Height,
    int FramesPerSecond,
    long FrameCount,
    TimeSpan Duration,
    StillVideoFormatProfile Format);

/// <summary>
/// 将多张图片编码为静止帧序列。编码始终先写到输出目录中的唯一临时文件，
/// 回读验证成功后才原子替换目标，因此取消或失败不会破坏已有视频。
/// </summary>
public static class StillVideoEncoder
{
    public static Task<StillVideoEncodingResult> EncodeAsync(
        StillVideoEncodingOptions options,
        IProgress<StillVideoEncodingProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        return Task.Run(() => Encode(options, progress, cancellationToken), cancellationToken);
    }

    /// <summary>按帧率把秒数转换为帧数，使用四舍五入并确保非零时长至少一帧。</summary>
    public static int CalculateFrameCount(double durationSeconds, int framesPerSecond)
    {
        long frameCount = CalculateFrameSchedule([durationSeconds], framesPerSecond)[0];
        if (frameCount > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(durationSeconds), "单张图片的播放时间过长。");
        }

        return checked((int)frameCount);
    }

    /// <summary>
    /// 按累计时间终点分配每一项的帧数，避免分别四舍五入造成长序列漂移。
    /// 每个正时长项目至少得到一帧；极短项目因此领先累计终点时，后续较长项目会自动补偿。
    /// </summary>
    public static IReadOnlyList<long> CalculateFrameSchedule(
        IReadOnlyList<double> durationSeconds,
        int framesPerSecond)
    {
        ArgumentNullException.ThrowIfNull(durationSeconds);
        if (framesPerSecond is < 1 or > 240)
        {
            throw new ArgumentOutOfRangeException(nameof(framesPerSecond), "帧率必须在 1 到 240 FPS 之间。");
        }

        var frameCounts = new long[durationSeconds.Count];
        double cumulativeSeconds = 0d;
        double compensation = 0d;
        long totalWrittenFrames = 0;
        for (int index = 0; index < durationSeconds.Count; index++)
        {
            double duration = durationSeconds[index];
            if (!double.IsFinite(duration) || duration <= 0d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(durationSeconds),
                    $"第 {index + 1} 项的播放时间必须大于 0 秒。");
            }

            // Kahan 求和减少大量小数时长累加时的浮点漂移。
            double adjustedDuration = duration - compensation;
            double nextCumulative = cumulativeSeconds + adjustedDuration;
            compensation = (nextCumulative - cumulativeSeconds) - adjustedDuration;
            cumulativeSeconds = nextCumulative;

            double exactCumulativeFrames = cumulativeSeconds * framesPerSecond;
            if (!double.IsFinite(exactCumulativeFrames) || exactCumulativeFrames >= long.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(durationSeconds), "视频累计播放时间过长。");
            }

            long roundedEndFrame = checked((long)Math.Round(
                exactCumulativeFrames,
                MidpointRounding.AwayFromZero));
            long minimumEndFrame = checked(totalWrittenFrames + 1L);
            long adjustedEndFrame = Math.Max(roundedEndFrame, minimumEndFrame);
            frameCounts[index] = adjustedEndFrame - totalWrittenFrames;
            totalWrittenFrames = adjustedEndFrame;
        }

        return frameCounts;
    }

    /// <summary>
    /// 返回编码器实际尺寸。mp4v 使用的色度采样要求偶数尺寸，奇数输入会仅在右侧或底部补一列黑色像素。
    /// 当前 FFmpeg 后端对三种已验证编码器都会截断奇数宽高，因此统一补到偶数尺寸。
    /// </summary>
    public static System.Drawing.Size ResolveEncodedSize(
        int requestedWidth,
        int requestedHeight,
        StillVideoFormat format)
    {
        ValidateDimension(requestedWidth, nameof(requestedWidth));
        ValidateDimension(requestedHeight, nameof(requestedHeight));

        StillVideoFormatProfile profile = StillVideoFormatCatalog.Get(format);
        int width = profile.RequiresEvenDimensions && (requestedWidth & 1) != 0
            ? checked(requestedWidth + 1)
            : requestedWidth;
        int height = profile.RequiresEvenDimensions && (requestedHeight & 1) != 0
            ? checked(requestedHeight + 1)
            : requestedHeight;
        return new System.Drawing.Size(width, height);
    }

    /// <summary>
    /// 等比缩放图片并在黑色画布中居中。返回的新 Mat 由调用方释放；源 Mat 不会被修改。
    /// requestedWidth/requestedHeight 是用户画布，encodedWidth/encodedHeight 可包含编码兼容补边。
    /// </summary>
    public static Mat FitImageToCanvas(
        Mat source,
        int requestedWidth,
        int requestedHeight,
        int encodedWidth,
        int encodedHeight)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source.Empty() || source.Type() != MatType.CV_8UC3)
        {
            throw new InvalidDataException("视频源图片必须是非空的 8 位三通道图像。");
        }

        ValidateDimension(requestedWidth, nameof(requestedWidth));
        ValidateDimension(requestedHeight, nameof(requestedHeight));
        if (encodedWidth < requestedWidth || encodedHeight < requestedHeight)
        {
            throw new ArgumentOutOfRangeException(
                nameof(encodedWidth),
                "编码尺寸不能小于用户请求的画布尺寸。");
        }

        double scale = Math.Min(
            requestedWidth / (double)source.Cols,
            requestedHeight / (double)source.Rows);
        int fittedWidth = Math.Clamp(
            (int)Math.Round(source.Cols * scale, MidpointRounding.AwayFromZero),
            1,
            requestedWidth);
        int fittedHeight = Math.Clamp(
            (int)Math.Round(source.Rows * scale, MidpointRounding.AwayFromZero),
            1,
            requestedHeight);

        // 图片相对于用户请求画布居中；编码器所需的额外一列/行只出现在右侧/底部。
        int left = (requestedWidth - fittedWidth) / 2;
        int top = (requestedHeight - fittedHeight) / 2;
        var canvas = new Mat(encodedHeight, encodedWidth, MatType.CV_8UC3, Scalar.Black);

        try
        {
            using var resized = new Mat();
            if (fittedWidth == source.Cols && fittedHeight == source.Rows)
            {
                source.CopyTo(resized);
            }
            else
            {
                InterpolationFlags interpolation = scale < 1d
                    ? InterpolationFlags.Area
                    : InterpolationFlags.Linear;
                Cv2.Resize(source, resized, new OpenCvSharp.Size(fittedWidth, fittedHeight), 0d, 0d, interpolation);
            }

            using var targetRegion = new Mat(canvas, new Rect(left, top, fittedWidth, fittedHeight));
            resized.CopyTo(targetRegion);
            return canvas;
        }
        catch
        {
            canvas.Dispose();
            throw;
        }
    }

    private static StillVideoEncodingResult Encode(
        StillVideoEncodingOptions options,
        IProgress<StillVideoEncodingProgress>? progress,
        CancellationToken cancellationToken)
    {
        ValidatedEncodingPlan plan = ValidateAndCreatePlan(options);
        cancellationToken.ThrowIfCancellationRequested();

        string? outputDirectory = Path.GetDirectoryName(plan.OutputPath);
        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            throw new InvalidOperationException("输出视频路径没有有效目录。");
        }

        Directory.CreateDirectory(outputDirectory);
        string temporaryPath = CreateTemporaryPath(plan.OutputPath, plan.Profile.Extension);
        bool committed = false;

        try
        {
            EncodeTemporaryVideo(plan, temporaryPath, progress, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            VerifyEncodedVideo(temporaryPath, plan.EncodedWidth, plan.EncodedHeight, plan.TotalFrames);
            cancellationToken.ThrowIfCancellationRequested();
            CommitAtomically(temporaryPath, plan.OutputPath);
            committed = true;

            return new StillVideoEncodingResult(
                plan.OutputPath,
                plan.EncodedWidth,
                plan.EncodedHeight,
                plan.FramesPerSecond,
                plan.TotalFrames,
                TimeSpan.FromSeconds(plan.TotalFrames / (double)plan.FramesPerSecond),
                plan.Profile);
        }
        finally
        {
            if (!committed)
            {
                TryDeleteFile(temporaryPath);
            }
        }
    }

    private static void EncodeTemporaryVideo(
        ValidatedEncodingPlan plan,
        string temporaryPath,
        IProgress<StillVideoEncodingProgress>? progress,
        CancellationToken cancellationToken)
    {
        using var writer = new VideoWriter();
        var frameSize = new OpenCvSharp.Size(plan.EncodedWidth, plan.EncodedHeight);
        FourCC fourCc = FourCC.FromString(plan.Profile.FourCc);

        bool opened = writer.Open(
            temporaryPath,
            VideoCaptureAPIs.FFMPEG,
            fourCc,
            plan.FramesPerSecond,
            frameSize,
            isColor: true);
        if (!opened)
        {
            writer.Release();
            TryDeleteFile(temporaryPath);
            opened = writer.Open(
                temporaryPath,
                fourCc,
                plan.FramesPerSecond,
                frameSize,
                isColor: true);
        }

        if (!opened || !writer.IsOpened())
        {
            throw new InvalidOperationException(
                $"当前 OpenCV 运行时无法打开 {plan.Profile.DisplayName} 编码器（FourCC={plan.Profile.FourCc}）。");
        }

        long completedFrames = 0;
        long reportInterval = Math.Max(1L, plan.TotalFrames / 500L);
        for (int itemIndex = 0; itemIndex < plan.Items.Count; itemIndex++)
        {
            PlannedItem item = plan.Items[itemIndex];
            cancellationToken.ThrowIfCancellationRequested();

            using Mat source = UnicodeImageLoader.LoadColor(item.ImagePath);
            using Mat frame = FitImageToCanvas(
                source,
                plan.RequestedWidth,
                plan.RequestedHeight,
                plan.EncodedWidth,
                plan.EncodedHeight);

            for (long frameIndex = 0; frameIndex < item.FrameCount; frameIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                writer.Write(frame);
                completedFrames++;

                if (completedFrames == plan.TotalFrames || completedFrames % reportInterval == 0)
                {
                    progress?.Report(new StillVideoEncodingProgress(
                        itemIndex,
                        plan.Items.Count,
                        completedFrames,
                        plan.TotalFrames,
                        item.ImagePath));
                }
            }
        }

        // 显式 Release 可确保 FFmpeg 在进入回读验证前写完索引和容器尾部。
        writer.Release();
    }

    private static void VerifyEncodedVideo(
        string path,
        int expectedWidth,
        int expectedHeight,
        long expectedFrameCount)
    {
        var fileInfo = new FileInfo(path);
        if (!fileInfo.Exists || fileInfo.Length == 0)
        {
            throw new InvalidDataException("视频编码器没有生成有效文件。");
        }

        using var capture = new VideoCapture(path, VideoCaptureAPIs.FFMPEG);
        if (!capture.IsOpened())
        {
            capture.Open(path);
        }

        if (!capture.IsOpened())
        {
            throw new InvalidDataException("生成的视频无法由 OpenCV 回读。");
        }

        int actualWidth = checked((int)Math.Round((double)capture.FrameWidth));
        int actualHeight = checked((int)Math.Round((double)capture.FrameHeight));
        long reportedFrames = checked((long)Math.Round((double)capture.FrameCount));
        if (actualWidth != expectedWidth || actualHeight != expectedHeight)
        {
            throw new InvalidDataException(
                $"视频回读尺寸为 {actualWidth}×{actualHeight}，期望 {expectedWidth}×{expectedHeight}。");
        }

        if (reportedFrames != expectedFrameCount)
        {
            throw new InvalidDataException(
                $"视频回读帧数为 {reportedFrames}，期望 {expectedFrameCount}。");
        }

        using var firstFrame = new Mat();
        if (!capture.Set(VideoCaptureProperties.PosFrames, 0d) || !capture.Read(firstFrame) || firstFrame.Empty())
        {
            throw new InvalidDataException("生成的视频第一帧无法读取。");
        }

        using var lastFrame = new Mat();
        if (!capture.Set(VideoCaptureProperties.PosFrames, expectedFrameCount - 1d) ||
            !capture.Read(lastFrame) ||
            lastFrame.Empty())
        {
            throw new InvalidDataException("生成的视频最后一帧无法读取。");
        }
    }

    private static ValidatedEncodingPlan ValidateAndCreatePlan(StillVideoEncodingOptions options)
    {
        if (options.Items is null || options.Items.Count == 0)
        {
            throw new ArgumentException("请至少添加一张图片。", nameof(options));
        }

        ValidateDimension(options.OutputWidth, nameof(options.OutputWidth));
        ValidateDimension(options.OutputHeight, nameof(options.OutputHeight));
        if (options.FramesPerSecond is < 1 or > 240)
        {
            throw new ArgumentOutOfRangeException(nameof(options.FramesPerSecond), "帧率必须在 1 到 240 FPS 之间。");
        }

        StillVideoFormatProfile profile = StillVideoFormatCatalog.Get(options.OutputFormat);
        string outputPath = Path.GetFullPath(options.OutputPath);
        if (!string.Equals(Path.GetExtension(outputPath), profile.Extension, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                $"{profile.DisplayName} 的输出文件必须使用 {profile.Extension} 扩展名。",
                nameof(options.OutputPath));
        }

        var sourceItems = new List<(string ImagePath, double DurationSeconds)>(options.Items.Count);
        foreach (StillVideoItemSettings sourceItem in options.Items)
        {
            if (sourceItem is null || string.IsNullOrWhiteSpace(sourceItem.ImagePath))
            {
                throw new ArgumentException("图片列表中存在空路径。", nameof(options.Items));
            }

            string imagePath = Path.GetFullPath(sourceItem.ImagePath);
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("找不到视频源图片。", imagePath);
            }

            sourceItems.Add((imagePath, sourceItem.DurationSeconds));
        }

        IReadOnlyList<long> frameCounts = CalculateFrameSchedule(
            sourceItems.Select(item => item.DurationSeconds).ToArray(),
            options.FramesPerSecond);
        var plannedItems = new List<PlannedItem>(sourceItems.Count);
        long totalFrames = 0;
        for (int index = 0; index < sourceItems.Count; index++)
        {
            (string imagePath, double durationSeconds) = sourceItems[index];
            long frameCount = frameCounts[index];
            totalFrames = checked(totalFrames + frameCount);
            plannedItems.Add(new PlannedItem(imagePath, durationSeconds, frameCount));
        }

        System.Drawing.Size encodedSize = ResolveEncodedSize(
            options.OutputWidth,
            options.OutputHeight,
            options.OutputFormat);
        return new ValidatedEncodingPlan(
            outputPath,
            options.OutputWidth,
            options.OutputHeight,
            encodedSize.Width,
            encodedSize.Height,
            options.FramesPerSecond,
            totalFrames,
            profile,
            plannedItems);
    }

    private static void ValidateDimension(int value, string parameterName)
    {
        if (value is < 1 or > 32766)
        {
            throw new ArgumentOutOfRangeException(parameterName, "视频尺寸必须在 1 到 32766 像素之间。");
        }
    }

    private static string CreateTemporaryPath(string outputPath, string extension)
    {
        string directory = Path.GetDirectoryName(outputPath)
            ?? throw new InvalidOperationException("输出视频路径没有有效目录。");
        string stem = Path.GetFileNameWithoutExtension(outputPath);
        return Path.Combine(
            directory,
            $".{stem}.{Environment.ProcessId}.{Guid.NewGuid():N}.tmp{extension}");
    }

    private static void CommitAtomically(string temporaryPath, string outputPath)
    {
        if (File.Exists(outputPath))
        {
            try
            {
                File.Replace(temporaryPath, outputPath, destinationBackupFileName: null, ignoreMetadataErrors: true);
            }
            catch (Exception exception) when (exception is IOException or PlatformNotSupportedException)
            {
                File.Move(temporaryPath, outputPath, overwrite: true);
            }
        }
        else
        {
            File.Move(temporaryPath, outputPath);
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
        catch
        {
            // 清理临时文件失败不能覆盖原始编码异常或取消状态。
        }
    }

    private sealed record PlannedItem(string ImagePath, double DurationSeconds, long FrameCount);

    private sealed record ValidatedEncodingPlan(
        string OutputPath,
        int RequestedWidth,
        int RequestedHeight,
        int EncodedWidth,
        int EncodedHeight,
        int FramesPerSecond,
        long TotalFrames,
        StillVideoFormatProfile Profile,
        IReadOnlyList<PlannedItem> Items);
}
