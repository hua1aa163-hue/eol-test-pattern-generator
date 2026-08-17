namespace EolTestPatternGenerator.Projection;

/// <summary>一次实时投图请求的终态。</summary>
public enum ProjectionFrameResult
{
    /// <summary>该帧已经调用系统 API 成功设置为桌面壁纸。</summary>
    Applied,

    /// <summary>系统调用前已有更新的待投帧，因此该帧被合并丢弃。</summary>
    Superseded,

    /// <summary>控制器停止，尚未应用的帧被取消。</summary>
    Stopped
}
