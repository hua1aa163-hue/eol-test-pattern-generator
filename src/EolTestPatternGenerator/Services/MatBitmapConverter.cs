using System.Drawing.Imaging;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

public static class MatBitmapConverter
{
    public static Bitmap ToPreviewBitmap(Mat source, int maximumWidth = 2048, int maximumHeight = 1200)
    {
        ArgumentNullException.ThrowIfNull(source);

        double scale = Math.Min(
            1.0,
            Math.Min(maximumWidth / (double)source.Cols, maximumHeight / (double)source.Rows));

        if (scale >= 1.0)
        {
            return ToBitmap(source);
        }

        int width = Math.Max(1, (int)Math.Round(source.Cols * scale));
        int height = Math.Max(1, (int)Math.Round(source.Rows * scale));
        using var preview = new Mat();
        Cv2.Resize(source, preview, new OpenCvSharp.Size(width, height), 0, 0, InterpolationFlags.Area);
        return ToBitmap(preview);
    }

    public static unsafe Bitmap ToBitmap(Mat source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.Type() != MatType.CV_8UC3)
        {
            throw new ArgumentException("预览转换只支持 CV_8UC3 图像。", nameof(source));
        }

        var bitmap = new Bitmap(source.Cols, source.Rows, PixelFormat.Format24bppRgb);
        var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        BitmapData? bitmapData = null;

        try
        {
            bitmapData = bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            int rows = source.Rows;
            int columns = source.Cols;
            int rowBytes = columns * 3;

            for (int y = 0; y < rows; y++)
            {
                byte* sourceRow = (byte*)source.Ptr(y);
                byte* destinationRow = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                Buffer.MemoryCopy(sourceRow, destinationRow, Math.Abs(bitmapData.Stride), rowBytes);
            }

            return bitmap;
        }
        catch
        {
            bitmap.Dispose();
            throw;
        }
        finally
        {
            if (bitmapData is not null)
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
