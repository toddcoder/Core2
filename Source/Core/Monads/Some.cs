﻿using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Some<T> : Maybe<T>, IEquatable<Some<T>> where T : notnull
{
   public static implicit operator bool(Some<T> _) => true;

   protected T value;

   internal Some(T value) => this.value = value;

   public override Maybe<TResult> Map<TResult>(Func<T, TResult> ifSome) => ifSome(value).Some();

   public override Maybe<TResult> Map<TResult>(Func<T, Maybe<TResult>> ifSome) => ifSome(value);

   public override T Required(string message) => value;

   public override Result<T> Result(string message) => value;

   public override Optional<T> Optional() => value;

   public override void Force(string message)
   {
   }

   public override void Deconstruct(out bool isSome, out T value)
   {
      isSome = true;
      value = this.value;
   }

   public override void MapOf(Action<T> action) => action(value);

   public override bool EqualToValueOf(Maybe<T> otherMaybe) => otherMaybe.Map(ValueEqualTo) | false;

   public override bool ValueEqualTo(T otherValue) => value.Equals(otherValue);

   public override Maybe<TResult> CastAs<TResult>()
   {
      if (value is TResult result)
      {
         return result;
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<T> Where(Predicate<T> predicate) => predicate(value) ? this : nil;

   public override Maybe<TResult> SelectMany<TResult>(Func<T, Result<TResult>> projection)
   {
      var _result = projection(value);
      if (_result is (true, var result))
      {
         return result;
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<T2> SelectMany<T1, T2>(Func<T, Result<T1>> func, Func<T, T1, T2> projection)
   {
      var _t1 = func(value);
      if (_t1 is (true, var t1))
      {
         return projection(value, t1);
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<T2> SelectMany<T1, T2>(Func<T, Optional<T1>> func, Func<T, T1, T2> projection)
   {
      var _t1 = func(value);
      if (_t1 is (true, var t1))
      {
         return projection(value, t1);
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection)
   {
      var _t1 = func(value);
      if (_t1 is (true, var t1))
      {
         return projection(value, t1);
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<T> Initialize(Func<T> initializer) => this;

   public override (T value, Maybe<T> maybe) Create(Func<T> initializer) => (value, this);

   public override (T value, Maybe<T> maybe) Lazy(Func<T> initializer)
   {
      value = initializer();
      return (value, this);
   }

   public override object ToObject() => value;

   public bool Equals(Some<T>? other) => other is not null && EqualityComparer<T>.Default.Equals(value, other.value);

   public override bool Equals(object? obj) => obj is Some<T> other && Equals(other);

   public override int GetHashCode() => value.GetHashCode();

   public override string ToString() => value.ToString() ?? "";
}