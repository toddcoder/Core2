using System;
using System.Linq;
using Core.Arrays;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
using Core.Monads;

namespace Core.Configurations;

public class SettingSetter(Setting setting, string key)
{
   public Setting Setting
   {
      set => setting.SetItem(key, value);
   }

   public Setting CurrentSetting => setting;

   public string Key => key;

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

   [Obsolete("Use Array")]
   public string[] Strings
   {
      set => setItem(value.ToString(", "));
   }

   public string[] Array
   {
      set
      {
         var arraySetting = new Setting(key) { IsArray = true };
         foreach (var (index, item) in value.Indexed())
         {
            arraySetting[$"${index}"] = item;
         }

         setting.SetItem(key, arraySetting);
      }
   }

   public string[] SettingTexts
   {
      set => setting.SetItem(key, new Setting(value.Indexed().Select(t => (key: $"${t.index}", value: t.item))));
   }

   public StringHash StringHash
   {
      set
      {
         var innerSetting = new Setting(key) { IsHash = true };
         foreach (var (hashKey, hashValue) in value)
         {
            innerSetting[hashKey] = hashValue;
         }

         setting.SetItem(key, innerSetting);
      }
   }

   public Result<Setting> Serialize<T>(T value, params string[] propertyNames) where T : class, new()
   {
      var predicate = Setting.ParamsToPredicate(propertyNames);
      var _newSetting = Setting.Serialize(value, true, predicate, key);
      if (_newSetting is (true, var newSetting))
      {
         Setting = newSetting;
      }

      return _newSetting;
   }

   public Result<Setting> Serialize(Type type, object value, params string[] propertyNames)
   {
      var predicate = Setting.ParamsToPredicate(propertyNames);
      var _newSetting = Setting.Serialize(type, value, true, predicate, key);
      if (_newSetting is (true, var newSetting))
      {
         Setting = newSetting;
      }

      return _newSetting;
   }

   protected static void setTuple<T>(Setting tupleSetting, string name, T value)
   {
      switch (value)
      {
         case null:
            break;
         case string s:
            tupleSetting.Set(name).String = s;
            break;
         case int i:
            tupleSetting.Set(name).Int32 = i;
            break;
         case long l:
            tupleSetting.Set(name).Int64 = l;
            break;
         case float f:
            tupleSetting.Set(name).Single = f;
            break;
         case double d:
            tupleSetting.Set(name).Double = d;
            break;
         case bool b:
            tupleSetting.Set(name).Boolean = b;
            break;
         case DateTime dt:
            tupleSetting.Set(name).DateTime = dt;
            break;
         case Guid guid:
            tupleSetting.Set(name).Guid = guid;
            break;
         case FileName file:
            tupleSetting.Set(name).FileName = file;
            break;
         case FolderName folder:
            tupleSetting.Set(name).FolderName = folder;
            break;
         case byte[] bytes:
            tupleSetting.Set(name).Bytes = bytes;
            break;
         case TimeSpan timeSpan:
            tupleSetting.Set(name).TimeSpan = timeSpan;
            break;
         case string[] array:
            tupleSetting.Set(name).Array = array;
            break;
         case StringHash stringHash:
            tupleSetting.Set(name).StringHash = stringHash;
            break;
         default:
            tupleSetting.Set(name).Serialize(value.GetType(), value);
            break;
      }
   }

   protected static string[] getFieldNames(Type tupleType) => [.. tupleType.GetFields().Select(f => f.Name)];

   public void Tuple<T1, T2>((string name0, T1 value0) tuple0, (string name1, T2 value1) tuple1)
   {
      var tupleSetting = new Setting(key);

      setTuple(tupleSetting, tuple0.name0, tuple0.value0);
      setTuple(tupleSetting, tuple1.name1, tuple1.value1);

      Setting = tupleSetting;
   }

   public void Tuple<T1, T2, T3>((string name0, T1 value0) tuple0, (string name1, T2 value1) tuple1, (string name2, T3 value2) tuple2)
   {
      var tupleSetting = new Setting(key);

      setTuple(tupleSetting, tuple0.name0, tuple0.value0);
      setTuple(tupleSetting, tuple1.name1, tuple1.value1);
      setTuple(tupleSetting, tuple2.name2, tuple2.value2);

      Setting = tupleSetting;
   }

   public void Tuple<T1, T2, T3, T4>((string name0, T1 value0) tuple0, (string name1, T2 value1) tuple1, (string name2, T3 value2) tuple2,
      (string name3, T4 value3) tuple3)
   {
      var tupleSetting = new Setting(key);

      setTuple(tupleSetting, tuple0.name0, tuple0.value0);
      setTuple(tupleSetting, tuple1.name1, tuple1.value1);
      setTuple(tupleSetting, tuple2.name2, tuple2.value2);
      setTuple(tupleSetting, tuple3.name3, tuple3.value3);

      Setting = tupleSetting;
   }

   public void Tuple<T1, T2, T3, T4, T5>((string name0, T1 value0) tuple0, (string name1, T2 value1) tuple1, (string name2, T3 value2) tuple2,
      (string name3, T4 value3) tuple3, (string name4, T5 value4) tuple4)
   {
      var tupleSetting = new Setting(key);

      setTuple(tupleSetting, tuple0.name0, tuple0.value0);
      setTuple(tupleSetting, tuple1.name1, tuple1.value1);
      setTuple(tupleSetting, tuple2.name2, tuple2.value2);
      setTuple(tupleSetting, tuple3.name3, tuple3.value3);
      setTuple(tupleSetting, tuple4.name4, tuple4.value4);

      Setting = tupleSetting;
   }

   public void Tuple<T1, T2, T3, T4, T5, T6>((string name0, T1 value0) tuple0, (string name1, T2 value1) tuple1, (string name2, T3 value2) tuple2,
      (string name3, T4 value3) tuple3, (string name4, T5 value4) tuple4, (string name5, T6 value5) tuple5)
   {
      var tupleSetting = new Setting(key);

      setTuple(tupleSetting, tuple0.name0, tuple0.value0);
      setTuple(tupleSetting, tuple1.name1, tuple1.value1);
      setTuple(tupleSetting, tuple2.name2, tuple2.value2);
      setTuple(tupleSetting, tuple3.name3, tuple3.value3);
      setTuple(tupleSetting, tuple4.name4, tuple4.value4);
      setTuple(tupleSetting, tuple5.name5, tuple5.value5);

      Setting = tupleSetting;
   }

   public void Tuple<T1, T2, T3, T4, T5, T6, T7>((string name0, T1 value0) tuple0, (string name1, T2 value1) tuple1, (string name2, T3 value2) tuple2,
      (string name3, T4 value3) tuple3, (string name4, T5 value4) tuple4, (string name5, T6 value5) tuple5, (string name6, T7 value6) tuple6)
   {
      var tupleSetting = new Setting(key);

      setTuple(tupleSetting, tuple0.name0, tuple0.value0);
      setTuple(tupleSetting, tuple1.name1, tuple1.value1);
      setTuple(tupleSetting, tuple2.name2, tuple2.value2);
      setTuple(tupleSetting, tuple3.name3, tuple3.value3);
      setTuple(tupleSetting, tuple4.name4, tuple4.value4);
      setTuple(tupleSetting, tuple5.name5, tuple5.value5);
      setTuple(tupleSetting, tuple6.name6, tuple6.value6);

      Setting = tupleSetting;
   }
}