using System;
using System.Collections.Generic;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

internal class ResultIterator<T> where T : notnull
{
   protected IEnumerable<Result<T>> enumerable;
   protected Maybe<Action<T>> _success;
   protected Maybe<Action<Exception>> _failure;

   public ResultIterator(IEnumerable<Result<T>> enumerable, Maybe<Action<T>> ifSuccess, Maybe<Action<Exception>> ifFailure)
   {
      this.enumerable = enumerable;
      _success = ifSuccess;
      _failure = ifFailure;
   }

   protected void handle(Result<T> result)
   {
      if (result is (true, var resultValue))
      {
         if (_success is (true, var success))
         {
            success(resultValue);
         }
      }
      else if (_failure is (true, var failure))
      {
         failure(result.Exception);
      }
   }

   public IEnumerable<Result<T>> All()
   {
      foreach (var result in enumerable)
      {
         handle(result);
      }

      return enumerable;
   }

   public IEnumerable<T> SuccessesOnly()
   {
      var list = new List<T>();

      foreach (var _result in enumerable)
      {
         handle(_result);
         if (_result is (true, var result))
         {
            list.Add(result);
         }
      }

      return list;
   }

   public IEnumerable<Exception> FailuresOnly()
   {
      var list = new List<Exception>();

      foreach (var _result in enumerable)
      {
         handle(_result);
         if (!_result)
         {
            list.Add(_result.Exception);
         }
      }

      return list;
   }

   public (IEnumerable<T> enumerable, Maybe<Exception> exception) SuccessesThenFailure()
   {
      var list = new List<T>();

      foreach (var _result in enumerable)
      {
         handle(_result);
         if (_result is (true, var result))
         {
            list.Add(result);
         }
         else
         {
            return (list, _result.Exception);
         }
      }

      return (list, nil);
   }

   public Result<IEnumerable<T>> IfAllSuccesses()
   {
      var list = new List<T>();

      foreach (var _result in enumerable)
      {
         handle(_result);
         if (_result)
         {
            list.Add(_result);
         }
         else
         {
            return _result.Exception;
         }
      }

      return list;
   }
}