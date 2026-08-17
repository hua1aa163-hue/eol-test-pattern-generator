using System.Runtime.InteropServices;

namespace EolTestPatternGenerator.Services;

/// <summary>
/// 统一检查 TIFF 页数。OpenCV 的单图解码入口会只返回多页 TIFF 的其中一页，
/// 批处理必须在解码前明确拒绝，避免静默丢页。
/// </summary>
internal static class TiffPageValidator
{
    public static void EnsureSinglePage(string path, bool isTiff)
    {
        if (!isTiff)
        {
            return;
        }

        string fullPath = Path.GetFullPath(path);
        try
        {
            // 检查期间不允许修改或删除源文件，避免读到不稳定的页数。
            using var stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            using Image image = Image.FromStream(
                stream,
                useEmbeddedColorManagement: false,
                validateImageData: false);
            Guid[] dimensions = image.FrameDimensionsList;
            if (dimensions.Length == 0)
            {
                throw new InvalidDataException($"无法确认 TIFF 页数，因此未处理：{fullPath}");
            }

            int frameCount = dimensions.Max(dimension =>
                image.GetFrameCount(new System.Drawing.Imaging.FrameDimension(dimension)));
            if (frameCount > 1)
            {
                throw new InvalidDataException(
                    $"多页 TIFF（{frameCount} 页）不能按单张图片处理；" +
                    $"请先拆分为单页图片：{fullPath}");
            }
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (Exception exception) when (
            exception is ArgumentException or ExternalException or IOException)
        {
            throw new InvalidDataException(
                $"无法可靠确认 TIFF 是否为单页，因此未处理：{fullPath}",
                exception);
        }
    }
}
