using System.Drawing.Drawing2D;

namespace Core.WinForms.Controls;

public class Clickable
{
   protected bool mouseInside;
   protected bool mouseDown;

   public bool ClickGlyph { get; set; } = true;

   public bool ShowFocus { get; set; } = true;

   public void OnPaint(Control control, PaintEventArgs e, Rectangle rectangle, Color color)
   {
      if (ClickGlyph)
      {
         using var pen = new Pen(color, 4);
         e.Graphics.DrawLine(pen, rectangle.Right - 4, 4, rectangle.Right - 4, rectangle.Bottom - 4);

         if (mouseInside || mouseDown)
         {
            using var dashedPen = new Pen(color, 1);
            dashedPen.DashStyle = DashStyle.Dash;
            var dashedRectangle = rectangle;
            dashedRectangle.Inflate(-2, -2);
            e.Graphics.DrawRectangle(dashedPen, dashedRectangle);
         }
      }

      if (ShowFocus && control.Focused)
      {
         using var dashedPen = new Pen(color, 2);
         dashedPen.DashStyle = DashStyle.Dot;
         var dashedRectangle = rectangle;
         dashedRectangle.Inflate(-8, -8);
         e.Graphics.DrawRectangle(dashedPen, dashedRectangle);
      }
   }

   public void OnMouseEnter(Control control)
   {
      if (!mouseInside)
      {
         mouseInside = true;
         control.Invalidate();
      }
   }

   public void OnMouseLeave(Control control)
   {
      if (mouseInside)
      {
         mouseInside = false;
         control.Invalidate();
      }
   }

   public void OnMouseDown(Control control)
   {
      if (!mouseDown)
      {
         mouseDown = true;
         control.Invalidate();
      }
   }

   public void OnMouseUp(Control control)
   {
      if (mouseDown)
      {
         mouseDown = false;
         control.Invalidate();
      }
   }
}