namespace EolTestPatternGenerator.Models;

/// <summary>
/// 静止图片序列可用的视频编码组合。枚举值同时决定封装格式、扩展名和 FourCC，
/// 不能只根据扩展名推断编码器。
/// </summary>
public enum StillVideoFormat
{
    /// <summary>MP4 容器中的 MPEG-4 Part 2（mp4v），兼容性好但有损。</summary>
    Mp4Mpeg4,

    /// <summary>Matroska 容器中的 FFV1，无损且适合长期保存测试图卡。</summary>
    MkvFfv1,

    /// <summary>AVI 容器中的 HuffYUV，无损但文件通常比 FFV1 大。</summary>
    AviHuffyuv
}
