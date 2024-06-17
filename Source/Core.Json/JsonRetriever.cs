using System.Net;
using Core.Collections;
using Core.Dates.DateIncrements;
using Core.Monads;
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

   public IEnumerable<(string propertyName, string value)> Enumerable(params string[] propertyNames)
   {
      var collection = new JsonIterateCollectionNameValue(json, options);
      return collection.Retrieve(propertyNames);
   }

   public Optional<string> Retrieve(string targetPropertyName)
   {
      var singleValue = new JsonSingleValue(json, options);
      return singleValue.Retrieve(targetPropertyName);
   }

   public StringHash RetrieveHash(params string[] propertyNames)
   {
      var hash = new JsonIterateStringHash(json, options);
      return hash.Retrieve(propertyNames).ToStringHash();
   }

   public StringHash<List<string>> RetrieveListHash(params string[] propertyNames)
   {
      var hash = new JsonIterateStringListHash(json, options);
      return hash.Retrieve(propertyNames).ToStringHash();
   }
}