using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;

namespace EolTestPatternGenerator.Controls;

/// <summary>
/// 在设计器中可复用的固定 8 像素串扰预设编辑器。
/// 六种排列沿用历史 -3 横向步进，界面只允许调整排列和倾斜角。
/// </summary>
public partial class CrosstalkCycleEditor : UserControl
{
    private CrosstalkPixelCycle _cycle = CrosstalkPixelCyclePresets.CreateLegacy(RgbPixelOrder.RGB);
    private bool _updating;

    public CrosstalkCycleEditor()
    {
        InitializeComponent();
        RefreshControlsFromCycle();
    }

    public event EventHandler? SettingsChanged;

    public int PeriodLength => _cycle.PeriodLength;

    public CrosstalkPixelCycle GetCycle() => _cycle.Clone();

    public void SetCycle(CrosstalkPixelCycle? cycle)
    {
        // 旧配置可能保存了任意周期、逐位置通道或横向步进。精简界面不再暴露这些设置，
        // 因此加载时选取通道差异最小的六种固定预设，同时保留原倾斜角。
        _cycle = CrosstalkPixelCyclePresets.NormalizeToNearestLegacyPreset(cycle);
        RefreshControlsFromCycle();
    }

    public void LoadLegacyPreset(RgbPixelOrder order)
    {
        _cycle = CrosstalkPixelCyclePresets.CreateLegacy(order);
        RefreshControlsFromCycle();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void numericTiltAngle_ValueChanged(object? sender, EventArgs e)
    {
        if (_updating)
        {
            return;
        }

        _cycle.SetTiltAngleDegrees((double)numericTiltAngle.Value);
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void comboLegacyPreset_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_updating || comboLegacyPreset.SelectedIndex is < 0 or >= 6)
        {
            return;
        }

        // 改变通道排列时不应意外恢复倾斜角；只替换固定 8 像素的通道预设。
        double tiltAngle = _cycle.ResolveTiltAngleDegrees();
        _cycle = CrosstalkPixelCyclePresets.CreateLegacy((RgbPixelOrder)comboLegacyPreset.SelectedIndex);
        _cycle.SetTiltAngleDegrees(tiltAngle);
        RefreshControlsFromCycle();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshControlsFromCycle()
    {
        // v1/v2 配置可能只有整数 RowAdvance。先由 atan(RowAdvance / 3) 换算成同语义角度，
        // 再裁剪到设计器数字框范围；之后保存会包含 TiltAngleDegrees 新字段。
        double tiltAngle = _cycle.ResolveTiltAngleDegrees();
        if (!double.IsFinite(tiltAngle))
        {
            tiltAngle = CrosstalkPixelCycle.DefaultTiltAngleDegrees;
        }

        tiltAngle = Math.Clamp(
            tiltAngle,
            (double)numericTiltAngle.Minimum,
            (double)numericTiltAngle.Maximum);
        _cycle.SetTiltAngleDegrees(tiltAngle);

        _updating = true;
        try
        {
            numericTiltAngle.Value = Math.Clamp(
                (decimal)_cycle.ResolveTiltAngleDegrees(),
                numericTiltAngle.Minimum,
                numericTiltAngle.Maximum);
            comboLegacyPreset.SelectedIndex = (int)CrosstalkPixelCyclePresets.FindNearestLegacyOrder(_cycle);
        }
        finally
        {
            _updating = false;
        }
    }
}
