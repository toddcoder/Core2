using System.Drawing.Drawing2D;

namespace Core.WinForms.Controls;

public class BusyTextProcessor
{
   protected const double TOTAL_CIRCLE = 360;
   protected const double HALF_CIRCLE = TOTAL_CIRCLE / 2;

   protected PointF center;
   protected Color color;
   protected Color[] colors;
   protected double[] angles;
   protected Rectangle drawRectangle;
   protected Rectangle textRectangle;
   protected int progressValue;
   protected int innerRadius = 5;
   protected int outerRadius = 11;
   protected int spokeCount = 12;
   protected int spokeThickness = 2;

   protected Color[] generatePalette(Color color)
   {
      static Color darken(Color spokeColor, int percent) => Color.FromArgb(percent, spokeColor.R, spokeColor.G, spokeColor.B);

      var spokeColors = new Color[spokeCount];
      var increment = (byte)(byte.MaxValue / spokeCount);
      var percentDarkened = 0;

      spokeColors[0] = color;

      for (var i = 1; i < spokeCount; i++)
      {
         percentDarkened += increment;
         if (percentDarkened > byte.MaxValue)
         {
            percentDarkened = byte.MaxValue;
         }

         spokeColors[i] = darken(color, percentDarkened);
      }

      return spokeColors;
   }

   public Rectangle TextRectangle => textRectangle;

   public Rectangle DrawRectangle => drawRectangle;

   protected double[] generateAngles()
   {
      var spokeAngles = new double[spokeCount];
      var angle = TOTAL_CIRCLE / spokeCount;
      spokeAngles[0] = angle;

      for (var i = 1; i < spokeCount; i++)
      {
         spokeAngles[i] = spokeAngles[i - 1] + angle;
      }

      return spokeAngles;
   }

   protected static Rectangle getDrawRectangle(Rectangle clientRectangle, bool leftSide)
   {
      var side = clientRectangle.Height;
      var drawRectangle = clientRectangle with { Width = side, Height = side };
      if (leftSide)
      {
         return drawRectangle;
      }
      else
      {
         return drawRectangle with { X = clientRectangle.Right - side };
      }
   }

   protected static Rectangle getTextRectangle(Rectangle drawRectangle, Rectangle clientRectangle, bool leftSide)
   {
      if (leftSide)
      {
         return clientRectangle with { X = clientRectangle.X + drawRectangle.Width, Width = clientRectangle.Width - drawRectangle.Width };
      }
      else
      {
         return clientRectangle with { Width = clientRectangle.Width - drawRectangle.Width };
      }
   }

   protected static PointF getCenter(Rectangle drawRectangle)
   {
      return new PointF(drawRectangle.X + drawRectangle.Height / 2, drawRectangle.Y + drawRectangle.Height / 2);
   }

   public BusyTextProcessor(Color color, Rectangle clientRectangle, bool leftSide = true)
   {
      this.color = color;

      colors = generatePalette(this.color);
      angles = generateAngles();
      drawRectangle = getDrawRectangle(clientRectangle, leftSide);
      textRectangle = getTextRectangle(drawRectangle, clientRectangle, leftSide);
      center = getCenter(drawRectangle);
   }

   protected void drawLine(Graphics graphics, PointF startPoint, PointF endPoint, Color color)
   {
      using var pen = new Pen(color, spokeThickness);
      pen.StartCap = LineCap.Round;
      pen.EndCap = LineCap.Round;
      graphics.DrawLine(pen, startPoint, endPoint);
   }

   protected PointF getCoordinate(PointF center, int radius, double angle)
   {
      var angleInRadians = Math.PI * angle / HALF_CIRCLE;
      return new PointF(center.X + radius * (float)Math.Cos(angleInRadians), center.Y + radius * (float)Math.Sin(angleInRadians));
   }

   public int InnerRadius
   {
      get => innerRadius;
      set=> innerRadius = value;
   }

   public int OuterRadius
   {
      get => outerRadius;
      set=> outerRadius = value;
   }

   public int SpokeCount
   {
      get => spokeCount;
      set=> spokeCount = value;
   }

   public int SpokeThickness
   {
      get => spokeThickness;
      set => spokeThickness = value;
   }

   public void OnTick() => progressValue = ++progressValue % spokeCount;

   public void OnPaint(PaintEventArgs e) => OnPaint(e.Graphics);

   public void OnPaint(Graphics g)
   {
      g.SmoothingMode = SmoothingMode.HighQuality;

      var position = progressValue;
      for (var i = 0; i < spokeCount; i++)
      {
         position %= spokeCount;
         var startPoint = getCoordinate(center, innerRadius, angles[position]);
         var endPoint = getCoordinate(center, outerRadius, angles[position]);
         drawLine(g, startPoint, endPoint, colors[i]);
         position++;
      }
   }
}