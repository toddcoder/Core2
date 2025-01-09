namespace Core.WinForms.Controls;

public class UrlChangedArgs(string url) : EventArgs
{
   public string Url { get; set; } = url;

   public bool Cancel { get; set; }
}