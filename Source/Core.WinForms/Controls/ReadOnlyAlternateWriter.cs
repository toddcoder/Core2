using System.Drawing.Drawing2D;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using static Core.Arrays.ArrayExtensions;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ReadOnlyAlternateWriter(UiAction uiAction, string[] alternates, bool autoSizeText, bool useEmojis) : IAlternateWriter
{
   protected readonly Color defaultForeColor = Color.White;
   protected readonly Color defaultBackColor = Color.CadetBlue;
   protected const FontStyle DEFAULT_FONT_STYLE = FontStyle.Regular;

   protected Hash<int, Color> foreColors = [];
   protected Hash<int, Color> backColors = [];
   protected Hash<int, FontStyle> fontStyles = [];

   public Maybe<Color> GetForeColor(int index) => foreColors.Maybe[index];

   public Color GetAlternateForeColor(int index) => foreColors.Maybe[index] | defaultForeColor;

   public void SetForeColor(int index, Color color) => foreColors[index] = color;

   public Maybe<Color> GetBackColor(int index) => backColors.Maybe[index];

   public Color GetAlternateBackColor(int index) => backColors.Maybe[index] | defaultBackColor;

   public void SetBackColor(int index, Color color) => backColors[index] = color;

   public FontStyle GetFontStyle(int index) => fontStyles.Maybe[index] | DEFAULT_FONT_STYLE;

   public void SetFontStyle(int index, FontStyle style) => fontStyles[index] = style;

   public Maybe<string> GetAlternate(int index) => alternates.Maybe(index);

   public void SetAlternate(int index, string alternate)
   {
      if (index.Between(0).Until(alternates.Length))
      {
         alternates[index] = alternate;
      }
   }

   public int SelectedIndex { get; set; } = -1;

   public int DisabledIndex { get; set; } = -1;

   public string Alternate => alternates[0];

   protected Maybe<Rectangle[]> getRectangles(Graphics g)
   {
      List<int> widths = [];
      var totalWidth = 0;
      foreach (var alternate in alternates)
      {
         var size = RectangleWriter.TextSize(g, alternate, uiAction.NonNullFont);
         widths.Add(size.Width);
         totalWidth += size.Width;
      }

      var clientRectangle = uiAction.ClientRectangle;
      var remainingWidths = clientRectangle.Width - totalWidth;
      if (remainingWidths > 0)
      {
         var padding = remainingWidths / alternates.Length;
         var rectangles = new Rectangle[alternates.Length];
         var left = clientRectangle.Left;
         var top = clientRectangle.Top;
         var height = clientRectangle.Height;

         for (var i = 0; i < alternates.Length; i++)
         {
            var width = widths[i] + padding;
            rectangles[i] = new Rectangle(left, top, width, height);
            left += width;
         }

         return rectangles;
      }
      else
      {
         return nil;
      }
   }

   public void OnPaint(Graphics g)
   {
      var rectangles = getRectangles(g) | (() => uiAction.Rectangles);
      uiAction.Rectangles = rectangles;

      foreach (var (i, (rectangle, alternate)) in rectangles.Zip(Alternates).Indexed())
      {
         var writer = new RectangleWriter(alternate, rectangle)
         {
            ForeColor = GetAlternateForeColor(i),
            BackColor = GetAlternateBackColor(i),
            BackgroundRestriction = new BackgroundRestriction.Fill(),
            Font = uiAction.NonNullFont,
            AutoSizeText = autoSizeText,
            UseEmojis = useEmojis
         };
         writer.Write(g);

         if (i > 0)
         {
            var start = rectangle.Location;
            var stop = start with { Y = rectangle.Bottom };
            using var pen = new Pen(writer.ForeColor);
            pen.DashStyle = DashStyle.Dash;
            g.DrawLine(pen, start, stop);
         }
      }
   }

   public string[] Alternates => alternates;
}