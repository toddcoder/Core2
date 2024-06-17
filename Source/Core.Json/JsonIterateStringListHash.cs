namespace Core.Json;

public class JsonIterateStringListHash(string json, JsonRetrieverOptions options) : JsonIterateHash<string, List<string>>(json, options)
{
   protected void addItem(string text)
   {
      if (hash.ContainsKey(fullPropertyName))
      {
         hash[fullPropertyName].Add(text);
      }
      else
      {
         hash[fullPropertyName] = [text];
      }
      depletePropertyNameSet();
   }

   public override void String() => addItem(value);

   public override void Number() => addItem(value);

   public override void False() => addItem("false");

   public override void True() => addItem("true");

   public override void Null() => addItem("");

   public override void SetRunningFlag() => stopAfterFirst(hash.Count > 0);
}