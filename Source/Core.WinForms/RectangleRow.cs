using System.Collections;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;

namespace Core.WinForms;

public class RectangleRow(Rectangle clientRectangle, RectangleAlignment alignment = RectangleAlignment.Left,
   RectangleDirection direction = RectangleDirection.Horizontal, int padding = 4) : IEnumerable<Rectangle>, IHash<string, Rectangle>
{
   protected RectangleAlignment alignment = alignment;
   protected RectangleDirection direction = direction;
   protected int top = clientRectangle.Top + padding;
   protected int width = clientRectangle.Width;
   protected List<Rectangle> row = [];
   protected bool rearrange = true;
   protected StringHash<Rectangle> rectangles = [];

   public RectangleAlignment Alignment
   {
      get => alignment;
      set
      {
         alignment = value;
         arrange();
      }
   }

   public RectangleDirection Direction
   {
      get => direction;
      set
      {
         direction = value;
         arrange();
      }
   }

   public Rectangle ClientRectangle => clientRectangle;

   public void Add(Rectangle rectangle)
   {
      row.Add(rectangle);
      arrange();
   }

   public void Add(Maybe<Rectangle> _rectangle)
   {
      if (_rectangle is (true, var rectangle))
      {
         Add(rectangle);
      }
   }

   public void AddRange(IEnumerable<Rectangle> rectangles)
   {
      foreach (var rectangle in rectangles)
      {
         row.Add(rectangle);
      }

      arrange();
   }

   public void Add(Rectangle rectangle, string key)
   {
      row.Add(rectangle);
      rectangles[key] = rectangle;

      arrange();
   }

   public void Add(Maybe<Rectangle> _rectangle, string key)
   {
      if (_rectangle is (true, var rectangle))
      {
         Add(rectangle, key);
      }
   }

   public void Add(Size size) => Add(new Rectangle(Point.Empty, size));

   public void Add(Maybe<Size> _size)
   {
      if (_size is (true, var size))
      {
         Add(size);
      }
   }

   public void AddRange(IEnumerable<Size> sizes)
   {
      foreach (var size in sizes)
      {
         row.Add(new Rectangle(Point.Empty, size));
      }

      arrange();
   }

   public void Add(Size size, string key) => Add(new Rectangle(Point.Empty, size), key);

   public void Add(Maybe<Size> _size, string key)
   {
      if (_size is (true, var size))
      {
         Add(size, key);
      }
   }

   public void Arrange() => arrange();

   public void BeginUpdate() => rearrange = false;

   public void EndUpdate()
   {
      rearrange = true;
      arrange();
   }

   protected IEnumerable<Rectangle> enumerable()
   {
      if (rearrange)
      {
         return alignment switch
         {
            RectangleAlignment.Left => leftEnumerable(),
            RectangleAlignment.Right => rightEnumerable(),
            RectangleAlignment.Center => centerEnumerable(),
            RectangleAlignment.Spread => spreadEnumerable(),
            _ => []
         };
      }
      else
      {
         return [];
      }
   }

   protected IEnumerable<Rectangle> leftEnumerable()
   {
      if (direction is RectangleDirection.Horizontal)
      {
         var x = padding;
         foreach (var rectangle in row)
         {
            var newRectangle = rectangle with { X = x, Y = top };
            yield return newRectangle;

            x += newRectangle.Width + padding;
         }
      }
      else
      {
         var y = padding;
         foreach (var rectangle in row)
         {
            var newRectangle = rectangle with { X = padding, Y = y };
            yield return newRectangle;

            y += newRectangle.Height + padding;
         }
      }
   }

   protected IEnumerable<Rectangle> rightEnumerable()
   {
      if (direction is RectangleDirection.Horizontal)
      {
         var right = clientRectangle.Right - padding;
         foreach (var rectangle in row.Reversed())
         {
            var x = right - rectangle.Width;
            var newRectangle = rectangle with { X = x, Y = top };
            yield return newRectangle;

            right = x - padding;
         }
      }
      else
      {
         var right = clientRectangle.Right - padding;
         var y = padding;
         foreach (var rectangle in row)
         {
            var x = right - rectangle.Width - padding;
            var newRectangle = rectangle with { X = x, Y = y };
            yield return newRectangle;

            y += rectangle.Height + padding;
         }
      }
   }

   protected IEnumerable<Rectangle> centerEnumerable()
   {
      var innerWidth = row.Select(r => r.Width).Sum();
      innerWidth += row.Count - 1;
      var remainder = width - innerWidth;
      var left = remainder / 2;
      foreach (var rectangle in row)
      {
         var newRectangle = rectangle with { X = left, Y = top };
         yield return newRectangle;

         left += newRectangle.Width + padding;
      }
   }

   protected IEnumerable<Rectangle> spreadEnumerable()
   {
      var innerWidth = row.Select(r => r.Width).Sum();
      var newPadding = (width - innerWidth) / (row.Count + 1);
      var left = newPadding;
      foreach (var rectangle in row)
      {
         var newRectangle = rectangle with { X = left, Y = top };
         yield return newRectangle;

         left += newRectangle.Width + newPadding;
      }
   }

   protected IEnumerable<Rectangle> enumerable(IEnumerable<Rectangle> rectangles)
   {
      if (rearrange)
      {
         return alignment switch
         {
            RectangleAlignment.Left => leftEnumerable(rectangles),
            RectangleAlignment.Right => rightEnumerable(rectangles).Reversed(),
            RectangleAlignment.Center => centerEnumerable(rectangles),
            RectangleAlignment.Spread => spreadEnumerable(rectangles),
            _ => []
         };
      }
      else
      {
         return [];
      }
   }

   protected IEnumerable<Rectangle> leftEnumerable(IEnumerable<Rectangle> rectangles)
   {
      var left = padding;
      foreach (var rectangle in rectangles)
      {
         var newRectangle = rectangle with { X = left, Y = top };
         yield return newRectangle;

         left += newRectangle.Width + padding;
      }
   }

   protected IEnumerable<Rectangle> rightEnumerable(IEnumerable<Rectangle> rectangles)
   {
      var right = width;

      foreach (var rectangle in rectangles.Reversed())
      {
         var left = right - rectangle.Width - padding;
         var newRectangle = rectangle with { X = left, Y = top };
         yield return newRectangle;

         right = left;
      }
   }

   protected IEnumerable<Rectangle> centerEnumerable(IEnumerable<Rectangle> rectangles)
   {
      Rectangle[] array = [.. rectangles];
      var innerWidth = array.Select(r => r.Width).Sum();
      innerWidth += array.Length - 1;
      var remainder = width - innerWidth;
      var left = remainder / 2;
      foreach (var rectangle in array)
      {
         var newRectangle = rectangle with { X = left, Y = top };
         yield return newRectangle;

         left += newRectangle.Width + padding;
      }
   }

   protected IEnumerable<Rectangle> spreadEnumerable(IEnumerable<Rectangle> rectangles)
   {
      Rectangle[] array = [.. rectangles];
      var innerWidth = array.Select(r => r.Width).Sum();
      var newPadding = (width - innerWidth) / (array.Length + 1);
      var left = newPadding;
      foreach (var rectangle in array)
      {
         var newRectangle = rectangle with { X = left, Y = top };
         yield return newRectangle;

         left += newRectangle.Width + newPadding;
      }
   }

   protected void arrange()
   {
      if (rearrange)
      {
         switch (alignment)
         {
            case RectangleAlignment.Left:
               arrangeLeft();
               break;
            case RectangleAlignment.Right:
               arrangeRight();
               break;
            case RectangleAlignment.Center:
               arrangeCenter();
               break;
            case RectangleAlignment.Spread:
               arrangeSpread();
               break;
         }
      }

      void arrangeLeft() => row = [.. leftEnumerable()];

      void arrangeRight()
      {
         if (direction is RectangleDirection.Horizontal)
         {
            row = [..rightEnumerable().Reversed()];
         }
         else
         {
            row = [.. rightEnumerable()];
         }
      }

      void arrangeCenter() => row = [.. centerEnumerable()];

      void arrangeSpread() => row = [..spreadEnumerable()];
   }

   public Rectangle this[int index] => row[index];

   public Rectangle this[Index index] => row[index];

   public Rectangle this[string key] => rectangles[key];

   public bool ContainsKey(string key) => rectangles.ContainsKey(key);

   public Hash<string, Rectangle> GetHash() => rectangles;

   public HashInterfaceMaybe<string, Rectangle> Items => new(rectangles);

   public bool MayContain(Rectangle rectangle)
   {
      return mayContain();

      bool mayContain()
      {
         return alignment switch
         {
            RectangleAlignment.Left => leftMayContain(),
            RectangleAlignment.Right => rightMayContain(),
            RectangleAlignment.Center => centerMayContain(),
            RectangleAlignment.Spread => spreadMayContain(),
            _ => false
         };

         bool contains(Rectangle rectangle) => clientRectangle.Contains(rectangle);

         bool leftMayContain()
         {
            IEnumerable<Rectangle> left = [.. leftEnumerable(), rectangle];
            return leftEnumerable(left).LastOrNone().Map(contains) | false;
         }

         bool rightMayContain()
         {
            IEnumerable<Rectangle> right = [.. rightEnumerable(), rectangle];
            var reversed = direction is RectangleDirection.Horizontal ? rightEnumerable(right).Reversed() : rightEnumerable(right);

            return reversed.LastOrNone().Map(contains) | false;
         }

         bool centerMayContain()
         {
            IEnumerable<Rectangle> center = [.. centerEnumerable(), rectangle];
            return centerEnumerable(center).LastOrNone().Map(contains) | false;
         }

         bool spreadMayContain()
         {
            IEnumerable<Rectangle> spread = [.. spreadEnumerable(), rectangle];
            return spreadEnumerable(spread).LastOrNone().Map(contains) | false;
         }
      }
   }

   public bool MayContain(Size size) => MayContain(new Rectangle(Point.Empty, size));

   public IEnumerator<Rectangle> GetEnumerator() => row.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public RectangleRow NextRow()
   {
      var nextRectangle = clientRectangle.BottomOf(clientRectangle);
      return new RectangleRow(nextRectangle, alignment, direction, padding);
   }

   public int Count => row.Count;
}