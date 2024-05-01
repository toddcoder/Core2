﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Core.Enumerables;
using Core.Matching;
using Core.Numbers;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public static class MonadExtensions
{
   public static Maybe<T> Some<T>(this T obj) where T : notnull
   {
      if (obj is ITuple tuple)
      {
         for (var i = 0; i < tuple.Length; i++)
         {
            if (tuple[i] is null)
            {
               return nil;
            }
         }

         return new Some<T>(obj);
      }
      else
      {
         return new Some<T>(obj);
      }
   }

   public static Optional<T> Just<T>(this T obj) where T : notnull
   {
      if (obj is ITuple tuple)
      {
         for (var i = 0; i < tuple.Length; i++)
         {
            if (tuple[i] is null)
            {
               return fail("No tuple item can be null");
            }
         }

         return new Just<T>(obj);
      }
      else
      {
         return new Just<T>(obj);
      }
   }

   public static Optional<T> Failed<T>(this string message) where T : notnull => fail(message);

   public static Maybe<T> NotNull<T>(this T? obj) where T : notnull
   {
      if (obj is not null)
      {
         return obj.Some();
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<string> NotEmpty(this string text) => maybe<string>() & text.IsNotEmpty() & text;

   public static Maybe<int> NotNegative(this int number) => maybe<int>() & number > -1 & number;

   public static Maybe<T> Item<T>(this T[] array, int index) where T : notnull
   {
      return maybe<T>() & index.Between(0).Until(array.Length) & (() => array[index]);
   }

   public static Maybe<Type> UnderlyingType(this object? obj)
   {
      if (obj is null)
      {
         return nil;
      }
      else
      {
         var type = obj.GetType();
         return type.UnderlyingTypeOf();
      }
   }

   public static Maybe<Type> UnderlyingTypeOf(this Type type)
   {
      if (type.Name.IsMatch("^ ('Maybe' | 'Some' | 'None') '`1'; f"))
      {
         return type.GetGenericArguments().FirstOrNone();
      }
      else
      {
         return nil;
      }
   }

   [DebuggerStepThrough]
   public static Result<TResult> SelectMany<T, TResult>(this Maybe<T> maybe, Func<T, Result<TResult>> projection) where T : notnull
      where TResult : notnull
   {
      return maybe.Map(projection) | (() => fail("Value not provided"));
   }

   [DebuggerStepThrough]
   public static Result<TResult> Select<T, TResult>(this Result<T> result, Func<T, TResult> func) where T : notnull where TResult : notnull
   {
      if (result is (true, var resultValue))
      {
         return func(resultValue);
      }
      else
      {
         return result.Exception;
      }
   }

   public static Result<T> Success<T>(this T value) where T : notnull
   {
      if (value is ITuple tuple)
      {
         for (var i = 0; i < tuple.Length; i++)
         {
            if (tuple[i] is null)
            {
               return fail("No tuple value can be null");
            }
         }

         return new Success<T>(value);
      }
      else
      {
         return new Success<T>(value);
      }
   }

   public static Result<T> Failure<T>(this string message) where T : notnull => fail(message);

   public static Result<T> Failure<T, TException>(this object firstItem, params object[] args) where T : notnull where TException : Exception
   {
      List<object> list = [firstItem];
      list.AddRange(args);

      return (TException)typeof(TException).Create([.. list])!;
   }

   public static Result<T> Result<T>(this bool test, Func<T> ifFunc, string exceptionMessage) where T : notnull
   {
      if (test)
      {
         return ifFunc();
      }
      else
      {
         return fail(exceptionMessage);
      }
   }

   public static Result<T> Result<T>(this bool test, Func<T> ifFunc, Func<string> exceptionMessage) where T : notnull
   {
      if (test)
      {
         return ifFunc();
      }
      else
      {
         return fail(exceptionMessage());
      }
   }

   public static Result<T> Result<T>(this bool test, Func<Result<T>> ifFunc, string exceptionMessage) where T : notnull
   {
      if (test)
      {
         return ifFunc();
      }
      else
      {
         return fail(exceptionMessage);
      }
   }

   public static Result<T> Result<T>(this bool test, Func<Result<T>> ifFunc, Func<string> exceptionMessage) where T : notnull
   {
      if (test)
      {
         return ifFunc();
      }
      else
      {
         return fail(exceptionMessage());
      }
   }

   public static IEnumerable<T> WhereIsSome<T>(this IEnumerable<Maybe<T>> enumerable) where T : notnull
   {
      foreach (var _maybe in enumerable)
      {
         if (_maybe is (true, var maybe))
         {
            yield return maybe;
         }
      }
   }

   public static IEnumerable<(T item, TMaybe maybe)> WhereIsSome<T, TMaybe>(this IEnumerable<T> enumerable, Func<T, Maybe<TMaybe>> predicate)
      where TMaybe : notnull
   {
      foreach (var item in enumerable)
      {
         var _value = predicate(item);
         if (_value is (true, var value))
         {
            yield return (item, value);
         }
      }
   }

   public static IEnumerable<T> WhereIsSuccessful<T>(this IEnumerable<Result<T>> enumerable) where T : notnull
   {
      foreach (var _result in enumerable)
      {
         if (_result is (true, var result))
         {
            yield return result;
         }
      }
   }

   public static IEnumerable<(T item, TResult result)> WhereIsSuccessful<T, TResult>(this IEnumerable<T> enumerable,
      Func<T, Result<TResult>> predicate) where T : notnull where TResult : notnull
   {
      foreach (var item in enumerable)
      {
         var _value = predicate(item);
         if (_value is (true, var value))
         {
            yield return (item, value);
         }
      }
   }

   public static IEnumerable<Either<T, Exception>> Successful<T>(this IEnumerable<Result<T>> enumerable) where T : notnull
   {
      foreach (var _result in enumerable)
      {
         if (_result is (true, var result))
         {
            yield return result;
         }
         else
         {
            yield return _result.Exception;
         }
      }
   }

   public static IEnumerable<T> WhereIsCompleted<T>(this IEnumerable<Completion<T>> enumerable) where T : notnull
   {
      foreach (var _completion in enumerable)
      {
         if (_completion is (true, var completion))
         {
            yield return completion;
         }
      }
   }

   public static IEnumerable<(T item, TCompletion completion)> WhereIsCompleted<T, TCompletion>(this IEnumerable<T> enumerable,
      Func<T, Completion<TCompletion>> predicate) where TCompletion : notnull
   {
      foreach (var item in enumerable)
      {
         var _value = predicate(item);
         if (_value is (true, var value))
         {
            yield return (item, value);
         }
      }
   }

   public static Maybe<IEnumerable<T>> AllAreSome<T>(this IEnumerable<Maybe<T>> enumerable) where T : notnull
   {
      List<T> result = [];
      foreach (var _value in enumerable)
      {
         if (_value is (true, var value))
         {
            result.Add(value);
         }
         else
         {
            return nil;
         }
      }

      return result;
   }

   public static IEnumerable<Result<T>> All<T>(this IEnumerable<Result<T>> enumerable, Maybe<Action<T>> success, Maybe<Action<Exception>> failure)
      where T : notnull
   {
      return new ResultIterator<T>(enumerable, success, failure).All();
   }

   public static IEnumerable<T> Successes<T>(this IEnumerable<Result<T>> enumerable, Maybe<Action<T>> success, Maybe<Action<Exception>> failure)
      where T : notnull
   {
      return new ResultIterator<T>(enumerable, success, failure).SuccessesOnly();
   }

   public static IEnumerable<Exception> Failures<T>(this IEnumerable<Result<T>> enumerable, Maybe<Action<T>> success,
      Maybe<Action<Exception>> failure) where T : notnull
   {
      return new ResultIterator<T>(enumerable, success, failure).FailuresOnly();
   }

   public static (IEnumerable<T> enumerable, Maybe<Exception> exception) SuccessesFirst<T>(this IEnumerable<Result<T>> enumerable,
      Maybe<Action<T>> success, Maybe<Action<Exception>> failure) where T : notnull
   {
      return new ResultIterator<T>(enumerable, success, failure).SuccessesThenFailure();
   }

   public static Result<IEnumerable<T>> IfAllSuccesses<T>(this IEnumerable<Result<T>> enumerable, Maybe<Action<T>> success,
      Maybe<Action<Exception>> failure) where T : notnull
   {
      return new ResultIterator<T>(enumerable, success, failure).IfAllSuccesses();
   }

   public static Result<TResult> ForAny<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> func) where TResult : notnull
   {
      try
      {
         Maybe<TResult> _firstItem = nil;
         foreach (var _result in enumerable.Select(item => tryTo(() => func(item))))
         {
            if (_result is (true, var result))
            {
               if (!_firstItem)
               {
                  _firstItem = result;
               }
            }
            else
            {
               return _result.Exception;
            }
         }

         return _firstItem.Result("Enumerable empty");
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<TResult> ForAny<TSource, TResult>(this Result<IEnumerable<TSource>> enumerable, Func<TSource, TResult> func)
      where TResult : notnull
   {
      return enumerable.Map(e => e.ForAny(func));
   }

   public static Result<TResult> ForAny<TSource, TResult>(this IEnumerable<TSource> enumerable, Action<TSource> action, TResult result)
      where TResult : notnull
   {
      try
      {
         foreach (var item in enumerable)
         {
            try
            {
               action(item);
            }
            catch (Exception exception)
            {
               return exception;
            }
         }

         return result;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<TResult> ForAny<TSource, TResult>(this Result<IEnumerable<TSource>> enumerable, Action<TSource> action, TResult result)
      where TResult : notnull
   {
      return enumerable.Map(e => e.ForAny(action, result));
   }

   public static Result<TResult> ForAny<TSource, TResult>(this IEnumerable<TSource> enumerable, Action<TSource> action, Func<TResult> result)
      where TResult : notnull => tryTo(() =>
   {
      foreach (var item in enumerable)
      {
         action(item);
      }

      return result();
   });

   public static Result<TResult> ForAny<TSource, TResult>(this Result<IEnumerable<TSource>> enumerable, Action<TSource> action, Func<TResult> result)
      where TResult : notnull
   {
      return enumerable.Map(e => e.ForAny(action, result));
   }

   public static Result<T> Flat<T>(this Result<Result<T>> result) where T : notnull => result.Recover(e => e);

   public static T ThrowIfFailed<T>(this Result<T> result) where T : notnull
   {
      if (result is (true, var resultValue))
      {
         return resultValue;
      }
      else
      {
         throw result.Exception;
      }
   }

   public static void ForEach<T>(this Result<IEnumerable<T>> enumerable, Action<T> ifSuccess, Action<Exception> ifFailure)
   {
      if (enumerable is (true, var enumerableValue))
      {
         enumerableValue.ForEach(ifSuccess, ifFailure);
      }
      else
      {
         ifFailure(enumerable.Exception);
      }
   }

   public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> ifSuccess, Action<Exception> ifFailure)
   {
      using var enumerator = enumerable.GetEnumerator();

      while (true)
      {
         var movedNext = false;
         var value = default(T);
         try
         {
            movedNext = enumerator.MoveNext();
            if (movedNext)
            {
               value = enumerator.Current;
            }
            else
            {
               break;
            }
         }
         catch (Exception exception)
         {
            ifFailure(exception);
         }

         try
         {
            if (movedNext && value is not null)
            {
               ifSuccess(value);
            }
         }
         catch (Exception exception)
         {
            ifFailure(exception);
         }
      }
   }

   public static Maybe<T> IfCast<T>(this object obj) where T : notnull => obj is T t ? t : nil;

   public static IEnumerable<T> SomeValue<T>(this IEnumerable<Maybe<T>> enumerable) where T : notnull
   {
      foreach (var _item in enumerable)
      {
         if (_item is (true, var item))
         {
            yield return item;
         }
      }
   }

   public static IEnumerable<T> SuccessfulValue<T>(this IEnumerable<Result<T>> enumerable) where T : notnull
   {
      foreach (var _result in enumerable)
      {
         if (_result is (true, var result))
         {
            yield return result;
         }
      }
   }

   public static Completion<T> Completed<T>(this T value) where T : notnull
   {
      if (value is ITuple tuple)
      {
         for (var i = 0; i < tuple.Length; i++)
         {
            if (tuple[i] is null)
            {
               return "No tuple item can be null".Interrupted<T>();
            }
         }

         return new Completed<T>(value);
      }
      else
      {
         return new Completed<T>(value);
      }
   }

   public static Completion<T> Completed<T>(this T value, CancellationToken token) where T : notnull
   {
      if (token.IsCancellationRequested)
      {
         return nil;
      }
      else
      {
         return value.Completed();
      }
   }

   public static Completion<T> Interrupted<T>(this string message) where T : notnull => new Interrupted<T>(new ApplicationException(message));

   public static Completion<T> Interrupted<T, TException>(this object firstItem, params object[] args) where T : notnull where TException : Exception
   {
      List<object> list = [firstItem];
      list.AddRange(args);

      return (TException)typeof(TException).Create([.. list])!;
   }

   public static Completion<T> Completion<T>(this Result<T> result) where T : notnull => result.Map(v => v.Completed()).Recover(e => e);

   public static Completion<T> Completion<T>(this Result<T> result, CancellationToken token) where T : notnull
   {
      return result.Map(v => v.Completed(token)).Recover(e => e);
   }

   public static Completion<T> Completion<T>(this Maybe<T> maybe) where T : notnull => maybe.Map(v => new Completed<T>(v)) | nil;

   public static Completion<T> Completion<T>(this Maybe<T> maybe, CancellationToken token) where T : notnull
   {
      return maybe.Map(v => v.Completed(token)) | (() => new Cancelled<T>());
   }

   private static Completion<T> cancelledOrInterrupted<T>(Exception exception) where T : notnull => exception switch
   {
      OperationCanceledException => nil,
      ObjectDisposedException => nil,
      FullStackException { InnerException: not null and not FullStackException } fullStackException => cancelledOrInterrupted<T>(fullStackException
         .InnerException),
      _ => exception
   };

   public static async Task<Completion<T3>> SelectMany<T1, T2, T3>(this Task<Completion<T1>> source, Func<T1, Task<Completion<T2>>> func,
      Func<T1, T2, T3> projection) where T1 : notnull where T2 : notnull where T3 : notnull
   {
      var _t = await source;
      if (_t is (true, var t))
      {
         var _u = await func(t);
         if (_u is (true, var u))
         {
            return projection(t, u).Completed();
         }
         else if (_u.Exception is (true, var exception))
         {
            return cancelledOrInterrupted<T3>(exception);
         }
         else
         {
            return nil;
         }
      }
      else if (_t.Exception is (true, var exception))
      {
         return cancelledOrInterrupted<T3>(exception);
      }
      else
      {
         return nil;
      }
   }

   public static Result<T> Result<T>(this Completion<T> completion) where T : notnull
   {
      if (completion is (true, var completionValue))
      {
         return completionValue;
      }
      else if (completion.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return fail("Cancelled");
      }
   }

   public static Maybe<T> MaxOrNone<T>(this IEnumerable<T> enumerable) where T : notnull
   {
      T[] array = [.. enumerable];
      return maybe<T>() & array.Length > 0 & (() => array.Max()!);
   }

   public static Maybe<T> MaxOrNone<T, TMax>(this IEnumerable<T> enumerable, Func<T, TMax> maxOnFunc) where T : notnull
   {
      T[] array = [.. enumerable];
      return maybe<T>() & array.Length > 0 & (() =>
      {
         var max = array.Select((v, i) => (v: maxOnFunc(v), i)).Max();
         return array[max.i];
      });
   }

   public static Maybe<T> MinOrNone<T>(this IEnumerable<T> enumerable) where T : notnull
   {
      T[] array = [.. enumerable];
      return maybe<T>() & array.Length > 0 & (() => array.Min()!);
   }

   public static Maybe<T> MinOrNone<T, TMin>(this IEnumerable<T> enumerable, Func<T, TMin> minOnFunc) where T : notnull
   {
      T[] array = [.. enumerable];
      return maybe<T>() & array.Length > 0 & (() =>
      {
         var min = array.Select((v, i) => (v: minOnFunc(v), i)).Min();
         return array[min.i];
      });
   }

   public static Result<T> MaxOrFail<T>(this IEnumerable<T> enumerable, Func<string> exceptionMessage) where T : notnull => tryTo(() =>
   {
      T[] array = [.. enumerable];
      return assert(array.Length > 0, () => array.Max()!, exceptionMessage);
   });

   public static Result<T> MaxOrFail<T, TMax>(this IEnumerable<T> enumerable, Func<T, TMax> maxOnFunc, Func<string> exceptionMessage)
      where T : notnull
   {
      return tryTo(() =>
      {
         T[] array = [.. enumerable];
         return assert(array.Length > 0, () =>
         {
            var max = array.Select((v, i) => (v: maxOnFunc(v), i)).Max();
            return array[max.i];
         }, exceptionMessage);
      });
   }

   public static Result<T> MinOrFail<T>(this IEnumerable<T> enumerable, Func<string> exceptionMessage) where T : notnull => tryTo(() =>
   {
      T[] array = [.. enumerable];
      return assert(array.Length > 0, () => array.Min()!, exceptionMessage);
   });

   public static Result<T> MinOrFail<T, TMin>(this IEnumerable<T> enumerable, Func<T, TMin> minOnFunc, Func<string> exceptionMessage)
      where T : notnull
   {
      return tryTo(() =>
      {
         T[] array = [.. enumerable];
         return assert(array.Length > 0, () =>
         {
            var min = array.Select((v, i) => (v: minOnFunc(v), i)).Min();
            return array[min.i];
         }, exceptionMessage);
      });
   }

   public static Maybe<TResult> Map<T1, T2, TResult>(this Maybe<(T1, T2)> maybe, Func<T1, T2, TResult> func) where TResult : notnull
   {
      return maybe.Map(t => func(t.Item1, t.Item2));
   }

   public static Maybe<TResult> Map<T1, T2, TResult>(this Maybe<(T1, T2)> maybe, Func<T1, T2, Maybe<TResult>> func) where TResult : notnull
   {
      return maybe.Map(t => func(t.Item1, t.Item2));
   }

   public static Maybe<TResult> Map<T1, T2, T3, TResult>(this Maybe<(T1, T2, T3)> maybe, Func<T1, T2, T3, TResult> func) where TResult : notnull
   {
      return maybe.Map(t => func(t.Item1, t.Item2, t.Item3));
   }

   public static Maybe<TResult> Map<T1, T2, T3, TResult>(this Maybe<(T1, T2, T3)> maybe, Func<T1, T2, T3, Maybe<TResult>> func)
      where TResult : notnull
   {
      return maybe.Map(t => func(t.Item1, t.Item2, t.Item3));
   }

   public static Maybe<TResult> Map<T1, T2, T3, T4, TResult>(this Maybe<(T1, T2, T3, T4)> maybe, Func<T1, T2, T3, T4, TResult> func)
      where TResult : notnull
   {
      return maybe.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
   }

   public static Maybe<TResult> Map<T1, T2, T3, T4, TResult>(this Maybe<(T1, T2, T3, T4)> maybe, Func<T1, T2, T3, T4, Maybe<TResult>> func)
      where TResult : notnull
   {
      return maybe.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
   }

   public static Result<TResult> Map<T1, T2, TResult>(this Result<(T1, T2)> result, Func<T1, T2, TResult> func) where TResult : notnull
   {
      return result.Map(t => func(t.Item1, t.Item2));
   }

   public static Result<TResult> Map<T1, T2, TResult>(this Result<(T1, T2)> result, Func<T1, T2, Result<TResult>> func) where TResult : notnull
   {
      return result.Map(t => func(t.Item1, t.Item2));
   }

   public static Result<TResult> Map<T1, T2, T3, TResult>(this Result<(T1, T2, T3)> result, Func<T1, T2, T3, TResult> func) where TResult : notnull
   {
      return result.Map(t => func(t.Item1, t.Item2, t.Item3));
   }

   public static Result<TResult> Map<T1, T2, T3, TResult>(this Result<(T1, T2, T3)> result, Func<T1, T2, T3, Result<TResult>> func)
      where TResult : notnull
   {
      return result.Map(t => func(t.Item1, t.Item2, t.Item3));
   }

   public static Result<TResult> Map<T1, T2, T3, T4, TResult>(this Result<(T1, T2, T3, T4)> result, Func<T1, T2, T3, T4, TResult> func)
      where TResult : notnull
   {
      return result.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
   }

   public static Result<TResult> Map<T1, T2, T3, T4, TResult>(this Result<(T1, T2, T3, T4)> result, Func<T1, T2, T3, T4, Result<TResult>> func)
      where TResult : notnull
   {
      return result.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
   }

   public static Completion<TResult> Map<T1, T2, TResult>(this Completion<(T1, T2)> completion, Func<T1, T2, TResult> func) where TResult : notnull
   {
      return completion.Map(t => func(t.Item1, t.Item2));
   }

   public static Completion<TResult> Map<T1, T2, TResult>(this Completion<(T1, T2)> completion, Func<T1, T2, Completion<TResult>> func)
      where TResult : notnull
   {
      return completion.Map(t => func(t.Item1, t.Item2));
   }

   public static Completion<TResult> Map<T1, T2, T3, TResult>(this Completion<(T1, T2, T3)> completion, Func<T1, T2, T3, TResult> func)
      where TResult : notnull
   {
      return completion.Map(t => func(t.Item1, t.Item2, t.Item3));
   }

   public static Completion<TResult> Map<T1, T2, T3, TResult>(this Completion<(T1, T2, T3)> completion,
      Func<T1, T2, T3, Completion<TResult>> func) where TResult : notnull
   {
      return completion.Map(t => func(t.Item1, t.Item2, t.Item3));
   }

   public static Completion<TResult> Map<T1, T2, T3, T4, TResult>(this Completion<(T1, T2, T3, T4)> completion,
      Func<T1, T2, T3, T4, TResult> func) where TResult : notnull
   {
      return completion.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
   }

   public static Completion<TResult> Map<T1, T2, T3, T4, TResult>(this Completion<(T1, T2, T3, T4)> completion,
      Func<T1, T2, T3, T4, Completion<TResult>> func) where TResult : notnull
   {
      return completion.Map(t => func(t.Item1, t.Item2, t.Item3, t.Item4));
   }

   public static Maybe<T> SomeIf<T>(this Func<bool> boolExpression, Func<T> value) where T : notnull
   {
      return boolExpression() ? value() : nil;
   }

   public static Maybe<T> SomeIf<T>(this Func<bool> boolExpression, Func<Maybe<T>> value) where T : notnull
   {
      return boolExpression() ? value() : nil;
   }

   public static Maybe<T> SomeIf<T>(this bool boolExpression, Func<T> value) where T : notnull
   {
      return boolExpression ? value() : nil;
   }

   public static Maybe<T> SomeIf<T>(this bool boolExpression, Func<Maybe<T>> value) where T : notnull
   {
      return boolExpression ? value() : nil;
   }

   public static Maybe<T> MaybeIf<T>(this T value, Func<T, bool> predicate) where T : notnull => predicate(value) ? value : nil;

   public static Result<T> ResultIf<T>(this T value, Func<T, bool> predicate, string failMessage) where T : notnull
   {
      try
      {
         return predicate(value) ? value : fail(failMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<T> ResultIf<T>(this T value, Func<T, bool> predicate, Func<string> failMessage) where T : notnull
   {
      try
      {
         return predicate(value) ? value : fail(failMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<T> ResultIf<T>(this T value, Func<T, bool> predicate, Exception exception) where T : notnull
   {
      try
      {
         return predicate(value) ? value : exception;
      }
      catch (Exception thrownException)
      {
         return thrownException;
      }
   }

   public static Completion<T> CompletionIf<T>(this T value, Func<T, bool> predicate, Maybe<string> _failMessage) where T : notnull
   {
      try
      {
         if (predicate(value))
         {
            return value;
         }
         else if (_failMessage)
         {
            return fail(_failMessage);
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Completion<T> CompletionIf<T>(this T value, Func<T, bool> predicate, Func<Maybe<string>> failMessage) where T : notnull
   {
      try
      {
         if (predicate(value))
         {
            return value;
         }
         else
         {
            var _failMessage = failMessage();
            if (_failMessage)
            {
               return fail(_failMessage);
            }
            else
            {
               return nil;
            }
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Completion<T> CompletionIf<T>(this T value, Func<T, bool> predicate, Maybe<Exception> _exception) where T : notnull
   {
      try
      {
         if (predicate(value))
         {
            return value;
         }
         else if (_exception)
         {
            return _exception;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<T> OptionalIf<T>(this T value, Func<T, bool> predicate) where T : notnull
   {
      try
      {
         return predicate(value) ? value : nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<T> OptionalIf<T>(this T value, Func<T, bool> predicate, string failMessage) where T : notnull
   {
      try
      {
         return predicate(value) ? value : fail(failMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<T> OptionalIf<T>(this T value, Func<T, bool> predicate, Func<string> failMessage) where T : notnull
   {
      try
      {
         return predicate(value) ? value : fail(failMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Optional<T> OptionalIf<T>(this T value, Func<T, bool> predicate, Exception exception) where T : notnull
   {
      try
      {
         return predicate(value) ? value : exception;
      }
      catch (Exception thrownException)
      {
         return thrownException;
      }
   }

   public static Optional<T> OptionalIf<T>(this T value, Func<T, bool> predicate, Maybe<Exception> _exception) where T : notnull
   {
      try
      {
         if (predicate(value))
         {
            return value;
         }
         else if (_exception)
         {
            return _exception;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static bool IsSome(this Maybe<bool> maybe) => maybe is Some<bool>;

   public static bool IsSuccess(this Result<bool> result) => result is Success<bool>;

   public static bool IsJust(this Optional<bool> optional) => optional is Just<bool>;

   public static bool IsCompletion(this Completion<bool> completion) => completion is Completed<bool>;

   public static IEnumerable<T> OnlyTrue<T>(this IEnumerable<Maybe<T>> enumerable) where T : notnull
   {
      foreach (var maybe in enumerable)
      {
         if (maybe is (true, var maybeValue))
         {
            yield return maybeValue;
         }
      }
   }

   public static IEnumerable<T> OnlyTrue<T>(this IEnumerable<Result<T>> enumerable) where T : notnull
   {
      foreach (var result in enumerable)
      {
         if (result is (true, var resultValue))
         {
            yield return resultValue;
         }
      }
   }

   public static IEnumerable<T> OnlyTrue<T>(this IEnumerable<Optional<T>> enumerable) where T : notnull
   {
      foreach (var optional in enumerable)
      {
         if (optional is (true, var optionalValue))
         {
            yield return optionalValue;
         }
      }
   }

   public static IEnumerable<T> OnlyTrue<T>(this IEnumerable<Completion<T>> enumerable) where T : notnull
   {
      foreach (var completion in enumerable)
      {
         if (completion is (true, var completionValue))
         {
            yield return completionValue;
         }
      }
   }

   [DebuggerStepThrough]
   public static Optional<TResult> SelectMany<T, TResult>(this Optional<T> maybe, Func<T, Optional<TResult>> projection) where T : notnull
      where TResult : notnull
   {
      return maybe.Map(projection) | (() => fail("Value not provided"));
   }

   [DebuggerStepThrough]
   public static Optional<TResult> Select<T, TResult>(this Optional<T> result, Func<T, TResult> func) where T : notnull where TResult : notnull
   {
      if (result is (true, var resultValue))
      {
         return func(resultValue);
      }
      else
      {
         return result.Exception;
      }
   }
}