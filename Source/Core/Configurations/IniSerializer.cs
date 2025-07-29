using System;
using System.IO;
using System.Text;
using Core.Computers;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations;

public class IniSerializer(Setting setting)
{
   public static Result<string> Serialize(Setting setting) => new IniSerializer(setting).Serialize();

   public static Result<Unit> Serialize(Setting setting, FileName file) =>
      from serialized in Serialize(setting)
      from result in file.TryTo.SetText(serialized, Encoding.UTF8)
      select unit;

   public Result<string> Serialize()
   {
      try
      {
         using var writer = new StringWriter();
         foreach (var (key, innerSetting) in setting.Settings())
         {
            writer.WriteLine($"[{key}]");

            foreach (var (itemKey, item) in innerSetting.items)
            {
               writer.WriteLine($"{itemKey}={item.Text}");
            }
         }

         return writer.ToString();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}