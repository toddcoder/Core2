namespace Core.WinForms.Controls;

public class ValidatingArgs(string text, AllowanceStatus status) : EventArgs
{
   public string Text => text;

   public AllowanceStatus Status
   {
      get => status;
      set => status = value;
   }
}