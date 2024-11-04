using Core.Monads;

namespace Core.WinForms.Controls;

public class ReadOnlyAlternateWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling,
   bool useEmojis) : AlternateWriter(uiAction, alternates, autoSizeText, _floor, _ceiling, useEmojis)
{
   protected override void onPaint(Graphics g, int index, Rectangle rectangle, UiActionWriter writer, string alternate)
   {
      writer.Font = getFont(index);
      writer.Color = Color.White;
      var backColor = Color.CadetBlue;
      fillRectangle(g, rectangle, backColor);

      writer.Rectangle = rectangle;

      writer.Write(g, alternate, uiAction.Type is UiActionType.NoStatus);
   }
}