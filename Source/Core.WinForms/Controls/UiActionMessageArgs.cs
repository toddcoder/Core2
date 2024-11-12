namespace Core.WinForms.Controls;

public class UiActionMessageArgs(string message, object cargo) : EventArgs
{
   public string Message => message;

   public object Cargo => cargo;
}