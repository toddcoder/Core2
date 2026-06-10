using System.Drawing.Drawing2D;
using Core.WinForms.Controls;

namespace Core.WinForms;

public static class GraphicsExtensions
{
   extension(Rectangle rectangle)
   {
      public GraphicsPath Rounded(int radius)
      {
         var diameter = radius * 2;
         var size = new Size(diameter, diameter);
         var arc = new Rectangle(rectangle.Location, size);
         var path = new GraphicsPath();

         if (radius == 0)
         {
            path.AddRectangle(rectangle);
            return path;
         }

         path.AddArc(arc, 180, 90);

         arc.X = rectangle.Right - diameter;
         path.AddArc(arc, 270, 90);

         arc.Y = rectangle.Bottom - diameter;
         path.AddArc(arc, 0, 90);

         arc.X = rectangle.Left;
         path.AddArc(arc, 90, 90);

         path.CloseFigure();
         return path;
      }

      public Rectangle Resize(int widthAmount, int heightAmount)
      {
         return rectangle with { Size = rectangle.Size.Resize(widthAmount, heightAmount) };
      }

      public Rectangle OffsetWidth(int amount) => rectangle with { Size = rectangle.Size.OffsetWidth(amount) };

      public Rectangle OffsetHeight(int amount) => rectangle with { Size = rectangle.Size.OffsetHeight(amount) };

      public Rectangle Reposition(int xOffset, int yOffset)
      {
         return rectangle with { Location = rectangle.Location, X = rectangle.Left + xOffset, Y = rectangle.Top + yOffset };
      }

      public Rectangle OffsetX(int xOffset) => rectangle with { Location = rectangle.Location.OffsetX(xOffset) };

      public Rectangle OffsetY(int yOffset) => rectangle with { Location = rectangle.Location.OffsetY(yOffset) };

      public Rectangle Align(Rectangle outerRectangle, CardinalAlignment alignment, int xMargin = 0,
         int yMargin = 0)
      {
         return alignment switch
         {
            CardinalAlignment.NorthWest => rectangle with { X = west(), Y = north() },
            CardinalAlignment.North => rectangle with { X = centerX(), Y = north() },
            CardinalAlignment.NorthEast => rectangle with { X = east(), Y = north() },
            CardinalAlignment.East => rectangle with { X = east(), Y = centerY() },
            CardinalAlignment.SouthEast => rectangle with { X = east(), Y = south() },
            CardinalAlignment.South => rectangle with { X = centerX(), Y = south() },
            CardinalAlignment.SouthWest => rectangle with { X = west(), Y = south() },
            CardinalAlignment.West => rectangle with { X = west(), Y = centerY() },
            CardinalAlignment.Center => rectangle with { X = centerX(), Y = centerY() },
            _ => rectangle
         };

         int north() => outerRectangle.Y + yMargin;

         int south() => outerRectangle.Bottom - rectangle.Height - yMargin;

         int west() => outerRectangle.X + xMargin;

         int east() => outerRectangle.Right - rectangle.Width - xMargin;

         int centerX() => outerRectangle.X + (outerRectangle.Width - rectangle.Width) / 2;

         int centerY() => outerRectangle.Y + (outerRectangle.Height - rectangle.Height) / 2;
      }

      public Point North(int xOffset = 0) => new(rectangle.Left + rectangle.Width / 2, rectangle.Top + xOffset);

      public Point North(int xOffset, int yOffset)
      {
         return new Point(rectangle.Left + rectangle.Width / 2 + xOffset, rectangle.Top + yOffset);
      }

      public Point NorthEast(int xOffset = 0) => new(rectangle.Right - xOffset, rectangle.Top + xOffset);

      public Point NorthEast(int xOffset, int yOffset) => new(rectangle.Right + xOffset, rectangle.Top + yOffset);
      public Point East(int xOffset = 0) => new(rectangle.Right - xOffset, rectangle.Top + rectangle.Height / 2);

      public Point East(int xOffset, int yOffset)
      {
         return new Point(rectangle.Right + xOffset, rectangle.Top + rectangle.Height / 2 + yOffset);
      }

