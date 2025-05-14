namespace Core.WinForms.Controls;

public record ControlFocusArgs<TControl>(TControl Control, int Index) where TControl : Control;