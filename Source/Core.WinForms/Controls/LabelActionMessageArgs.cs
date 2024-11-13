namespace Core.WinForms.Controls;

public class LabelActionMessageArgs(UiAction uiAction, string message, object cargo)
{
   public UiAction UiAction => uiAction;

   public string Message => message;

   public object Cargo => cargo;
}