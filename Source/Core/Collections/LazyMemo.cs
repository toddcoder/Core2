using System;
using Core.Objects;

namespace Core.Collections;

public class LazyMemo<TKey, TValue> : Memo<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected LateLazy<Func<TKey, TValue>> defaultValue = new();

   public override TValue GetValue(TKey key) => defaultValue.Value(key);

   public Func<TKey, TValue> DefaultValue
   {
      get => defaultValue.Value;
      set => defaultValue.ActivateWith(() => value);
   }
}