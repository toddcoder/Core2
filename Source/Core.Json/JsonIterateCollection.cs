using System.Text.Json;

namespace Core.Json;

public abstract class JsonIterateCollection<T>(string json, JsonRetrieverOptions options) : JsonIterator(json, options)
{
   protected List<T> list = [];

   public virtual IEnumerable<T> Retrieve(params string[] propertyNames)
   {
      prefixes = [];
      propertyNameSet = [.. propertyNames];
      list = [];

      var reader = getReader();

      propertyName = "";
      fullPropertyName = "";
      isRunning = true;

      while (reader.Read() && isRunning)
      {
         switch (reader.TokenType)
         {
            case JsonTokenType.StartObject when bits[JsonRetrieverOptions.UsesPath]:
               StartObject();
               break;
            case JsonTokenType.StartArray when bits[JsonRetrieverOptions.UsesPath]:
               StartArray();
               break;
            case JsonTokenType.EndObject when bits[JsonRetrieverOptions.UsesPath]:
               EndObject();
               break;
            case JsonTokenType.EndArray when bits[JsonRetrieverOptions.UsesPath]:
               EndArray();
               break;
            case JsonTokenType.PropertyName:
               setValue(reader);
               setPropertyName();
               break;
            case JsonTokenType.String when keyMatches():
               setValue(reader);
               String();
               break;
            case JsonTokenType.Number when keyMatches():
               setNumber(reader);
               Number();
               break;
            case JsonTokenType.False when keyMatches():
               False();
               break;
            case JsonTokenType.True when keyMatches():
               True();
               break;
            case JsonTokenType.Null when keyMatches():
               Null();
               break;
         }

         SetRunningFlag();
      }

      return list;
   }
}