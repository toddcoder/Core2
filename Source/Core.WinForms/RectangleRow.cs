using System.Collections;
using Core.Enumerables;

namespace Core.WinForms;

public class RectangleRow(Rectangle clientRectangle, RectangleAlignment alignment = RectangleAlignment.Left, int padding = 4) : IEnumerable<Rectangle>
{
   protected RectangleAlignment alignment = alignment;
   protected int top = clientRectangle.Top + padding;
   protected int width = clientRectangle.Width;
   protected List<Rectangle> row = [];
   protected bool rearrange = true;

   public RectangleAlignment Alignment
   {
      get => alignment;
      set
      {
         alignment = value;
         arrange();
      }
   }

   public bool Add(Rectangle rectangle)
   {
      row.Add(rectangle);
      return arrange();
   }

   public void Add(Size size) => Add(new Rectangle(Point.Empty, size));

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
         row = [..rightEnumerable()];
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

   public bool MayContain()
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

         bool leftMayContain() => leftEnumerable().FirstOrNone().Map(clientRectangle.Contains) | false;

         bool rightMayContain() => rightEnumerable().FirstOrNone().Map(clientRectangle.Contains) | false;

         bool centerMayContain() => centerEnumerable().FirstOrNone().Map(clientRectangle.Contains) | false;

         bool spreadMayContain() => spreadEnumerable().FirstOrNone().Map(clientRectangle.Contains) | false;
      }
   }

   public IEnumerator<Rectangle> GetEnumerator() => row.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}