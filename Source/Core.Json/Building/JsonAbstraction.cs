using Core.Monads;

namespace Core.Json.Building;

public abstract class JsonAbstraction
{
   public static JsonAbstraction operator +(JsonAbstraction abstraction, string value) => abstraction.Write(value);

   public static JsonAbstraction operator +(JsonAbstraction abstraction, int value) => abstraction.Write(value);

   public static JsonAbstraction operator +(JsonAbstraction abstraction, double value) => abstraction.Write(value);

   public static JsonAbstraction operator +(JsonAbstraction abstraction, bool value) => abstraction.Write(value);

   public static JsonAbstraction operator +(JsonAbstraction abstraction, DateTime value) => abstraction.Write(value);

   public static JsonAbstraction operator +(JsonAbstraction abstraction, Guid value) => abstraction.Write(value);

   public static JsonAbstraction operator +(JsonAbstraction abstraction, (string propertyName, string value) tuple)
   {
      return abstraction.Write(tuple.propertyName, tuple.value);
   }

   public static JsonAbstraction operator +(JsonAbstraction abstraction, (string propertyName, int value) tuple)
   {
      return abstraction.Write(tuple.propertyName, tuple.value);
   }

   public static JsonAbstraction operator +(JsonAbstraction abstraction, (string propertyName, double value) tuple)
   {
      return abstraction.Write(tuple.propertyName, tuple.value);
   }

   public static JsonAbstraction operator +(JsonAbstraction abstraction, (string propertyName, bool value) tuple)
   {
      return abstraction.Write(tuple.propertyName, tuple.value);
   }

   public static JsonAbstraction operator +(JsonAbstraction abstraction, (string propertyName, DateTime value) tuple)
   {
      return abstraction.Write(tuple.propertyName, tuple.value);
   }

   public static JsonAbstraction operator +(JsonAbstraction abstraction, (string propertyName, Guid value) tuple)
   {
      return abstraction.Write(tuple.propertyName, tuple.value);
   }

   public static JsonAbstraction operator +(JsonAbstraction abstraction, (string propertyName, byte[] value) tuple)
   {
      return abstraction.Write(tuple.propertyName, tuple.value);
   }

   public static JsonAbstraction operator +(JsonAbstraction abstraction, (string propertyName, string[] values) tuple)
   {
      return abstraction.Write(tuple.propertyName, tuple.values);
   }

   public static JsonAbstraction operator +(JsonAbstraction abstraction, (string propertyName, Nil) tuple)
   {
      return abstraction.WriteNull(tuple.propertyName);
   }

   public static JsonAbstraction operator +(JsonAbstraction abstraction, string[] values)
   {
      return abstraction.Write(values);
   }

   public static JsonAbstraction operator +(JsonAbstraction abstraction, Nil _) => abstraction.End();

   public abstract JsonAbstraction Object();

   public abstract JsonAbstraction Object(string propertyName);

   public abstract JsonAbstraction Array();

   public abstract JsonAbstraction Array(string propertyName);

   public abstract JsonAbstraction End(bool pop = true);

   public abstract JsonAbstraction Write(string value);

   public abstract JsonAbstraction Write(int value);

   public abstract JsonAbstraction Write(double value);

   public abstract JsonAbstraction Write(bool value);

   public abstract JsonAbstraction Write(DateTime value, bool zulu = false);

   public abstract JsonAbstraction Write(Guid value);

   public abstract JsonAbstraction Write(string propertyName, string value);

   public abstract JsonAbstraction Write(string propertyName, int value);

   public abstract JsonAbstraction Write(string propertyName, double value);

   public abstract JsonAbstraction Write(string propertyName, bool value);

   public abstract JsonAbstraction Write(string propertyName, DateTime value);

   public abstract JsonAbstraction Write(string propertyName, Guid value);

   public abstract JsonAbstraction Write(string propertyName, byte[] value);

   public abstract JsonAbstraction Write(string propertyName, string[] values);

   public abstract JsonAbstraction WriteNull(string propertyName);

   public abstract JsonAbstraction Write(string[] values);
}