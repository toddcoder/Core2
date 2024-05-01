namespace Core.WinForms.Drawing;

public class ColorFader(Color startColor, Color endColor)
{
   public void OnPaint(Graphics g, Rectangle clientRectangle, int index, int maximum)
   {
      var color = GetCurrentColor(index, maximum);
      using var brush = new SolidBrush(color);
      g.FillRectangle(brush, clientRectangle);
   }

   protected Color getCurrentColor(float progress)
   {
      var a = interpolate(startColor.A, endColor.A, progress);
      var r = interpolate(startColor.R, endColor.R, progress);
      var g = interpolate(startColor.G, endColor.G, progress);
      var b = interpolate(startColor.B, endColor.B, progress);

      return Color.FromArgb((int)a, (int)r, (int)g, (int)b);
   }

   public Color GetCurrentColor(int index, int maximum)
   {
      var progress = (float)index / maximum;
      return getCurrentColor(progress);
   }

   protected float interpolate(float start, float end, float progress) => start < end ? start + (end - start) * progress : start - (start - end) * progress;
}