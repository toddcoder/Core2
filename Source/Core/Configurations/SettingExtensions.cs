using System;
using System.Collections.Generic;
using Core.Exceptions;
using Core.Monads;

namespace Core.Configurations;

public static class SettingExtensions
{
   public static Result<Setting> ToSetting<T>(this IEnumerable<T> enumerable, Func<T, string> keyMap, string settingName = Setting.ROOT_NAME) where T : class, new()
   {
      var setting = new Setting(settingName);
      var exceptions = new MultiExceptions();

      foreach (var obj in enumerable)
      {
         var _result = Setting.Serialize(obj, false, keyMap(obj));
         if (_result is (true, var subSetting))
         {
            setting.Set(subSetting.Key).Setting = subSetting;
         }
         else
         {
            exceptions.Add(_result.Exception);
         }
      }

      return exceptions.Count == 0 ? setting : exceptions;
   }
}