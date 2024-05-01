using Core.Configurations;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Data.Configurations;

public class DataSettings
{
   public DataSettings()
   {
      ConnectionsSetting = nil;
      CommandsSetting = nil;
      AdaptersSetting = nil;
   }

   public Maybe<Setting> ConnectionsSetting { get; init; }

   public Maybe<Setting> CommandsSetting { get; init; }

   public Maybe<Setting> AdaptersSetting { get; init; }

   public Result<string> Command(string adapterName)
   {
      var _adapterSetting = AdaptersSetting.Map(s => s.Maybe.Setting(adapterName));
      if (_adapterSetting is (true, var adapterSetting))
      {
         var commandName = adapterSetting.Maybe.String("command") | adapterName;
         var _commandSetting = CommandsSetting.Map(s => s.Maybe.Setting(commandName));
         if (_commandSetting is (true, var commandSetting))
         {
            var command = new Command(commandSetting);
            return command.Text;
         }
         else
         {
            return fail($"Didn't find command setting '{commandName}'");
         }
      }
      else
      {
         return fail($"Didn't find adapter setting '{adapterName}'");
      }
   }
}