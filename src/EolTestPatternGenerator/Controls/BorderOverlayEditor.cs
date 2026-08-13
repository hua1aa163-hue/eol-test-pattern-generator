using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Controls;

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
        set => _canvasSize = value;
    }

    public BorderOverlaySettings GetSettings()
    {
        return new BorderOverlaySettings
        {
            Enabled = checkEnabled.Checked,
            X = (int)numericX.Value,
            Y = (int)numericY.Value,
            Width = (int)numericWidth.Value,
            Height = (int)numericHeight.Value,
            LineWidth = (int)numericLineWidth.Value
        };
    }

    public void SetSettings(BorderOverlaySettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _updating = true;
        try
        {
            checkEnabled.Checked = settings.Enabled;
            SetValue(numericX, settings.X);
            SetValue(numericY, settings.Y);
            SetValue(numericWidth, settings.Width);
            SetValue(numericHeight, settings.Height);
            SetValue(numericLineWidth, settings.LineWidth);
        }
        finally
        {
            _updating = false;
        }

        UpdateEnabledState();
    }

    private void ParameterChanged(object? sender, EventArgs e)
    {
        UpdateEnabledState();
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
            SetValue(numericX, (int)Math.Floor((_canvasSize.Width - (double)numericWidth.Value) / 2));
            SetValue(numericY, (int)Math.Floor((_canvasSize.Height - (double)numericHeight.Value) / 2));
        }
        finally
        {
            _updating = false;
        }

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
}
