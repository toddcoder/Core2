using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class HashInterfaceMaybe<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected IHash<TKey, TValue> hash;

   public HashInterfaceMaybe(IHash<TKey, TValue> hash)
   {
      this.hash = hash;
   }

   public Maybe<TValue> this[TKey key]
   {
      get
      {
         if (hash.ContainsKey(key))
         {
            return hash[key];
         }
         else
         {
            return nil;
         }
      }
   }
}