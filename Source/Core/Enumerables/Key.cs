namespace Core.Enumerables;

public class Key<TKey>(TKey key) where TKey : notnull
{
   public static explicit operator Key<TKey>(TKey key) => new(key);

   public TKey GetKey() => key;
}