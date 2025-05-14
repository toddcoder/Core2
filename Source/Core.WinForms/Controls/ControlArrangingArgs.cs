namespace Core.WinForms.Controls;

public record ControlArrangingArgs<TControl>(TControl Control, Size Size, int Index) where TControl : Control;