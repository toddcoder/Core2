using System.Text;
using Core.Computers;
using Core.Configurations;
using Core.Matching;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Json;

public class Serializer(Setting setting)
{
   public Result<string> Serialize()
   {
      try
      {
         using var writer = new JsonWriter();
         writer.BeginObject();
         foreach (var item in setting)
         {
            writeItem(writer, item);
         }

         writer.EndObject();

         return writer.ToString();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected static void writeItem(JsonWriter writer, ConfigurationItem configurationItem)
   {
      switch (configurationItem)
      {
         case Item { IsNull: true } nullItem:
            writeNull(writer, nullItem);
            break;
         case Item item:
            write(writer, item);
            break;
         case Setting { IsArray: true } arraySetting:
            writeArray(writer, arraySetting);
            break;
         case Setting objectSetting:
            writeObject(writer, objectSetting);
            break;
      }
   }

   protected static bool isGeneratedKey(string key) => key.IsMatch("^ '$' /d+ $; f");

   protected static Maybe<string> getName(ConfigurationItem item) => maybe<string>() & !isGeneratedKey(item.Key) & (() => item.Key);

   protected static void writeText(JsonWriter writer, Maybe<string> _name, string text)
   {
      LazyMaybe<int> _int = nil;
      LazyMaybe<double> _float = nil;
      LazyMaybe<bool> _bool = nil;

      writer.WritePropertyNameIf(_name);

      if (_int.ValueOf(text.Maybe().Int32()) is (true, var @int))
      {
         writer.Write(@int);
      }
      else if (_float.ValueOf(text.Maybe().Double()) is (true, var @float))
      {
         writer.Write(@float);
      }
      else if (_bool.ValueOf(text.Maybe().Boolean()) is (true, var @bool))
      {
         writer.Write(@bool);
      }
      else
      {
         writer.Write(text);
      }
   }

   protected static void write(JsonWriter writer, Item item)
   {
      var _name = getName(item);
      writeText(writer, _name, item.Text);
   }

   protected static void writeNull(JsonWriter writer, Item item) => writer.WriteNull(item.Key);

   protected static void writeArray(JsonWriter writer, Setting setting)
   {
      var _name = getName(setting);
      writer.WritePropertyNameIf(_name);
      writer.BeginArray();

      foreach (var item in setting)
      {
         writeItem(writer, item);
      }

      writer.EndArray();
   }

   protected static void writeObject(JsonWriter writer, Setting setting)
   {
      var _name = getName(setting);
      writer.WritePropertyNameIf(_name);
      writer.BeginObject();

      foreach (var item in setting)
      {
         writeItem(writer, item);
      }

      writer.EndObject();
   }

   public static Result<string> Serialize(FileName file, Setting setting)
   {
      return
         from json in Serialize(setting)
         from _ in file.TryTo.SetText(json, Encoding.UTF8)
         select json;
   }

   public static Result<string> Serialize(Setting setting)
   {
      var serializer = new Serializer(setting);
      return serializer.Serialize();
   }
}