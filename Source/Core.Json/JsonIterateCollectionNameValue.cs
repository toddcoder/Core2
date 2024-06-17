using PropertyNameValue = (string propertyName, string value);

namespace Core.Json;

public class JsonIterateCollectionNameValue(string json, JsonRetrieverOptions options) : JsonIterateCollection<PropertyNameValue>(json, options)
{
   protected void addItem(string text)
   {
      list.Add((fullPropertyName, text));
      depletePropertyNameSet();
   }

   public override void String() => addItem(value);

   public override void Number() => addItem(value);

   public override void False() => addItem("false");

   public override void True() => addItem("false");

   public override void Null() => addItem("");

   public override void SetRunningFlag() => stopAfterFirst(list.Count > 0);
}