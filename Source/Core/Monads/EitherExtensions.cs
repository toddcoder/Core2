namespace Core.Monads;

public static class EitherExtensions
{
   public static Either<TLeft, TRight> Either<TLeft, TRight>(this TLeft left) where TLeft : notnull where TRight : notnull
   {
      return new Left<TLeft, TRight>(left);
   }

   public static Either<TLeft, TRight> Either<TLeft, TRight>(this TRight right) where TLeft : notnull where TRight : notnull
   {
      return new Right<TLeft, TRight>(right);
   }
}