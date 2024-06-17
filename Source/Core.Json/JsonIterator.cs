using System.Buffers;
using System.Text;
using System.Text.Json;
using Core.Collections;
using Core.DataStructures;
using Core.Enumerables;
using Core.Numbers;
using Core.Strings;

namespace Core.Json;

public abstract class JsonIterator(string json, JsonRetrieverOptions options)
{
   protected Bits32<JsonRetrieverOptions> bits = options;
   protected MaybeStack<string> prefixes = [];
   protected string propertyName = "";
   protected string fullPropertyName = "";
   protected StringSet propertyNameSet = [];
   protected bool isRunning;
   protected string value = "";

   protected Utf8JsonReader getReader()
   {
      var bytes = Encoding.UTF8.GetBytes(json);
      var sequence = new ReadOnlySequence<byte>(bytes);

      return new Utf8JsonReader(sequence);
   }

   protected void depletePropertyNameSet()
   {
      if (bits[JsonRetrieverOptions.StopAfterParametersConsumed])
      {
         propertyNameSet.Remove(fullPropertyName);
         if (propertyNameSet.Count == 0)
         {
            isRunning = false;
         }
      }
   }

   protected void stopAfterFirst(bool predicate)
   {
      if (predicate && bits[JsonRetrieverOptions.StopAfterFirstRetrieval])
      {
         isRunning = false;
      }
   }

   protected void setPropertyName()
   {
      propertyName = value;
      fullPropertyName = getFullPropertyName();
   }

   protected void setValue(Utf8JsonReader reader) => value = reader.GetString() ?? "";

   public virtual void StartObject()
   {
      if (propertyName.IsNotEmpty())
      {
         prefixes.Push(propertyName);
         propertyName = "";
      }
   }

   public virtual void StartArray()
   {
      if (propertyName.IsNotEmpty())
      {
         prefixes.Push(propertyName);
         propertyName = "";
      }
   }

   public virtual void EndObject()
   {
      propertyName = prefixes.Pop() | "";
   }

   public virtual void EndArray()
   {
      propertyName = prefixes.Pop() | "";
   }

   public abstract void String();

   public abstract void Number();

   public abstract void False();

   public abstract void True();

   public abstract void Null();

   public abstract void SetRunningFlag();

   protected bool keyMatches()
   {
      fullPropertyName = getFullPropertyName();
      var matches = propertyNameSet.Contains(fullPropertyName);
      if (bits[JsonRetrieverOptions.StopAfterParametersConsumed])
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

   protected string getFullPropertyName()
   {
      return bits[JsonRetrieverOptions.UsesPath] && prefixes.Count > 0 ? $"{prefixes.ToString(".")}.{propertyName}" : propertyName;
   }
}