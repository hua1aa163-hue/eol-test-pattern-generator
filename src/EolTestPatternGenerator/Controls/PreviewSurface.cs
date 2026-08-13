using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace EolTestPatternGenerator.Controls;

[DesignerCategory("Code")]
public sealed class PreviewSurface : Control
{
    private const float MinimumZoom = 0.01F;
    private const float MaximumZoom = 128F;
    private const int FitPadding = 24;

    private Bitmap? _image;
    private float _zoom = 1F;
    private PointF _viewCenterImage;
    private bool _autoFit = true;
    private bool _dragging;
    private Point _lastMouse;

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

    [Browsable(false)]
    public float Zoom => _zoom;

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

        Size oldSize = _image?.Size ?? Size.Empty;
        Bitmap? oldImage = _image;
        _image = image;
        oldImage?.Dispose();

        if (_image is null)
        {
            _zoom = 1F;
            _viewCenterImage = PointF.Empty;
            _autoFit = true;
        }
        else if (!preserveView || oldSize != _image.Size || oldSize.IsEmpty)
        {
            FitToWindow();
            return;
        }

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
        Invalidate();
        OnViewChanged();
    }

    public void ResetView()
    {
        FitToWindow();
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
        if (!_dragging || _image is null)
        {
            return;
        }

        int dx = e.X - _lastMouse.X;
        int dy = e.Y - _lastMouse.Y;
        _viewCenterImage = new PointF(
            _viewCenterImage.X - (dx / _zoom),
            _viewCenterImage.Y - (dy / _zoom));
        _lastMouse = e.Location;
        _autoFit = false;
        Invalidate();
        OnViewChanged();
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
        if (!Capture)
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
        PointF center = GetClientCenter();
        return new PointF(
            _viewCenterImage.X + ((point.X - center.X) / _zoom),
            _viewCenterImage.Y + ((point.Y - center.Y) / _zoom));
    }

    private PointF GetClientCenter()
    {
        return new PointF(ClientSize.Width / 2F, ClientSize.Height / 2F);
    }

    private void EndDrag()
    {
        _dragging = false;
        Capture = false;
        Cursor = Cursors.Default;
    }

    private void OnViewChanged()
    {
        ViewChanged?.Invoke(this, EventArgs.Empty);
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
