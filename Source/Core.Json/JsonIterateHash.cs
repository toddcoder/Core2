using Core.Collections;
using System.Text.Json;

namespace Core.Json;

public abstract class JsonIterateHash<TKey, TValue>(string json, JsonRetrieverOptions options) :
   JsonIterator(json, options) where TKey : notnull where TValue : notnull
{
   protected Hash<TKey, TValue> hash = [];

   public virtual Hash<TKey, TValue> Retrieve(params string[] propertyNames)
   {
      try
      {
         prefixes = [];
         propertyNameSet = [.. propertyNames];
         hash = [];

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

         return hash;
      }
      catch
      {
         return [];
      }
   }
}