      public Point SouthEast(int xOffset = 0) => new(rectangle.Right - xOffset, rectangle.Bottom - xOffset);
      public Point SouthEast(int xOffset, int yOffset) => new(rectangle.Right + xOffset, rectangle.Bottom + yOffset);
      public Point South(int xOffset = 0) => new(rectangle.Left + rectangle.Width / 2, rectangle.Bottom - xOffset);

      public Point South(int xOffset, int yOffset)
      {
         return new Point(rectangle.Left + rectangle.Width / 2 + xOffset, rectangle.Bottom + yOffset);
      }

      public Point SouthWest(int xOffset = 0) => new(rectangle.Left + xOffset, rectangle.Bottom - xOffset);

      public Point SouthWest(int xOffset, int yOffset) => new(rectangle.Left + xOffset, rectangle.Bottom + yOffset);
      public Point West(int xOffset = 0) => new(rectangle.Left + xOffset, rectangle.Top + rectangle.Height / 2);

      public Point West(int xOffset, int yOffset)
      {
         return new Point(rectangle.Left + xOffset, rectangle.Top + rectangle.Height / 2 + yOffset);
      }

      public Point NorthWest(int xOffset = 0) => new(rectangle.Left + xOffset, rectangle.Top + xOffset);

      public Point NorthWest(int xOffset, int yOffset) => new(rectangle.Left + xOffset, rectangle.Top + yOffset);

      public Rectangle Shrink(int offset = 0)
      {
         return rectangle.Reposition(offset, offset).Resize(-2 * offset, -2 * offset);
      }

      public Rectangle Expand(int offset = 0)
      {
         return rectangle.Reposition(-offset, -offset).Resize(2 * offset, 2 * offset);
      }

      public Rectangle RightOf(Size size, int offset = 0)
      {
         return rectangle with { X = rectangle.X + rectangle.Width + offset, Size = size };
      }

      public Rectangle RightOf(Rectangle referenceRectangle, int offset = 0)
      {
         return rectangle with { X = referenceRectangle.X + referenceRectangle.Width + offset };
      }

      public Rectangle LeftOf(Size size, int offset = 0)
      {
         return rectangle with { X = rectangle.Width - size.Width - offset };
      }

      public Rectangle LeftOf(Rectangle referenceRectangle, int offset = 0)
      {
         return rectangle with { X = referenceRectangle.X - rectangle.Width - offset };
      }

      public Rectangle BottomOf(Size size, int offset = 0)
      {
         return rectangle with { Y = rectangle.Y + rectangle.Height + offset, Size = size };
      }

      public Rectangle BottomOf(Rectangle referenceRectangle, int offset = 0)
      {
         return rectangle with { Y = referenceRectangle.Top + referenceRectangle.Height + offset };
      }

      public Rectangle TopOf(Size size, int offset = 0)
      {
         return rectangle with { Height = rectangle.Height - size.Height - offset };
      }

      public Rectangle TopOf(Rectangle referenceRectangle, int offset = 0)
      {
         return rectangle with { Y = referenceRectangle.Top - rectangle.Width - offset };
      }

      public Rectangle AlignToTop(Rectangle referenceRectangle)
      {
         return rectangle with { Y = referenceRectangle.Top };
      }

      public Rectangle AlignToBottom(Rectangle referenceRectangle)
      {
         return rectangle with { Y = referenceRectangle.Bottom };
      }

      public Rectangle AlignToLeft(Rectangle referenceRectangle)
      {
         return rectangle with { X = referenceRectangle.Left };
      }

      public Rectangle AlignToRight(Rectangle referenceRectangle)
      {
         return rectangle with { X = referenceRectangle.Right };
      }
   }

   extension(Graphics graphics)
   {
      public void HighQuality()
      {
         graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
         graphics.SmoothingMode = SmoothingMode.AntiAlias;
      }

      public void SetPixel(int x, int y, Color color)
      {
         using var bitmap = new Bitmap(1, 1);
         bitmap.SetPixel(0, 0, color);
         graphics.DrawImageUnscaled(bitmap, x, y);
      }

      public void FillRectangle(Color color, Rectangle rectangle)
      {
         using var brush = new SolidBrush(color);
         graphics.FillRectangle(brush, rectangle);
      }

      public void DrawRectangle(Color color, Rectangle rectangle, DashStyle dashStyle = DashStyle.Solid, float width = 1f)
      {
         using var pen = new Pen(color, width);
         pen.DashStyle = dashStyle;
         graphics.DrawRectangle(pen, rectangle);
      }
   }

