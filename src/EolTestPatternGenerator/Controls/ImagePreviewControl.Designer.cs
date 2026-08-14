#nullable disable

namespace EolTestPatternGenerator.Controls;

partial class ImagePreviewControl
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Component Designer generated code

    private void InitializeComponent()
    {
        toolStrip = new ToolStrip();
        buttonFit = new ToolStripButton();
        buttonActualSize = new ToolStripButton();
        buttonResetView = new ToolStripButton();
        separatorView = new ToolStripSeparator();
        labelZoom = new ToolStripLabel();
        separatorOverlay = new ToolStripSeparator();
        buttonCenterCrosshair = new ToolStripButton();
        buttonPixelCoordinates = new ToolStripButton();
        labelPixelCoordinate = new ToolStripLabel();
        labelHint = new ToolStripLabel();
        previewSurface = new PreviewSurface();
        toolStrip.SuspendLayout();
        SuspendLayout();
        //
        // toolStrip
        //
        toolStrip.BackColor = Color.FromArgb(245, 247, 250);
        toolStrip.GripStyle = ToolStripGripStyle.Hidden;
        toolStrip.Items.AddRange(new ToolStripItem[] { buttonFit, buttonActualSize, buttonResetView, separatorView, labelZoom, separatorOverlay, buttonCenterCrosshair, buttonPixelCoordinates, labelPixelCoordinate, labelHint });
        toolStrip.Location = new Point(0, 0);
        toolStrip.Name = "toolStrip";
        toolStrip.Padding = new Padding(8, 2, 4, 2);
        toolStrip.Size = new Size(800, 29);
        toolStrip.TabIndex = 0;
        //
        // buttonFit
        //
        buttonFit.DisplayStyle = ToolStripItemDisplayStyle.Text;
        buttonFit.Name = "buttonFit";
        buttonFit.Size = new Size(60, 22);
        buttonFit.Text = "适合窗口";
        buttonFit.Click += buttonFit_Click;
        //
        // buttonActualSize
        //
        buttonActualSize.DisplayStyle = ToolStripItemDisplayStyle.Text;
        buttonActualSize.Name = "buttonActualSize";
        buttonActualSize.Size = new Size(42, 22);
        buttonActualSize.Text = "100%";
        buttonActualSize.Click += buttonActualSize_Click;
        //
        // buttonResetView
        //
        buttonResetView.DisplayStyle = ToolStripItemDisplayStyle.Text;
        buttonResetView.Name = "buttonResetView";
        buttonResetView.Size = new Size(60, 22);
        buttonResetView.Text = "重置视图";
        buttonResetView.Click += buttonResetView_Click;
        //
        // separatorView
        //
        separatorView.Name = "separatorView";
        separatorView.Size = new Size(6, 25);
        //
        // labelZoom
        //
        labelZoom.Name = "labelZoom";
        labelZoom.Size = new Size(45, 22);
        labelZoom.Text = "100.0%";
        //
        // separatorOverlay
        //
        separatorOverlay.Name = "separatorOverlay";
        separatorOverlay.Size = new Size(6, 25);
        //
        // buttonCenterCrosshair
        //
        buttonCenterCrosshair.Checked = true;
        buttonCenterCrosshair.CheckOnClick = true;
        buttonCenterCrosshair.CheckState = CheckState.Checked;
        buttonCenterCrosshair.DisplayStyle = ToolStripItemDisplayStyle.Text;
        buttonCenterCrosshair.Name = "buttonCenterCrosshair";
        buttonCenterCrosshair.Size = new Size(60, 22);
        buttonCenterCrosshair.Text = "中心十字";
        buttonCenterCrosshair.ToolTipText = "显示或隐藏原图中心的十字虚线";
        buttonCenterCrosshair.CheckedChanged += buttonCenterCrosshair_CheckedChanged;
        //
        // buttonPixelCoordinates
        //
        buttonPixelCoordinates.Checked = true;
        buttonPixelCoordinates.CheckOnClick = true;
        buttonPixelCoordinates.CheckState = CheckState.Checked;
        buttonPixelCoordinates.DisplayStyle = ToolStripItemDisplayStyle.Text;
        buttonPixelCoordinates.Name = "buttonPixelCoordinates";
        buttonPixelCoordinates.Size = new Size(60, 22);
        buttonPixelCoordinates.Text = "像素坐标";
        buttonPixelCoordinates.ToolTipText = "显示或隐藏鼠标所在的原图像素坐标";
        buttonPixelCoordinates.CheckedChanged += buttonPixelCoordinates_CheckedChanged;
        //
        // labelPixelCoordinate
        //
        labelPixelCoordinate.ForeColor = Color.FromArgb(45, 55, 72);
        labelPixelCoordinate.Name = "labelPixelCoordinate";
        labelPixelCoordinate.Size = new Size(56, 22);
        labelPixelCoordinate.Text = "像素：—";
        //
        // labelHint
        //
        labelHint.Alignment = ToolStripItemAlignment.Right;
        labelHint.ForeColor = Color.DimGray;
        labelHint.Name = "labelHint";
        labelHint.Size = new Size(176, 22);
        labelHint.Text = "单击后滚轮缩放 · 按住鼠标左键拖动";
        //
        // previewSurface
        //
        previewSurface.BackColor = Color.FromArgb(224, 228, 234);
        previewSurface.Dock = DockStyle.Fill;
        previewSurface.Location = new Point(0, 29);
        previewSurface.Name = "previewSurface";
        previewSurface.Size = new Size(800, 471);
        previewSurface.TabIndex = 1;
        previewSurface.MousePixelChanged += previewSurface_MousePixelChanged;
        previewSurface.ViewChanged += previewSurface_ViewChanged;
        //
        // ImagePreviewControl
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(224, 228, 234);
        Controls.Add(previewSurface);
        Controls.Add(toolStrip);
        Name = "ImagePreviewControl";
        Size = new Size(800, 500);
        toolStrip.ResumeLayout(false);
        toolStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private ToolStrip toolStrip;
    private ToolStripButton buttonFit;
    private ToolStripButton buttonActualSize;
    private ToolStripButton buttonResetView;
    private ToolStripSeparator separatorView;
    private ToolStripLabel labelZoom;
    private ToolStripSeparator separatorOverlay;
    private ToolStripButton buttonCenterCrosshair;
    private ToolStripButton buttonPixelCoordinates;
    private ToolStripLabel labelPixelCoordinate;
    private ToolStripLabel labelHint;
    private PreviewSurface previewSurface;
}
