using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class DisabledWriter : UiActionWriter
{
   public static DisabledWriter FromUiAction(UiAction uiAction)
   {
      return new DisabledWriter(uiAction.MessageAlignment, uiAction.AutoSizeText)
      {
         Rectangle = uiAction.ClientRectangle,
         Font = new Font(uiAction.Font, FontStyle.Italic | FontStyle.Bold),
         Color = Color.White,
         EmptyTextTitle = uiAction.EmptyTextTitle,
         IsPath = uiAction.IsPath,
         Required = uiAction.Required
      };
   }
   public DisabledWriter(CardinalAlignment messageAlignment, bool autoSizeText)
      : base(messageAlignment, autoSizeText, nil, nil, UiActionButtonType.Normal)
   {
   }

   public static void OnPaintBackground(Graphics g, Rectangle clientRectangle) => g.FillRectangle(SystemBrushes.ControlDark, clientRectangle);

   public override Result<Unit> Write(Graphics g, string text) => base.Write(g, text.ToLower());
}