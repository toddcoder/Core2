using Core.DataStructures;
using Core.Monads;

namespace Core.Json.Building;

public class JsonBuilder
{
   public static JsonBuilder WithObject() => new();

   public static JsonBuilder WithArray() => new(false);

   protected JsonWriter writer = new();
   protected MaybeStack<JsonAbstraction> ends = [];
   protected JsonAbstraction outer;

   protected JsonBuilder(bool startWithObject = true)
   {
      if (startWithObject)
      {
         outer = Object();
      }
      else
      {
         outer = Array();
      }
   }

   public JsonWriter Writer => writer;

   public JsonAbstraction Outer => outer;

   public void Begin(JsonAbstraction abstraction)
   {
      switch (abstraction)
      {
         case JsonArray:
            writer.BeginArray();
            break;
         case JsonObject:
            writer.BeginObject();
            break;
      }
   }

   public void Begin(JsonAbstraction abstraction, string propertyName)
   {
      switch (abstraction)
      {
         case JsonArray:
            writer.BeginArray(propertyName);
            break;
         case JsonObject:
            writer.BeginObject(propertyName);
            break;
      }
   }

   public JsonAbstraction Object() => new JsonObject(this);

   public JsonAbstraction Object(string propertyName) => new JsonObject(this, propertyName);

   public JsonAbstraction Array() => new JsonArray(this);

   public JsonAbstraction Array(string propertyName) => new JsonArray(this, propertyName);

   public JsonAbstraction Push(JsonAbstraction abstraction)
   {
      ends.Push(abstraction);
      return abstraction;
   }

   public Maybe<JsonAbstraction> Pop() => ends.Pop();

   public string End()
   {
      while (ends.Pop() is (true, var abstraction))
      {
         abstraction.End(false);
      }

      /*if (startWithObject)
      {
         writer.EndObject();
      }
      else
      {
         writer.EndArray();
      }*/

      return writer.ToString();
   }

   public override string ToString() => End();
}