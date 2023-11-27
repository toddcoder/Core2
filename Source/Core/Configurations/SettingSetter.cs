using System;
using System.Linq;
using Core.Arrays;
using Core.Computers;
using Core.Enumerables;

namespace Core.Configurations;

public class SettingSetter(Setting setting, string key)
{
   public Setting Setting
   {
      set => setting.SetItem(key, value);
   }

   protected void setItem(string value) => setting.SetItem(key, new Item(key, value));

   protected void setItem(object obj) => setting.SetItem(key, new Item(key, obj.ToString()!));

   public string String
   {
      set => setItem(value);
   }

   public int Int32
   {
      set => setItem(value);
   }

   public long Int64
   {
      set => setItem(value);
   }

   public float Single
   {
      set => setItem(value);
   }

   public double Double
   {
      set => setItem(value);
   }

   public bool Boolean
   {
      set => setItem(value.ToString().ToLower());
   }

   public DateTime DateTime
   {
      set => setItem(value);
   }

   public Guid Guid
   {
      set => setItem(value);
   }

   public FileName FileName
   {
      set => setItem(value.FullPath);
   }

   public FolderName FolderName
   {
      set => setItem(value.FullPath);
   }

   public byte[] Bytes
   {
      set => setItem(value.ToBase64());
   }

   public TimeSpan TimeSpan
   {
      set => setItem(value);
   }

   public string[] Strings
   {
      set => setItem(value.ToString(", "));
   }

   public string[] SettingTexts
   {
      set => setting.SetItem(key, new Setting(value.Indexed().Select(t => (key: $"${t.index}", value: t.item))));
   }
}