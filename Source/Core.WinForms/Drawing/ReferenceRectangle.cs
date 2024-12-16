using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Drawing;

public class ReferenceRectangle(Rectangle rectangle, Either<Size, Rectangle> referenced, int padding = 2)
{
   protected static Maybe<Size> getPoint(Either<Size, Rectangle> referenced) => referenced.ToObject() is Size size ? size : nil;

   protected static Maybe<Rectangle> getRectangle(Either<Size, Rectangle> referenced) =>
      referenced.ToObject() is Rectangle rectangle ? rectangle : nil;

   protected Rectangle rectangle = rectangle;
   protected Maybe<Size> _referencedSize = getPoint(referenced);
   protected Maybe<Rectangle> _referencedRectangle = getRectangle(referenced);
   public Maybe<Rectangle> _result = nil;

   public Rectangle Rectangle
   {
      get => rectangle;
      set => rectangle = value;
   }

   public Rectangle Result => _result | Rectangle.Empty;

   public Size ReferencedSize
   {
      get => _referencedSize | Size.Empty;
      set
      {
         _referencedSize = value;
         _result = nil;
      }
   }

   public Rectangle ReferencedRectangle
   {
      get => _referencedRectangle | Rectangle.Empty;
      set
      {
         _referencedRectangle = value;
         _result = nil;
      }
   }

   protected ReferenceRectangle move(Func<Rectangle, Rectangle> fromRectangle)
   {
      if (_result is (true, var result))
      {
         _result = fromRectangle(result);
      }

      if (_referencedSize is (true, var referenceSize))
      {
         _result = fromRectangle(new Rectangle(new Point(0, 0), referenceSize));
      }
      else if (_referencedRectangle is (true, var referenceRectangle))
      {
         _result = fromRectangle(referenceRectangle);
      }

      return this;
   }

   protected Rectangle centerHorizontally(Rectangle innerRectangle) => innerRectangle with { X = (rectangle.Width - innerRectangle.Width) / 2 };

   protected Rectangle centerVertically(Rectangle innerRectangle) => innerRectangle with { Y = (rectangle.Height - innerRectangle.Height) / 2 };

   public ReferenceRectangle NorthWest() => move(r => r with { Location = new Point(padding, padding) });

   public ReferenceRectangle North() => move(r => centerHorizontally(r) with { Y = padding });

   public ReferenceRectangle NorthEast() => move(r => r with { X = rectangle.Right - r.Width - padding, Y = padding });

   public ReferenceRectangle West() => move(r => centerVertically(r) with { X = padding });

   public ReferenceRectangle Center() => move(r => centerHorizontally(centerVertically(r)));

   public ReferenceRectangle East() => move(r => centerVertically(r) with { X = rectangle.Right - r.Width - padding });

   public ReferenceRectangle SouthWest() => move(r => r with { X = padding, Y = rectangle.Bottom - r.Height - padding });

   public ReferenceRectangle South() => move(r => centerHorizontally(r) with { Y = rectangle.Bottom - r.Height - padding });

   public ReferenceRectangle SouthEast() => move(r => r with { X = rectangle.Right - r.Width - padding, Y = rectangle.Bottom - r.Height - padding });

   public ReferenceRectangle Up() => move(r => r with { Y = rectangle.Top - r.Height - padding });

   public ReferenceRectangle Down() => move(r => r with { Y = rectangle.Bottom + r.Height + padding });

   public ReferenceRectangle Left() => move(r => r with { X = rectangle.Left - r.Width - padding });

   public ReferenceRectangle Right() => move(r => r with { X = rectangle.Right + r.Width + padding });
}