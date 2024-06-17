using System.Buffers;
using System.Net;
using System.Text.Json;
using System.Text;
using Core.Collections;
using Core.DataStructures;
using Core.Dates.DateIncrements;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Json;

public class JsonRetriever(string json, JsonRetrieverOptions options = JsonRetrieverOptions.None)
{
   internal static HttpClient httpClient = null!;

   internal static Result<string> getJson(string url)
   {
      try
      {
         var response = httpClient.GetAsync(url).Result;
         var body = response.Content.ReadAsStringAsync().Result;
         if (response.StatusCode == HttpStatusCode.OK)
         {
            return body;
         }
         else
         {
            return fail(body);
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<JsonRetriever> FromUrl(string url, JsonRetrieverOptions options = JsonRetrieverOptions.None)
   {
      try
      {
         var handler = new HttpClientHandler { Credentials = CredentialCache.DefaultNetworkCredentials };
         httpClient = new HttpClient(handler) { Timeout = 10.Seconds() };
         return
            from json in getJson(url)
            select new JsonRetriever(json, options);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Bits32<JsonRetrieverOptions> retrieverOptions = options;
   protected StringSet propertyNameSet = [];
   protected MaybeStack<string> prefixes = [];
   protected string propertyName = "";
   protected string fullPropertyName = "";

   public IEnumerable<(string propertyName, string value)> Enumerable(params string[] propertyNames)
   {
      propertyNameSet = [.. propertyNames];
      prefixes = [];

      var bytes = Encoding.UTF8.GetBytes(json);
      var sequence = new ReadOnlySequence<byte>(bytes);
      var options = new JsonReaderOptions();
      var reader = new Utf8JsonReader(sequence, options);

      propertyName = "";
      fullPropertyName = "";
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
            case JsonTokenType.String or JsonTokenType.Number when keyMatches(propertyName):
            {
               var value = reader.GetString()!;
               list.Add((fullPropertyName, value));
               break;
            }
            case JsonTokenType.False when keyMatches(propertyName):
               list.Add((fullPropertyName, "false"));

               break;
            case JsonTokenType.True when keyMatches(propertyName):
               list.Add((fullPropertyName, "true"));

               break;
            case JsonTokenType.Null when keyMatches(propertyName):
               list.Add((fullPropertyName, ""));

               break;
         }

         if (list.Count > 0 && retrieverOptions[JsonRetrieverOptions.StopAfterFirstRetrieval])
         {
            return list;
         }
      }

      return list;
   }

   public Optional<string> Retrieve(string targetPropertyName)
   {
      prefixes = [];

      var bytes = Encoding.UTF8.GetBytes(json);
      var sequence = new ReadOnlySequence<byte>(bytes);
      var options = new JsonReaderOptions();
      var reader = new Utf8JsonReader(sequence, options);

      propertyName = "";
      fullPropertyName = "";

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
               fullPropertyName = getFullPropertyName(propertyName);
               break;
            case JsonTokenType.String or JsonTokenType.Number when fullPropertyName == targetPropertyName:
               return reader.GetString() ?? "";
            case JsonTokenType.False when fullPropertyName == targetPropertyName:
               return "false";
            case JsonTokenType.True when fullPropertyName == targetPropertyName:
               return "true";
            case JsonTokenType.Null when fullPropertyName == targetPropertyName:
               return "";
         }
      }

      return nil;
   }

   public StringHash RetrieveHash(params string[] propertyNames)
   {
      propertyNameSet = [.. propertyNames];
      prefixes = [];

      var bytes = Encoding.UTF8.GetBytes(json);
      var sequence = new ReadOnlySequence<byte>(bytes);
      var options = new JsonReaderOptions();
      var reader = new Utf8JsonReader(sequence, options);

      propertyName = "";
      fullPropertyName = "";
      StringHash hash = [];

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
            case JsonTokenType.String or JsonTokenType.Number when keyMatches(propertyName):
            {
               var value = reader.GetString()!;
               hash[fullPropertyName] = value;
               break;
            }
            case JsonTokenType.False when keyMatches(propertyName):
               hash[fullPropertyName] = "false";

               break;
            case JsonTokenType.True when keyMatches(propertyName):
               hash[fullPropertyName] = "true";

               break;
            case JsonTokenType.Null when keyMatches(propertyName):
               hash[fullPropertyName] = "";

               break;
         }

         if (hash.Count > 0 && retrieverOptions[JsonRetrieverOptions.StopAfterFirstRetrieval])
         {
            return hash;
         }
      }

      return hash;
   }

   public AutoStringHash<List<string>> RetrieveListHash(params string[] propertyNames)
   {
      propertyNameSet = [.. propertyNames];
      prefixes = [];

      var bytes = Encoding.UTF8.GetBytes(json);
      var sequence = new ReadOnlySequence<byte>(bytes);
      var options = new JsonReaderOptions();
      var reader = new Utf8JsonReader(sequence, options);

      propertyName = "";
      fullPropertyName = "";
      var hash = new AutoStringHash<List<string>>(_ => new List<string>(), true);

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
            case JsonTokenType.String or JsonTokenType.Number when keyMatches(propertyName):
            {
               var value = reader.GetString()!;
               hash[fullPropertyName].Add(value);
               break;
            }
            case JsonTokenType.False when keyMatches(propertyName):
               hash[fullPropertyName].Add("false");

               break;
            case JsonTokenType.True when keyMatches(propertyName):
               hash[fullPropertyName].Add("true");

               break;
            case JsonTokenType.Null when keyMatches(propertyName):
               hash[fullPropertyName].Add("");

               break;
         }

         if (hash.Count > 0 && retrieverOptions[JsonRetrieverOptions.StopAfterFirstRetrieval])
         {
            return hash;
         }
      }

      return hash;
   }

   protected bool keyMatches(string propertyName)
   {
      fullPropertyName = getFullPropertyName(propertyName);
      var matches = propertyNameSet.Contains(fullPropertyName);
      if (retrieverOptions[JsonRetrieverOptions.StopAfterParametersConsumed])
      {
         if (matches)
         {
            propertyNameSet.Remove(fullPropertyName);
         }

         return matches;
      }
      else
      {
         return matches;
      }
   }

   protected string getFullPropertyName(string propertyName)
   {
      return retrieverOptions[JsonRetrieverOptions.UsesPath] && prefixes.Count > 0 ? $"{prefixes.ToString(".")}.{propertyName}" : propertyName;
   }
}