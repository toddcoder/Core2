using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Core.Collections;
using Core.Monads;
using Core.Strings.Emojis;
using Core.WinForms.Controls;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms;

public class RectangleWriter(string text, Rectangle rectangle, CardinalAlignment alignment = CardinalAlignment.Center, bool wordWrap = false,
   bool ellipses = false)
{
   protected const string DEFAULT_FONT_NAME = "Consolas";
   protected const float DEFAULT_FONT_SIZE = 12f;
   protected const FontStyle DEFAULT_FONT_STYLE = FontStyle.Regular;

   protected static StringHash<RectangleWriter> stashedWriters = [];

   public static Maybe<RectangleWriter> Retrieve(string key) => stashedWriters.Maybe[key];

   public static RectangleWriter RetrieveHard(string key) => stashedWriters[key];

   protected static TextFormatFlags getFlags(CardinalAlignment alignment, bool wordWrap, bool ellipses)
   {
      var alignmentFlags = alignment switch
      {
         CardinalAlignment.NorthWest => TextFormatFlags.Left | TextFormatFlags.Top,
         CardinalAlignment.North => TextFormatFlags.HorizontalCenter | TextFormatFlags.Top,
         CardinalAlignment.NorthEast => TextFormatFlags.Right | TextFormatFlags.Top,
         CardinalAlignment.East => TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
         CardinalAlignment.SouthEast => TextFormatFlags.Right | TextFormatFlags.Bottom,
         CardinalAlignment.South => TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom,
         CardinalAlignment.SouthWest => TextFormatFlags.Left | TextFormatFlags.Bottom,
         CardinalAlignment.West => TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
         CardinalAlignment.Center => TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
         _ => TextFormatFlags.Default
      };

      var textFormatFlags = alignmentFlags | TextFormatFlags.NoPrefix;
      if (wordWrap)
      {
         textFormatFlags |= TextFormatFlags.WordBreak;
      }

      if (ellipses)
      {
         textFormatFlags |= TextFormatFlags.EndEllipsis;
      }

      return textFormatFlags;
   }

   public static Font DefaultFont() => new(DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE, DEFAULT_FONT_STYLE);

   protected string text = text;
   protected Rectangle rectangle = rectangle;
   protected CardinalAlignment alignment = alignment;
   protected bool autoSizeText = true;
   protected string fontName = DEFAULT_FONT_NAME;
   protected float fontSize = DEFAULT_FONT_SIZE;
   protected float minimumFontSize = 6f;
   protected FontStyle fontStyle = DEFAULT_FONT_STYLE;
   protected Maybe<Font> _font = nil;
   protected Color foreColor = Color.Black;
   protected Maybe<Color> _backColor = nil;
   protected bool outline;
   protected float penSize = 1f;
   protected DashStyle dashStyle = DashStyle.Solid;
   protected bool useEmojis = true;
   protected TextFormatFlags flags = getFlags(alignment, wordWrap, ellipses);

   public string Text
   {
      get => text;
      set => text = value;
   }

   public Rectangle Rectangle
   {
      get => rectangle;
      set => rectangle = value;
   }

   public CardinalAlignment Alignment
   {
      get => alignment;
      set => alignment = value;
   }

   public bool AutoSizeText
   {
      get => autoSizeText;
      set
      {
         autoSizeText = value;
         _font = nil;
      }
   }

   public string FontName
   {
      get => fontName;
      set
      {
         fontName = value;
         _font = nil;
      }
   }

   public float FontSize
   {
      get => fontSize;
      set
      {
         fontSize = value;
         _font = nil;
      }
   }

   public float MinimumFontSize
   {
      get => minimumFontSize;
      set
      {
         minimumFontSize = value;
         _font = nil;
      }
   }

   public FontStyle FontStyle
   {
      get => fontStyle;
      set
      {
         fontStyle = value;
         _font = nil;
      }
   }

   public Color ForeColor
   {
      get => foreColor;
      set => foreColor = value;
   }

   public Maybe<Color> BackColor
   {
      get => _backColor;
      set => _backColor = value;
   }

   public bool Outline
   {
      get => outline;
      set => outline = value;
   }

   public float PenSize
   {
      get => penSize;
      set => penSize = value;
   }

   public DashStyle DashStyle
   {
      get => dashStyle;
      set => dashStyle = value;
   }

   public bool UseEmojis
   {
      get => useEmojis;
      set
      {
         useEmojis = value;
         _font = nil;
      }
   }

   public Maybe<Font> Font
   {
      get => _font;
      set => _font = value;
   }

   public Rectangle UsedRectangle { get; set; } = Rectangle.Empty;

   public TextFormatFlags UsedFlags { get; set; } = TextFormatFlags.Default;

   public float UsedFontSize { get; set; }

   public bool Ellipses { get; set; }

   public static Size TextSize(Graphics g, string text, Font font, CardinalAlignment alignment = CardinalAlignment.Center, bool wordWrap = false,
      bool ellipses = false)
   {
      return TextRenderer.MeasureText(g, text, font, Size.Empty, getFlags(alignment, wordWrap, ellipses));
   }

   public static Size TextSize(Graphics g, string text, CardinalAlignment alignment = CardinalAlignment.Center, bool wordWrap = false,
      bool ellipses = false)
   {
      return TextSize(g, text, DefaultFont(), alignment, wordWrap, ellipses);
   }

   protected Maybe<Font> getAdjustedFont(Graphics g, string expandedText)
   {
      for (var size = fontSize; size >= minimumFontSize; size--)
      {
         var testFont = new Font(fontName, size, fontStyle);
         var textSize = TextSize(g, expandedText, testFont);
         if (rectangle.Width > textSize.Width && rectangle.Height > textSize.Height)
         {
            return testFont;
         }
      }

      return nil;
   }

   protected Font getFont(Graphics g, string expandedText)
   {
      if (_font is (true, var font))
      {
         return font;
      }
      else if (autoSizeText && getAdjustedFont(g, expandedText) is (true, var adjustedFont))
      {
         _font = adjustedFont;
         return adjustedFont;
      }
      else
      {
         font = new Font(fontName, fontSize, fontStyle);
         _font = font;

         return font;
      }
   }

   protected string getExpandedText() => useEmojis ? text.EmojiSubstitutions() : text;

   public BackgroundRestriction BackgroundRestriction { get; set; } = new BackgroundRestriction.Fill();

   protected Rectangle getRestrictedRectangle(Graphics g, string expandedText, Font font, CardinalAlignment restrictionAlignment, int xMargin,
      int yMargin)
   {
      var size = TextSize(g, expandedText, font);
      return size.Rectangle(rectangle, restrictionAlignment, xMargin, yMargin);
   }

   public void Stash(string stashName) => stashedWriters[stashName] = this;

   public void Write(Graphics g)
   {
      g.HighQuality();
      g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

      var expandedText = getExpandedText();
      using var font = getFont(g, expandedText);

      var writingRectangle = rectangle;
      var writingFlags = flags;

      if (_backColor is (true, var backColor))
      {
         switch (BackgroundRestriction)
         {
            case BackgroundRestriction.Fill:
            {
               fillRectangle(writingRectangle, backColor);
               break;
            }
            case BackgroundRestriction.Restricted restricted:
            {
               var restrictedRectangle = getRestrictedRectangle(g, expandedText, font, restricted.Alignment, restricted.XMargin, restricted.YMargin);
               fillRectangle(restrictedRectangle, backColor);
               writingRectangle = restrictedRectangle;
               writingFlags = getFlags(restricted.Alignment, wordWrap, Ellipses);
               break;
            }
            case BackgroundRestriction.UseWriterAlignment useWriterAlignment:
            {
               var restrictedRectangle = getRestrictedRectangle(g, expandedText, font, alignment, useWriterAlignment.XMargin,
                  useWriterAlignment.YMargin);
               fillRectangle(restrictedRectangle, backColor);
               writingRectangle = restrictedRectangle;
               break;
            }
         }
      }

      TextRenderer.DrawText(g, expandedText, font, writingRectangle, foreColor, writingFlags);

      UsedRectangle = writingRectangle;
      UsedFlags = flags;
      UsedFontSize = font.Size;

      if (outline)
      {
         using var pen = new Pen(foreColor, penSize);
         pen.DashStyle = dashStyle;
         var outlineRectangle = writingRectangle.Resize(-2, -2).Reposition(1, 1);
         g.DrawRectangle(pen, outlineRectangle);
      }

      return;

      void fillRectangle(Rectangle rectangleToFill, Color backColor)
      {
         using var brush = new SolidBrush(backColor);
         g.FillRectangle(brush, rectangleToFill);
      }
   }
}