using System;

namespace Core.Monads;

public abstract class Either<TLeft, TRight> where TLeft : notnull where TRight : notnull
{
   public static implicit operator Either<TLeft, TRight>(TLeft value) => new Left<TLeft, TRight>(value);

   public static implicit operator Either<TLeft, TRight>(TRight value) => new Right<TLeft, TRight>(value);

   public static implicit operator Either<TLeft, TRight>(Nil _) => new Neither<TLeft, TRight>();

   public static TLeft operator |(Either<TLeft, TRight> either, TLeft defaultValue)
   {
      return either is var (_left, _) ? _left | defaultValue : defaultValue;
   }

   public static TLeft operator |(Either<TLeft, TRight> either, Func<TLeft> defaultValue)
   {
      return either is var (_left, _) ? _left | defaultValue : defaultValue();
   }

   public static TRight operator |(Either<TLeft, TRight> either, TRight defaultValue)
   {
      return either is var (_, _right) ? _right | defaultValue : defaultValue;
   }

   public static TRight operator |(Either<TLeft, TRight> either, Func<TRight> defaultValue)
   {
      return either is var (_, _right) ? _right | defaultValue : defaultValue();
   }

   public abstract Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, TLeftResult> leftMap,
      Func<TRight, TRightResult> rightMap) where TLeftResult : notnull where TRightResult : notnull;

   public abstract Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, Either<TLeftResult, TRightResult>> leftMap,
      Func<TRight, Either<TLeftResult, TRightResult>> rightMap) where TLeftResult : notnull where TRightResult : notnull;

   [Obsolete("Use two variable deconstruction")]
   public abstract void Deconstruct(out bool isLeft, out TLeft left, out TRight right);

   public abstract void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right);

   public abstract object ToObject();
}