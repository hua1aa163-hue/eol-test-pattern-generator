using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

/// <summary>对“二合一（水平复制）”执行不依赖文件系统的像素级验证。</summary>
public static class HorizontalImageComposerVerifier
{
    public static void Run()
    {
        using var source = new Mat(2, 3, MatType.CV_8UC3, Scalar.Black);
        source.Set(0, 0, new Vec3b(1, 2, 3));
        source.Set(0, 1, new Vec3b(4, 5, 6));
        source.Set(0, 2, new Vec3b(7, 8, 9));
        source.Set(1, 0, new Vec3b(10, 11, 12));
        source.Set(1, 1, new Vec3b(13, 14, 15));
        source.Set(1, 2, new Vec3b(16, 17, 18));

        using Mat duplicated = HorizontalImageComposer.DuplicateToRight(source);
        if (duplicated.Rows != source.Rows || duplicated.Cols != source.Cols * 2 ||
            duplicated.Type() != source.Type())
        {
            throw new InvalidOperationException("二合一没有生成预期的 2W × H 三通道图像。");
        }

        int rows = source.Rows;
        int columns = source.Cols;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vec3b expected = source.At<Vec3b>(y, x);
                if (duplicated.At<Vec3b>(y, x) != expected ||
                    duplicated.At<Vec3b>(y, x + columns) != expected)
                {
                    throw new InvalidOperationException($"二合一在 ({x}, {y}) 的左右像素不一致。");
                }
            }
        }

        AssertRejected(
            () => HorizontalImageComposer.ValidateAndGetOutputWidth(1, int.MaxValue, 3),
            "宽度整数溢出未被拒绝");
        AssertRejected(
            () => HorizontalImageComposer.ValidateAndGetOutputWidth(
                HorizontalImageComposer.MaximumOutputDimension,
                HorizontalImageComposer.MaximumOutputDimension / 2,
                3),
            "超大内存分配未被拒绝");
    }

    private static void AssertRejected(Action operation, string message)
    {
        try
        {
            operation();
        }
        catch (ArgumentOutOfRangeException)
        {
            return;
        }

        throw new InvalidOperationException(message + "。");
    }
}
