namespace Core.WinForms.Controls;

public class AllowArgs(string text, bool allowed) : EventArgs
{
   public string Text => text;

   public bool Allowed => allowed;
}