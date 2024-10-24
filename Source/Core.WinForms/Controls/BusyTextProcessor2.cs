using System.Drawing.Drawing2D;

namespace Core.WinForms.Controls;

public class BusyTextProcessor2(Color foreColor, Color backColor, Rectangle rectangle)
{
   public static void OnPaint(Graphics g, Color foreColor, Color backColor, Rectangle rectangle, int statusAlpha)
   {
      using var brush = new LinearGradientBrush(Point.Empty, new Point(200, 0), foreColor.WithAlpha(statusAlpha), backColor.WithAlpha(statusAlpha));
      using var pen = new Pen(brush, 10);
      g.DrawRectangle(pen, rectangle);
   }

   protected Point left = Point.Empty;
   protected Point right = new(200, 0);

   public void OnTick() => (right, left) = (left, right);

   public void OnPaintAfterTick(Graphics g, int statusAlpha)
   {
      using var brush = new LinearGradientBrush(left, right, foreColor.WithAlpha(statusAlpha), backColor.WithAlpha(statusAlpha));
      using var pen = new Pen(brush, 10);
      g.DrawRectangle(pen, rectangle);
   }
}