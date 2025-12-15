using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class DisabledWriter : UiActionWriter
{
   public static DisabledWriter FromUiAction(UiAction uiAction, bool useEmojis)
   {
      return new DisabledWriter(uiAction.MessageAlignment, uiAction.AutoSizeText, useEmojis)
      {
         Rectangle = uiAction.ClientRectangle,
         Font = new Font(uiAction.NonNullFont, FontStyle.Italic | FontStyle.Bold),
         Color = Color.White,
         EmptyTextTitle = uiAction.EmptyTextTitle,
         IsPath = uiAction.IsPath,
         Required = uiAction.Required
      };
   }
   public DisabledWriter(CardinalAlignment messageAlignment, bool autoSizeText, bool useEmojis)
      : base(messageAlignment, autoSizeText, nil, nil, UiActionButtonType.Normal, useEmojis, false)
   {
   }

   public static void OnPaintBackground(Graphics g, Rectangle clientRectangle) => g.FillRectangle(SystemBrushes.ControlDark, clientRectangle);

   public override Result<Unit> Write(Graphics g, string text, bool lower) => base.Write(g, text, true);
}