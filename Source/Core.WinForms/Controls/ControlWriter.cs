using Core.Monads;
using Core.Strings.Emojis;
using System.Drawing.Text;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ControlWriter
{
   protected const int DEFAULT_FLOOR = 6;
   protected const int DEFAULT_CEILING = 12;

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

   public Maybe<int> Floor { get; set; } = nil;

   public Maybe<int> Ceiling { get; set; } = nil;

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

   protected Rectangle narrowRectangle()
   {
      var rectangle = Rectangle;

      if (Floor is (true, var floor and >= 0))
      {
         rectangle = rectangle with { X = floor, Width = rectangle.Width - floor };
      }

      if (Ceiling is (true, var ceiling))
      {
         rectangle = rectangle with { Width = ceiling - rectangle.X };
      }

      return rectangle;
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

         if (Floor || Ceiling)
         {
            var _adjustedFont = getAdjustedFont(g, text, Font, Rectangle.Width, Floor | DEFAULT_FLOOR, Ceiling | DEFAULT_CEILING);
            if (_adjustedFont is (true, var adjustedFont))
            {
               var narrowedRectangle = narrowRectangle();
               using var brush = new SolidBrush(Color);
               using var stringFormat = GetFormat(Alignment);
               g.DrawString(text, adjustedFont, brush, narrowedRectangle, stringFormat);
            }
            else
            {
               var smallestFont = getFont(Font, Floor | DEFAULT_FLOOR);
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