   extension(Size size)
   {
      public Size Resize(int widthAmount, int heightAmount) => new(size.Width + widthAmount, size.Height + heightAmount);

      public Size OffsetWidth(int amount) => size with { Width = size.Width + amount };

      public Size OffsetHeight(int amount) => size with { Height = size.Height + amount };

      public Rectangle North(Rectangle outerRectangle, int xMargin = 0, int yMargin = 0)
      {
         return new Rectangle(Point.Empty, size).Align(outerRectangle, CardinalAlignment.North, xMargin, yMargin);
      }

      public Rectangle NorthEast(Rectangle outerRectangle, int xMargin = 0, int yMargin = 0)
      {
         return new Rectangle(Point.Empty, size).Align(outerRectangle, CardinalAlignment.NorthEast, xMargin, yMargin);
      }

      public Rectangle East(Rectangle outerRectangle, int xMargin = 0, int yMargin = 0)
      {
         return new Rectangle(Point.Empty, size).Align(outerRectangle, CardinalAlignment.East, xMargin, yMargin);
      }

      public Rectangle SouthEast(Rectangle outerRectangle, int xMargin = 0, int yMargin = 0)
      {
         return new Rectangle(Point.Empty, size).Align(outerRectangle, CardinalAlignment.SouthEast, xMargin, yMargin);
      }

      public Rectangle South(Rectangle outerRectangle, int xMargin = 0, int yMargin = 0)
      {
         return new Rectangle(Point.Empty, size).Align(outerRectangle, CardinalAlignment.South, xMargin, yMargin);
      }

      public Rectangle SouthWest(Rectangle outerRectangle, int xMargin = 0, int yMargin = 0)
      {
         return new Rectangle(Point.Empty, size).Align(outerRectangle, CardinalAlignment.SouthWest, xMargin, yMargin);
      }

      public Rectangle West(Rectangle outerRectangle, int xMargin = 0, int yMargin = 0)
      {
         return new Rectangle(Point.Empty, size).Align(outerRectangle, CardinalAlignment.West, xMargin, yMargin);
      }

      public Rectangle NorthWest(Rectangle outerRectangle, int xMargin = 0, int yMargin = 0)
      {
         return new Rectangle(Point.Empty, size).Align(outerRectangle, CardinalAlignment.NorthWest, xMargin, yMargin);
      }

      public Rectangle Rectangle(Rectangle outerRectangle, CardinalAlignment alignment, int xMargin = 0, int yMargin = 0)
      {
         var location = alignment switch
         {
            CardinalAlignment.NorthWest => new Point(west(), north()),
            CardinalAlignment.North => new Point(centerX(), north()),
            CardinalAlignment.NorthEast => new Point(east(), north()),
            CardinalAlignment.East => new Point(east(), centerY()),
            CardinalAlignment.SouthEast => new Point(east(), south()),
            CardinalAlignment.South => new Point(centerX(), south()),
            CardinalAlignment.SouthWest => new Point(west(), south()),
            CardinalAlignment.West => new Point(west(), centerY()),
            CardinalAlignment.Center => new Point(centerX(), centerY()),
            _ => outerRectangle.Location
         };
         return new Rectangle(location, size);

         int north() => outerRectangle.Y + yMargin;

         int south() => outerRectangle.Bottom - size.Height - yMargin;

         int west() => outerRectangle.X + xMargin;

         int east() => outerRectangle.Right - size.Width - xMargin;

         int centerX() => outerRectangle.X + (outerRectangle.Width - size.Width) / 2;

         int centerY() => outerRectangle.Y + (outerRectangle.Height - size.Height) / 2;
      }
   }

   extension(Point point)
   {
      public Point Reposition(int xOffset, int yOffset) => new(point.X + xOffset, point.Y + yOffset);

      public Point OffsetX(int xOffset) => point with { X = point.X + xOffset };

      public Point OffsetY(int yOffset) => point with { Y = point.Y + yOffset };
   }

   extension(Color color)
   {
      public Color WithAlpha(int alpha) => Color.FromArgb(alpha, color);

      public Color OffSet(int red, int green, int blue) =>
         Color.FromArgb(color.A, color.R + red, color.G + green, color.B + blue);
   }
}