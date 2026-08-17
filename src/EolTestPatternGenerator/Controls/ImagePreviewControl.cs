using System.ComponentModel;

namespace EolTestPatternGenerator.Controls;

/// <summary>
/// 可由WinForms设计器复用的预览组件，封装缩放工具栏和图像画布。
/// </summary>
public partial class ImagePreviewControl : UserControl
{
    public ImagePreviewControl()
    {
        InitializeComponent();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float Zoom => previewSurface.Zoom;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool HasImage => previewSurface.HasImage;

    /// <summary>
    /// 原始预览图片被替换或清空后触发。视图缩放、拖动及辅助线变化不会触发，
    /// 便于实时投图只处理真正变化的图卡内容。
    /// </summary>
    public event EventHandler? ImageChanged;

    [Category("辅助显示")]
    [DefaultValue(true)]
    [Description("在预览图片的几何中心显示十字虚线。")]
    public bool ShowCenterCrosshair
    {
        get => previewSurface.ShowCenterCrosshair;
        set
        {
            previewSurface.ShowCenterCrosshair = value;
            if (buttonCenterCrosshair.Checked != value)
            {
                buttonCenterCrosshair.Checked = value;
            }
        }
    }

    [Category("辅助显示")]
    [DefaultValue(true)]
    [Description("在预览图片上显示鼠标所在的原图像素坐标。")]
    public bool ShowPixelCoordinates
    {
        get => previewSurface.ShowPixelCoordinates;
        set
        {
            previewSurface.ShowPixelCoordinates = value;
            if (buttonPixelCoordinates.Checked != value)
            {
                buttonPixelCoordinates.Checked = value;
            }

            labelPixelCoordinate.Visible = value;
            UpdatePixelCoordinateLabel();
        }
    }

    public void SetImage(Bitmap? image, bool preserveView = true)
    {
        // 位图所有权转交给内部 PreviewSurface，调用方不应再次 Dispose。
        previewSurface.SetImage(image, preserveView);
        UpdateZoomLabel();
        ImageChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// 取得当前原始图卡的独立副本；不包含中心十字、像素坐标等界面叠加层。
    /// 返回的位图由调用方负责释放。
    /// </summary>
    public Bitmap? CloneCurrentImage() => previewSurface.CloneImage();

    public void FitToWindow()
    {
        previewSurface.FitToWindow();
    }

    public void ShowActualSize()
    {
        previewSurface.ShowActualSize();
    }

    public void ResetView()
    {
        previewSurface.ResetView();
    }

    private void buttonFit_Click(object? sender, EventArgs e)
    {
        FitToWindow();
    }

    private void buttonActualSize_Click(object? sender, EventArgs e)
    {
        ShowActualSize();
    }

    private void buttonResetView_Click(object? sender, EventArgs e)
    {
        ResetView();
    }

    private void buttonCenterCrosshair_CheckedChanged(object? sender, EventArgs e)
    {
        ShowCenterCrosshair = buttonCenterCrosshair.Checked;
    }

    private void buttonPixelCoordinates_CheckedChanged(object? sender, EventArgs e)
    {
        ShowPixelCoordinates = buttonPixelCoordinates.Checked;
    }

    private void previewSurface_ViewChanged(object? sender, EventArgs e)
    {
        UpdateZoomLabel();
    }

    private void previewSurface_MousePixelChanged(object? sender, EventArgs e)
    {
        UpdatePixelCoordinateLabel();
    }

    private void UpdateZoomLabel()
    {
        labelZoom.Text = $"{Zoom * 100:0.0}%";
    }

    private void UpdatePixelCoordinateLabel()
    {
        Point? pixel = previewSurface.MousePixel;
        labelPixelCoordinate.Text = ShowPixelCoordinates && pixel.HasValue
            ? $"像素：X={pixel.Value.X}, Y={pixel.Value.Y}"
            : "像素：—";
    }
}
