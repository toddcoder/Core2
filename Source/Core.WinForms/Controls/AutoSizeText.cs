namespace Core.WinForms.Controls;

public abstract record AutoSizeText
{
   public sealed record True : AutoSizeText;

   public sealed record False : AutoSizeText;
}