using System;
using System.Collections.Generic;

namespace Core.Monads;

public class OptionalIterator<T> where T : notnull
{
   protected IEnumerable<Optional<T>> enumerable;
   protected Maybe<Action<T>> _just;
   protected Maybe<Action> _empty;
   protected Maybe<Action<Exception>> _failed;

   public OptionalIterator(IEnumerable<Optional<T>> enumerable, Maybe<Action<T>> response, Maybe<Action> noResponse, Maybe<Action<Exception>> failure)
   {
      this.enumerable = enumerable;
      _just = response;
      _empty = noResponse;
      _failed = failure;
   }

   protected void handle(Optional<T> optional)
   {
      if (optional is (true, var optionalValue) && _just is (true, var action))
      {
         action(optionalValue);
      }
      else if (optional.Exception is (true, var exception) && _failed is (true, var failed))
      {
         failed(exception);
      }
      else if (_empty is (true, var empty))
      {
         empty();
      }
   }

   public IEnumerable<Optional<T>> All()
   {
      foreach (var optional in enumerable)
      {
         handle(optional);
      }

      return enumerable;
   }

   public IEnumerable<T> JustOnly()
   {
      foreach (var optional in enumerable)
      {
         handle(optional);
         if (optional)
         {
            yield return optional;
         }
      }
   }

   public IEnumerable<Exception> FailedOnly()
   {
      foreach (var optional in enumerable)
      {
         handle(optional);
         if (!optional && optional.Exception is (true, var exception))
         {
            yield return exception;
         }
      }
   }
}