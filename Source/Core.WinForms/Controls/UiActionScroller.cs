﻿using Core.Collections;
using Core.Numbers;
using Core.Strings;

namespace Core.WinForms.Controls;

public class UiActionScroller
{
   protected Font font;
   protected Rectangle clientRectangle;
   protected Color foreColor;
   protected Color backColor;
   protected int height;
   protected int lineCount;
   protected int currentLine;
   protected int lastLine;
   protected string[] lines;
   protected Memo<string, Size> sizes;

   public UiActionScroller(Font font, Rectangle clientRectangle, Color foreColor, Color backColor)
   {
      this.font = font;
      this.clientRectangle = clientRectangle;
      this.foreColor = foreColor;
      this.backColor = backColor;

      var size = TextRenderer.MeasureText("Wy", font);
      height = size.Height;
      lineCount = clientRectangle.Height / height;
      lastLine = lineCount - 1;
      currentLine = 0;
      lines = [.. 0.Until(lineCount).Select(_ => "")];
      sizes = new Memo<string, Size>.Function(t => TextRenderer.MeasureText(t, this.font));
   }

   protected Size lineSize(string text) => sizes[text];

   protected void advance()
   {
      if (currentLine <= lastLine)
      {
         currentLine++;
      }
      else
      {
         pushUp();
      }
   }

   protected void pushUp()
   {
      for (var i = 0; i < lastLine; i++)
      {
         lines[i] = lines[i + 1];
      }

      lines[lastLine] = "";
      currentLine = lastLine;
   }

   public void WriteLine(object obj)
   {
      lines[currentLine > lastLine ? lastLine : currentLine] = obj.ToNonNullString();
      advance();
   }

   public virtual void OnPaintBackground(Graphics graphics)
   {
      using var brush = new SolidBrush(backColor);
      graphics.FillRectangle(brush, clientRectangle);
   }

   public virtual void OnPaint(Graphics graphics)
   {
      var top = 0;
      foreach (var text in lines)
      {
         var size = lineSize(text);
         using var brush = new SolidBrush(foreColor);
         var point = new Point(0, top);
         TextRenderer.DrawText(graphics, text, font, point, foreColor);
         top += size.Height;
      }
   }
}