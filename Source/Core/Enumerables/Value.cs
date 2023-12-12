namespace Core.Enumerables;

public class Value<TValue>(TValue value) where TValue : notnull
{
   public static explicit operator Value<TValue>(TValue value) => new(value);

   public TValue GetValue() => value;
}