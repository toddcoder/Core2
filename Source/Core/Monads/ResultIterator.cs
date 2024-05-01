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
      foreach (var _result in enumerable)
      {
         handle(_result);
         if (_result is (true, var result))
         {
            yield return result;
         }
      }
   }

   public IEnumerable<Exception> FailuresOnly()
   {
      foreach (var _result in enumerable)
      {
         handle(_result);
         if (!_result)
         {
            yield return _result.Exception;
         }
      }
   }

   public (IEnumerable<T> enumerable, Maybe<Exception> exception) SuccessesThenFailure()
   {
      IEnumerable<Either<T, Exception>> getEnumerable()
      {
         foreach (var _result in enumerable)
         {
            handle(_result);
            if (_result is (true, var result))
            {
               yield return result;
            }
            else
            {
               yield return _result.Exception;

               yield break;
            }
         }
      }

      List<T> list = [];
      foreach (var either in getEnumerable())
      {
         switch (either)
         {
            case (true, var result, _):
               list.Add(result);
               break;
            case (false, _, var exception):
               return (list, exception);
         }
      }

      return (list, nil);
   }

   public Result<IEnumerable<T>> IfAllSuccesses()
   {
      List<T> list = [];

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