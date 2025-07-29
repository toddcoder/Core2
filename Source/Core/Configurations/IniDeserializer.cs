using System;
using System.Text;
using Core.Computers;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;

namespace Core.Configurations;

public class IniDeserializer(string source)
{
   public static Result<Setting> Deserialize(string source) => new IniDeserializer(source).Deserialize();

   public static Result<Setting> Deserialize(FileName file) =>
      from source in file.TryTo.GetText(Encoding.UTF8)
      from setting in Deserialize(source)
      select setting;

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
}