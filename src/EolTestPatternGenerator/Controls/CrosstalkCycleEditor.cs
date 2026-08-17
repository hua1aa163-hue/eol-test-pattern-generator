using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;

namespace EolTestPatternGenerator.Controls;

/// <summary>
/// 在设计器中可复用的串扰预设编辑器。
/// 六种排列沿用历史 -3 横向规则；界面允许调整周期像素数和倾斜角。
/// </summary>
public partial class CrosstalkCycleEditor : UserControl
{
    private CrosstalkPixelCycle _cycle = CrosstalkPixelCyclePresets.CreateLegacy(RgbPixelOrder.RGB);
    private RgbPixelOrder _selectedOrder = RgbPixelOrder.RGB;
    private bool _updating;

    public CrosstalkCycleEditor()
    {
        InitializeComponent();
        RefreshControlsFromCycle();
    }

    public event EventHandler? SettingsChanged;

    public int PeriodLength => _cycle.PeriodLength;

    /// <summary>
    /// 当前明确选择的通道排列。短周期可能无法仅凭像素内容唯一反推排列，
    /// 因而保存设置时必须同时持久化这个值。
    /// </summary>
    public RgbPixelOrder SelectedOrder => _selectedOrder;

    public CrosstalkPixelCycle GetCycle() => _cycle.Clone();

    public void SetCycle(CrosstalkPixelCycle? cycle, RgbPixelOrder? preferredOrder = null)
    {
        // 旧配置可能保存了逐位置通道或横向步进。精简界面不再暴露这些设置，
        // 因此加载时选取通道差异最小的六种预设，同时保留周期像素数和倾斜角。
        // 如果 PixelOrder 与按当前长度截取/补灭的周期完全一致，则优先采用它，
        // 以免周期很短时多个排列像素相同而丢失用户的明确选择。
        int periodLength = cycle?.PeriodLength ?? 8;
        periodLength = Math.Clamp(
            periodLength <= 0 ? 8 : periodLength,
            CrosstalkPixelCycle.MinimumPeriodLength,
            CrosstalkPixelCycle.MaximumPeriodLength);

        _selectedOrder = preferredOrder is RgbPixelOrder savedOrder &&
            CrosstalkPixelCyclePresets.MatchesResizedLegacyOrder(cycle, savedOrder)
                ? savedOrder
                : CrosstalkPixelCyclePresets.FindNearestLegacyOrder(cycle);

        double tiltAngle = cycle?.ResolveTiltAngleDegrees() ?? CrosstalkPixelCycle.DefaultTiltAngleDegrees;
        if (!double.IsFinite(tiltAngle))
        {
            tiltAngle = CrosstalkPixelCycle.DefaultTiltAngleDegrees;
        }

        RebuildCycle(_selectedOrder, periodLength, Math.Clamp(
            tiltAngle,
            CrosstalkPixelCycle.MinimumTiltAngleDegrees,
            CrosstalkPixelCycle.MaximumTiltAngleDegrees));
        RefreshControlsFromCycle();
    }

    public void LoadLegacyPreset(RgbPixelOrder order)
    {
        CrosstalkPixelCycle preset = CrosstalkPixelCyclePresets.CreateLegacy(order);
        _selectedOrder = order;
        _cycle = preset;
        RefreshControlsFromCycle();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void numericPeriodLength_ValueChanged(object? sender, EventArgs e)
    {
        if (_updating)
        {
            return;
        }

        // 每次都从明确选择的排列重建，避免 8→4→8 时第 5–8 个预设位置
        // 因破坏性缩短而永久丢失；长度超过 8 的新增位置仍按 Resize 规则全灭。
        RebuildCycle(
            _selectedOrder,
            (int)numericPeriodLength.Value,
            _cycle.ResolveTiltAngleDegrees());
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

        // 改变通道排列时不应意外恢复周期长度和倾斜角；先建立兼容预设，
        // 再按 CrosstalkPixelCycle.Resize 的既有规则恢复当前周期像素数。
        int periodLength = _cycle.PeriodLength;
        double tiltAngle = _cycle.ResolveTiltAngleDegrees();
        _selectedOrder = (RgbPixelOrder)comboLegacyPreset.SelectedIndex;
        RebuildCycle(_selectedOrder, periodLength, tiltAngle);
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
            numericPeriodLength.Value = Math.Clamp(
                _cycle.PeriodLength,
                (int)numericPeriodLength.Minimum,
                (int)numericPeriodLength.Maximum);
            numericTiltAngle.Value = Math.Clamp(
                (decimal)_cycle.ResolveTiltAngleDegrees(),
                numericTiltAngle.Minimum,
                numericTiltAngle.Maximum);
            comboLegacyPreset.SelectedIndex = (int)_selectedOrder;
        }
        finally
        {
            _updating = false;
        }
    }

    private void RebuildCycle(RgbPixelOrder order, int periodLength, double tiltAngle)
    {
        _cycle = CrosstalkPixelCyclePresets.CreateLegacy(order);
        _cycle.Resize(periodLength);
        _cycle.SetTiltAngleDegrees(tiltAngle);
    }
}
