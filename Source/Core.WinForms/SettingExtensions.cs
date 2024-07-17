using Core.Configurations;
using Core.Monads;

namespace Core.WinForms;

public static class SettingExtensions
{
   public static void Point(this SettingSetter setter, Point point)
   {
      var pointSetting = new Setting(setter.Key);

      pointSetting.Set("x").Int32 = point.X;
      pointSetting.Set("y").Int32 = point.Y;

      setter.CurrentSetting.Set(setter.Key).Setting = pointSetting;
   }

   public static void Rectangle(this SettingSetter setter, Rectangle rectangle)
   {
      var rectangleSetting = new Setting(setter.Key);

      rectangleSetting.Set("x").Int32 = rectangle.X;
      rectangleSetting.Set("y").Int32 = rectangle.Y;
      rectangleSetting.Set("width").Int32 = rectangle.Width;
      rectangleSetting.Set("height").Int32 = rectangle.Height;

      setter.CurrentSetting.Set(setter.Key).Setting = rectangleSetting;
   }

   public static Point Point(this ConfigurationValue configurationValue, string key)
   {
      var pointSetting = configurationValue.Setting(key);

      var x = pointSetting.Value.Int32("x");
      var y = pointSetting.Value.Int32("y");

      return new Point(x, y);
   }

   public static Rectangle Rectangle(this ConfigurationValue configurationValue, string key)
   {
      var rectangleSetting = configurationValue.Setting(key);

      var x = rectangleSetting.Value.Int32("x");
      var y = rectangleSetting.Value.Int32("y");
      var width = rectangleSetting.Value.Int32("width");
      var height = rectangleSetting.Value.Int32("height");

      return new Rectangle(x, y, width, height);
   }

   public static Maybe<Point> Point(this ConfigurationMaybe configurationMaybe, string key)
   {
      return
         from pointSetting in configurationMaybe.Setting(key)
         from x in pointSetting.Maybe.Int32("x")
         from y in pointSetting.Maybe.Int32("y")
         select new Point(x, y);
   }

   public static Maybe<Rectangle> Rectangle(this ConfigurationMaybe configurationMaybe, string key)
   {
      return
         from rectangleSetting in configurationMaybe.Setting(key)
         from x in rectangleSetting.Maybe.Int32("x")
         from y in rectangleSetting.Maybe.Int32("y")
         from width in rectangleSetting.Maybe.Int32("width")
         from height in rectangleSetting.Maybe.Int32("height")
         select new Rectangle(x, y, width, height);
   }

   public static Result<Point> Point(this ConfigurationResult configurationResult, string key)
   {
      return
         from pointSetting in configurationResult.Setting(key)
         from x in pointSetting.Result.Int32("x")
         from y in pointSetting.Result.Int32("y")
         select new Point(x, y);
   }

   public static Result<Rectangle> Rectangle(this ConfigurationResult configurationResult, string key)
   {
      return
         from rectangleSetting in configurationResult.Setting(key)
         from x in rectangleSetting.Result.Int32("x")
         from y in rectangleSetting.Result.Int32("y")
         from width in rectangleSetting.Result.Int32("width")
         from height in rectangleSetting.Result.Int32("height")
         select new Rectangle(x, y, width, height);
   }

   public static Optional<Point> Point(this ConfigurationOptional configurationOptional, string key)
   {
      return
         from pointSetting in configurationOptional.Setting(key)
         from x in pointSetting.Optional.Int32("x")
         from y in pointSetting.Optional.Int32("y")
         select new Point(x, y);
   }

   public static Optional<Rectangle> Rectangle(this ConfigurationOptional configurationOptional, string key)
   {
      return
         from rectangleSetting in configurationOptional.Setting(key)
         from x in rectangleSetting.Optional.Int32("x")
         from y in rectangleSetting.Optional.Int32("y")
         from width in rectangleSetting.Optional.Int32("width")
         from height in rectangleSetting.Optional.Int32("height")
         select new Rectangle(x, y, width, height);
   }
}