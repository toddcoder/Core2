using Core.Monads;
using Core.Strings.Emojis;
using System.Drawing.Text;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ControlWriter
{
   protected static StringFormat GetFormat(CardinalAlignment alignment)
   {
      var stringFormat = new StringFormat();
      switch (alignment)
      {
         case CardinalAlignment.NorthWest:
            break;
         case CardinalAlignment.North:
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Near;
            break;
         case CardinalAlignment.NorthEast:
            stringFormat.Alignment = StringAlignment.Far;
            stringFormat.LineAlignment = StringAlignment.Near;
            break;
         case CardinalAlignment.East:
            stringFormat.Alignment = StringAlignment.Far;
            stringFormat.LineAlignment = StringAlignment.Center;
            break;
         case CardinalAlignment.SouthEast:
            stringFormat.Alignment = StringAlignment.Far;
            stringFormat.LineAlignment = StringAlignment.Far;
            break;
         case CardinalAlignment.South:
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Far;
            break;
         case CardinalAlignment.SouthWest:
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Far;
            break;
         case CardinalAlignment.West:
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Center;
            break;
         case CardinalAlignment.Center:
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            break;
      }

      return stringFormat;
   }

   protected static string withEmojis(bool useEmojis, string text) => useEmojis ? text.EmojiSubstitutions() : text;

   protected string withEmojis(string text) => UseEmojis ? text.EmojiSubstitutions() : text;

   public static Size TextSize(Graphics g, string text, Font font, Rectangle rectangle, StringFormat stringFormat, bool useEmojis)
   {
      return g.MeasureString(withEmojis(useEmojis, text), font, rectangle.Size, stringFormat).ToSize();
   }

   public static Size TextSize(string text, Font font, TextFormatFlags flags, bool useEmojis)
   {
      var proposedSize = new Size(int.MaxValue, int.MaxValue);
      return TextRenderer.MeasureText(withEmojis(useEmojis, text), font, proposedSize, flags);
   }

   public CardinalAlignment Alignment { get; set; } = CardinalAlignment.Center;

   public required Font Font { get; set; }

   public required Color Color { get; set; }

   public required Rectangle Rectangle { get; set; }

   public bool AutoSizeText { get; set; }

   public int MinimumSize { get; set; } = 6;

   public int MaximumSize { get; set; } = 12;

   public bool UseEmojis { get; set; } = true;

   public Size TextSize(Graphics g, string text)
   {
      using var stringFormat = GetFormat(Alignment);
      return TextSize(g, text, Font, Rectangle, stringFormat, UseEmojis);
   }

   public Rectangle TextRectangle(string text, Graphics graphics)
   {
      var textSize = TextSize(graphics, text);
      textSize = textSize with { Height = textSize.Height + 8, Width = textSize.Width + 8 };
      var x = Rectangle.X + (Rectangle.Width - textSize.Width) / 2;
      var y = Rectangle.Y + (Rectangle.Height - textSize.Height) / 2;

      return new Rectangle(x, y, textSize.Width, textSize.Height);
   }

   protected Maybe<Font> getAdjustedFont(Graphics g, string text, Font originalFont, int containerWidth, int floor, int ceiling)
   {
      for (var size = ceiling; size >= floor; size--)
      {
         var font = new Font(originalFont.Name, size, originalFont.Style);
         var textSize = TextSize(g, text, font, new Rectangle(Point.Empty, new Size(containerWidth, int.MaxValue)), StringFormat.GenericDefault,
            UseEmojis);
         if (containerWidth > textSize.Width)
         {
            return font;
         }
      }

      return nil;
   }

   protected static Font getFont(Font originalFont, int fontSize) => new(originalFont.Name, fontSize, originalFont.Style);

   public virtual Optional<Unit> Write(Graphics g, string text)
   {
      text = withEmojis(text);

      try
      {
         g.HighQuality();
         g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

         if (AutoSizeText)
         {
            var _adjustedFont = getAdjustedFont(g, text, Font, Rectangle.Width, MinimumSize, MaximumSize);
            if (_adjustedFont is (true, var adjustedFont))
            {
               using var brush = new SolidBrush(Color);
               using var stringFormat = GetFormat(Alignment);
               g.DrawString(text, adjustedFont, brush, Rectangle, stringFormat);
            }
            else
            {
               var smallestFont = getFont(Font, MinimumSize);
               using var brush = new SolidBrush(Color);
               using var stringFormat = GetFormat(Alignment);
               g.DrawString(text, smallestFont, brush, Rectangle, stringFormat);
            }
         }
         else
         {
            using var brush = new SolidBrush(Color);
            using var stringFormat = GetFormat(Alignment);
            g.DrawString(text, Font, brush, Rectangle, stringFormat);
         }

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}