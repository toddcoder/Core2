namespace Core.WinForms.Controls;

public class ControlFocusArgs<TControl>(TControl control, long id) : EventArgs where TControl : Control
{
   public TControl Control => control;

   public long Index => id;
}