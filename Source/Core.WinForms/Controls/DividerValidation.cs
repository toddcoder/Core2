namespace Core.WinForms.Controls;

public abstract record DividerValidation
{
   public sealed record None : DividerValidation;

   public sealed record Valid : DividerValidation;

   public sealed record Invalid(string Message) : DividerValidation;

   public sealed record Failure(string Message) : DividerValidation;

   public sealed record Error(Exception Exception) : DividerValidation;
}