namespace Core.WinForms.Controls;

public class TrendArgs(string text, bool trending) : EventArgs
{
   public string Text => text;

   public bool Trending => trending;
}