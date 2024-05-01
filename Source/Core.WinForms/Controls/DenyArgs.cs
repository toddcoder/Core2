namespace Core.WinForms.Controls;

public class DenyArgs(string text, bool denied) : EventArgs
{
   public string Text => text;

   public bool Denied => denied;
}