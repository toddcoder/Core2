namespace Core.Json.Building;

public class JsonObject : JsonAbstraction
{
   protected JsonBuilder builder;

   public JsonObject(JsonBuilder builder)
   {
      this.builder = builder;

      builder.Begin(this);
      builder.Push(this);
   }

   public JsonObject(JsonBuilder builder, string propertyName)
   {
      this.builder = builder;

      builder.Begin(this, propertyName);
      builder.Push(this);
   }

   public JsonBuilder Builder => builder;

   public override JsonAbstraction Object() => new JsonObject(builder);

   public override JsonAbstraction Object(string propertyName) => new JsonObject(builder, propertyName);

   public override JsonAbstraction Array() => new JsonArray(builder);

   public override JsonAbstraction Array(string propertyName) => new JsonArray(builder, propertyName);

   public override JsonAbstraction End(bool pop = true)
   {
      if (pop)
      {
         builder.Pop();
      }

      builder.Writer.EndObject();
      return this;
   }

   public override JsonAbstraction Write(string value)
   {
      builder.Writer.Write(value);
      return this;
   }

   public override JsonAbstraction Write(int value)
   {
      builder.Writer.Write(value);
      return this;
   }

   public override JsonAbstraction Write(double value)
   {
      builder.Writer.Write(value);
      return this;
   }

   public override JsonAbstraction Write(bool value)
   {
      builder.Writer.Write(value);
      return this;
   }

   public override JsonAbstraction Write(DateTime value, bool zulu = false)
   {
      builder.Writer.Write(value, zulu);
      return this;
   }

   public override JsonAbstraction Write(Guid value)
   {
      builder.Writer.Write(value);
      return this;
   }

   public override JsonAbstraction Write(string propertyName, string value)
   {
      builder.Writer.Write(propertyName, value);
      return this;
   }

   public override JsonAbstraction Write(string propertyName, int value)
   {
      builder.Writer.Write(propertyName, value);
      return this;
   }

   public override JsonAbstraction Write(string propertyName, double value)
   {
      builder.Writer.Write(propertyName, value);
      return this;
   }

   public override JsonAbstraction Write(string propertyName, bool value)
   {
      builder.Writer.Write(propertyName, value);
      return this;
   }

   public override JsonAbstraction Write(string propertyName, DateTime value)
   {
      builder.Writer.Write(propertyName, value);
      return this;
   }

   public override JsonAbstraction Write(string propertyName, Guid value)
   {
      builder.Writer.Write(propertyName, value);
      return this;
   }

   public override JsonAbstraction Write(string propertyName, byte[] value)
   {
      builder.Writer.Write(propertyName, value);
      return this;
   }

   public override JsonAbstraction Write(string propertyName, string[] values)
   {
      builder.Writer.Write(propertyName, values);
      return this;
   }

   public override JsonAbstraction WriteNull(string propertyName)
   {
      builder.Writer.WriteNull(propertyName);
      return this;
   }

   public override JsonAbstraction Write(string[] values)
   {
      foreach (var value in values)
      {
         builder.Writer.Write(value);
      }

      return this;
   }
}