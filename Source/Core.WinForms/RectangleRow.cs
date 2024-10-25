using System.Collections;
using Core.Enumerables;

namespace Core.WinForms;

public class RectangleRow(int top, int width, RectangleAlignment alignment = RectangleAlignment.Left, int padding = 4) : IEnumerable<Rectangle>
{
   protected RectangleAlignment alignment = alignment;
   protected int top = top + padding;
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
         var left = padding;
         var right = 0;
         List<Rectangle> list = [];
         foreach (var rectangle in row)
         {
            var newRectangle = rectangle with { X = left, Y = top };
            list.Add(newRectangle);
            left += newRectangle.Width + padding;
            right = newRectangle.Right;
         }

         row = list;

         return right <= width;
      }

      bool arrangeRight()
      {
         var right = width;

         List<Rectangle> list = [];
         foreach (var rectangle in row.Reversed())
         {
            var left = right - rectangle.Width - padding;
            var newRectangle = rectangle with { X = left, Y = top };
            list.Add(newRectangle);
            right = left;
         }

         row = [.. list.Reversed()];

         return right >= 0;
      }

      bool arrangeCenter()
      {
         var innerWidth = row.Select(r => r.Width).Sum();
         innerWidth += row.Count - 1;
         var remainder = width - innerWidth;
         var left = remainder / 2;
         List<Rectangle> list = [];
         foreach (var rectangle in row)
         {
            var newRectangle = rectangle with { X = left, Y = top };
            list.Add(newRectangle);
            left += newRectangle.Width + padding;
         }

         row = list;

         return remainder >= 0;
      }

      bool arrangeSpread()
      {
         var innerWidth = row.Select(r => r.Width).Sum();
         var newPadding = (width - innerWidth) / (row.Count + 1);
         var left = newPadding;
         List<Rectangle> list = [];
         foreach (var rectangle in row)
         {
            var newRectangle = rectangle with { X = left, Y = top };
            list.Add(newRectangle);
            left += newRectangle.Width + newPadding;
         }

         row = list;

         return newPadding >= 0;
      }
   }

   public IEnumerator<Rectangle> GetEnumerator() => row.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}