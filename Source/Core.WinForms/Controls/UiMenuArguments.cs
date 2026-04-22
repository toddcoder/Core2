namespace Core.WinForms.Controls;

public record UiMenuArguments(UiMenuItemData Data, int Height, int Width, Color BackgroundColor, Color HoverColor, Color TextColor,
   Color DisabledColor, Color SeparatorColor, Font Font, int IconSize, int IconPadding, int TextPadding);