using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Core.Monads;
using Core.Strings.Emojis;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms;

public class RectangleWriter(string text, Rectangle rectangle)
{
   protected const TextFormatFlags FLAGS = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;

   protected bool autoSizeText = true;
   protected string fontName = "Consolas";
   protected float fontSize = 12f;
   protected float minimumFontSize = 6f;
   protected FontStyle fontStyle = FontStyle.Regular;
   protected Maybe<Font> _font = nil;
   protected Color foreColor = Color.Black;
   protected Maybe<Color> _backColor = nil;
   protected bool outline;
   protected float penSize = 1f;
   protected DashStyle dashStyle = DashStyle.Solid;
   protected bool useEmojis = true;

   public string Text => text;

   public Rectangle Rectangle => rectangle;

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

   protected Maybe<Font> getAdjustedFont(Graphics g, string expandedText)
   {
      for (var size = fontSize; size >= minimumFontSize; size--)
      {
         var testFont = new Font(fontName, size, fontStyle);
         var textWidth = TextRenderer.MeasureText(g, expandedText, testFont, Size.Empty, FLAGS).Width;
         if (rectangle.Width > textWidth)
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

   public void Write(Graphics g)
   {
      g.HighQuality();
      g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

      var expandedText = getExpandedText();
      var font = getFont(g, expandedText);

      if (_backColor is (true, var backColor))
      {
         using var brush = new SolidBrush(backColor);
         g.FillRectangle(brush, rectangle);
      }

      TextRenderer.DrawText(g, expandedText, font, rectangle, foreColor, FLAGS);

      if (outline)
      {
         using var pen = new Pen(foreColor, penSize);
         pen.DashStyle = dashStyle;
         var outlineRectangle = rectangle.Resize(-2, -2).Reposition(1, 1);
         g.DrawRectangle(pen, outlineRectangle);
      }
   }
}