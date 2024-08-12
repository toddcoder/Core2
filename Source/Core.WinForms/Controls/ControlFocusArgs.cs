namespace Core.WinForms.Controls;

public class ControlFocusArgs<TControl>(TControl control, int index) : EventArgs where TControl : Control
{
   public TControl Control => control;

   public int Index => index;
}