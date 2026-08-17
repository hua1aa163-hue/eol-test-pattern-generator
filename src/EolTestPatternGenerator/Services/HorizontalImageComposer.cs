using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 对已经完整生成的图像执行水平组合。这里在图卡及白框全部绘制完成后复制，
/// 因而左右两半逐像素完全相同。
/// </summary>
public static class HorizontalImageComposer
{
    /// <summary>限制单次组合结果，避免异常参数令预览或导出耗尽进程内存。</summary>
    // 预览还会同时持有同尺寸 Bitmap；限制输出 Mat 为 128 MiB，把常见的
    // Mat + 新旧预览峰值控制在约 384 MiB 内，避免大画布二合一导致进程 OOM。
    public const long MaximumOutputBytes = 128L * 1024L * 1024L;

    /// <summary>GDI+ 预览和常见编码器都能稳定处理的保守单边尺寸上限。</summary>
    public const int MaximumOutputDimension = 32_768;

    /// <summary>
    /// 将源图像复制为左右相邻的两份。调用方仍拥有 <paramref name="source"/>，
    /// 返回的新 <see cref="Mat"/> 由调用方负责释放。
    /// </summary>
    public static Mat DuplicateToRight(Mat source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.Empty())
        {
            throw new ArgumentException("二合一的源图像不能为空。", nameof(source));
        }

        int outputWidth = ValidateAndGetOutputWidth(source.Rows, source.Cols, source.ElemSize());
        var output = new Mat(source.Rows, outputWidth, source.Type());

        try
        {
            using var left = new Mat(output, new Rect(0, 0, source.Cols, source.Rows));
            using var right = new Mat(output, new Rect(source.Cols, 0, source.Cols, source.Rows));
            source.CopyTo(left);
            source.CopyTo(right);
            return output;
        }
        catch
        {
            output.Dispose();
            throw;
        }
    }

    /// <summary>
    /// 在分配内存前校验目标尺寸，并返回加倍后的宽度。
    /// 独立方法也便于不创建超大图像即可验证边界条件。
    /// </summary>
    internal static int ValidateAndGetOutputWidth(int rows, int columns, long elementSizeBytes)
    {
        if (rows <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rows), rows, "图像高度必须大于 0。");
        }

        if (columns <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(columns), columns, "图像宽度必须大于 0。");
        }

        if (elementSizeBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(elementSizeBytes),
                elementSizeBytes,
                "像素字节数必须大于 0。");
        }

        int outputWidth;
        try
        {
            outputWidth = checked(columns * 2);
        }
        catch (OverflowException)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columns),
                columns,
                "二合一后的图像宽度超过支持范围。");
        }

        if (rows > MaximumOutputDimension || outputWidth > MaximumOutputDimension)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columns),
                $"二合一后的尺寸 {outputWidth:N0} × {rows:N0} 超过单边 " +
                $"{MaximumOutputDimension:N0} 像素的安全上限。");
        }

        long outputBytes;
        try
        {
            outputBytes = checked((long)rows * outputWidth * elementSizeBytes);
        }
        catch (OverflowException)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columns),
                columns,
                "二合一后的图像内存大小超过支持范围。");
        }

        if (outputBytes > MaximumOutputBytes)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columns),
                $"二合一后的图像预计占用 {outputBytes / (1024d * 1024d):N1} MiB，" +
                $"超过 {MaximumOutputBytes / (1024 * 1024):N0} MiB 的安全上限。请减小画布尺寸。");
        }

        return outputWidth;
    }
}
