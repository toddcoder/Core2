namespace Core.Collections;

public class OneShotHash<TKey, TValue> : Hash<TKey, TValue> where TKey : notnull where TValue : notnull
{
   public new TValue this[TKey key]
   {
      get => base[key];
      set
      {
         if (!ContainsKey(key))
         {
            base[key] = value;
         }
      }
   }

   public new void Add(TKey key, TValue value)
   {
      if (!ContainsKey(key))
      {
         base.Add(key, value);
      }
   }
}