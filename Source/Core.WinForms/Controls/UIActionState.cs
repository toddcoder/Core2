namespace Core.WinForms.Controls;

public abstract record UiActionState(UiActionType Type)
{
   public sealed record None() : UiActionState(UiActionType.Uninitialized);

   public sealed record Standard(string Message, UiActionType Type) : UiActionState(Type);

   public sealed record Display(string Message, Color ForeColor, Color BackColor) : UiActionState(UiActionType.Display);

   public sealed record Error(Exception Exception) : UiActionState(UiActionType.Exception);

   public sealed record Tape() : UiActionState(UiActionType.Tape);

   public sealed record Button(string Message, UiActionButtonType ButtonType) : UiActionState(UiActionType.Button);

   public sealed record SymbolType(UiActionSymbol Symbol, Color ForeColor, Color BackColor) : UiActionState(UiActionType.Symbol);

   public sealed record Alternate(string[] Alternates) : UiActionState(UiActionType.Alternate);

   public sealed record AlternateReadOnly(string[] Alternates) : UiActionState(UiActionType.ReadOnlyAlternate);

   public sealed record CheckBox(string Message, bool IsChecked) : UiActionState(UiActionType.CheckBox);

   public sealed record Busy(bool BusyState) : UiActionState(UiActionType.Busy);
}