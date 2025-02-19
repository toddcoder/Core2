using System;

namespace Core.Monads;

public abstract class Either<TLeft, TRight> where TLeft : notnull where TRight : notnull
{
   public static implicit operator Either<TLeft, TRight>(TLeft value) => new Left<TLeft, TRight>(value);

   public static implicit operator Either<TLeft, TRight>(TRight value) => new Right<TLeft, TRight>(value);

   public static TLeft operator |(Either<TLeft, TRight> either, TLeft defaultValue) => either is (true, var left, _) ? left : defaultValue;

   public static TLeft operator |(Either<TLeft, TRight> either, Func<TLeft> defaultValue) => either is (true, var left, _) ? left : defaultValue();

   public static TRight operator |(Either<TLeft, TRight> either, TRight defaultValue) => either is (false, _, var right) ? right : defaultValue;

   public static TRight operator |(Either<TLeft, TRight> either, Func<TRight> defaultValue)
   {
      return either is (false, _, var right) ? right : defaultValue();
   }

   public abstract Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, TLeftResult> leftMap,
      Func<TRight, TRightResult> rightMap) where TLeftResult : notnull where TRightResult : notnull;

   public abstract Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, Either<TLeftResult, TRightResult>> leftMap,
      Func<TRight, Either<TLeftResult, TRightResult>> rightMap) where TLeftResult : notnull where TRightResult : notnull;

   public abstract void Deconstruct(out bool isLeft, out TLeft left, out TRight right);

   public abstract void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right);

   public abstract object ToObject();
}