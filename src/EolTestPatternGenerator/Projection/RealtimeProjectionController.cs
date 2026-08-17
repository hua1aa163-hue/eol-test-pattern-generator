namespace EolTestPatternGenerator.Projection;

/// <summary>
/// 将快速到达的位图合并为“只保留最新待投帧”，并在唯一后台工作线程中串行调用
/// <see cref="IDesktopDisplayService.SetWallpaper"/>。控制器会克隆传入位图，调用方仍拥有原对象。
/// </summary>
public sealed class RealtimeProjectionController : IDisposable, IAsyncDisposable
{
    private readonly object _sync = new();
    private readonly IDesktopDisplayService _displayService;
    private readonly OwnedWallpaperFrameCache _frameCache;
    private readonly SemaphoreSlim _frameSignal = new(0);
    private readonly bool _restoreOriginalOnDispose;

    private CancellationTokenSource? _workerCancellation;
    private Task? _workerTask;
    private PendingFrame? _pendingFrame;
    private Task? _stopTask;
    private Task? _disposeTask;
    private bool _restoreRequested;
    private bool _isRunning;
    private bool _isStopping;
    private bool _disposeStarted;
    private bool _isDisposed;
    private bool _originalCaptured;
    private long _nextSequence;
    private string? _originalWallpaper;
    private string? _lastProjectedPath;
    private Exception? _lastError;

    public RealtimeProjectionController(
        IDesktopDisplayService displayService,
        bool restoreOriginalOnDispose = true,
        string? frameCacheRoot = null)
    {
        _displayService = displayService ?? throw new ArgumentNullException(nameof(displayService));
        _restoreOriginalOnDispose = restoreOriginalOnDispose;
        _frameCache = new OwnedWallpaperFrameCache(frameCacheRoot);
    }

    /// <summary>后台工作循环是否接受新的投图帧。</summary>
    public bool IsRunning
    {
        get
        {
            lock (_sync)
            {
                return _isRunning;
            }
        }
    }

    public bool HasOriginalWallpaper
    {
        get
        {
            lock (_sync)
            {
                // 空字符串是 Windows“没有壁纸”的有效快照，也必须可以恢复。
                return _originalCaptured;
            }
        }
    }

    /// <summary>最近一次成功设置的控制器自有 BMP 路径。</summary>
    public string? LastProjectedWallpaperPath
    {
        get
        {
            lock (_sync)
            {
                return _lastProjectedPath;
            }
        }
    }

    /// <summary>最近一次后台投图或恢复错误；下一次成功投图后清空。</summary>
    public Exception? LastError
    {
        get
        {
            lock (_sync)
            {
                return _lastError;
            }
        }
    }

    /// <summary>后台帧失败时触发；事件在线程池线程上调用，界面订阅者必须自行切回 UI 线程。</summary>
    public event EventHandler<ProjectionFailedEventArgs>? ProjectionFailed;

    /// <summary>
    /// 保存首次读取到的原壁纸路径。该快照在成功恢复前保持不变，避免暂停后再次启动时
    /// 把程序自己的投图误当成“原壁纸”。只能在控制器未运行时调用。
    /// </summary>
    public string? CaptureOriginal()
    {
        lock (_sync)
        {
            ThrowIfUnavailable();
            if (_isRunning || _isStopping)
            {
                throw new InvalidOperationException("请在启动实时投图前保存原壁纸。");
            }

            if (_originalCaptured)
            {
                return _originalWallpaper;
            }

            // 在生命周期锁内完成同步读取，避免 Start(false)/Dispose 与快照采集交叉，
            // 把程序刚投出的帧误保存成原壁纸。
            string? captured = _displayService.GetCurrentWallpaper();
            if (captured is not null)
            {
                _originalWallpaper = captured;
                _originalCaptured = true;
            }

            return captured;
        }
    }

