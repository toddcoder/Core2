namespace Core.Json;

public class JsonIterateStringHash(string json, JsonRetrieverOptions options) : JsonIterateHash<string, string>(json, options)
{
   protected void setItem(string text)
   {
      hash[fullPropertyName] = text;
      depletePropertyNameSet();
   }

   public override void String() => setItem(value);

   public override void Number() => setItem(value);

   public override void False() => setItem("false");

   public override void True() => setItem("true");

   public override void Null() => setItem("");

   public override void SetRunningFlag() => stopAfterFirst(hash.Count > 0);
}