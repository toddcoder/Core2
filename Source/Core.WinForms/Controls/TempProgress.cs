using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class TempProgress : TempMessage
{
   protected int value = -1;

   public Maybe<int> Maximum { get; set; } = nil;

   public void Progress(string message)
   {
      this.message = message;
      value++;
      Invalidate();
   }

   protected override void OnPaint(PaintEventArgs e)
   {
      if (Maximum is (true, var maximum))
      {
         if (value <= maximum)
         {
            e.Graphics.HighQuality();

            var percentage = (float)value / maximum;
            var width = (int)(ClientRectangle.Width * percentage);
            var greenRectangle = ClientRectangle with { Width = width };
            var text = message.IsNotEmpty() ? message : $"{(int)(percentage * 100)}%";

            using var redBrush = new SolidBrush(Color.Red);
            e.Graphics.FillRectangle(redBrush, ClientRectangle);

            using var greenBrush = new SolidBrush(Color.Green);
            e.Graphics.FillRectangle(greenBrush, greenRectangle);

            var writer = new ControlWriter
            {
               Color = Color.White,
               Font = Font,
               Rectangle = ClientRectangle,
               UseEmojis = UseEmojis,
               AutoSizeText = AutoSizeText
            };
            writer.Write(e.Graphics, text);
         }
         else
         {
            Display("");
         }
      }
      else
      {
         base.OnPaint(e);
      }
   }

   protected override void OnPaintBackground(PaintEventArgs e)
   {
      if (Maximum)
      {
      }
      else
      {
         base.OnPaintBackground(e);
      }
   }

   public void Reset()
   {
      value = -1;
      Maximum = nil;
   }
}