﻿using System;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Interrupted<T> : Completion<T>, IEquatable<Interrupted<T>> where T : notnull
{
   // ReSharper disable once UnusedParameter.Global
   public static implicit operator bool(Interrupted<T> _) => false;

   internal Exception exception;

   internal Interrupted(Exception exception)
   {
      this.exception = exception is FullStackException ? exception : new FullStackException(exception);
   }

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted) => exception;

   public override Completion<TResult> Map<TResult>(Func<T, TResult> ifCompleted) => exception;

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled)
   {
      return exception;
   }

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Exception, Completion<TResult>> ifInterrupted)
   {
      return ifInterrupted(exception);
   }

   public override Completion<TResult> Map<TResult>(Func<T, Completion<TResult>> ifCompleted, Func<Completion<TResult>> ifCancelled,
      Func<Exception, Completion<TResult>> ifInterrupted)
   {
      return ifInterrupted(exception);
   }

   public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifCancelled, Func<Exception, TResult> ifInterrupted)
   {
      return ifInterrupted(exception);
   }

   public override TResult FlatMap<TResult>(Func<T, TResult> ifCompleted, Func<TResult> ifNotCompleted) => ifNotCompleted();

   public override Completion<T> Map(Action<T> action) => this;

   public override Completion<T> UnMap(Action action) => this;

   public override Completion<T> UnMap(Action<Exception> action)
   {
      action(exception);
      return this;
   }

   public override Completion<T> Do(Action<T> ifCompleted, Action ifNotCompleted)
   {
      ifNotCompleted();
      return this;
   }

   public override Completion<T> Do(Action<T> ifCompleted, Action ifCancelled, Action<Exception> ifInterrupted)
   {
      ifInterrupted(exception);
      return this;
   }

   public override Completion<TResult> SelectMany<TResult>(Func<T, Completion<TResult>> projection) => exception;

   public override Completion<TResult> SelectMany<TResult>(Func<T, Maybe<TResult>> projection) => exception;

   public override Completion<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> projection) => exception;

   public override Completion<T2> SelectMany<T1, T2>(Func<T, Completion<T1>> func, Func<T, T1, T2> projection) => exception;

   public override Completion<T2> SelectMany<T1, T2>(Func<T, Maybe<T1>> func, Func<T, T1, T2> projection) => exception;

   public override Completion<T2> SelectMany<T1, T2>(Func<T, Optional<T1>> func, Func<T, T1, T2> projection) => exception;

   public override Completion<TResult> SelectMany<TResult>(Func<T, TResult> func) => exception;

   public override Completion<TResult> Select<TResult>(Completion<T> result, Func<T, TResult> func) => exception;

   public override bool IfCancelled() => false;

   public override Completion<TOther> NotCompleted<TOther>() => exception;

   public override void Force() => throw exception;

   public override T ForceValue() => throw exception;

   public override Completion<T> CancelledOnly() => throw exception;

   public override Completion<TOther> CancelledOnly<TOther>() => throw exception;

   public override Completion<TOther> NotCompletedOnly<TOther>() => exception;

   public override void Deconstruct(out bool isCompleted, out T value)
   {
      isCompleted = false;
      value = default!;
   }

   public override Completion<T> OnCompleted(Action<T> action) => this;

   public override Completion<T> OnCancelled(Action action) => this;

   public override Completion<T> OnInterrupted(Action<Exception> action)
   {
      action(exception);
      return this;
   }

   public override bool ValueEqualTo(Completion<T> otherCompletion) => false;

   public override bool EqualToValueOf(T otherValue) => false;

   public Completion<object> AsObject() => exception;

   public override Completion<TResult> CastAs<TResult>() => exception;

   public override Completion<T> Where(Predicate<T> predicate) => this;

   public override Completion<T> Where(Predicate<T> predicate, string exceptionMessage) => this;

   public override Completion<T> Where(Predicate<T> predicate, Func<string> exceptionMessage) => this;

   public override T DefaultTo(Func<Maybe<Exception>, T> defaultFunc) => defaultFunc(exception);

   public override Maybe<T> Maybe() => nil;

   public override Result<T> Result() => exception;

   public override Optional<T> Optional() => exception;

   public override Maybe<Exception> Exception => exception;

   public override object ToObject() => exception;

   public override Completion<T> Initialize(Func<T> initializer) => initializer();

   public override void MapOf(Action<T> action)
   {
   }

   public bool Equals(Interrupted<T>? other) => other is not null && Equals(exception, other.Exception);

   public override bool Equals(object? obj) => obj is Interrupted<T> other && Equals(other);

   public override int GetHashCode() => exception.GetHashCode();

   public override string ToString() => $"Interrupted({exception.Message.Elliptical(60, ' ')})";
}