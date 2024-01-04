using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyRepeatingMonads
{
   protected static Maybe<LazyRepeatingMonads> _function;

   static LazyRepeatingMonads()
   {
      _function = nil;
   }

   public static LazyRepeatingMonads lazyRepeating
   {
      get
      {
         if (!_function)
         {
            _function = new LazyRepeatingMonads();
         }

         return _function;
      }
   }

   [Obsolete("Use Maybe<T>")]
   public LazyMaybe<T> maybe<T>() where T : notnull => new();

   [Obsolete("Use Result<T>")]
   public LazyResult<T> result<T>() where T : notnull => new();

   [Obsolete("Use Optional<T>")]
   public LazyOptional<T> optional<T>() where T : notnull => new();

   [Obsolete("Use Completion<T>")]
   public LazyCompletion<T> completion<T>() where T : notnull => new();
}