using System.Drawing.Drawing2D;

namespace Core.WinForms.Controls;

public class PieProgressProcessor2(Rectangle rectangle, int maximum)
{
   protected int index = 1;
   protected Point left = rectangle.Location;
   protected Point right = rectangle.Location with { X = rectangle.Right };

   public void OnTick()
   {
      if (index < maximum)
      {
         index++;
      }
   }

   public void OnPaint(Graphics g)
   {
      using var pen1 = new Pen(Color.CadetBlue, 10);
      g.DrawLine(pen1, left, right);

      using var pen2 = new Pen(Color.Coral, 10);
      var newRight = right with { X = (int)((right.X - left.X) * ((float)index / maximum)) };
      g.DrawLine(pen2, left, newRight);
   }
}