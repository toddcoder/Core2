namespace Core.WinForms.Controls;

public class PieProgressProcessor(Rectangle clientRectangle, int maximum, Color foreColor) : BusyProcessor(clientRectangle)
{
   protected int index = 1;
   protected float sweepAngle;

   public override void Advance()
   {
      if (index <= maximum)
      {
         sweepAngle = (float)index / maximum * 360;
      }

      index++;
   }

   public override void OnPaint(Graphics g)
   {
      g.HighQuality();
      g.FillEllipse(Brushes.CadetBlue, clientRectangle);
      g.FillPie(Brushes.Coral, clientRectangle, 0, sweepAngle);
      using var pen = new Pen(foreColor, 2);
      g.DrawEllipse(pen, clientRectangle);
   }
}