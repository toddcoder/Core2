namespace Core.WinForms.Forms;

public abstract record ResponseType
{
   public sealed record Positive : ResponseType;

   public sealed record Negative : ResponseType;

   public sealed record Cancel : ResponseType;
}