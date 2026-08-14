using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Controls;

/// <summary>
/// 用图案最外缘到画布四边的距离编辑一个矩形区域，并实时显示派生宽高。
/// </summary>
public partial class RegionMarginsEditor : UserControl
{
    private bool _updating;
    private Size _canvasSize = new(1920, 1080);

    public RegionMarginsEditor()
    {
        InitializeComponent();
        UpdateComputedSize();
    }

    public event EventHandler? MarginsChanged;

    public Size CanvasSize
    {
        get => _canvasSize;
        set
        {
            _canvasSize = new Size(Math.Max(1, value.Width), Math.Max(1, value.Height));
            UpdateComputedSize();
        }
    }

    public bool InputsEnabled
    {
        get => numericLeft.Enabled;
        set
        {
            numericLeft.Enabled = value;
            numericTop.Enabled = value;
            numericRight.Enabled = value;
            numericBottom.Enabled = value;
            buttonCenter.Enabled = value;
        }
    }

    public RegionMargins GetMargins()
    {
        return new RegionMargins(
            (int)numericLeft.Value,
            (int)numericTop.Value,
            (int)numericRight.Value,
            (int)numericBottom.Value);
    }

    public void SetMargins(RegionMargins margins)
    {
        ArgumentNullException.ThrowIfNull(margins);
        _updating = true;
        try
        {
            SetValue(numericLeft, margins.Left);
            SetValue(numericTop, margins.Top);
            SetValue(numericRight, margins.Right);
            SetValue(numericBottom, margins.Bottom);
        }
        finally
        {
            _updating = false;
        }

        UpdateComputedSize();
    }

    public (long Width, long Height) GetCalculatedSize()
    {
        RegionMargins margins = GetMargins();
        return (
            margins.CalculateWidth(_canvasSize.Width),
            margins.CalculateHeight(_canvasSize.Height));
    }

    public void CenterPreservingSize()
    {
        (long width, long height) = GetCalculatedSize();
        if (width <= 0 || height <= 0 || width > int.MaxValue || height > int.MaxValue)
        {
            return;
        }

        long left = ((long)_canvasSize.Width - width) / 2L;
        long top = ((long)_canvasSize.Height - height) / 2L;
        long right = (long)_canvasSize.Width - width - left;
        long bottom = (long)_canvasSize.Height - height - top;

        _updating = true;
        try
        {
            SetValue(numericLeft, CheckedInt(left));
            SetValue(numericTop, CheckedInt(top));
            SetValue(numericRight, CheckedInt(right));
            SetValue(numericBottom, CheckedInt(bottom));
        }
        finally
        {
            _updating = false;
        }

        UpdateComputedSize();
        MarginsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void numericMargin_ValueChanged(object? sender, EventArgs e)
    {
        UpdateComputedSize();
        if (!_updating)
        {
            MarginsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void buttonCenter_Click(object? sender, EventArgs e)
    {
        CenterPreservingSize();
    }

    private void UpdateComputedSize()
    {
        (long width, long height) = GetCalculatedSize();
        bool valid = width > 0 && height > 0 && width <= int.MaxValue && height <= int.MaxValue;
        labelComputedSize.ForeColor = valid ? Color.DimGray : Color.Firebrick;
        labelComputedSize.Text = valid
            ? $"实时图案尺寸：{width:N0} × {height:N0} px"
            : $"实时图案尺寸无效：{width:N0} × {height:N0} px";
    }

    private static void SetValue(NumericUpDown control, int value)
    {
        control.Value = Math.Min(control.Maximum, Math.Max(control.Minimum, value));
    }

    private static int CheckedInt(long value)
    {
        if (value is < int.MinValue or > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "居中后的边距超出整数范围。");
        }

        return (int)value;
    }
}
