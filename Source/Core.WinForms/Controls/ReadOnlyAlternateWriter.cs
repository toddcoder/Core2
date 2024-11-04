using System.Drawing.Drawing2D;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ReadOnlyAlternateWriter(UiAction uiAction, string[] alternates, bool autoSizeText) : IAlternateWriter
{
   protected Hash<int, Color> foreColors = [];
   protected Hash<int, Color> backColors = [];
   protected Hash<int, FontStyle> fontStyles = [];

   public Maybe<Color> GetForeColor(int index) => foreColors.Maybe[index] | Color.White;

   public Color GetAlternateForeColor(int index) => backColors.Maybe[index] | Color.CadetBlue;

   public void SetForeColor(int index, Color color) => foreColors[index] = color;

   public Maybe<Color> GetBackColor(int index) => backColors.Maybe[index];

   public Color GetAlternateBackColor(int index) => backColors.Maybe[index] | Color.CadetBlue;

   public void SetBackColor(int index, Color color) => backColors[index] = color;

   public FontStyle GetFontStyle(int index) => fontStyles.Maybe[index] | FontStyle.Regular;

   public void SetFontStyle(int index, FontStyle style) => fontStyles[index] = style;

   public Maybe<string> GetAlternate(int index) => maybe<string>() & index.Between(0).Until(alternates.Length) & (() => alternates[index]);

   public int SelectedIndex { get; set; }= -1;

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
      var clientRectangle = uiAction.ClientRectangle;
      using var brush = new SolidBrush(Color.CadetBlue);
      g.FillRectangle(brush, clientRectangle);

      var rectangles = getRectangles(g) | (() => uiAction.Rectangles);
      uiAction.Rectangles=rectangles;

      foreach (var (i, (rectangle, alternate)) in rectangles.Zip(Alternates).Indexed())
      {
         var writer = new RectangleWriter(alternate, rectangle)
         {
            ForeColor = Color.White,
            Font = uiAction.NonNullFont,
            AutoSizeText = autoSizeText
         };
         writer.Write(g);

         if (i > 0)
         {
            var start = rectangle.Location;
            var stop = start with { Y = rectangle.Bottom };
            using var pen = new Pen(Color.White);
            pen.DashStyle = DashStyle.Dash;
            g.DrawLine(pen, start, stop);
         }
      }
   }

   public string[] Alternates => alternates;
}