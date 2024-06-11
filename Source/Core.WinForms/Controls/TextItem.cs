using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class TextItem(UiAction uiAction, string text, string fontName, float fontSize, FontStyle fontStyle, Color foreColor, Color backColor)
{
   public Maybe<Point> Render(Point location, Size clientSize, int padding)
   {
      uiAction.Location = location;
      uiAction.Font = new Font(fontName, fontSize, fontStyle);
      uiAction.Display(text, foreColor, backColor);

      var size = UiActionWriter.TextSize(uiAction.Text, uiAction.Font, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
      return advanceLocation(location, size, clientSize, padding);
   }

   protected Maybe<Point> advanceLocation(Point location, Size size, Size clientSize, int padding)
   {
      var y = location.Y + size.Height;

      if (Line)
      {
         if (y < clientSize.Width)
         {
            return new Point(padding, y);
         }
         else
         {
            return nil;
         }
      }
      else if (y < clientSize.Width)
      {
         return location with { Y = y };
      }
      else
      {
         return nil;
      }
   }

   public bool Line { get; set; }
}