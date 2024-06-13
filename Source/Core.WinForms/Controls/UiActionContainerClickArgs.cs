namespace Core.WinForms.Controls;

public class UiActionContainerClickArgs(UiAction uiAction, int index) : EventArgs
{
   public UiAction UiAction => uiAction;

   public int Index => index;
}