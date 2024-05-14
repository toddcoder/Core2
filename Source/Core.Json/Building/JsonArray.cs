namespace Core.Json.Building;

public class JsonArray : JsonObject
{
   public JsonArray(JsonBuilder builder) : base(builder)
   {
   }

   public JsonArray(JsonBuilder builder, string propertyName) : base(builder, propertyName)
   {
   }

   public override JsonAbstraction End(bool pop = true)
   {
      if (pop)
      {
         builder.Pop();
      }

      builder.Writer.EndArray();
      return this;
   }
}