using System.Diagnostics;
using Core.Monads;
using Core.Strings;
using static Core.Dates.DateTimeExtensions;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ProgressDefiniteProcessor
{
   protected Font font;
   protected Rectangle percentRectangle;
   protected Rectangle textRectangle;
   protected Maybe<Stopwatch> _stopwatch;
   protected bool showToGo;
   protected bool pendingRectangleChange;

   public ProgressDefiniteProcessor(Font font, Graphics graphics, Rectangle clientRectangle, bool showToGo)
   {
      this.font = font;

      percentRectangle = getPercentRectangle(graphics, clientRectangle, showToGo);
      textRectangle = getTextRectangle(clientRectangle);

      _stopwatch = nil;
      ShowToGo = showToGo;
      pendingRectangleChange = false;
   }

   public Rectangle PercentRectangle => percentRectangle;

   public Rectangle TextRectangle => textRectangle;

   public bool ShowToGo
   {
      get => showToGo;
      set
      {
         var lastShowToGo = showToGo;
         showToGo = value;
         if (!lastShowToGo && !_stopwatch)
         {
            _stopwatch = maybe<Stopwatch>() & showToGo & (() => new Stopwatch());
         }

         pendingRectangleChange = true;
      }
   }

   protected Rectangle getPercentRectangle(Graphics graphics, Rectangle clientRectangle, bool showToGo)
   {
      var text = showToGo ? " 88 wwwwwww to go " : "100%";
      var size = TextRenderer.MeasureText(graphics, text, font);
      size = size with { Height = clientRectangle.Height };

      return new Rectangle(clientRectangle.Location, size);
   }

   protected Maybe<Rectangle> getTextRectangle(Rectangle clientRectangle)
   {
      return clientRectangle with { X = clientRectangle.X + percentRectangle.Width, Width = clientRectangle.Width - percentRectangle.Width };
   }

   public void OnPaint(Graphics g, int percent, Color foreColor, Rectangle clientRectangle)
   {
      if (pendingRectangleChange)
      {
         percentRectangle = getPercentRectangle(g, clientRectangle, showToGo);
         textRectangle = getTextRectangle(clientRectangle);
         pendingRectangleChange = false;
      }

      if (_stopwatch)
      {
         var timeToGo = TimeToGo(percent);
         if (timeToGo.IsNotEmpty())
         {
            var size = UiActionWriter.TextSize(g, timeToGo, font, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            var rectangle = percentRectangle with { Y = percentRectangle.Y + percentRectangle.Height - size.Height, Height = size.Height };
            var stringFormat = new StringFormat
            {
               Alignment = StringAlignment.Center,
               LineAlignment = StringAlignment.Center
            };
            using var brush = new SolidBrush(foreColor);
            g.DrawString(timeToGo, font, brush, rectangle.ToRectangleF(), stringFormat);
         }
      }
   }

   public void OnPaintBackground(Graphics g)
   {
      if (_stopwatch is (true, { IsRunning: false } stopwatch))
      {
         stopwatch.Start();
      }

      using var percentBrush = new SolidBrush(Color.LightSteelBlue);
      g.FillRectangle(percentBrush, percentRectangle);
   }

   public string TimeToGo(int percent) => _stopwatch.Map(sw => sw.Elapsed.DescriptionToGo(percent)) | "";
}