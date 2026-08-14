using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace EolTestPatternGenerator.Controls;

/// <summary>
/// 图像预览画布。控件拥有传入的 <see cref="Bitmap"/>，替换图片或释放控件时会一并释放。
/// </summary>
[DesignerCategory("Code")]
public sealed class PreviewSurface : Control
{
    private const float MinimumZoom = 0.01F;
    private const float MaximumZoom = 128F;
    private const int FitPadding = 24;
    private const int CoordinateBadgePadding = 5;
    private const int CoordinateBadgeOffset = 14;

    private Bitmap? _image;
    private float _zoom = 1F;
    private PointF _viewCenterImage;
    private bool _autoFit = true;
    private bool _dragging;
    private Point _lastMouse;
    private Point _mouseClientPosition;
    private bool _hasMousePosition;
    private bool _showCenterCrosshair = true;
    private bool _showPixelCoordinates = true;
    private Point? _mousePixel;

    public PreviewSurface()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.Selectable,
            true);
        TabStop = true;
        Cursor = Cursors.Default;
    }

    public event EventHandler? ViewChanged;

    /// <summary>
    /// 鼠标所在的原图像素发生变化时触发；鼠标位于图片外时值为 <see langword="null"/>。
    /// </summary>
    public event EventHandler? MousePixelChanged;

    [Browsable(false)]
    public float Zoom => _zoom;

    [Browsable(false)]
    public Point? MousePixel => _mousePixel;

    [Category("辅助显示")]
    [DefaultValue(true)]
    [Description("在原图几何中心显示水平和垂直十字虚线。")]
    public bool ShowCenterCrosshair
    {
        get => _showCenterCrosshair;
        set
        {
            if (_showCenterCrosshair == value)
            {
                return;
            }

            _showCenterCrosshair = value;
            Invalidate();
        }
    }

    [Category("辅助显示")]
    [DefaultValue(true)]
    [Description("在鼠标附近显示以图片左上角为原点的像素坐标。")]
    public bool ShowPixelCoordinates
    {
        get => _showPixelCoordinates;
        set
        {
            if (_showPixelCoordinates == value)
            {
                return;
            }

            _showPixelCoordinates = value;
            RecalculateMousePixel();
            Invalidate();
        }
    }

    public void SetImage(Bitmap? image, bool preserveView)
    {
        if (ReferenceEquals(_image, image))
        {
            if (preserveView)
            {
                Invalidate();
            }
            else
            {
                FitToWindow();
            }

            return;
        }

        // SetImage 会接管新位图的所有权；旧位图在替换后立即释放。
        Size oldSize = _image?.Size ?? Size.Empty;
        Bitmap? oldImage = _image;
        _image = image;
        oldImage?.Dispose();

        if (_image is null)
        {
            _zoom = 1F;
            _viewCenterImage = PointF.Empty;
            _autoFit = true;
            SetMousePixel(null);
        }
        else if (!preserveView || oldSize != _image.Size || oldSize.IsEmpty)
        {
            FitToWindow();
            return;
        }

        RecalculateMousePixel();
        Invalidate();
        OnViewChanged();
    }

    public void FitToWindow()
    {
        if (_image is null || ClientSize.Width <= 0 || ClientSize.Height <= 0)
        {
            return;
        }

        float availableWidth = Math.Max(1, ClientSize.Width - (2 * FitPadding));
        float availableHeight = Math.Max(1, ClientSize.Height - (2 * FitPadding));
        _zoom = Math.Clamp(
            Math.Min(availableWidth / _image.Width, availableHeight / _image.Height),
            MinimumZoom,
            MaximumZoom);
        _viewCenterImage = new PointF(_image.Width / 2F, _image.Height / 2F);
        _autoFit = true;
        RecalculateMousePixel();
        Invalidate();
        OnViewChanged();
    }

    public void ShowActualSize()
    {
        if (_image is null)
        {
            return;
        }

        _zoom = 1F;
        _viewCenterImage = new PointF(_image.Width / 2F, _image.Height / 2F);
        _autoFit = false;
        RecalculateMousePixel();
        Invalidate();
        OnViewChanged();
    }

    public void ResetView()
    {
        FitToWindow();
    }

    /// <summary>
    /// 将当前预览客户区位置换算成原图的零基像素坐标；图片外返回空值。
    /// </summary>
    internal Point? GetImagePixelAtClient(Point clientPoint)
    {
        if (_image is null)
        {
            return null;
        }

        return PreviewCoordinateTransform.TryGetImagePixel(
            clientPoint,
            ClientSize,
            _viewCenterImage,
            _zoom,
            _image.Size);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(BackColor);

        if (_image is null)
        {
            return;
        }

        RectangleF destination = GetImageRectangle();
        e.Graphics.CompositingMode = CompositingMode.SourceCopy;
        e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
        e.Graphics.SmoothingMode = SmoothingMode.None;
        e.Graphics.DrawImage(
            _image,
            destination,
            new RectangleF(0, 0, _image.Width, _image.Height),
            GraphicsUnit.Pixel);

        e.Graphics.CompositingMode = CompositingMode.SourceOver;
        using var borderPen = new Pen(Color.FromArgb(105, 112, 122), 1F);
        e.Graphics.DrawRectangle(borderPen, destination.X, destination.Y, destination.Width, destination.Height);

        if (_showCenterCrosshair)
        {
            DrawCenterCrosshair(e.Graphics, destination);
        }

        if (_showPixelCoordinates && _mousePixel.HasValue && _hasMousePosition)
        {
            DrawPixelCoordinate(e.Graphics, destination, _mousePixel.Value);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();
        if (e.Button != MouseButtons.Left || _image is null)
        {
            return;
        }

        _dragging = true;
        _lastMouse = e.Location;
        Capture = true;
        Cursor = Cursors.Hand;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (_dragging && _image is not null)
        {
            int dx = e.X - _lastMouse.X;
            int dy = e.Y - _lastMouse.Y;
            // 图像中心与鼠标移动方向相反更新，视觉效果就是“抓住图片拖动”。
            _viewCenterImage = new PointF(
                _viewCenterImage.X - (dx / _zoom),
                _viewCenterImage.Y - (dy / _zoom));
            _lastMouse = e.Location;
            _autoFit = false;
            OnViewChanged();
        }

        _mouseClientPosition = e.Location;
        // 捕获拖动时，即使指针已移出控件仍会收到 MouseMove；区外不应继续显示坐标。
        _hasMousePosition = ClientRectangle.Contains(e.Location);
        RecalculateMousePixel();
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _mouseClientPosition = PointToClient(MousePosition);
        _hasMousePosition = ClientRectangle.Contains(_mouseClientPosition);
        RecalculateMousePixel();
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hasMousePosition = false;
        SetMousePixel(null);
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button == MouseButtons.Left)
        {
            EndDrag();
        }
    }

    protected override void OnMouseCaptureChanged(EventArgs e)
    {
        base.OnMouseCaptureChanged(e);
        if (!Capture && _dragging)
        {
            EndDrag();
        }
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        if (_image is null || e.Delta == 0)
        {
            return;
        }

        Focus();
        // 先记录指针下的图像坐标，再反算新中心，使缩放锚点固定在鼠标位置。
        PointF anchorImage = ClientToImage(e.Location);
        float factor = (float)Math.Pow(1.2, e.Delta / 120.0);
        float newZoom = Math.Clamp(_zoom * factor, MinimumZoom, MaximumZoom);
        if (Math.Abs(newZoom - _zoom) < 0.000001F)
        {
            return;
        }

        PointF clientCenter = GetClientCenter();
        _zoom = newZoom;
        _viewCenterImage = new PointF(
            anchorImage.X - ((e.X - clientCenter.X) / _zoom),
            anchorImage.Y - ((e.Y - clientCenter.Y) / _zoom));
        _autoFit = false;
        _mouseClientPosition = e.Location;
        _hasMousePosition = ClientRectangle.Contains(e.Location);
        RecalculateMousePixel();
        Invalidate();
        OnViewChanged();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (_autoFit)
        {
            FitToWindow();
        }
        else
        {
            RecalculateMousePixel();
            Invalidate();
        }
    }

    private RectangleF GetImageRectangle()
    {
        if (_image is null)
        {
            return RectangleF.Empty;
        }

        PointF center = GetClientCenter();
        return new RectangleF(
            center.X - (_viewCenterImage.X * _zoom),
            center.Y - (_viewCenterImage.Y * _zoom),
            _image.Width * _zoom,
            _image.Height * _zoom);
    }

    private PointF ClientToImage(Point point)
    {
        return PreviewCoordinateTransform.ClientToImage(
            point,
            ClientSize,
            _viewCenterImage,
            _zoom);
    }

    private PointF GetClientCenter()
    {
        return new PointF(ClientSize.Width / 2F, ClientSize.Height / 2F);
    }

    private void EndDrag()
    {
        _dragging = false;
        if (Capture)
        {
            Capture = false;
        }

        Cursor = Cursors.Default;

        // 松开捕获后重新读取实际指针位置，避免区外释放时残留最后一个像素坐标。
        _mouseClientPosition = PointToClient(MousePosition);
        _hasMousePosition = ClientRectangle.Contains(_mouseClientPosition);
        RecalculateMousePixel();
        Invalidate();
    }

    private void OnViewChanged()
    {
        ViewChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RecalculateMousePixel()
    {
        if (!_showPixelCoordinates ||
            !_hasMousePosition ||
            !ClientRectangle.Contains(_mouseClientPosition) ||
            _image is null)
        {
            SetMousePixel(null);
            return;
        }

        SetMousePixel(GetImagePixelAtClient(_mouseClientPosition));
    }

    private void SetMousePixel(Point? value)
    {
        if (_mousePixel == value)
        {
            return;
        }

        _mousePixel = value;
        MousePixelChanged?.Invoke(this, EventArgs.Empty);
    }

    private void DrawCenterCrosshair(Graphics graphics, RectangleF destination)
    {
        RectangleF visibleImage = GetVisibleImageRectangle(destination);
        if (visibleImage.Width <= 0F || visibleImage.Height <= 0F)
        {
            return;
        }

        PointF imageCenter = PreviewCoordinateTransform.ImageToClient(
            new PointF(_image!.Width / 2F, _image.Height / 2F),
            ClientSize,
            _viewCenterImage,
            _zoom);

        GraphicsState state = graphics.Save();
        try
        {
            graphics.SetClip(visibleImage);
            graphics.SmoothingMode = SmoothingMode.None;

            // 两条相同节距的黑白虚线叠加，确保在纯黑和纯白图卡上都清晰可见。
            using var darkPen = new Pen(Color.FromArgb(220, 0, 0, 0), 3F)
            {
                DashStyle = DashStyle.Custom,
                DashPattern = [2F, 2F]
            };
            using var lightPen = new Pen(Color.FromArgb(245, 255, 255, 255), 1F)
            {
                DashStyle = DashStyle.Custom,
                DashPattern = [6F, 6F]
            };

            DrawCrosshairLines(graphics, darkPen, destination, imageCenter);
            DrawCrosshairLines(graphics, lightPen, destination, imageCenter);
        }
        finally
        {
            graphics.Restore(state);
        }
    }

    private void DrawPixelCoordinate(Graphics graphics, RectangleF destination, Point pixel)
    {
        string text = $"X={pixel.X}, Y={pixel.Y}";
        const TextFormatFlags flags = TextFormatFlags.NoPadding |
                                      TextFormatFlags.NoPrefix |
                                      TextFormatFlags.SingleLine;
        Size textSize = TextRenderer.MeasureText(text, Font, Size.Empty, flags);
        var badgeSize = new Size(
            textSize.Width + (CoordinateBadgePadding * 2),
            textSize.Height + (CoordinateBadgePadding * 2));

        RectangleF visibleImage = GetVisibleImageRectangle(destination);
        Rectangle bounds = Rectangle.Ceiling(visibleImage);
        if (bounds.Width < badgeSize.Width || bounds.Height < badgeSize.Height)
        {
            bounds = ClientRectangle;
        }

        int x = _mouseClientPosition.X + CoordinateBadgeOffset;
        int y = _mouseClientPosition.Y + CoordinateBadgeOffset;
        if (x + badgeSize.Width > bounds.Right)
        {
            x = _mouseClientPosition.X - badgeSize.Width - CoordinateBadgeOffset;
        }

        if (y + badgeSize.Height > bounds.Bottom)
        {
            y = _mouseClientPosition.Y - badgeSize.Height - CoordinateBadgeOffset;
        }

        x = Math.Clamp(x, bounds.Left, Math.Max(bounds.Left, bounds.Right - badgeSize.Width));
        y = Math.Clamp(y, bounds.Top, Math.Max(bounds.Top, bounds.Bottom - badgeSize.Height));

        var badge = new Rectangle(new Point(x, y), badgeSize);
        using var backgroundBrush = new SolidBrush(Color.FromArgb(215, 25, 28, 34));
        using var borderPen = new Pen(Color.FromArgb(235, 255, 255, 255), 1F);
        graphics.FillRectangle(backgroundBrush, badge);
        graphics.DrawRectangle(borderPen, badge.X, badge.Y, badge.Width - 1, badge.Height - 1);

        var textBounds = new Rectangle(
            badge.X + CoordinateBadgePadding,
            badge.Y + CoordinateBadgePadding,
            textSize.Width,
            textSize.Height);
        TextRenderer.DrawText(graphics, text, Font, textBounds, Color.White, flags);
    }

    private RectangleF GetVisibleImageRectangle(RectangleF destination)
    {
        var client = new RectangleF(0F, 0F, ClientSize.Width, ClientSize.Height);
        return RectangleF.Intersect(destination, client);
    }

    private static void DrawCrosshairLines(
        Graphics graphics,
        Pen pen,
        RectangleF destination,
        PointF imageCenter)
    {
        graphics.DrawLine(pen, destination.Left, imageCenter.Y, destination.Right, imageCenter.Y);
        graphics.DrawLine(pen, imageCenter.X, destination.Top, imageCenter.X, destination.Bottom);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Bitmap? image = _image;
            _image = null;
            image?.Dispose();
        }

        base.Dispose(disposing);
    }
}

