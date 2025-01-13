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

public class FolderConfiguration(FolderName baseFolder)
{
   protected readonly Pattern validNamePattern = "^ ['A-Za-z_$@']  [/w '.-']* $; f";

   public Optional<Setting> Setting()
   {
      try
      {
         if (baseFolder)
         {
            var setting = new Setting();
            loadSetting(setting, baseFolder);

            return setting;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<Unit> Update(Setting setting)
   {
      try
      {
         if (baseFolder)
         {
            saveSetting(setting, baseFolder);
            return unit;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected void loadSetting(Setting currentSetting, FolderName currentFolder)
   {
      foreach (var file in currentFolder.Files.Where(f => f.Name.IsMatch(validNamePattern) && f.Extension == ".setting"))
      {
         loadFile(currentSetting, file);
      }

      foreach (var folder in currentFolder.Folders.Where(f => f.Name.IsMatch(validNamePattern)))
      {
         var folderSetting = new Setting(folder.Name);
         loadSetting(folderSetting, folder);
         currentSetting.Set(folder.Name).Setting = folderSetting;
      }
   }

   protected static void loadFile(Setting currentSetting, FileName file)
   {
      {
         var text = file.TryTo.Text | "";
         if (text.IsNotEmpty())
         {
            if (file.Name.Matches("^ /(-['.']+) '.' /(.+) $; f").Map(r => (r.FirstGroup, r.SecondGroup)) is (true, var (key, type)))
            {
               switch (type)
               {
                  case "string":
                     currentSetting.Set(key).String = text;
                     break;
                  case "boolean":
                     currentSetting.Set(key).Boolean = text == "true";
                     break;
                  case "int32":
                     currentSetting.Set(key).Int32 = text.Value().Int32();
                     break;
                  case "double":
                     currentSetting.Set(key).Double = text.Value().Double();
                     break;
                  case "base64":
                     currentSetting.Set(key).Bytes = text.FromBase64();
                     break;
                  case "array":
                     currentSetting.Set(key).Array = text.Lines();
                     break;
                  case "hash":
                     currentSetting.Set(key).StringHash = text.Lines().Select(l => getHashItem(l) | (l, "")).ToStringHash(t => t.key, t => t.value);
                     break;
                  case "redirect":
                  {
                     FileName redirectTo = text;
                     if (redirectTo)
                     {
                        loadFile(currentSetting, redirectTo);
                     }
                     break;
                  }
                  default:
                     currentSetting.Set(key).String = text;
                     break;
               }
            }
         }
      }

      return;

      Maybe<(string key, string value)> getHashItem(string line)
      {
         if (line.Find("\t") is (true, var index))
         {
            var key = line.Keep(index);
            var value = line.Drop(index + 1);
            return (key, value);
         }
         else
         {
            return nil;
         }
      }
   }

   protected void saveSetting(Setting currentSetting, FolderName currentFolder)
   {
      foreach (var file in currentFolder.Files.Where(f => f.Name.IsMatch(validNamePattern) && f.Extension == ".setting"))
      {
         file.Delete();
      }

      foreach (var (key, text) in currentSetting.Items())
      {
         var type = "string";
         var content = text;
         if (content is "true" or "false")
         {
            type = "boolean";
         }
         else if (content.Maybe().Int32())
         {
            type = "int32";
         }
         else if (content.Maybe().Double())
         {
            type = "double";
         }
         else if (content.IsBase64())
         {
            type = "base64";
         }
         else if (content.StartsWith("->"))
         {
            type = "redirect";
            content = content.Drop(2).Trim();
         }

         var file = currentFolder + $"{key}.{type}.setting";
         file.Text = content;
      }

      foreach (var (key, setting) in currentSetting.Settings().Where(t => t.setting.IsArray))
      {
         string[] array = [..setting.Items().Select(i => i.text)];
         var file = currentFolder + $"{key}.array.setting";
         file.Lines = array;
      }

      foreach (var (key, setting) in currentSetting.Settings().Where(t => t.setting.IsHash))
      {
         string[] array = [..setting.Items().Select(i => $"{i.key}\t{i.text}")];
         var file = currentFolder + $"{key}.hash.setting";
         file.Lines = array;
      }

      foreach (var folder in currentFolder.Folders.Where(f => f.Name.IsMatch(validNamePattern)))
      {
         folder.DeleteFiles();
         folder.Delete();
      }

      foreach (var (key, setting) in currentSetting.Settings().Where(t => t.setting is { IsArray: false, IsHash: false }))
      {
         var folder = currentFolder[key].Guarantee();
         saveSetting(setting, folder);
      }
   }
}