    /// <summary>启动后台投图循环；重复调用不会创建第二个工作线程。</summary>
    public void Start(bool captureOriginal = true)
    {
        lock (_sync)
        {
            ThrowIfUnavailable();
            if (_isStopping)
            {
                throw new InvalidOperationException("实时投图正在停止，请等待停止完成后再启动。");
            }

            if (_isRunning)
            {
                return;
            }

            if (captureOriginal && !_originalCaptured)
            {
                string? captured = _displayService.GetCurrentWallpaper();
                if (captured is null)
                {
                    throw new InvalidOperationException(
                        "无法读取当前桌面壁纸，实时投图尚未启动；可检查系统权限后重试，" +
                        "或明确使用 Start(false) 放弃自动恢复。");
                }

                _originalWallpaper = captured;
                _originalCaptured = true;
            }

            _workerCancellation?.Dispose();
            _workerCancellation = new CancellationTokenSource();
            CancellationToken token = _workerCancellation.Token;
            _isRunning = true;
            _restoreRequested = false;
            _stopTask = null;
            _workerTask = Task.Run(() => WorkerLoopAsync(token), CancellationToken.None);
        }
    }

    /// <summary>
    /// 提交最新位图。尚未进入系统调用的旧待投帧会立即返回
    /// <see cref="ProjectionFrameResult.Superseded"/>；系统调用始终串行，因此旧调用不会在
    /// 新调用之后完成并覆盖新壁纸。
    /// </summary>
    public Task<ProjectionFrameResult> ProjectLatest(Bitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        // 克隆后调用方可以立即复用或 Dispose 原位图；实际 24 位转换在后台完成。
        Bitmap ownedBitmap = (Bitmap)bitmap.Clone();
        PendingFrame? superseded;
        PendingFrame frame;
        lock (_sync)
        {
            try
            {
                ThrowIfUnavailable();
                if (!_isRunning || _isStopping)
                {
                    throw new InvalidOperationException("实时投图尚未启动或正在停止。");
                }

                frame = new PendingFrame(checked(++_nextSequence), ownedBitmap);
                superseded = _pendingFrame;
                _pendingFrame = frame;
                // 一个待处理槽只需要一个唤醒令牌；替换尚未消费的帧时复用已有令牌，
                // 避免高频预览在信号量中留下大量无效计数。
                if (superseded is null)
                {
                    // Release 与入队位于同一生命周期锁内，Dispose 无法在两者之间销毁信号量。
                    _frameSignal.Release();
                }
            }
            catch
            {
                ownedBitmap.Dispose();
                throw;
            }
        }

        superseded?.Complete(ProjectionFrameResult.Superseded);
        return frame.Completion.Task;
    }

    /// <summary>
    /// 停止接收新帧并等待正在执行的系统调用退出。restoreOriginal=true 时尝试恢复首次快照；
    /// 多个并发停止请求会合并，任一调用要求恢复就会执行恢复。
    /// </summary>
    public Task StopAsync(bool restoreOriginal = true)
    {
        return RequestStop(restoreOriginal, allowDuringDispose: false);
    }

    private Task RequestStop(bool restoreOriginal, bool allowDuringDispose)
    {
        PendingFrame? pending = null;
        Task? worker = null;
        CancellationTokenSource? cancellation = null;
        TaskCompletionSource<bool>? completion = null;

        lock (_sync)
        {
            if (allowDuringDispose)
            {
                if (_isDisposed)
                {
                    return Task.CompletedTask;
                }
            }
            else
            {
                ThrowIfUnavailable();
            }

            _restoreRequested |= restoreOriginal;
            if (_isStopping)
            {
                return _stopTask ?? Task.CompletedTask;
            }

            _isStopping = true;
            _isRunning = false;
            pending = _pendingFrame;
            _pendingFrame = null;
            worker = _workerTask;
            cancellation = _workerCancellation;
            completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _stopTask = completion.Task;
        }

        pending?.Complete(ProjectionFrameResult.Stopped);
        cancellation?.Cancel();

        _ = Task.Run(() => CompleteStopAsync(worker, cancellation, completion!));
        return completion!.Task;
    }

