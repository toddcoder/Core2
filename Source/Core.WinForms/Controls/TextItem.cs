using Core.Monads;
using Core.Strings.Emojis;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class TextItem(string text, string fontName, float fontSize, FontStyle fontStyle, Color foreColor, Color backColor)
{
   public Maybe<Point> Render(Graphics g, Point location, Size clientSize, int padding)
   {
      var finalText = text.EmojiSubstitutions();
      var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
      using var font = new Font(fontName, fontSize, fontStyle);
      var proposedSize = new Size(int.MaxValue, int.MaxValue);
      var size = TextRenderer.MeasureText(g, finalText, font, proposedSize, flags);
      var rectangle = new Rectangle(location, size);

      TextRenderer.DrawText(g, finalText, font, rectangle, foreColor, backColor, flags);

      return advanceLocation(location, size, clientSize, padding);
   }

   protected Maybe<Point> advanceLocation(Point location, Size size, Size clientSize, int padding)
   {
      var x = location.X + size.Width;
      if (Line)
      {
         var y = location.Y + size.Height + padding;
         if (y < clientSize.Height)
         {
            return new Point(padding, y);
         }
         else
         {
            return nil;
         }
      }
      else if (x < clientSize.Width)
      {
         return location with { X = x };
      }
      else
      {
         return nil;
      }
   }

   public bool Line { get; set; }
}