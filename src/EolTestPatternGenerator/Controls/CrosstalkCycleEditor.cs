using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;

namespace EolTestPatternGenerator.Controls;

/// <summary>
/// 在设计器中可复用的串扰周期编辑器。周期行是数据，不在业务代码中创建按钮或输入控件。
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
        _cycle = (cycle ?? CrosstalkPixelCyclePresets.CreateLegacy(RgbPixelOrder.RGB)).Clone();
        ValidateAndNormalizeCycle(_cycle);
        RefreshControlsFromCycle();
    }

    public void LoadLegacyPreset(RgbPixelOrder order)
    {
        _cycle = CrosstalkPixelCyclePresets.CreateLegacy(order);
        RefreshControlsFromCycle();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void numericPeriodLength_ValueChanged(object? sender, EventArgs e)
    {
        if (_updating)
        {
            return;
        }

        _cycle.Resize((int)numericPeriodLength.Value);
        RefreshGrid();
        SelectCustomOrMatchingPreset();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void numericAdvance_ValueChanged(object? sender, EventArgs e)
    {
        if (_updating)
        {
            return;
        }

        _cycle.ColumnAdvance = (int)numericColumnAdvance.Value;
        _cycle.RowAdvance = (int)numericRowAdvance.Value;
        SelectCustomOrMatchingPreset();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void comboLegacyPreset_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_updating || comboLegacyPreset.SelectedIndex is < 0 or >= 6)
        {
            return;
        }

        _cycle = CrosstalkPixelCyclePresets.CreateLegacy((RgbPixelOrder)comboLegacyPreset.SelectedIndex);
        RefreshControlsFromCycle();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void gridCycle_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (gridCycle.IsCurrentCellDirty)
        {
            gridCycle.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void gridCycle_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_updating || e.RowIndex < 0 || e.RowIndex >= _cycle.PeriodLength || e.ColumnIndex < 1)
        {
            return;
        }

        DataGridViewRow row = gridCycle.Rows[e.RowIndex];
        RgbChannelMask mask = RgbChannelMask.None;
        if (Convert.ToBoolean(row.Cells[columnRed.Index].Value ?? false))
        {
            mask |= RgbChannelMask.Red;
        }

        if (Convert.ToBoolean(row.Cells[columnGreen.Index].Value ?? false))
        {
            mask |= RgbChannelMask.Green;
        }

        if (Convert.ToBoolean(row.Cells[columnBlue.Index].Value ?? false))
        {
            mask |= RgbChannelMask.Blue;
        }

        _cycle.Pixels[e.RowIndex] = mask;
        SelectCustomOrMatchingPreset();
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshControlsFromCycle()
    {
        // 手工编辑的配置可能超出数字框范围。显式裁剪并回写模型，
        // 避免界面显示 ±4096，生成器却暗中使用另一个步进值。
        _cycle.ColumnAdvance = Math.Clamp(
            _cycle.ColumnAdvance,
            decimal.ToInt32(numericColumnAdvance.Minimum),
            decimal.ToInt32(numericColumnAdvance.Maximum));
        _cycle.RowAdvance = Math.Clamp(
            _cycle.RowAdvance,
            decimal.ToInt32(numericRowAdvance.Minimum),
            decimal.ToInt32(numericRowAdvance.Maximum));

        _updating = true;
        try
        {
            numericPeriodLength.Value = Math.Clamp(
                _cycle.PeriodLength,
                (int)numericPeriodLength.Minimum,
                (int)numericPeriodLength.Maximum);
            numericColumnAdvance.Value = Math.Clamp(
                _cycle.ColumnAdvance,
                (int)numericColumnAdvance.Minimum,
                (int)numericColumnAdvance.Maximum);
            numericRowAdvance.Value = Math.Clamp(
                _cycle.RowAdvance,
                (int)numericRowAdvance.Minimum,
                (int)numericRowAdvance.Maximum);
            RefreshGridCore();
            comboLegacyPreset.SelectedIndex = CrosstalkPixelCyclePresets.TryGetLegacyOrder(_cycle, out RgbPixelOrder order)
                ? (int)order
                : 6;
        }
        finally
        {
            _updating = false;
        }
    }

    private void RefreshGrid()
    {
        _updating = true;
        try
        {
            RefreshGridCore();
        }
        finally
        {
            _updating = false;
        }
    }

    private void RefreshGridCore()
    {
        gridCycle.Rows.Clear();
        for (int index = 0; index < _cycle.PeriodLength; index++)
        {
            RgbChannelMask mask = _cycle.Pixels[index];
            gridCycle.Rows.Add(
                index + 1,
                (mask & RgbChannelMask.Red) != 0,
                (mask & RgbChannelMask.Green) != 0,
                (mask & RgbChannelMask.Blue) != 0);
        }
    }

    private void SelectCustomOrMatchingPreset()
    {
        _updating = true;
        try
        {
            comboLegacyPreset.SelectedIndex = CrosstalkPixelCyclePresets.TryGetLegacyOrder(_cycle, out RgbPixelOrder order)
                ? (int)order
                : 6;
        }
        finally
        {
            _updating = false;
        }
    }

    private static void ValidateAndNormalizeCycle(CrosstalkPixelCycle cycle)
    {
        cycle.Pixels ??= [];
        if (cycle.Pixels.Count == 0)
        {
            cycle.Resize(CrosstalkPixelCycle.MinimumPeriodLength);
        }

        if (cycle.Pixels.Count > CrosstalkPixelCycle.MaximumPeriodLength)
        {
            cycle.Resize(CrosstalkPixelCycle.MaximumPeriodLength);
        }

        for (int index = 0; index < cycle.Pixels.Count; index++)
        {
            cycle.Pixels[index] &= RgbChannelMask.All;
        }
    }
}
