using System.Drawing.Drawing2D;

namespace Core.WinForms.Controls;

public class GradientBusyProcessor(Rectangle clientRectangle) : BusyProcessor(clientRectangle)
{
   protected int alpha = 255;

   public override void Advance()
   {
      alpha -= 5;
      if (alpha <= 0)
      {
         alpha = 255;
      }
   }

   public override void OnPaint(Graphics g)
   {
      using var brush = new LinearGradientBrush(Point.Empty, new Point(0, 200), Color.Aqua, Color.Green);
      g.FillRectangle(brush, clientRectangle);

      using var backPen = new Pen(Color.AntiqueWhite, 10);
      g.DrawRectangle(backPen, clientRectangle);

      using var forePen = new Pen(Color.LightGreen.WithAlpha(alpha), 10);
      g.DrawRectangle(forePen, clientRectangle);
   }
}