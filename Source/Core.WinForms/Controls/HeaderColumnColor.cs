namespace Core.WinForms.Controls;

public abstract record HeaderColumnColor
{
   public sealed record ForeColor(Color Color) : HeaderColumnColor;

   public sealed record BackColor(Color Color) : HeaderColumnColor;
}