/// <summary>
/// 预览客户区和原图坐标之间的纯数学换算；绘制、缩放和坐标显示共用同一套公式。
/// </summary>
internal static class PreviewCoordinateTransform
{
    internal static PointF ClientToImage(
        PointF clientPoint,
        Size clientSize,
        PointF viewCenterImage,
        float zoom)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(zoom);
        var clientCenter = new PointF(clientSize.Width / 2F, clientSize.Height / 2F);
        return new PointF(
            viewCenterImage.X + ((clientPoint.X - clientCenter.X) / zoom),
            viewCenterImage.Y + ((clientPoint.Y - clientCenter.Y) / zoom));
    }

    internal static PointF ImageToClient(
        PointF imagePoint,
        Size clientSize,
        PointF viewCenterImage,
        float zoom)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(zoom);
        var clientCenter = new PointF(clientSize.Width / 2F, clientSize.Height / 2F);
        return new PointF(
            clientCenter.X + ((imagePoint.X - viewCenterImage.X) * zoom),
            clientCenter.Y + ((imagePoint.Y - viewCenterImage.Y) * zoom));
    }

    internal static Point? TryGetImagePixel(
        Point clientPoint,
        Size clientSize,
        PointF viewCenterImage,
        float zoom,
        Size imageSize)
    {
        PointF imagePoint = ClientToImage(clientPoint, clientSize, viewCenterImage, zoom);
        if (imagePoint.X < 0F || imagePoint.Y < 0F ||
            imagePoint.X >= imageSize.Width || imagePoint.Y >= imageSize.Height)
        {
            return null;
        }

        // 像素区域采用左闭右开区间，因此必须向下取整，不能四舍五入。
        return new Point((int)MathF.Floor(imagePoint.X), (int)MathF.Floor(imagePoint.Y));
    }
}
