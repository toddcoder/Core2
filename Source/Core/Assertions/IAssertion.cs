﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Monads;

namespace Core.Assertions;

public interface IAssertion<T> : ICanBeTrue where T : notnull
{
   T Value { get; }

   IEnumerable<Constraint> Constraints { get; }

   IAssertion<T> Named(string name);

   void OrThrow();

   void OrThrow(string message);

   void OrThrow(Func<string> messageFunc);

   void OrThrow<TException>(params object[] args) where TException : Exception;

   T Force();

   T Force(string message);

   T Force(Func<string> messageFunc);

   T Force<TException>(params object[] args) where TException : Exception;

   TResult Force<TResult>();

   TResult Force<TResult>(string message);

   TResult Force<TResult>(Func<string> messageFunc);

   TResult Force<TException, TResult>(params object[] args) where TException : Exception;

   Result<T> OrFailure();

   Result<T> OrFailure(string message);

   Result<T> OrFailure(Func<string> messageFunc);

   Maybe<T> OrNone();

   Optional<T> OrEmpty();

   Optional<T> OrFailed();

   Optional<T> OrFailed(string message);

   Optional<T> OrFailed(Func<string> messageFunc);

   Task<Completion<T>> OrFailureAsync(CancellationToken token);

   Task<Completion<T>> OrFailureAsync(string message, CancellationToken token);

   Task<Completion<T>> OrFailureAsync(Func<string> messageFunc, CancellationToken token);

   bool OrReturn();
}