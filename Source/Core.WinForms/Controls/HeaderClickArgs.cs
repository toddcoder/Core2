namespace Core.WinForms.Controls;

public class HeaderClickArgs(string name, UiAction uiAction) : EventArgs
{
   public string Name => name;

   public UiAction UiAction => uiAction;
}