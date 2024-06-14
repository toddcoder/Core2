using System.Buffers;
using System.Text.Json;
using System.Text;
using Core.Collections;
using Core.DataStructures;
using Core.Enumerables;
using Core.Numbers;
using Core.Strings;

namespace Core.Json;

public class JsonRetriever(string json, JsonRetrieverOptions options = JsonRetrieverOptions.None)
{
   protected Bits32<JsonRetrieverOptions> retrieverOptions = options;

   public IEnumerable<(string propertyName, string value)> Enumerable(params string[] propertyNames)
   {
      StringSet propertyNameSet = [.. propertyNames];
      MaybeStack<string> prefixes = [];

      var bytes = Encoding.UTF8.GetBytes(json);
      var sequence = new ReadOnlySequence<byte>(bytes);
      var options = new JsonReaderOptions();
      var reader = new Utf8JsonReader(sequence, options);

      var propertyName = "";
      List<(string, string)> list = [];

      while (reader.Read())
      {
         switch (reader.TokenType)
         {
            case JsonTokenType.StartObject or JsonTokenType.StartArray when retrieverOptions[JsonRetrieverOptions.UsesPath]:
               if (propertyName.IsNotEmpty())
               {
                  prefixes.Push(propertyName);
                  propertyName = "";
               }

               break;
            case JsonTokenType.EndObject or JsonTokenType.EndArray when retrieverOptions[JsonRetrieverOptions.UsesPath]:
               propertyName = prefixes.Pop() | "";
               break;
            case JsonTokenType.PropertyName:
               propertyName = reader.GetString() ?? "";
               break;
            case JsonTokenType.String or JsonTokenType.Number when keyMatches():
            {
               var value = reader.GetString()!;
               list.Add((propertyName, value));
               break;
            }
            case JsonTokenType.False when keyMatches():
               list.Add((propertyName, "false"));

               break;
            case JsonTokenType.True when keyMatches():
               list.Add((propertyName, "true"));

               break;
            case JsonTokenType.Null when keyMatches():
               list.Add((propertyName, ""));

               break;
         }

         if (list.Count > 0 && retrieverOptions[JsonRetrieverOptions.StopAfterFirstRetrieval])
         {
            return list;
         }
      }

      return list;

      string getPropertyName()
      {
         return retrieverOptions[JsonRetrieverOptions.UsesPath] && prefixes.Count > 0 ? $"{prefixes.ToString(".")}.{propertyName}" : propertyName;
      }

      bool keyMatches()
      {
         var propertyName = getPropertyName();
         var matches = propertyNameSet.Contains(propertyName);
         if (retrieverOptions[JsonRetrieverOptions.StopAfterParametersConsumed])
         {
            if (matches)
            {
               propertyNameSet.Remove(propertyName);
            }

            return matches;
         }
         else
         {
            return matches;
         }
      }
   }
}