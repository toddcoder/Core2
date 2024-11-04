using Core.Monads;

namespace Core.WinForms.Controls;

public interface IAlternateWriter
{
   Maybe<Color> GetForeColor(int index);

   Color GetAlternateForeColor(int index);

   void SetForeColor(int index, Color color);

   Maybe<Color> GetBackColor(int index);

   Color GetAlternateBackColor(int index);

   void SetBackColor(int index, Color color);

   FontStyle GetFontStyle(int index);

   void SetFontStyle(int index, FontStyle style);

   Maybe<string> GetAlternate(int index);

   int SelectedIndex { get; set; }

   int DisabledIndex { get; set; }

   string Alternate { get; }

   void OnPaint(Graphics g);

   string[] Alternates { get; }
}