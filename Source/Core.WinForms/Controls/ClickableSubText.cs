﻿using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Core.WinForms.Controls;

public class ClickableSubText : SubText
{
   public event EventHandler? Click;

   public ClickableSubText(UiAction subTextHost, string text, int x, int y, Size size, bool clickGlyph, bool chooserGlyph, bool invert = false,
      bool transparentBackground = false) : base(subTextHost, text, x, y, size, clickGlyph, chooserGlyph, invert, transparentBackground)
   {
   }

   public virtual void RaiseClick() => Click?.Invoke(this, EventArgs.Empty);

   public virtual void DrawFocus(Graphics g, Color color, Point mouseLocation)
   {
      var (measuredSize, _, _, font) = TextSize(g);

      try
      {
         var location = new Point(X, Y);
         var rectangle = new Rectangle(location, measuredSize);

         if (rectangle.Contains(mouseLocation))
         {
            var alpha = alphaFromTransparency();
            color = Color.FromArgb(alpha, color);

            g.HighQuality();
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using var pen = new Pen(color);
            pen.DashStyle = DashStyle.Dot;
            g.DrawRectangle(pen, rectangle);
         }
      }
      finally
      {
         font.Dispose();
      }
   }

   public virtual bool Contains(Graphics g, Point mouseLocation)
   {
      var (measuredSize, _, _, font) = TextSize(g);

      try
      {
         var location = new Point(X, Y);
         var rectangle = new Rectangle(location, measuredSize);

         return rectangle.Contains(mouseLocation);
      }
      finally
      {
         font.Dispose();
      }
   }
}