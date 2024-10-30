using System.Collections;
using Core.Collections;
using Core.Enumerables;

namespace Core.WinForms;

public class RectangleRow(Rectangle clientRectangle, RectangleAlignment alignment = RectangleAlignment.Left, int padding = 4)
   : IEnumerable<Rectangle>, IHash<string, Rectangle>
{
   protected RectangleAlignment alignment = alignment;
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

   public Rectangle ClientRectangle => clientRectangle;

   public bool Add(Rectangle rectangle)
   {
      row.Add(rectangle);
      return arrange();
   }

   public bool AddRange(IEnumerable<Rectangle> rectangles)
   {
      foreach (var rectangle in rectangles)
      {
         row.Add(rectangle);
      }

      return arrange();
   }

   public bool Add(Rectangle rectangle, string key)
   {
      row.Add(rectangle);
      rectangles[key] = rectangle;

      return arrange();
   }

   public void Add(Size size) => Add(new Rectangle(Point.Empty, size));

   public bool AddRange(IEnumerable<Size> sizes)
   {
      foreach (var size in sizes)
      {
         row.Add(new Rectangle(Point.Empty, size));
      }

      return arrange();
   }

   public void Add(Size size, string key) => Add(new Rectangle(Point.Empty, size), key);

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
      var left = padding;
      foreach (var rectangle in row)
      {
         var newRectangle = rectangle with { X = left, Y = top };
         yield return newRectangle;

         left += newRectangle.Width + padding;
      }
   }

   protected IEnumerable<Rectangle> rightEnumerable()
   {
      var right = width;

      foreach (var rectangle in row.Reversed())
      {
         var left = right - rectangle.Width - padding;
         var newRectangle = rectangle with { X = left, Y = top };
         yield return newRectangle;

         right = left;
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

   protected bool arrange()
   {
      if (rearrange)
      {
         return alignment switch
         {
            RectangleAlignment.Left => arrangeLeft(),
            RectangleAlignment.Right => arrangeRight(),
            RectangleAlignment.Center => arrangeCenter(),
            RectangleAlignment.Spread => arrangeSpread(),
            _ => true
         };
      }
      else
      {
         return false;
      }

      bool arrangeLeft()
      {
         row = [.. leftEnumerable()];
         return row.LastOrNone().Map(clientRectangle.Contains) | false;
      }

      bool arrangeRight()
      {
         row = [..rightEnumerable().Reversed()];
         return row.LastOrNone().Map(clientRectangle.Contains) | false;
      }

      bool arrangeCenter()
      {
         row = [.. centerEnumerable()];
         return row.LastOrNone().Map(clientRectangle.Contains) | false;
      }

      bool arrangeSpread()
      {
         row = [..spreadEnumerable()];
         return row.LastOrNone().Map(clientRectangle.Contains) | false;
      }
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
            return rightEnumerable(right).Reversed().LastOrNone().Map(contains) | false;
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
      return new RectangleRow(nextRectangle, alignment, padding);
   }

   public int Count() => row.Count;
}