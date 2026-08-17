namespace EolTestPatternGenerator.Projection;

/// <summary>后台实时投图失败事件参数。</summary>
public sealed class ProjectionFailedEventArgs : EventArgs
{
    public ProjectionFailedEventArgs(long frameSequence, Exception exception)
    {
        FrameSequence = frameSequence;
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    /// <summary>失败帧的单调递增序号；0 表示恢复原壁纸等非帧操作。</summary>
    public long FrameSequence { get; }

    public Exception Exception { get; }
}
