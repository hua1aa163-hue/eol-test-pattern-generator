using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Controls;

/// <summary>
/// 可在WinForms设计器中复用的白框参数编辑器；只负责编辑配置，不直接绘图。
/// </summary>
public partial class BorderOverlayEditor : UserControl
{
    private bool _updating;
    private Size _canvasSize = new(1920, 1080);

    public BorderOverlayEditor()
    {
        InitializeComponent();
        UpdateEnabledState();
    }

    public event EventHandler? SettingsChanged;

    public Size CanvasSize
    {
        get => _canvasSize;
        set
        {
            _canvasSize = new Size(Math.Max(1, value.Width), Math.Max(1, value.Height));
            UpdateComputedSize();
        }
    }

    public BorderOverlaySettings GetSettings()
    {
        var settings = new BorderOverlaySettings
        {
            Enabled = checkEnabled.Checked,
            LineWidth = (int)numericLineWidth.Value
        };
        settings.SetMargins(new RegionMargins(
            (int)numericX.Value,
            (int)numericY.Value,
            (int)numericWidth.Value,
            (int)numericHeight.Value));
        return settings;
    }

    public void SetSettings(BorderOverlaySettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        // 批量回写控件时抑制事件，避免加载配置触发重复预览和递归保存。
        _updating = true;
        try
        {
            checkEnabled.Checked = settings.Enabled;
            RegionMargins margins = settings.GetMargins();
            SetValue(numericX, margins.Left);
            SetValue(numericY, margins.Top);
            SetValue(numericWidth, margins.Right);
            SetValue(numericHeight, margins.Bottom);
            SetValue(numericLineWidth, settings.LineWidth);
        }
        finally
        {
            _updating = false;
        }

        UpdateEnabledState();
        UpdateComputedSize();
    }

    private void ParameterChanged(object? sender, EventArgs e)
    {
        UpdateEnabledState();
        UpdateComputedSize();
        if (!_updating)
        {
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void buttonCenter_Click(object? sender, EventArgs e)
    {
        _updating = true;
        try
        {
            long width = (long)_canvasSize.Width - (int)numericX.Value - (int)numericWidth.Value;
            long height = (long)_canvasSize.Height - (int)numericY.Value - (int)numericHeight.Value;
            if (width <= 0 || height <= 0)
            {
                return;
            }

            int left = checked((int)(((long)_canvasSize.Width - width) / 2));
            int top = checked((int)(((long)_canvasSize.Height - height) / 2));
            SetValue(numericX, left);
            SetValue(numericY, top);
            SetValue(numericWidth, checked((int)((long)_canvasSize.Width - width - left)));
            SetValue(numericHeight, checked((int)((long)_canvasSize.Height - height - top)));
        }
        finally
        {
            _updating = false;
        }

        UpdateComputedSize();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateEnabledState()
    {
        bool enabled = checkEnabled.Checked;
        numericX.Enabled = enabled;
        numericY.Enabled = enabled;
        numericWidth.Enabled = enabled;
        numericHeight.Enabled = enabled;
        numericLineWidth.Enabled = enabled;
        buttonCenter.Enabled = enabled;
    }

    private static void SetValue(NumericUpDown control, int value)
    {
        control.Value = Math.Min(control.Maximum, Math.Max(control.Minimum, value));
    }

    private void UpdateComputedSize()
    {
        long width = (long)_canvasSize.Width - (int)numericX.Value - (int)numericWidth.Value;
        long height = (long)_canvasSize.Height - (int)numericY.Value - (int)numericHeight.Value;
        bool valid = width > 0 && height > 0;
        labelHelp.ForeColor = valid ? Color.DimGray : Color.Firebrick;
        labelHelp.Text = valid
            ? $"实时白框尺寸：{width:N0} × {height:N0} px\n四边距允许为负值，越界部分会裁剪。"
            : $"白框尺寸无效：{width:N0} × {height:N0} px";
    }
}
