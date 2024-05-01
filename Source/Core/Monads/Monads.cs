using System;
using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Monads
{
   protected static Maybe<Monads> _function;

   static Monads()
   {
      _function = nil;
   }

   public static Monads monads
   {
      get
      {
         if (!_function)
         {
            _function = new Monads();
         }

         return _function;
      }
   }

   [Obsolete("Use nil")]
   public Maybe<T> maybe<T>() where T : notnull => nil;

   [Obsolete("Use nil")]
   public Result<T> result<T>() where T : notnull => nil;

   [Obsolete("Use nil")]
   public Optional<T> optional<T>() where T : notnull => nil;

   [Obsolete("Use nil")]
   public Completion<T> completion<T>() where T : notnull => nil;
}