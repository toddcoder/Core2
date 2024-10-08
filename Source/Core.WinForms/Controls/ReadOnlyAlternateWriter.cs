using Core.Monads;

namespace Core.WinForms.Controls;

public class ReadOnlyAlternateWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling,
   bool useEmojis) : AlternateWriter(uiAction, alternates, autoSizeText, _floor, _ceiling, useEmojis)
{
   protected override (int penSize, Rectangle textRectangle, Rectangle smallRectangle) splitRectangle(Rectangle rectangle)
   {
      var penSize = getPenSize(rectangle);
      return (penSize, rectangle, Rectangle.Empty);
   }

   protected override void onPaint(Graphics g, int index, Rectangle rectangle, UiActionWriter writer, string alternate)
   {
      var (_, textRectangle, _) = splitRectangle(rectangle);

      writer.Font = uiAction.NonNullFont;
      writer.Color = GetAlternateForeColor(index);
      var backColor = GetAlternateBackColor(index);
      fillRectangle(g, rectangle, backColor);

      writer.Rectangle = textRectangle;

      writer.Write(g, alternate, uiAction.Type is UiActionType.NoStatus);
   }
}