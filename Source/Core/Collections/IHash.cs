namespace Core.Collections;

public interface IHash<TKey, TValue> where TKey : notnull where TValue : notnull
{
   TValue this[TKey key] { get; }

   bool ContainsKey(TKey key);

   Hash<TKey, TValue> GetHash();

   HashInterfaceMaybe<TKey, TValue> Items { get; }
}