using System.Buffers;
using System.Text;
using System.Text.Json;
using Core.Computers;
using Core.Configurations;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Json;

public class Deserializer(string source)
{
   public Result<Setting> Deserialize()
   {
      var rootSetting = new Setting("/");
      var stack = new Stack<Setting>();
      stack.Push(rootSetting);
      var parentSetting = rootSetting;

      Maybe<string> _propertyName = nil;

      string getKey(Maybe<string> _propertyName)
      {
         if (_propertyName is (true, var propertyName))
         {
            return propertyName;
         }
         else
         {
            return $"${parentSetting.Count}";
         }
      }

      void setItem(string value)
      {
         var key = getKey(_propertyName);
         parentSetting.SetItem(key, new Item(key, value));
         _propertyName = nil;
      }

      void setItemNull()
      {
         var key = getKey(_propertyName);
         parentSetting.SetItem(key, new Item(key, "") { IsNull = true });
         _propertyName = nil;
      }

      try
      {
         var bytes = Encoding.UTF8.GetBytes(source);
         var sequence = new ReadOnlySequence<byte>(bytes);
         var options = new JsonReaderOptions();
         var reader = new Utf8JsonReader(sequence, options);
         var firstObjectProcessed = false;

         while (reader.Read())
         {
            switch (reader.TokenType)
            {
               case JsonTokenType.StartObject when firstObjectProcessed:
               {
                  var key = getKey(_propertyName);
                  _propertyName = nil;
                  var setting = new Setting(key);
                  parentSetting.SetItem(key, setting);

                  stack.Push(parentSetting);
                  parentSetting = setting;
                  break;
               }
               case JsonTokenType.StartObject:
                  firstObjectProcessed = true;
                  break;
               case JsonTokenType.EndObject:
                  if (stack.Count == 0)
                  {
                     return fail("No parent group available");
                  }

                  parentSetting = stack.Pop();
                  break;
               case JsonTokenType.StartArray:
               {
                  var key = getKey(_propertyName);
                  _propertyName = nil;
                  var setting = new Setting(key) { IsArray = true };
                  parentSetting.SetItem(setting.Key, setting);

                  stack.Push(parentSetting);
                  parentSetting = setting;

                  break;
               }
               case JsonTokenType.EndArray:
                  if (stack.Count == 0)
                  {
                     return fail("No parent setting available");
                  }

                  parentSetting = stack.Pop();
                  break;
               case JsonTokenType.PropertyName:
               {
                  var propertyName = reader.GetString();
                  if (propertyName is null)
                  {
                     _propertyName = nil;
                  }
                  else
                  {
                     _propertyName = propertyName;
                  }

                  break;
               }
               case JsonTokenType.String:
               {
                  var value = reader.GetString()!;
                  setItem(value);

                  break;
               }
               case JsonTokenType.Number:
               {
                  if (reader.TryGetInt32(out var intValue))
                  {
                     setItem(intValue.ToString());
                  }
                  else if (reader.TryGetSingle(out var floatValue))
                  {
                     setItem(floatValue.ToString("F"));
                  }
                  else
                  {
                     setItem(reader.GetString()!);
                  }

                  break;
               }
               case JsonTokenType.False:
                  setItem("false");
                  break;
               case JsonTokenType.True:
                  setItem("true");
                  break;
               case JsonTokenType.Null:
                  setItemNull();
                  break;
               default:
                  setItem("");
                  break;
            }
         }

         return rootSetting;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Setting> Deserialize(FileName file) => file.TryTo.GetText(Encoding.UTF8).Map(Deserialize);

   public static Result<Setting> Deserialize(string json)
   {
      var deserializer = new Deserializer(json);
      return deserializer.Deserialize();
   }
}