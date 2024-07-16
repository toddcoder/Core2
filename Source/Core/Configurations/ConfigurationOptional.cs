using Core.Computers;
using Core.Monads;
using System;
using System.Linq;
using Core.Collections;
using Core.Matching;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using System.Reflection;

namespace Core.Configurations;

public class ConfigurationOptional
{
   protected IConfigurationItemGetter getter;

   internal ConfigurationOptional(IConfigurationItemGetter getter)
   {
      this.getter = getter;
   }

   public Optional<Setting> Setting(string key) => getter.GetSetting(key).Optional();

   public Optional<Item> Item(string key) => getter.GetItem(key).Optional();

   public Optional<string> String(string key) => getter.GetItem(key).Map(i => i.Text).Optional();

   public Optional<int> Int32(string key) => String(key).Map(i => i.Optional().Int32());

   public Optional<long> Int64(string key) => String(key).Map(l => l.Optional().Int64());

   public Optional<float> Single(string key) => String(key).Map(s => s.Optional().Single());

   public Optional<double> Double(string key) => String(key).Map(d => d.Optional().Double());

   public Optional<bool> Boolean(string key) => String(key).Map(b => b.Optional().Boolean());

   public Optional<DateTime> DateTime(string key) => String(key).Map(d => d.Optional().DateTime());

   public Optional<Guid> Guid(string key) => String(key).Map(g => g.Optional().Guid());

   public Optional<FileName> FileName(string key)
   {
      try
      {
         var _fileName = String(key);
         if ((_fileName | "").IsEmpty())
         {
            return nil;
         }

         var _file = _fileName.Map(s => (FileName)s);
         return _file.Map(f => f.IsValid) | false ? _file : nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<FolderName> FolderName(string key)
   {
      try
      {
         var _folderName = String(key);
         if ((_folderName | "").IsEmpty())
         {
            return nil;
         }

         var _folder = _folderName.Map(s => (FolderName)s);
         return _folder.Map(f => f.IsValid) | false ? _folder : nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<byte[]> Bytes(string key)
   {
      try
      {
         return String(key).Map(s => s.FromBase64());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<TimeSpan> TimeSpan(string key) => String(key).Map(s => s.Optional().TimeSpan());

   [Obsolete("Use Array")]
   public Optional<string[]> Strings(string key) => String(key).Map(s => s.Unjoin("/s* ',' /s*"));

   public Optional<string[]> Array(string key) => Setting(key).Map(s => (string[]) [.. s.Items().Select(i => i.text)]);

   public Optional<string[]> Keys(string key) => Setting(key).Map(s => (string[]) [.. s.Items().Select(i => i.key)]);

   public Optional<StringHash> StringHash(string key) => Setting(key).Map(s => s.Items().ToStringHash(i => i.key, i => i.text));

   public Optional<T> Deserialize<T>(string key, Func<PropertyInfo, bool> predicate) where T : class, new()
   {
      return Setting(key).Map(s => s.Deserialize<T>(predicate).Optional());
   }

   public Optional<T> Deserialize<T>(string key) where T : class, new() => Setting(key).Map(s => s.Deserialize<T>().Optional());

   public Optional<object> Deserialize(string key, Type type, Func<PropertyInfo, bool> predicate)
   {
      return Setting(key).Map(s => s.Deserialize(type, predicate).Optional());
   }

   public Optional<object> Deserialize(string key, Type type) => Setting(key).Map(s => s.Deserialize(type).Optional());

   public Optional<Setting> Tuple(string key, params string[] names)
   {
      var _innerSetting = Setting(key);
      if (_innerSetting is (true, var innerSetting))
      {
         var tupleSetting = new Setting(key);
         foreach (var name in names)
         {
            var _value = innerSetting.Optional.String(name);
            if (_value is (true, var value))
            {
               tupleSetting[name] = value;
            }
            else if (_value.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               return nil;
            }
         }

         return tupleSetting;
      }
      else if (_innerSetting.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }
}