using EolTestPatternGenerator.Models;
using EolTestPatternGenerator.Services;

namespace EolTestPatternGenerator.Controls;

/// <summary>
/// 逐位置编辑串扰周期的 Designer 用户控件。表格的第 N 行表示水平方向周期中的
/// 第 N 个像素位置；每个位置可以独立点亮 R/G/B 的任意组合。
/// </summary>
public partial class CrosstalkPixelGridEditor : UserControl
{
    private CrosstalkPixelCycle _cycle = CrosstalkPixelCyclePresets.CreateLegacy(RgbPixelOrder.RGB);
    private bool _updating;

    public CrosstalkPixelGridEditor()
    {
        InitializeComponent();
        NormalizeCycle(_cycle);
        RefreshControlsFromCycle();
    }

    public event EventHandler? SettingsChanged;

    public int PeriodLength => _cycle.PeriodLength;

    public CrosstalkPixelCycle GetCycle() => _cycle.Clone();

    /// <summary>载入独立保存的逐位置周期，并把异常旧值安全限制到控件范围。</summary>
    public void SetCycle(CrosstalkPixelCycle? cycle)
    {
        _cycle = (cycle ?? CrosstalkPixelCyclePresets.CreateLegacy(RgbPixelOrder.RGB)).Clone();
        NormalizeCycle(_cycle);
        RefreshControlsFromCycle();
    }

    /// <summary>恢复最初版 8 像素 RGB 掩码、横向步进 -3 和 18.435°。</summary>
    public void LoadReferenceDefault()
    {
        _cycle = CrosstalkPixelCyclePresets.CreateLegacy(RgbPixelOrder.RGB);
        NormalizeCycle(_cycle);
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
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void numericAdvance_ValueChanged(object? sender, EventArgs e)
    {
        if (_updating)
        {
            return;
        }

        _cycle.ColumnAdvance = (int)numericColumnAdvance.Value;
        _cycle.SetTiltAngleDegrees((double)numericTiltAngle.Value);
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void buttonColumnAdvanceHelp_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            this,
            "横向步进定义：图案局部 X 每向右 1 像素，周期索引按该有符号值取模移动。\n\n" +
            "例如 -3 表示向右 1 像素时，在周期中回退 3 个位置；超出首尾后按当前周期长度循环。\n\n" +
            "表格序号 1、2、3……表示水平方向周期内的像素位置，不是图像的纵向行号。",
            "横向步进说明",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
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
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshControlsFromCycle()
    {
        double tiltAngle = _cycle.ResolveTiltAngleDegrees();
        if (!double.IsFinite(tiltAngle))
        {
            tiltAngle = CrosstalkPixelCycle.DefaultTiltAngleDegrees;
        }

        _cycle.ColumnAdvance = Math.Clamp(
            _cycle.ColumnAdvance,
            decimal.ToInt32(numericColumnAdvance.Minimum),
            decimal.ToInt32(numericColumnAdvance.Maximum));
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
            numericColumnAdvance.Value = _cycle.ColumnAdvance;
            numericTiltAngle.Value = Math.Clamp(
                (decimal)_cycle.ResolveTiltAngleDegrees(),
                numericTiltAngle.Minimum,
                numericTiltAngle.Maximum);
            RefreshGridCore();
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

    private static void NormalizeCycle(CrosstalkPixelCycle cycle)
    {
        cycle.Pixels ??= [];
        if (cycle.Pixels.Count == 0)
        {
            cycle.Resize(CrosstalkPixelCycle.MinimumPeriodLength);
        }
        else if (cycle.Pixels.Count > CrosstalkPixelCycle.MaximumPeriodLength)
        {
            cycle.Resize(CrosstalkPixelCycle.MaximumPeriodLength);
        }

        for (int index = 0; index < cycle.Pixels.Count; index++)
        {
            cycle.Pixels[index] &= RgbChannelMask.All;
        }
    }
}
