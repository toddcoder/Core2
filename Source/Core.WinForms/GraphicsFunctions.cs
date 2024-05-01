namespace Core.WinForms;

public class GraphicsFunctions
{
   private static double toRadians(double degrees) => degrees * Math.PI / 180;

   private static double toDegrees(double radians) => radians * 180 / Math.PI;

   public static Point polarToCartesian(double angle, double radius)
   {
      var angleInRadians = toRadians(angle);
      var x = radius * Math.Cos(angleInRadians);
      var y = radius * Math.Sin(angleInRadians);

      return new Point((int)x, (int)y);
   }

   public static (int radius, int angle) cartesianToPolar(double x, double y)
   {
      var radius = Math.Sqrt(x * x + y * y);
      var angle = Math.Atan2(y, x);
      var angleInDegrees = toDegrees(angle);

      return ((int)radius, (int)angleInDegrees);
   }
}