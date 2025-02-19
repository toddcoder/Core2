using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Right<TLeft, TRight> : Either<TLeft, TRight>, IEquatable<Right<TLeft, TRight>> where TLeft : notnull where TRight : notnull
{
   public static implicit operator TRight(Right<TLeft, TRight> right) => right.Value;

   protected TRight value;

   internal Right(TRight value) => this.value = value;

   public TRight Value => value;

   public override Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, TLeftResult> leftMap,
      Func<TRight, TRightResult> rightMap)
   {
      return new Right<TLeftResult, TRightResult>(rightMap(value));
   }

   public override Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, Either<TLeftResult, TRightResult>> leftMap,
      Func<TRight, Either<TLeftResult, TRightResult>> rightMap)
   {
      return rightMap(value);
   }

   public override void Deconstruct(out bool isLeft, out TLeft left, out TRight right)
   {
      isLeft = false;
      left = default!;
      right = value;
   }

   public override void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right)
   {
      left = nil;
      right = value;
   }

   public override object ToObject() => value;

   public bool Equals(Right<TLeft, TRight>? other) => other is not null && EqualityComparer<TRight>.Default.Equals(value, other.value);

   public override bool Equals(object? obj) => obj is Right<TLeft, TRight> other && Equals(other);

   public override int GetHashCode() => EqualityComparer<TRight>.Default.GetHashCode(value);

   public override string ToString() => value.ToString() ?? "";
}