﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Assertions.AssertionFunctions;

namespace Core.Assertions.Monads;

public class MaybeAssertion<T> : IAssertion<T> where T : notnull
{
   public static implicit operator bool(MaybeAssertion<T> assertion) => assertion.BeEquivalentToTrue();

   public static bool operator &(MaybeAssertion<T> x, ICanBeTrue y) => and(x, y);

   public static bool operator |(MaybeAssertion<T> x, ICanBeTrue y) => or(x, y);

   protected Maybe<T> maybe;
   protected List<Constraint> constraints;
   protected bool not;
   protected string name;

   public MaybeAssertion(Maybe<T> maybe)
   {
      this.maybe = maybe;
      constraints = [];
      not = false;
      name = "Optional";
   }

   public bool BeEquivalentToTrue() => beEquivalentToTrue(this);

   public T Value => maybe.Required($"{name} value not available");

   public IEnumerable<Constraint> Constraints => constraints;

   public MaybeAssertion<T> Not
   {
      get
      {
         not = true;
         return this;
      }
   }

   protected MaybeAssertion<T> add(object obj, Func<T, bool> constraintFunction, string message)
   {
      switch (obj)
      {
         case null:
            constraints.Add(Constraint.Failing("$name must be non-null", name));
            break;
         case T otherT:
            constraints.Add(new Constraint(() => constraintFunction(otherT), message, not, name, maybeImage(maybe)));
            break;
         case Maybe<T> _value:
            if (_value is (true, var value))
            {
               constraints.Add(new Constraint(() => constraintFunction(value), message, not, name, maybeImage(maybe)));
            }
            else
            {
               constraints.Add(Constraint.Failing("$name not available", name));
            }

            break;
         default:
            constraints.Add(Constraint.Failing($"$name must be of type {typeof(T)}", name));
            break;
      }

      not = false;
      return this;
   }

   protected MaybeAssertion<T> add(Func<bool> constraintFunction, string message)
   {
      constraints.Add(new Constraint(constraintFunction, message, not, name, maybeImage(maybe)));
      not = false;

      return this;
   }

   public MaybeAssertion<T> HaveValue() => add(() => maybe, "$name must $not have a value");

   public MaybeAssertion<T> EqualToValueOf(Maybe<T> otherMaybe)
   {
      return add(() => maybe.EqualToValueOf(otherMaybe), $"Value of $name must $not equal to value of {maybeImage(otherMaybe)}");
   }

   public MaybeAssertion<T> ValueEqualTo(T otherValue)
   {
      return add(() => maybe.ValueEqualTo(otherValue), $"Value of $name must $not equal to {otherValue}");
   }

   public IAssertion<T> Named(string name)
   {
      this.name = name;
      return this;
   }

   public void OrThrow() => orThrow(this);

   public void OrThrow(string message) => orThrow(this, message);

   public void OrThrow(Func<string> messageFunc) => orThrow(this, messageFunc);

   public void OrThrow<TException>(params object[] args) where TException : Exception => orThrow<TException, T>(this, args);

   public T Force() => force(this);

   public T Force(string message) => force(this, message);

   public T Force(Func<string> messageFunc) => force(this, messageFunc);

   public T Force<TException>(params object[] args) where TException : Exception => force<TException, T>(this, args);

   public TResult Force<TResult>() => forceConvert<T, TResult>(this);

   public TResult Force<TResult>(string message) => forceConvert<T, TResult>(this, message);

   public TResult Force<TResult>(Func<string> messageFunc) => forceConvert<T, TResult>(this, messageFunc);

   public TResult Force<TException, TResult>(params object[] args) where TException : Exception
   {
      return forceConvert<T, TimeoutException, TResult>(this);
   }

   public Result<T> OrFailure() => orFailure(this);

   public Result<T> OrFailure(string message) => orFailure(this, message);

   public Result<T> OrFailure(Func<string> messageFunc) => orFailure(this, messageFunc);

   public Maybe<T> OrNone() => orNone(this);

   public Optional<T> OrEmpty() => orEmpty(this);

   public Optional<T> OrFailed() => orFailed(this);

   public Optional<T> OrFailed(string message) => orFailed(this, message);

   public Optional<T> OrFailed(Func<string> messageFunc) => orFailed(this, messageFunc);

   public async Task<Completion<T>> OrFailureAsync(CancellationToken token) => await orFailureAsync(this, token);

   public async Task<Completion<T>> OrFailureAsync(string message, CancellationToken token) => await orFailureAsync(this, message, token);

   public async Task<Completion<T>> OrFailureAsync(Func<string> messageFunc, CancellationToken token)
   {
      return await orFailureAsync(this, messageFunc, token);
   }

   public bool OrReturn() => orReturn(this);
}