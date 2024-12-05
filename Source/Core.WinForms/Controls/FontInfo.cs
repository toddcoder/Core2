namespace Core.WinForms.Controls;

public abstract record FontInfo
{
   public sealed record Name(string Value) : FontInfo;

   public sealed record Size(float Value) : FontInfo;

   public sealed record Style(FontStyle Value) : FontInfo;
}