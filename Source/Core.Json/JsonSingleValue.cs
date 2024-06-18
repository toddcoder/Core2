using Core.Monads;
using System.Text.Json;
using static Core.Monads.MonadFunctions;

namespace Core.Json;

public class JsonSingleValue(string json, JsonRetrieverOptions options) : JsonIterator(json, options)
{
   protected Optional<string> _result = nil;

   public override void String() => _result = value;

   public override void Number() => _result = value;

   public override void False() => _result = "false";

   public override void True() => _result = "true";

   public override void Null() => _result = "";

   public override void SetRunningFlag()
   {
      if (_result)
      {
         isRunning = false;
      }
   }

   public Optional<string> Retrieve(string targetPropertyName)
   {
      prefixes = [];

      var reader = getReader();

      propertyName = "";
      fullPropertyName = "";

      while (reader.Read())
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
            case JsonTokenType.String when fullPropertyName == targetPropertyName:
               setValue(reader);
               String();
               break;
            case JsonTokenType.Number when fullPropertyName == targetPropertyName:
               setNumber(reader);
               Number();
               break;
            case JsonTokenType.False when fullPropertyName == targetPropertyName:
               False();
               break;
            case JsonTokenType.True when fullPropertyName == targetPropertyName:
               True();
               break;
            case JsonTokenType.Null when fullPropertyName == targetPropertyName:
               Null();
               break;
         }

         SetRunningFlag();
      }

      return _result;
   }
}