    private async Task CompleteStopAsync(
        Task? worker,
        CancellationTokenSource? cancellation,
        TaskCompletionSource<bool> completion)
    {
        Exception? failure = null;
        bool restored = false;
        try
        {
            if (worker is not null)
            {
                try
                {
                    await worker.ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellation?.IsCancellationRequested == true)
                {
                    // 正常停止路径。
                }
            }

        }
        catch (Exception exception)
        {
            failure = exception;
            RecordFailure(0, exception);
        }
        finally
        {
            cancellation?.Dispose();

            // 停止状态保持到恢复决策和缓存清理全部结束。若并发 Stop(true) 在清理期间到达，
            // 下一轮会看到新请求并在完成同一个停止任务前补做恢复。
            while (true)
            {
                bool shouldRestore;
                lock (_sync)
                {
                    // 即使后台帧先失败，也仍要尽力恢复原壁纸；两类异常会分别记录并合并返回。
                    shouldRestore = _restoreRequested && !restored;
                    _restoreRequested = false;
                }

                if (shouldRestore)
                {
                    try
                    {
                        restored = RestoreOriginalCore();
                    }
                    catch (Exception exception)
                    {
                        failure = CombineFailures(failure, exception);
                        RecordFailure(0, exception);
                    }
                }

                string? retainedPath;
                lock (_sync)
                {
                    retainedPath = restored ? null : _lastProjectedPath;
                }

                try
                {
                    if (restored || retainedPath is null)
                    {
                        _frameCache.DeleteAll();
                    }
                    else
                    {
                        // 未恢复时保留 Windows 当前引用的一个 BMP，其余帧全部清理。
                        _frameCache.KeepOnly(retainedPath);
                    }
                }
                catch (Exception exception)
                {
                    failure = CombineFailures(failure, exception);
                    RecordFailure(0, exception);
                }

                lock (_sync)
                {
                    if (_restoreRequested && !restored)
                    {
                        continue;
                    }

                    _workerCancellation = null;
                    _workerTask = null;
                    _isStopping = false;
                    _restoreRequested = false;
                    break;
                }
            }
        }

        if (failure is null)
        {
            completion.TrySetResult(true);
        }
        else
        {
            completion.TrySetException(failure);
        }
    }

    private async Task WorkerLoopAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            await _frameSignal.WaitAsync(cancellationToken).ConfigureAwait(false);

            PendingFrame? frame;
            lock (_sync)
            {
                frame = _pendingFrame;
                _pendingFrame = null;
            }

            if (frame is null)
            {
                continue;
            }

