using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Numbers;
using Core.Strings;
using Core.Strings.Emojis;
using Core.WinForms.Drawing;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiActionWriter
{
   protected const string CHECK_MARK = "\u2713";
   protected const int REQUIRE_SIZE = 4;
   protected const int REQUIRE_SIZE2 = 2 * REQUIRE_SIZE;

   public static TextFormatFlags GetFlags(bool center)
   {
      var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
      if (center)
      {
         flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
      }
      else
      {
         flags |= TextFormatFlags.VerticalCenter | TextFormatFlags.LeftAndRightPadding;
      }

      return flags;
   }

   protected TextFormatFlags getFlags(CardinalAlignment alignment)
   {
      Bits32<TextFormatFlags> flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;
      flags[TextFormatFlags.PathEllipsis] = isPath;
      switch (alignment)
      {
         case CardinalAlignment.NorthWest:
            break;
         case CardinalAlignment.North:
            flags[TextFormatFlags.Top] = true;
            flags[TextFormatFlags.HorizontalCenter] = true;
            break;
         case CardinalAlignment.NorthEast:
            flags[TextFormatFlags.Top] = true;
            flags[TextFormatFlags.Right] = true;
            break;
         case CardinalAlignment.East:
            flags[TextFormatFlags.Right] = true;
            flags[TextFormatFlags.VerticalCenter] = true;
            break;
         case CardinalAlignment.SouthEast:
            flags[TextFormatFlags.Bottom] = true;
            flags[TextFormatFlags.Right] = true;
            break;
         case CardinalAlignment.South:
            flags[TextFormatFlags.Bottom] = true;
            flags[TextFormatFlags.HorizontalCenter] = true;
            break;
         case CardinalAlignment.SouthWest:
            flags[TextFormatFlags.Bottom] = true;
            flags[TextFormatFlags.Left] = true;
            break;
         case CardinalAlignment.West:
            flags[TextFormatFlags.Left] = true;
            flags[TextFormatFlags.VerticalCenter] = true;
            break;
         case CardinalAlignment.Center:
            flags[TextFormatFlags.HorizontalCenter] = true;
            flags[TextFormatFlags.VerticalCenter] = true;
            break;
      }

      return flags;
   }

   protected bool autoSizeText;
   protected Maybe<int> _floor;
   protected Maybe<int> _ceiling;
   protected CheckStyle checkStyle = CheckStyle.None;
   protected Maybe<string> _emptyTextTitle = nil;
   protected bool isPath;
   protected Result<Rectangle> _rectangle = fail("Rectangle not set");
   protected Result<Font> _font = fail("Font not set");
   protected Result<Color> _color = fail("Color not set");
   protected UiActionButtonType buttonType;
   protected bool useEmojis;

   public UiActionWriter(CardinalAlignment messageAlignment, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling, UiActionButtonType buttonType,
      bool useEmojis)
   {
      Align(messageAlignment);
      this.autoSizeText = autoSizeText;
      this._floor = _floor;
      this._ceiling = _ceiling;
      this.buttonType = buttonType;
      this.useEmojis = useEmojis;
   }

   public UiActionWriter(Rectangle rectangle, Font font, Color color, CardinalAlignment messageAlignment = CardinalAlignment.Center,
      bool autoSizeText = true) : this(messageAlignment, autoSizeText, nil, nil, UiActionButtonType.Normal, true)
   {
      _rectangle = rectangle;
      _font = font;
      _color = color;
   }

   public void Center(bool center) => Flags = GetFlags(center);

   public void Align(CardinalAlignment messageAlignment) => Flags = getFlags(messageAlignment);

   public CheckStyle CheckStyle
   {
      get => checkStyle;
      set => checkStyle = value;
   }

   public bool IsPath
   {
      get => isPath;
      set => isPath = value;
   }

   public Maybe<string> EmptyTextTitle
   {
      set => _emptyTextTitle = value;
   }

   public Rectangle Rectangle
   {
      set => _rectangle = value;
   }

   public Font Font
   {
      set => _font = value;
   }

   public Color Color
   {
      set => _color = value;
   }

   public TextFormatFlags Flags { get; set; }

   public bool AutoSizeText
   {
      get => autoSizeText;
      set => autoSizeText = value;
   }

   public bool Required { get; set; }

   public bool UseEmojis
   {
      get => useEmojis;
      set => useEmojis = value;
   }

   public Size TextSize(Graphics g, string text)
   {
      var font = _font | (() => new Font("Consolas", 12f));
      return TextSize(g, text, font, Flags, useEmojis);
   }

   protected static string withEmojis(bool useEmojis, string text) => useEmojis ? text.EmojiSubstitutions() : text;

   protected string withEmojis(string text) => useEmojis ? text.EmojiSubstitutions() : text;

   public static Size TextSize(Graphics g, string text, Font font, TextFormatFlags flags, bool useEmojis)
   {
      var proposedSize = new Size(int.MaxValue, int.MaxValue);
      return TextRenderer.MeasureText(g, withEmojis(useEmojis, text), font, proposedSize, flags);
   }

   public static Size TextSize(string text, Font font, TextFormatFlags flags, bool useEmojis)
   {
      var proposedSize = new Size(int.MaxValue, int.MaxValue);
      return TextRenderer.MeasureText(withEmojis(useEmojis, text), font, proposedSize, flags);
   }

   public Rectangle TextRectangle(string text, Graphics graphics, Maybe<Rectangle> _rectangleToUse)
   {
      Rectangle rectangle;
      if (_rectangleToUse is (true, var rectangleToUse))
      {
         rectangle = rectangleToUse;
      }
      else if (_rectangle is (true, var currentRectangle))
      {
         rectangle = currentRectangle;
      }
      else
      {
         rectangle = graphics.ClipBounds.ToRectangle();
      }

      var textSize = TextSize(graphics, text);
      textSize = textSize with { Height = textSize.Height + 8, Width = textSize.Width + 8 };
      var x = rectangle.X + (rectangle.Width - textSize.Width) / 2;
      var y = rectangle.Y + (rectangle.Height - textSize.Height) / 2;

      return new Rectangle(x, y, textSize.Width, textSize.Height);
   }

   public Rectangle TextRectangle(string text, Graphics graphics) => TextRectangle(text, graphics, nil);

   protected void drawButtonType(Graphics g, Lazy<Rectangle> textRectangle, Color foreColor)
   {
      if (buttonType is not UiActionButtonType.Normal)
      {
         using var pen = new Pen(foreColor, 2);
         if (buttonType is UiActionButtonType.Cancel)
         {
            pen.DashStyle = DashStyle.Dot;
         }

         g.DrawRectangle(pen, textRectangle.Value);
      }
   }

   protected static void negate(bool not, Graphics g, Rectangle rectangle, Color foreColor)
   {
      if (not)
      {
         var length = Math.Min(rectangle.Width, rectangle.Height);
         var x = (rectangle.Width - length) / 2 + rectangle.X;
         var y = (rectangle.Height - length) / 2 + rectangle.Y;
         var lineRectangle = new Rectangle(x, y, length, length);
         var circleRectangle = lineRectangle.Resize(-4, -4).OffsetX(2).OffsetY(2);

         var color = Color.FromArgb(128, foreColor);
         using var pen = new Pen(color);
         g.DrawEllipse(pen, circleRectangle);
         g.DrawLine(pen, lineRectangle.NorthEast(), lineRectangle.SouthWest());
      }
   }

   protected void require(bool required, Graphics g, Color foreColor, Lazy<Rectangle> textRectangle)
   {
      if (required)
      {
         var outerTextRectangle = textRectangle.Value.Reposition(-REQUIRE_SIZE, -REQUIRE_SIZE).Resize(REQUIRE_SIZE2, REQUIRE_SIZE2);
         var color = foreColor == Color.Coral ? Color.White : Color.Coral;
         using var pen = new Pen(color, 4);
         g.DrawRectangle(pen, outerTextRectangle);
      }
   }

   public virtual Result<Unit> Write(Graphics g, string text, bool lower)
   {
      var not = false;
      if (text.StartsWith("!"))
      {
         not = true;
         text = text[1..];
      }

      text = text.Replace("/!", "!");
      text = withEmojis(text);
      if (lower)
      {
         text = text.ToLower();
      }

      var _existingRectangle = new LazyResult<Rectangle>(() => _rectangle);
      var _existingFont = _existingRectangle.Then(_font);
      var _existingColor = _existingFont.Then(_color);
      if (_existingColor)
      {
         Rectangle rectangle = _existingRectangle;
         Font font = _existingFont;
         Color color = _existingColor;

         try
         {
            g.HighQuality();
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            var isReplaced = text.IsEmpty() && _emptyTextTitle;
            if (isReplaced)
            {
               text = _emptyTextTitle;
               font = new Font(font, FontStyle.Italic);
            }

            var textRectangle = new Lazy<Rectangle>(() => TextRectangle(text, g));

            if (autoSizeText)
            {
               rectangle = AutoSizingWriter.NarrowRectangle(rectangle, _floor, _ceiling);
               drawButtonType(g, textRectangle, color);
               var writer = new AutoSizingWriter(text, rectangle, color, font, isPath);
               writer.Write(g);
               negate(not, g, rectangle, color);
               require(Required, g, color, textRectangle);
            }
            else
            {
               drawButtonType(g, textRectangle, color);
               TextRenderer.DrawText(g, text, font, rectangle, color, Flags);
               negate(not, g, rectangle, color);
               require(Required, g, color, textRectangle);
            }

            if (checkStyle is not CheckStyle.None)
            {
               using var pen = new Pen(color, 1);
               var location = new Point(2, 2);
               var size = new Size(12, 12);
               var boxRectangle = new Rectangle(location, size);
               g.DrawRectangle(pen, boxRectangle);

               if (checkStyle is CheckStyle.Checked)
               {
                  boxRectangle.Offset(1, 0);
                  boxRectangle.Inflate(8, 8);
                  using var checkFont = new Font("Consolas", 8, FontStyle.Bold);
                  TextRenderer.DrawText(g, CHECK_MARK, font, boxRectangle, color, Flags);
               }
            }

            return unit;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
      else
      {
         return _existingColor.Exception;
      }
   }
}