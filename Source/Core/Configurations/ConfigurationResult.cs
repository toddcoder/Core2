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

public class ConfigurationResult
{
   protected IConfigurationItemGetter getter;

   internal ConfigurationResult(IConfigurationItemGetter getter)
   {
      this.getter = getter;
   }

   public Result<Setting> Setting(string key) => getter.GetSetting(key).Result($"Setting {key} required");

   public Result<Item> Item(string key) => getter.GetItem(key).Result($"Item {key} required");

   public Result<string> String(string key) => getter.GetItem(key).Map(i => i.Text).Result($"Item {key} required");

   public Result<int> Int32(string key) => String(key).Map(i => i.Result().Int32());

   public Result<long> Int64(string key) => String(key).Map(l => l.Result().Int64());

   public Result<float> Single(string key) => String(key).Map(s => s.Result().Single());

   public Result<double> Double(string key) => String(key).Map(d => d.Result().Double());

   public Result<bool> Boolean(string key) => String(key).Map(b => b.Result().Boolean());

   public Result<DateTime> DateTime(string key) => String(key).Map(d => d.Result().DateTime());

   public Result<Guid> Guid(string key) => String(key).Map(g => g.Result().Guid());

   public Result<FileName> FileName(string key)
   {
      try
      {
         var _fileName = String(key);
         if (_fileName is (true, var fileName))
         {
            if (fileName.IsEmpty())
            {
               return fail("File name is empty");
            }
         }
         else
         {
            return _fileName.Exception;
         }

         var _file = _fileName.Map(s => (FileName)s);
         return _file.Map(f => f.IsValid) | false ? _file : _file.Exception;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Result<FolderName> FolderName(string key)
   {
      try
      {
         var _folderName = String(key);
         if (_folderName is (true, var folderName))
         {
            if (folderName.IsEmpty())
            {
               return fail("Folder name is empty");
            }
         }
         else
         {
            return _folderName.Exception;
         }

         var _folder = _folderName.Map(s => (FolderName)s);
         return _folder.Map(f => f.IsValid) | false ? _folder : _folder.Exception;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Result<byte[]> Bytes(string key)
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

   public Result<TimeSpan> TimeSpan(string key) => String(key).Map(t => t.Result().TimeSpan());

   [Obsolete("Use Array")]
   public Result<string[]> Strings(string key) => String(key).Map(s => s.Unjoin("/s* ',' /s*"));

   public Result<string[]> Array(string key) => Setting(key).Map(s => (string[]) [.. s.Items().Select(i => i.text)]);

   public Result<string[]> Keys(string key) => Setting(key).Map(s => (string[]) [.. s.Items().Select(i => i.key)]);

   public Result<StringHash> StringHash(string key) => Setting(key).Map(s => s.Items().ToStringHash(i => i.key, i => i.text));
}