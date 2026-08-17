namespace EolTestPatternGenerator.Projection;

/// <summary>
/// 桌面投图控制器的无系统副作用回归验证。所有用例仅使用可注入替身，绝不调用 user32。
/// </summary>
internal static class ProjectionVerifier
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    /// <summary>供 Program 的 --self-test 入口同步调用。</summary>
    public static void RunAll()
    {
        RunAllAsync().GetAwaiter().GetResult();
    }

    private static async Task RunAllAsync()
    {
        await VerifyLatestFrameAndRestoreAsync().ConfigureAwait(false);
        await VerifyStopWithoutRestoreAsync().ConfigureAwait(false);
        await VerifyEmptyWallpaperRestoreAsync().ConfigureAwait(false);
        await VerifyCaptureFailureCanRetryAsync().ConfigureAwait(false);
        await VerifyFailurePropagationAsync().ConfigureAwait(false);
        await VerifyConcurrentStopAndDisposeAsync().ConfigureAwait(false);
    }

    /// <summary>读取失败的 null 不是有效快照，修复环境后再次 Start 必须重新采集。</summary>
    private static async Task VerifyCaptureFailureCanRetryAsync()
    {
        string testRoot = CreateTestRoot();
        var service = new FakeDesktopDisplayService(originalWallpaper: null);
        var controller = new RealtimeProjectionController(service, frameCacheRoot: testRoot);

        try
        {
            AssertThrows<InvalidOperationException>(
                () => controller.Start(),
                "读取原壁纸失败时 Start 应明确失败。");
            Assert(!controller.IsRunning, "原壁纸读取失败时不得留下运行中的工作循环。");
            Assert(!controller.HasOriginalWallpaper, "null 读取结果不得标记为已采集快照。");
            Assert(service.GetWallpaperCallCount == 1, "首次 Start 应读取一次原壁纸。");

            service.CurrentWallpaper = "fake-retry-original.bmp";
            controller.Start();
            Assert(service.GetWallpaperCallCount == 2, "读取失败后再次 Start 应允许重新采集。");
            await controller.StopAsync(restoreOriginal: true).WaitAsync(TestTimeout).ConfigureAwait(false);
            Assert(
                service.GetAppliedPaths()[^1] == "fake-retry-original.bmp",
                "重试采集成功后应能恢复新取得的原壁纸。");
            AssertTestRootEmpty(testRoot, "无投图帧的采集重试不应遗留缓存。");
        }
        finally
        {
            await DisposeIgnoringTestFailureAsync(controller).ConfigureAwait(false);
            DeleteTestRoot(testRoot);
        }
    }

    /// <summary>
    /// 阻塞第一帧的同步系统调用，再快速提交一组新帧，以确定只有第一帧和最后一帧会被应用。
    /// 同时在提交后立即修改、释放源位图，验证控制器持有的是自己的克隆。
    /// </summary>
    private static async Task VerifyLatestFrameAndRestoreAsync()
    {
        string testRoot = CreateTestRoot();
        var service = new FakeDesktopDisplayService("fake-original.bmp")
        {
            BlockFirstProjectedWallpaper = true
        };
        var controller = new RealtimeProjectionController(service, frameCacheRoot: testRoot);

        try
        {
            controller.Start();
            controller.Start();
            Assert(service.GetWallpaperCallCount == 1, "重复 Start 不应重复采集原壁纸。");

            var tasks = new List<Task<ProjectionFrameResult>>();
            using (var first = CreateSolidBitmap(Color.FromArgb(241, 17, 23)))
            {
                tasks.Add(controller.ProjectLatest(first));
                first.SetPixel(0, 0, Color.Black);
            }

            service.WaitForBlockedProjection();

            const int frameCount = 20;
            for (int index = 1; index < frameCount; index++)
            {
                Color color = index == frameCount - 1
                    ? Color.FromArgb(13, 29, 251)
                    : Color.FromArgb(index, 100, 200);
                using var source = CreateSolidBitmap(color);
                tasks.Add(controller.ProjectLatest(source));
                source.SetPixel(0, 0, Color.White);
            }

            service.ReleaseBlockedProjection();
            ProjectionFrameResult[] results = await Task.WhenAll(tasks)
                .WaitAsync(TestTimeout)
                .ConfigureAwait(false);

            Assert(results[0] == ProjectionFrameResult.Applied, "第一帧应在已进入系统调用后完成。");
            Assert(results[^1] == ProjectionFrameResult.Applied, "最后一帧应成为最终投图。");
            Assert(
                results.Skip(1).Take(frameCount - 2).All(result => result == ProjectionFrameResult.Superseded),
                "第一帧执行期间到达的中间帧应全部被最新帧合并。");

            FakeWallpaperCall[] projectedCalls = service.GetProjectedCalls();
            Assert(projectedCalls.Length == 2, "合并后系统服务应只收到第一帧和最后一帧。");
            Assert(
                projectedCalls[0].Pixel.ToArgb() == Color.FromArgb(241, 17, 23).ToArgb(),
                "控制器必须克隆第一帧，调用方修改/释放不能改变投图内容。");
            Assert(
                projectedCalls[1].Pixel.ToArgb() == Color.FromArgb(13, 29, 251).ToArgb(),
                "旧帧不得在最终帧之后覆盖桌面。");

            string retainedPath = controller.LastProjectedWallpaperPath
                ?? throw new InvalidOperationException("成功投图后缺少最后 BMP 路径。");
            Assert(File.Exists(retainedPath), "Windows 当前引用的最后 BMP 必须保留。");

            await controller.StopAsync(restoreOriginal: true)
                .WaitAsync(TestTimeout)
                .ConfigureAwait(false);
            Assert(service.GetAppliedPaths()[^1] == "fake-original.bmp", "停止时应恢复首次原壁纸路径。");
            Assert(!controller.HasOriginalWallpaper, "恢复成功后应清除已消费的原壁纸快照。");
            Assert(controller.LastProjectedWallpaperPath is null, "恢复成功后不应保留当前投图路径。");
            Assert(!File.Exists(retainedPath), "恢复原壁纸后应删除控制器自有 BMP。");
            AssertTestRootEmpty(testRoot, "恢复原壁纸后应清空会话缓存。");
        }
        finally
        {
            service.ReleaseBlockedProjection();
            await DisposeIgnoringTestFailureAsync(controller).ConfigureAwait(false);
            DeleteTestRoot(testRoot);
        }
    }

    /// <summary>Stop(false) 必须保留 Windows 正在引用的最后一帧，后续 Stop(true) 再清理。</summary>
    private static async Task VerifyStopWithoutRestoreAsync()
    {
        string testRoot = CreateTestRoot();
        var service = new FakeDesktopDisplayService("fake-stop-original.bmp");
        var controller = new RealtimeProjectionController(service, frameCacheRoot: testRoot);

        try
        {
            controller.Start();
            using var source = CreateSolidBitmap(Color.Gold);
            ProjectionFrameResult result = await controller.ProjectLatest(source)
                .WaitAsync(TestTimeout)
                .ConfigureAwait(false);
            Assert(result == ProjectionFrameResult.Applied, "Stop(false) 用例的测试帧应先成功应用。");

            string retainedPath = controller.LastProjectedWallpaperPath
                ?? throw new InvalidOperationException("Stop(false) 前缺少最后 BMP 路径。");
            await controller.StopAsync(restoreOriginal: false)
                .WaitAsync(TestTimeout)
                .ConfigureAwait(false);
            Assert(File.Exists(retainedPath), "Stop(false) 不得删除 Windows 当前引用的 BMP。");
            Assert(
                service.GetAppliedPaths()[^1] == retainedPath,
                "Stop(false) 不应额外调用服务恢复原壁纸。");
            Assert(controller.HasOriginalWallpaper, "未恢复时应继续保存首次原壁纸快照。");

            controller.Start();
            Assert(service.GetWallpaperCallCount == 1, "未恢复的再次启动不应把程序投图采集为原壁纸。");
            await controller.StopAsync(restoreOriginal: true)
                .WaitAsync(TestTimeout)
                .ConfigureAwait(false);
            Assert(service.GetAppliedPaths()[^1] == "fake-stop-original.bmp", "后续恢复请求应恢复原壁纸。");
            Assert(!File.Exists(retainedPath), "后续恢复成功后应清理先前保留的 BMP。");
            AssertTestRootEmpty(testRoot, "最终恢复后应清空 Stop(false) 保留的缓存。");
        }
        finally
        {
            await DisposeIgnoringTestFailureAsync(controller).ConfigureAwait(false);
            DeleteTestRoot(testRoot);
        }
    }

    /// <summary>Windows 返回空字符串表示原先没有壁纸，该有效快照也必须能被恢复。</summary>
    private static async Task VerifyEmptyWallpaperRestoreAsync()
    {
        string testRoot = CreateTestRoot();
        var service = new FakeDesktopDisplayService(string.Empty);
        var controller = new RealtimeProjectionController(service, frameCacheRoot: testRoot);

        try
        {
            controller.Start();
            Assert(controller.HasOriginalWallpaper, "空字符串仍是成功采集的原桌面状态。");
            using var source = CreateSolidBitmap(Color.CadetBlue);
            await controller.ProjectLatest(source).WaitAsync(TestTimeout).ConfigureAwait(false);
            await controller.StopAsync(restoreOriginal: true).WaitAsync(TestTimeout).ConfigureAwait(false);

            Assert(service.GetAppliedPaths()[^1].Length == 0, "没有壁纸的原桌面应以空字符串恢复。");
            Assert(!controller.HasOriginalWallpaper, "空壁纸恢复后也应清除快照状态。");
            AssertTestRootEmpty(testRoot, "空壁纸恢复后应清理会话 BMP。");
        }
        finally
        {
            await DisposeIgnoringTestFailureAsync(controller).ConfigureAwait(false);
            DeleteTestRoot(testRoot);
        }
    }

    /// <summary>单帧失败要同时传播给任务和事件，但不能终止后续帧的工作循环。</summary>
    private static async Task VerifyFailurePropagationAsync()
    {
        string testRoot = CreateTestRoot();
        var service = new FakeDesktopDisplayService("fake-failure-original.bmp")
        {
            NextProjectedFailure = new InvalidOperationException("fake projection failure")
        };
        var controller = new RealtimeProjectionController(service, frameCacheRoot: testRoot);
        var failureEvent = new TaskCompletionSource<ProjectionFailedEventArgs>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        int failureEventCount = 0;
        controller.ProjectionFailed += (_, args) =>
        {
            Interlocked.Increment(ref failureEventCount);
            failureEvent.TrySetResult(args);
        };

        try
        {
            controller.Start();
            InvalidOperationException projectedFailure;
            using (var failing = CreateSolidBitmap(Color.DarkRed))
            {
                projectedFailure = await AssertThrowsAsync<InvalidOperationException>(
                        controller.ProjectLatest(failing),
                        "系统服务失败应令对应帧任务失败。")
                    .ConfigureAwait(false);
            }

            ProjectionFailedEventArgs eventArgs = await failureEvent.Task
                .WaitAsync(TestTimeout)
                .ConfigureAwait(false);
            Assert(ReferenceEquals(eventArgs.Exception, projectedFailure), "失败任务和事件应暴露同一原始异常。");
            Assert(eventArgs.FrameSequence > 0, "帧失败事件应包含有效的单调序号。");
            Assert(ReferenceEquals(controller.LastError, projectedFailure), "LastError 应保留最近一次投图异常。");

            using var succeeding = CreateSolidBitmap(Color.DarkGreen);
            ProjectionFrameResult result = await controller.ProjectLatest(succeeding)
                .WaitAsync(TestTimeout)
                .ConfigureAwait(false);
            Assert(result == ProjectionFrameResult.Applied, "单帧失败后工作循环应继续接受新帧。");
            Assert(controller.LastError is null, "下一帧成功后应清除 LastError。");
            Assert(Volatile.Read(ref failureEventCount) == 1, "单帧失败事件不应重复触发。");

            await controller.StopAsync(restoreOriginal: true).WaitAsync(TestTimeout).ConfigureAwait(false);
            AssertTestRootEmpty(testRoot, "失败后恢复成功也应清空会话缓存。");
        }
        finally
        {
            await DisposeIgnoringTestFailureAsync(controller).ConfigureAwait(false);
            DeleteTestRoot(testRoot);
        }
    }

    /// <summary>既有 Stop(false) 与 Dispose(true) 并发时，恢复要求不能在停止收尾窗口丢失。</summary>
    private static async Task VerifyConcurrentStopAndDisposeAsync()
    {
        string testRoot = CreateTestRoot();
        var service = new FakeDesktopDisplayService("fake-dispose-original.bmp")
        {
            BlockFirstProjectedWallpaper = true
        };
        var controller = new RealtimeProjectionController(
            service,
            restoreOriginalOnDispose: true,
            frameCacheRoot: testRoot);

        try
        {
            controller.Start();
            using var source = CreateSolidBitmap(Color.MediumPurple);
            Task<ProjectionFrameResult> frameTask = controller.ProjectLatest(source);
            service.WaitForBlockedProjection();

            using var pendingSource = CreateSolidBitmap(Color.OrangeRed);
            Task<ProjectionFrameResult> pendingTask = controller.ProjectLatest(pendingSource);

            Task stopTask = controller.StopAsync(restoreOriginal: false);
            Task disposeTask = controller.DisposeAsync().AsTask();
            Task secondDisposeTask = controller.DisposeAsync().AsTask();
            Assert(ReferenceEquals(disposeTask, secondDisposeTask), "并发 DisposeAsync 应共享同一个终态任务。");
            AssertThrows<ObjectDisposedException>(
                () => controller.Start(),
                "DisposeAsync 一开始就应拒绝新的 Start。");
            using (var rejectedSource = CreateSolidBitmap(Color.Black))
            {
                AssertThrows<ObjectDisposedException>(
                    () => _ = controller.ProjectLatest(rejectedSource),
                    "DisposeAsync 一开始就应拒绝新的投图帧。");
            }

            service.ReleaseBlockedProjection();

            await Task.WhenAll(stopTask, disposeTask, secondDisposeTask)
                .WaitAsync(TestTimeout)
                .ConfigureAwait(false);
            ProjectionFrameResult frameResult = await frameTask.WaitAsync(TestTimeout).ConfigureAwait(false);
            ProjectionFrameResult pendingResult = await pendingTask.WaitAsync(TestTimeout).ConfigureAwait(false);
            Assert(frameResult == ProjectionFrameResult.Applied, "已进入且成功完成系统调用的帧应报告 Applied。");
            Assert(pendingResult == ProjectionFrameResult.Stopped, "尚未进入系统调用的待处理帧应报告 Stopped。");
            Assert(
                service.GetAppliedPaths()[^1] == "fake-dispose-original.bmp",
                "Dispose(true) 必须提升并发 Stop(false) 的恢复要求。");
            AssertTestRootEmpty(testRoot, "并发停止并恢复后应清空会话缓存。");

            AssertThrows<ObjectDisposedException>(
                () => controller.Start(),
                "Dispose 完成后不得重新启动控制器。");
        }
        finally
        {
            service.ReleaseBlockedProjection();
            await DisposeIgnoringTestFailureAsync(controller).ConfigureAwait(false);
            DeleteTestRoot(testRoot);
        }
    }

    private static Bitmap CreateSolidBitmap(Color color)
    {
        var bitmap = new Bitmap(3, 2);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(color);
        return bitmap;
    }

    private static string CreateTestRoot()
    {
        string root = Path.Combine(
            Path.GetTempPath(),
            "EolTestPatternGenerator",
            "ProjectionVerifier",
            $"run-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        return root;
    }

    private static void AssertTestRootEmpty(string root, string message)
    {
        if (Directory.Exists(root) && Directory.EnumerateFileSystemEntries(root).Any())
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void DeleteTestRoot(string root)
    {
        string fullRoot = Path.GetFullPath(root);
        string safeParent = Path.GetFullPath(Path.Combine(
            Path.GetTempPath(),
            "EolTestPatternGenerator",
            "ProjectionVerifier"));
        string expectedPrefix = Path.TrimEndingDirectorySeparator(safeParent) + Path.DirectorySeparatorChar;
        if (!fullRoot.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase)
            || !Path.GetFileName(fullRoot).StartsWith("run-", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("拒绝清理未经确认的投图回归验证临时目录。");
        }

        if (Directory.Exists(fullRoot))
        {
            Directory.Delete(fullRoot, recursive: true);
        }
    }

    private static async Task DisposeIgnoringTestFailureAsync(RealtimeProjectionController controller)
    {
        try
        {
            await controller.DisposeAsync().AsTask().WaitAsync(TestTimeout).ConfigureAwait(false);
        }
        catch
        {
            // 主断言异常应保留诊断优先级；正常路径会单独验证 Dispose/Stop 结果。
        }
    }

    private static async Task<TException> AssertThrowsAsync<TException>(Task task, string message)
        where TException : Exception
    {
        try
        {
            await task.WaitAsync(TestTimeout).ConfigureAwait(false);
        }
        catch (TException exception)
        {
            return exception;
        }

        throw new InvalidOperationException(message);
    }

    private static void AssertThrows<TException>(Action action, string message)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException(message);
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed class FakeDesktopDisplayService : IDesktopDisplayService
    {
        private readonly object _sync = new();
        private readonly List<FakeWallpaperCall> _calls = new();
        private readonly ManualResetEventSlim _blockedProjectionEntered = new(initialState: false);
        private readonly ManualResetEventSlim _releaseBlockedProjection = new(initialState: false);
        private int _getWallpaperCallCount;
        private int _projectedWallpaperCallCount;
        private string? _currentWallpaper;

        public FakeDesktopDisplayService(string? originalWallpaper)
        {
            _currentWallpaper = originalWallpaper;
        }

        public bool BlockFirstProjectedWallpaper { get; init; }

        public Exception? NextProjectedFailure { get; set; }

        public int GetWallpaperCallCount => Volatile.Read(ref _getWallpaperCallCount);

        public string? CurrentWallpaper
        {
            get
            {
                lock (_sync)
                {
                    return _currentWallpaper;
                }
            }
            set
            {
                lock (_sync)
                {
                    _currentWallpaper = value;
                }
            }
        }

        public string? GetCurrentWallpaper()
        {
            Interlocked.Increment(ref _getWallpaperCallCount);
            return CurrentWallpaper;
        }

        public void ApplyTopology(DisplayTopology topology)
        {
            // 本验证只测试实时壁纸控制器；接口保留空实现可保证不会触碰真实显示拓扑。
        }

        public void SetWallpaper(string imagePath)
        {
            ArgumentNullException.ThrowIfNull(imagePath);
            if (!File.Exists(imagePath))
            {
                lock (_sync)
                {
                    _calls.Add(new FakeWallpaperCall(imagePath, Color.Empty, IsProjected: false));
                }

                return;
            }

            int projectedCall = Interlocked.Increment(ref _projectedWallpaperCallCount);
            if (BlockFirstProjectedWallpaper && projectedCall == 1)
            {
                _blockedProjectionEntered.Set();
                if (!_releaseBlockedProjection.Wait(TestTimeout))
                {
                    throw new TimeoutException("等待释放投图替身超时。");
                }
            }

            Exception? failure;
            lock (_sync)
            {
                failure = NextProjectedFailure;
                NextProjectedFailure = null;
            }

            if (failure is not null)
            {
                throw failure;
            }

            using var bitmap = new Bitmap(imagePath);
            Color pixel = bitmap.GetPixel(0, 0);
            lock (_sync)
            {
                _calls.Add(new FakeWallpaperCall(imagePath, pixel, IsProjected: true));
            }
        }

        public void WaitForBlockedProjection()
        {
            if (!_blockedProjectionEntered.Wait(TestTimeout))
            {
                throw new TimeoutException("投图替身未在预期时间内进入第一帧。");
            }
        }

        public void ReleaseBlockedProjection()
        {
            _releaseBlockedProjection.Set();
        }

        public FakeWallpaperCall[] GetProjectedCalls()
        {
            lock (_sync)
            {
                return _calls.Where(call => call.IsProjected).ToArray();
            }
        }

        public string[] GetAppliedPaths()
        {
            lock (_sync)
            {
                return _calls.Select(call => call.Path).ToArray();
            }
        }
    }

    private sealed record FakeWallpaperCall(string Path, Color Pixel, bool IsProjected);
}
