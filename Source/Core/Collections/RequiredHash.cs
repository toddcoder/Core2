using Core.Assertions;
using Core.Monads;

namespace Core.Collections;

public class RequiredHash<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected IHash<TKey, TValue> hash;

   internal RequiredHash(IHash<TKey, TValue> hash)
   {
      this.hash = hash.Must().Force<IHash<TKey, TValue>>();
   }

   public Result<TValue> this[TKey key] => hash.Must().HaveKeyOf(key).OrFailure().Map(d => d[key]);
}