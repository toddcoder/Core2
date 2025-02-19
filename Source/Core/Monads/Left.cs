﻿using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Left<TLeft, TRight> : Either<TLeft, TRight>, IEquatable<Left<TLeft, TRight>> where TLeft : notnull where TRight : notnull
{
   public static implicit operator TLeft(Left<TLeft, TRight> left) => left.Value;

   protected TLeft value;

   internal Left(TLeft value) => this.value = value;

   public TLeft Value => value;

   public override Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, TLeftResult> leftMap,
      Func<TRight, TRightResult> rightMap)
   {
      return new Left<TLeftResult, TRightResult>(leftMap(value));
   }

   public override Either<TLeftResult, TRightResult> Map<TLeftResult, TRightResult>(Func<TLeft, Either<TLeftResult, TRightResult>> leftMap,
      Func<TRight, Either<TLeftResult, TRightResult>> rightMap)
   {
      return leftMap(value);
   }

   public override void Deconstruct(out bool isLeft, out TLeft left, out TRight right)
   {
      isLeft = true;
      left = value;
      right = default!;
   }

   public override void Deconstruct(out Maybe<TLeft> left, out Maybe<TRight> right)
   {
      left = value;
      right = nil;
   }

   public override object ToObject() => value;

   public bool Equals(Left<TLeft, TRight>? other) => other is not null && EqualityComparer<TLeft>.Default.Equals(value, other.value);

   public override bool Equals(object? obj) => obj is Left<TLeft, TRight> other && Equals(other);

   public override int GetHashCode() => EqualityComparer<TLeft>.Default.GetHashCode(value);

   public override string ToString() => value.ToString() ?? "";
}