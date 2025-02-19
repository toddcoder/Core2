using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Neither<TLeft, TRight> : Either<TLeft, TRight>, IEquatable<Neither<TLeft, TRight>> where TLeft : notnull where TRight : notnull
{
   public override Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, TLeftResult> leftMap,
      Func<TRight, TRightResult> rightMap) => nil;

   public override Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, Either<TLeftResult, TRightResult>> leftMap,
      Func<TRight, Either<TLeftResult, TRightResult>> rightMap) => nil;

   [Obsolete("Use two variable deconstruction")]
   public override void Deconstruct(out bool isLeft, out TLeft left, out TRight right)
   {
      isLeft = false;
      left = default!;
      right = default!;
   }

   public override void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right)
   {
      left = nil;
      right = nil;
   }

   public override object ToObject() => nil;

   public bool Equals(Neither<TLeft, TRight>? other) => true;

   public override bool Equals(object? obj) => obj is Neither<TLeft, TRight> neither && Equals(neither);

   public override int GetHashCode() => nil.GetHashCode();

   public static bool operator ==(Neither<TLeft, TRight>? left, Neither<TLeft, TRight>? right) => Equals(left, right);

   public static bool operator !=(Neither<TLeft, TRight>? left, Neither<TLeft, TRight>? right) => !Equals(left, right);

   public override string ToString() => $"neither<{typeof(TLeft).Name}, {typeof(TRight).Name}>";
}