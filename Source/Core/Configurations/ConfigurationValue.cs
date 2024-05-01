using System;
using System.Linq;
using Core.Collections;
using Core.Computers;
using Core.Matching;
using Core.Objects;
using Core.Strings;

namespace Core.Configurations;

public class ConfigurationValue
{
   protected IConfigurationItemGetter getter;

   internal ConfigurationValue(IConfigurationItemGetter getter)
   {
      this.getter = getter;
   }

   public Setting Setting(string key) => getter.GetSetting(key) | (() => new Setting());

   public Item Item(string key) => getter.GetItem(key) | (() => new Item("", ""));

   public string String(string key) => getter.GetItem(key).Map(i => i.Text) | "";

   public int Int32(string key) => String(key).Value().Int32();

   public long Int64(string key) => String(key).Value().Int64();

   public float Single(string key) => String(key).Value().Single();

   public double Double(string key) => String(key).Value().Double();

   public bool Boolean(string key) => String(key).Value().Boolean();

   public DateTime DateTime(string key) => String(key).Value().DateTime();

   public Guid Guid(string key) => String(key).Value().Guid();

   public FileName FileName(string key) => String(key);

   public FolderName FolderName(string key) => String(key);

   public byte[] Bytes(string key) => String(key).FromBase64();

   public TimeSpan TimeSpan(string key) => String(key).Value().TimeSpan();

   [Obsolete("Use Array")]
   public string[] Strings(string key) => String(key).Unjoin("/s* ',' /s*");

   public string[] Array(string key) => [.. Setting(key).Items().Select(i => i.text)];

   public string[] Keys(string key) => [.. Setting(key).Items().Select(i => i.key)];

   public StringHash StringHash(string key) => Setting(key).Items().ToStringHash(i => i.key, i => i.text);
}