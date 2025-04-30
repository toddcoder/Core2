using System.Drawing.Text;
using Core.Collections;
using static Core.Numbers.NumberExtensions;

namespace Core.WinForms.Controls;

public class SineBusyProcessor : BusyProcessor
{
   protected const double MAXIMUM_VALUE = 720.0;

   protected int width;
   protected int height;
   protected int margin;
   protected double start;
   protected Memo<double, double> sineValues;
   protected Memo<double, int> yValues;
   protected Memo<double, int> xValues;

   public SineBusyProcessor(Rectangle clientRectangle) : base(clientRectangle)
   {
      width = clientRectangle.Width;
      height = clientRectangle.Height;
      margin = 2;

      start = 0.nextRandom(740, 20);
      sineValues = new Memo<double, double>.Function(d => Math.Sin(d * Math.PI / 180) + 1);
      yValues = new Memo<double, int>.Function(d => (int)(d / 2.0 * height + margin / 2));
      xValues = new Memo<double, int>.Function(d => (int)(d / MAXIMUM_VALUE * width + margin / 2));
   }

   public override void Advance()
   {
      if (start < MAXIMUM_VALUE)
      {
         start += 20;
      }
      else
      {
         start = 0;
      }
   }

   public override void OnPaint(Graphics graphics)
   {
      graphics.HighQuality();
      graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

      var right = MAXIMUM_VALUE + start;
      for (var i = start; i <= right; i += 0.1)
      {
         draw(i);
      }

      return;

      void draw(double i)
      {
         var value = sineValues[i];
         var y = yValues[value];
         var x = xValues[i - start];
         graphics.SetPixel(x, y, Color.White);
      }
   }
}