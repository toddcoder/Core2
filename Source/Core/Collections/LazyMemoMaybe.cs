using Core.Monads;

namespace Core.Collections;

public class LazyMemoMaybe<TKey, TValue>(LazyMemo<TKey, TValue> lazyMemo) where TKey : notnull where TValue : notnull
{
   public Maybe<TValue> this[TKey key]
   {
      get
      {
         lazyMemo.ForceValue(key);
         return lazyMemo.GetHash().Maybe[key];
      }
   }
}