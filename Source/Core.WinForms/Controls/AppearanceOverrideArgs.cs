namespace Core.WinForms.Controls;

public class AppearanceOverrideArgs : EventArgs
{
   public AppearanceOverrideArgs(int index, string text, Color foreColor, Color backColor)
   {
      Index = index;
      Text = text;
      ForeColor = foreColor;
      BackColor = backColor;
      Override = false;

      Italic = false;
      Bold = false;
   }

   public int Index { get; }

   public string Text { get; set; }

   public Color ForeColor { get; set; }

   public Color BackColor { get; set; }

   public bool Italic { get; set; }

   public bool Bold { get; set; }

   public bool Override { get; set; }
}