            await ProcessFrameAsync(frame, cancellationToken).ConfigureAwait(false);
        }
    }

    private Task ProcessFrameAsync(PendingFrame frame, CancellationToken cancellationToken)
    {
        string? preparedPath = null;
        bool wallpaperApplied = false;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            preparedPath = _frameCache.Write(frame.Bitmap, frame.Sequence);

            // 转换期间若到达了更新帧，就不要再把已过期帧交给 Windows。
            lock (_sync)
            {
                if (_pendingFrame is not null && _pendingFrame.Sequence > frame.Sequence)
                {
                    _frameCache.Delete(preparedPath);
                    frame.Complete(ProjectionFrameResult.Superseded);
                    return Task.CompletedTask;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            _displayService.SetWallpaper(preparedPath);
            wallpaperApplied = true;

            string? previousPath;
            lock (_sync)
            {
                previousPath = _lastProjectedPath;
                _lastProjectedPath = preparedPath;
                _lastError = null;
            }

            // 新壁纸已成功提交后，旧源文件不再需要；缓存始终至多保留当前帧。
            if (!string.Equals(previousPath, preparedPath, StringComparison.OrdinalIgnoreCase))
            {
                _frameCache.KeepOnly(preparedPath);
            }

            // Stop 可能在同步系统调用期间到达，但 SetWallpaper 一旦成功返回，该帧就确实
            // 被应用过；Stopped 仅用于尚未提交给系统的待处理帧。
            frame.Complete(ProjectionFrameResult.Applied);
            return Task.CompletedTask;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            if (preparedPath is not null)
            {
                _frameCache.Delete(preparedPath);
            }

            frame.Complete(ProjectionFrameResult.Stopped);
            throw;
        }
        catch (Exception exception)
        {
            if (wallpaperApplied)
            {
                // 系统已引用 preparedPath，后续缓存整理失败时绝不能删除该文件或把成功帧
                // 伪装成失败；通过事件/LastError 单独报告清理问题。
                frame.Complete(ProjectionFrameResult.Applied);
                RecordFailure(frame.Sequence, exception);
                return Task.CompletedTask;
            }

            if (preparedPath is not null)
            {
                _frameCache.Delete(preparedPath);
            }

            frame.Fail(exception);
            RecordFailure(frame.Sequence, exception);
            return Task.CompletedTask;
        }
    }

    /// <summary>仅在后台工作线程彻底停止后调用，因此不会与 SetWallpaper 并发。</summary>
    private bool RestoreOriginalCore()
    {
        string? original;
        lock (_sync)
        {
            original = _originalWallpaper;
        }

        if (!_originalCaptured || original is null)
        {
            return false;
        }

        // 文件存在性由可注入的服务实现负责判断；测试替身可以使用逻辑路径。
        _displayService.SetWallpaper(original);
        lock (_sync)
        {
            _originalWallpaper = null;
            _originalCaptured = false;
            _lastProjectedPath = null;
            _lastError = null;
        }

        return true;
    }

    private void RecordFailure(long frameSequence, Exception exception)
    {
        lock (_sync)
        {
            _lastError = exception;
        }

        try
        {
            ProjectionFailed?.Invoke(this, new ProjectionFailedEventArgs(frameSequence, exception));
        }
        catch
        {
            // 订阅者异常不能终止实时投图工作线程或遮蔽原始异常。
        }
    }

    private static Exception CombineFailures(Exception? existing, Exception next)
    {
        return existing is null
            ? next
            : new AggregateException("停止实时投图时发生多个错误。", existing, next);
    }

    public void Dispose()
    {
        try
        {
            GetOrStartDisposeTask().GetAwaiter().GetResult();
        }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return new ValueTask(GetOrStartDisposeTask());
    }

    private Task GetOrStartDisposeTask()
    {
        TaskCompletionSource<bool> completion;
        lock (_sync)
        {
            if (_isDisposed)
            {
                return Task.CompletedTask;
            }

            if (_disposeTask is not null)
            {
                return _disposeTask;
            }

            // 在首次 Dispose 的同一把锁内封禁 Start/ProjectLatest/CaptureOriginal/公共 Stop。
            _disposeStarted = true;
            completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _disposeTask = completion.Task;
        }

        _ = Task.Run(() => CompleteDisposeAsync(completion));
        return completion.Task;
    }

    private async Task CompleteDisposeAsync(TaskCompletionSource<bool> completion)
    {
        Exception? failure = null;
        try
        {
            await RequestStop(_restoreOriginalOnDispose, allowDuringDispose: true).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            // 即使恢复原壁纸失败，也必须进入已释放终态。
            failure = exception;
        }
        finally
        {
            lock (_sync)
            {
                _isDisposed = true;
                _pendingFrame?.Complete(ProjectionFrameResult.Stopped);
                _pendingFrame = null;
            }

            _frameSignal.Dispose();
        }

        if (failure is null)
        {
            completion.TrySetResult(true);
        }
        else
        {
            completion.TrySetException(failure);
        }
    }

    private void ThrowIfUnavailable()
    {
        ObjectDisposedException.ThrowIf(_disposeStarted || _isDisposed, this);
    }

    private sealed class PendingFrame
    {
        private int _completed;

        public PendingFrame(long sequence, Bitmap bitmap)
        {
            Sequence = sequence;
            Bitmap = bitmap;
            Completion = new TaskCompletionSource<ProjectionFrameResult>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        }

        public long Sequence { get; }

        public Bitmap Bitmap { get; }

        public TaskCompletionSource<ProjectionFrameResult> Completion { get; }

        public void Complete(ProjectionFrameResult result)
        {
            if (Interlocked.Exchange(ref _completed, 1) != 0)
            {
                return;
            }

            Bitmap.Dispose();
            Completion.TrySetResult(result);
        }

        public void Fail(Exception exception)
        {
            if (Interlocked.Exchange(ref _completed, 1) != 0)
            {
                return;
            }

            Bitmap.Dispose();
            Completion.TrySetException(exception);
        }
    }
}
