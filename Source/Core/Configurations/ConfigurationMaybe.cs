using System;
using System.Linq;
using Core.Collections;
using Core.Computers;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations;

public class ConfigurationMaybe
{
   protected IConfigurationItemGetter getter;

   internal ConfigurationMaybe(IConfigurationItemGetter getter)
   {
      this.getter = getter;
   }

   public Maybe<Setting> Setting(string key) => getter.GetSetting(key);

   public Maybe<Item> Item(string key) => getter.GetItem(key);

   public Maybe<string> String(string key) => getter.GetItem(key).Map(i => i.Text);

   public Maybe<int> Int32(string key) => String(key).Map(i => i.Maybe().Int32());

   public Maybe<long> Int64(string key) => String(key).Map(l => l.Maybe().Int64());

   public Maybe<float> Single(string key) => String(key).Map(s => s.Maybe().Single());

   public Maybe<double> Double(string key) => String(key).Map(d => d.Maybe().Double());

   public Maybe<bool> Boolean(string key) => String(key).Map(b => b.Maybe().Boolean());

   public Maybe<DateTime> DateTime(string key) => String(key).Map(d => d.Maybe().DateTime());

   public Maybe<Guid> Guid(string key) => String(key).Map(g => g.Maybe().Guid());

   public Maybe<FileName> FileName(string key)
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
      catch
      {
         return nil;
      }
   }

   public Maybe<FolderName> FolderName(string key)
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
      catch
      {
         return nil;
      }
   }

   public Maybe<byte[]> Bytes(string key)
   {
      try
      {
         return String(key).Map(s => s.FromBase64());
      }
      catch
      {
         return nil;
      }
   }

   public Maybe<TimeSpan> TimeSpan(string key) => String(key).Map(s => s.Maybe().TimeSpan());

   [Obsolete("Use Array")]
   public Maybe<string[]> Strings(string key) => String(key).Map(s => s.Unjoin("/s* ',' /s*"));

   public Maybe<string[]> Array(string key) => Setting(key).Map(s => (string[]) [.. s.Items().Select(i => i.text)]);

   public Maybe<string[]> Keys(string key) => Setting(key).Map(s => (string[]) [.. s.Items().Select(i => i.key)]);

   public Maybe<StringHash> StringHash(string key) => Setting(key).Map(s => s.Items().ToStringHash(i => i.key, i => i.text));
}