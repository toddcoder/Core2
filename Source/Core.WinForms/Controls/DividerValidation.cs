namespace Core.WinForms.Controls;

public abstract record DividerValidation
{
   public sealed record None : DividerValidation;

   public sealed record Valid : DividerValidation;

   public sealed record Invalid(string Message) : DividerValidation;

   public sealed record Failure(string Message) : DividerValidation;

   public sealed record Error(Exception Exception) : DividerValidation;

   public static DividerValidation operator &(DividerValidation left, DividerValidation right) => left is Valid ? right : left;

   public static DividerValidation operator |(DividerValidation left, DividerValidation right) => left is Valid ? left : right;

   public static DividerValidation IsValid(bool value, string message) => value ? new Valid() : new Invalid(message);

   public static DividerValidation IsValid(bool value, Func<string> message) => value ? new Valid() : new Invalid(message());
}