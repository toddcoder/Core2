using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads.Lazy;

public class LazyMonads
{
   protected static Maybe<LazyMonads> _function;

   static LazyMonads()
   {
      _function = nil;
   }

   public static LazyMonads lazy
   {
      get
      {
         if (!_function)
         {
            _function = new LazyMonads();
         }

         return _function;
      }
   }

   [Obsolete("Use Maybe<T>")]
   public LazyMaybe<T> maybe<T>(Func<Maybe<T>> func) where T : notnull => new(func);

   [Obsolete("Use Maybe<T>")]
   public LazyMaybe<T> maybe<T>(Maybe<T> maybe) where T : notnull => new(maybe);

   [Obsolete("Use Maybe<T>")]
   public LazyMaybe<T> maybe<T>() where T : notnull => new();

   [Obsolete("Use Result<T>")]
   public LazyResult<T> result<T>(Func<Result<T>> func) where T : notnull => new(func);

   [Obsolete("Use Result<T>")]
   public LazyResult<T> result<T>(Result<T> result) where T : notnull => new(result);

   [Obsolete("Use Result<T>")]
   public LazyResult<T> result<T>() where T : notnull => new();

   [Obsolete("Use Optional<T>")]
   public LazyOptional<T> optional<T>(Func<Optional<T>> func) where T : notnull => new(func);

   [Obsolete("Use Optional<T>")]
   public LazyOptional<T> optional<T>(Optional<T> optional) where T : notnull => new(optional);

   [Obsolete("Use Optional<T>")]
   public LazyOptional<T> optional<T>() where T : notnull => new();

   [Obsolete("Use Completion<T>")]
   public LazyCompletion<T> completion<T>(Func<Completion<T>> func) where T : notnull => new(func);

   [Obsolete("Use Completion<T>")]
   public LazyCompletion<T> completion<T>(Completion<T> completion) where T : notnull => new(completion);

   [Obsolete("Use Completion<T>")]
   public LazyCompletion<T> completion<T>() where T : notnull => new();
}