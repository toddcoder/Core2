using System.Collections;
using Core.Collections;
using Core.Monads;

namespace Core.WinForms;

public class RectangleArranger(Graphics g, string fontName = "Consolas", float fontSize = 12f, FontStyle fontStyle = FontStyle.Regular)
   : IEnumerable<ArrangedRectangle>
{
   protected string fontName = fontName;
   protected float fontSize = fontSize;
   protected FontStyle fontStyle = fontStyle;
   protected StringHash<ArrangedRectangle> arrangedRectangles = [];

   public string FontName
   {
      get => fontName;
      set => fontName = value;
   }

   public float FontSize
   {
      get => fontSize;
      set => fontSize = value;
   }

   public FontStyle FontStyle
   {
      get => fontStyle;
      set => fontStyle = value;
   }

   protected Font getFont() => new(fontName, fontSize, fontStyle);

   public void Add(string key, string text, Rectangle rectangle) =>
      arrangedRectangles[key] = new ArrangedRectangle(text, fontName, fontSize, fontStyle, rectangle);

   public Rectangle Add(string key, string text, Point location)
   {
      using var font = getFont();
      var size = RectangleWriter.TextSize(g, text);
      var rectangle = new Rectangle(location, size);
      Add(key, text, rectangle);

      return rectangle;
   }

   public Rectangle Add(string key, string text, int x, int y) => Add(key, text, new Point(x, y));

   public void RightOf(string key, string referenceKey, string text, Rectangle rectangle, int offset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         rectangle = data.Rectangle.RightOf(rectangle, offset);
      }

      Add(key, text, rectangle);
   }

   public Rectangle RightOf(string key, string referenceKey, string text, int offset = 0)
   {
      using var font = getFont();
      var size = RectangleWriter.TextSize(g, text);
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         var rectangle = data.Rectangle.RightOf(size, offset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public void LeftOf(string key, string referenceKey, string text, Rectangle rectangle, int offset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         rectangle = data.Rectangle.LeftOf(rectangle, offset);
      }

      Add(key, text, rectangle);
   }

   public Rectangle LeftOf(string key, string referenceKey, string text, int offset = 0)
   {
      using var font = getFont();
      var size = RectangleWriter.TextSize(g, text);
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         var rectangle = data.Rectangle.LeftOf(size, offset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public void TopOf(string key, string referenceKey, string text, Rectangle rectangle, int offset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         rectangle = data.Rectangle.TopOf(rectangle, offset);
      }

      Add(key, text, rectangle);
   }

   public Rectangle TopOf(string key, string referenceKey, string text, int offset = 0)
   {
      using var font = getFont();
      var size = RectangleWriter.TextSize(g, text);
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         var rectangle = data.Rectangle.TopOf(size, offset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public void BottomOf(string key, string referenceKey, string text, Rectangle rectangle, int offset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         rectangle = data.Rectangle.BottomOf(rectangle, offset);
      }

      Add(key, text, rectangle);
   }

   public Rectangle BottomOf(string key, string referenceKey, string text, int offset = 0)
   {
      using var font = getFont();
      var size = RectangleWriter.TextSize(g, text);
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         var rectangle = data.Rectangle.BottomOf(size, offset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public Rectangle NorthWest(string key, string referenceKey, string text, int xOffset = 0, int yOffset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         using var font = getFont();
         var size = RectangleWriter.TextSize(g, text);
         var rectangle = size.NorthWest(data.Rectangle, xOffset, yOffset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public Rectangle North(string key, string referenceKey, string text, int xOffset = 0, int yOffset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         using var font = getFont();
         var size = RectangleWriter.TextSize(g, text);
         var rectangle = size.North(data.Rectangle, xOffset, yOffset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public Rectangle NorthEast(string key, string referenceKey, string text, int xOffset = 0, int yOffset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         using var font = getFont();
         var size = RectangleWriter.TextSize(g, text);
         var rectangle = size.NorthEast(data.Rectangle, xOffset, yOffset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public Rectangle East(string key, string referenceKey, string text, int xOffset = 0, int yOffset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         using var font = getFont();
         var size = RectangleWriter.TextSize(g, text);
         var rectangle = size.East(data.Rectangle, xOffset, yOffset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public Rectangle SouthEast(string key, string referenceKey, string text, int xOffset = 0, int yOffset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         using var font = getFont();
         var size = RectangleWriter.TextSize(g, text);
         var rectangle = size.SouthEast(data.Rectangle, xOffset, yOffset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public Rectangle South(string key, string referenceKey, string text, int xOffset = 0, int yOffset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         using var font = getFont();
         var size = RectangleWriter.TextSize(g, text);
         var rectangle = size.South(data.Rectangle, xOffset, yOffset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public Rectangle SouthWest(string key, string referenceKey, string text, int xOffset = 0, int yOffset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         using var font = getFont();
         var size = RectangleWriter.TextSize(g, text);
         var rectangle = size.SouthWest(data.Rectangle, xOffset, yOffset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public Rectangle West(string key, string referenceKey, string text, int xOffset = 0, int yOffset = 0)
   {
      if (arrangedRectangles.Maybe[referenceKey] is (true, var data))
      {
         using var font = getFont();
         var size = RectangleWriter.TextSize(g, text);
         var rectangle = size.West(data.Rectangle, xOffset, yOffset);
         Add(key, text, rectangle);

         return rectangle;
      }
      else
      {
         return Rectangle.Empty;
      }
   }

   public Maybe<Rectangle> this[string key] => arrangedRectangles.Maybe[key].Map(d => d.Rectangle);

   public IEnumerator<ArrangedRectangle> GetEnumerator() => arrangedRectangles.Values.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}