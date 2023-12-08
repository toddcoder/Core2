using System.Diagnostics;
using Core.Monads;
using static Core.Dates.DateTimeExtensions;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ProgressDefiniteProcessor
{
   protected Rectangle percentRectangle;
   protected Rectangle textRectangle;
   protected Font font;
   protected Maybe<Stopwatch> _stopwatch;
   protected Maybe<SubText> _subText;

   public ProgressDefiniteProcessor(Font font, Graphics graphics, Rectangle clientRectangle, Maybe<UiAction> _uiAction)
   {
      this.font = font;

      percentRectangle = getPercentRectangle(graphics, clientRectangle);
      textRectangle = getTextRectangle(clientRectangle);

      if (_uiAction is (true, var uiAction))
      {
         _stopwatch = new Stopwatch();
         _subText = uiAction.SubText("").Set.FontSize(8).Invert().Alignment(CardinalAlignment.SouthWest).SubText;
      }
      else
      {
         _stopwatch = nil;
         _subText = nil;
      }
   }

   public Rectangle PercentRectangle => percentRectangle;

   public Rectangle TextRectangle => textRectangle;

   protected Rectangle getPercentRectangle(Graphics graphics, Rectangle clientRectangle)
   {
      var size = TextRenderer.MeasureText(graphics, "100%", font);
      size = size with { Height = clientRectangle.Height };

      return new Rectangle(clientRectangle.Location, size);
   }

   protected Rectangle getTextRectangle(Rectangle clientRectangle)
   {
      return clientRectangle with { X = clientRectangle.X + percentRectangle.Width, Width = clientRectangle.Width - percentRectangle.Width };
   }

   public void OnPaint(int percent)
   {
      if (_subText is (true, var subText))
      {
         var timeToGo = TimeToGo(percent);
         subText.Text = timeToGo;
      }
   }

   public void OnPaintBackground(Graphics graphics)
   {
      if (_stopwatch is (true, { IsRunning: false } stopwatch))
      {
         stopwatch.Start();
      }

      using var percentBrush = new SolidBrush(Color.LightSteelBlue);
      graphics.FillRectangle(percentBrush, percentRectangle);
   }

   public string TimeToGo(int percent)
   {
      if (percent == 0)
      {
         return "Unknown";
      }
      else if (_stopwatch is (true, var stopwatch))
      {
         var elapsed = stopwatch.Elapsed;
         var totalMilliseconds = elapsed.TotalMilliseconds;
         var millisecondsPerPercent = totalMilliseconds / percent;
         var remainingMilliseconds = millisecondsPerPercent * (100 - percent);

         return remainingMilliseconds.DescriptionToGo();
      }
      else
      {
         return "";
      }
   }
}