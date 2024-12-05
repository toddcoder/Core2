namespace Core.WinForms.Controls;

public abstract record UseEmojis
{
   public sealed record True : UseEmojis;

   public sealed record False : UseEmojis;
}