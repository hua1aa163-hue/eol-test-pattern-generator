namespace EolTestPatternGenerator.Controls;

/// <summary>
/// 全局拦截 NumericUpDown 的滚轮消息，避免滚动参数面板时意外改值。
/// 键盘、上下按钮和直接输入仍保持可用。
/// </summary>
internal sealed class NumericMouseWheelFilter : IMessageFilter
{
    private const int WmMouseWheel = 0x020A;

    public bool PreFilterMessage(ref Message message)
    {
        if (message.Msg != WmMouseWheel)
        {
            return false;
        }

        Control? target = Control.FromHandle(message.HWnd);
        while (target is not null)
        {
            if (target is NumericUpDown)
            {
                return true;
            }

            target = target.Parent;
        }

        return false;
    }
}
