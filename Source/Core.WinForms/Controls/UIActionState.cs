namespace Core.WinForms.Controls;

public abstract record UIActionState(UiActionType Type)
{
   public sealed record None() : UIActionState(UiActionType.Uninitialized);

   public sealed record Standard(string Message, UiActionType Type) : UIActionState(Type);

   public sealed record Display(string Message, Color ForeColor, Color BackColor) : UIActionState(UiActionType.Display);

   public sealed record Error(Exception Exception) : UIActionState(UiActionType.Exception);

   public sealed record Tape() : UIActionState(UiActionType.Tape);

   public sealed record Button(string Message, UiActionButtonType ButtonType) : UIActionState(UiActionType.Button);

   public sealed record SymbolType(UiActionSymbol Symbol, Color ForeColor, Color BackColor) : UIActionState(UiActionType.Symbol);

   public sealed record Alternate(string[] Alternates) : UIActionState(UiActionType.Alternate);

   public sealed record AlternateReadOnly(string[] Alternates) : UIActionState(UiActionType.ReadOnlyAlternate);

   public sealed record CheckBox(string Message, bool IsChecked) : UIActionState(UiActionType.CheckBox);
}