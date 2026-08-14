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
    }

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
