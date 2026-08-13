using System.ComponentModel;

namespace EolTestPatternGenerator.Controls;

public partial class ImagePreviewControl : UserControl
{
    public ImagePreviewControl()
    {
        InitializeComponent();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float Zoom => previewSurface.Zoom;

    public void SetImage(Bitmap? image, bool preserveView = true)
    {
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

    private void previewSurface_ViewChanged(object? sender, EventArgs e)
    {
        UpdateZoomLabel();
    }

    private void UpdateZoomLabel()
    {
        labelZoom.Text = $"{Zoom * 100:0.0}%";
    }
}
