using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public struct CachedValue<T>(Func<T> factory) where T : notnull
{
   private Maybe<T> _value = nil;

   public T Value
   {
      get
      {
         (var value, _value) = _value.Create(factory);
         return value;
      }
   }

   public void Reset() => _value = nil;
}