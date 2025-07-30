using Core.Collections;
using Core.Computers;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using System;
using System.Text;

namespace Core.Configurations;

public class IniDeserializer(string source)
{
   public static Result<Setting> Deserialize(string source) => new IniDeserializer(source).Deserialize();

   public static Result<Hash<(string section, string key), string>> DeserializeToHash(string source) => new IniDeserializer(source).ToHash();

   public static Result<Setting> Deserialize(FileName file) =>
      from source in file.TryTo.GetText(Encoding.UTF8)
      from setting in Deserialize(source)
      select setting;

   public static Result<Hash<(string section, string key), string>> DeserializeToHash(FileName file) =>
      from source in file.TryTo.GetText(Encoding.UTF8)
      from hash in DeserializeToHash(source)
      select hash;

   public Result<Setting> Deserialize()
   {
      try
      {
         var setting = new Setting();
         var innerSetting = new LateLazy<Setting>(true);
         foreach (var line in source.Lines())
         {
            if (line.Matches("'[' /(-[']']+) ']'; f") is (true, var result1))
            {
               var key = result1.FirstGroup;
               innerSetting.ActivateWith(() => new Setting(key));
               setting.Set(key).Setting = innerSetting.Value;
            }
            else if (line.Matches("^ /(-['=']+) '=' /(.+) $") is (true, var result2))
            {
               var key = result2.FirstGroup;
               var value = result2.SecondGroup;
               innerSetting.Value[key] = value;
            }
         }

         return setting;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Result<Hash<(string section, string key), string>> ToHash()
   {
      try
      {
         Hash<(string section, string key), string> hash = [];
         var section = new LateLazy<string>(true);

         foreach (var line in source.Lines())
         {
            if (line.Matches("'[' /(-[']']+) ']'; f") is (true, var result1))
            {
               var key = result1.FirstGroup;
               section.ActivateWith(() => key);
            }
            else if (line.Matches("^ /(-['=']+) '=' /(.+) $") is (true, var result2))
            {
               var key = result2.FirstGroup;
               var value = result2.SecondGroup;
               hash[(section.Value, key)] = value;
            }
         }

         return hash;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}