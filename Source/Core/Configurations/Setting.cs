﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Arrays;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.DataStructures;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Configurations;

public class Setting(string rootKey = "_$root") : ConfigurationItem, IHash<string, string>, IEnumerable<ConfigurationItem>, IConfigurationItemGetter
{
   public const string ROOT_NAME = "_$root";

   public static Configuration operator +(Setting setting, FileName file) => new(file, setting.items, setting.Key);

   protected static Set<Type> baseTypes =
   [
      typeof(string), typeof(int), typeof(long), typeof(float), typeof(double), typeof(bool), typeof(DateTime), typeof(Guid), typeof(FileName),
      typeof(FolderName), typeof(byte[])
   ];

   public static implicit operator Setting(string source) => FromString(source).ForceValue();

   public static Result<Setting> FromString(string source)
   {
      var parser = new Parser(source);
      return parser.Parse();
   }

   public static Func<PropertyInfo, bool> ParamsToPredicate(string[] propertyNames)
   {
      if (propertyNames.Length == 0)
      {
         return _ => true;
      }
      else
      {
         StringSet propertyNamesSet = [.. propertyNames];
         return p => propertyNamesSet.Contains(p.Name);
      }
   }

   protected bool isGeneratedKey = rootKey.StartsWith("__$key");
   internal StringHash<ConfigurationItem> items = [];

   public Setting(IEnumerable<(string key, string value)> items, string key = ROOT_NAME) : this(key)
   {
      foreach (var (itemKey, value) in items)
      {
         this.items[itemKey] = new Item(itemKey, value);
      }
   }

   public override string Key => rootKey;

   public override string Text => items.Values.Select(i => i.Text).ToString(" ");

   public bool IsArray { get; set; }

   public bool IsHash { get; set; }

   public SettingSetter Set(string key) => new(this, key);

   Maybe<Setting> IConfigurationItemGetter.GetSetting(string key)
   {
      if (items.Maybe[key] is (true, Setting setting))
      {
         return setting;
      }
      else
      {
         return nil;
      }
   }

   Maybe<Item> IConfigurationItemGetter.GetItem(string key)
   {
      if (items.Maybe[key] is (true, Item item))
      {
         return item;
      }
      else
      {
         return nil;
      }
   }

   public override void SetItem(string key, ConfigurationItem item) => items[key] = item;

   public override void RemoveItem(string key) => items.Maybe[key] = nil;

   public bool IsGeneratedKey => isGeneratedKey;

   public string this[string key]
   {
      get => Value.String(key);
      set => items[key] = new Item(key, value);
   }

   public bool ContainsKey(string key) => items.ContainsKey(key);

   public Hash<string, string> GetHash() => items.ToStringHash(i => i.Key, i => i.Value.ToString() ?? "");

   HashInterfaceMaybe<string, string> IHash<string, string>.Items => new(this);

   public StringHash ToStringHash() => Items().ToHash(t => t.key, t => t.text).ToStringHash();

   public override IEnumerable<(string key, string text)> Items()
   {
      foreach (var item in items.Where(i => i.Value is Item).Select(i => (Item)i.Value))
      {
         yield return (item.Key, item.Text);
      }
   }

   public override IEnumerable<(string key, ConfigurationItem)> ConfigurationItems()
   {
      foreach (var (key, item) in items)
      {
         yield return (key, item);
      }
   }

   public override IEnumerable<(string key, Setting setting)> Settings()
   {
      foreach (var (key, item) in items.Where(i => i.Value is Setting))
      {
         yield return (key, (Setting)item);
      }
   }

   public override int Count => items.Count;

   public string ToString(int indent, bool ignoreSelf = false)
   {
      using var writer = new StringWriter();

      if (!ignoreSelf)
      {
         if (isGeneratedKey)
         {
            writer.WriteLine($"{indentation()}[");
         }
         else
         {
            writer.WriteLine($"{indentation()}{Key} [");
         }

         indent++;
      }

      foreach (var (_, value) in items)
      {
         switch (value)
         {
            case Setting setting:
               writer.Write(setting.ToString(indent));
               break;
            case Item item:
               item.Indentation = indent;
               writer.WriteLine($"{indentation()}{item}");
               break;
         }
      }

      if (!ignoreSelf)
      {
         indent--;
         writer.WriteLine($"{indentation()}]");
      }

      return writer.ToString();

      string indentation() => " ".Repeat(indent * 3);
   }

   public string ToString(bool ignoreSelf) => ToString(0, ignoreSelf);

   public override string ToString() => ToString(true);

   public IEnumerator<ConfigurationItem> GetEnumerator() => items.Values.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   protected static bool isBaseType(Type type) => baseTypes.Contains(type) || type.IsEnum;

   protected static object makeArray(Type elementType, string[] sourceArray)
   {
      var length = sourceArray.Length;
      var newArray = System.Array.CreateInstance(elementType, length);
      for (var i = 0; i < length; i++)
      {
         var item = sourceArray[i];
         var _object = getConversion(elementType, item);
         if (_object is (true, var @object))
         {
            newArray.SetValue(@object, i);
         }
      }

      return newArray;
   }

   protected static Maybe<object> makeArray(Type? elementType, Setting[] settings)
   {
      if (elementType is null)
      {
         return nil;
      }

      var length = settings.Length;
      var newArray = System.Array.CreateInstance(elementType, length);
      for (var i = 0; i < length; i++)
      {
         var setting = settings[i];
         var _element = setting.Deserialize(elementType);
         if (_element is (true, var element))
         {
            newArray.SetValue(element, i);
         }
         else
         {
            return nil;
         }
      }

      return newArray;
   }

   protected static Maybe<object> getConversion(Type type, string source)
   {
      string sourceWithoutQuotes()
      {
         var withoutQuotes = source.StartsWith(@"""") && source.EndsWith(@"""") ? source.Drop(1).Drop(-1) : source;
         var unescaped = withoutQuotes.ReplaceAll(("`t", "\t"), ("`r", "\r"), ("`", "\n"), ("``", "`"));

         return unescaped;
      }

      if (type == typeof(string))
      {
         return sourceWithoutQuotes().Some<object>();
      }
      else if (type == typeof(int))
      {
         return source.Value().Int32().Some<object>();
      }
      else if (type == typeof(long))
      {
         return source.Value().Int64().Some<object>();
      }
      else if (type == typeof(float))
      {
         return source.Value().Single().Some<object>();
      }
      else if (type == typeof(double))
      {
         return source.Value().Double().Some<object>();
      }
      else if (type == typeof(bool))
      {
         return source.Same("true").Some<object>();
      }
      else if (type == typeof(DateTime))
      {
         return source.Value().DateTime().Some<object>();
      }
      else if (type == typeof(Guid))
      {
         return source.Value().Guid().Some<object>();
      }
      else if (type == typeof(FileName))
      {
         return new FileName(sourceWithoutQuotes()).Some<object>();
      }
      else if (type == typeof(FolderName))
      {
         return new FolderName(sourceWithoutQuotes()).Some<object>();
      }
      else if (type == typeof(byte[]))
      {
         return source.FromBase64().Some<object>();
      }
      else if (type.IsEnum)
      {
         return source.ToBaseEnumeration(type).Some<object>();
      }
      else if (type.IsArray)
      {
         LazyResult<Setting> _arraySetting = nil;
         var elementType = type.GetElementType();
         if (elementType is not null)
         {
            if (isBaseType(elementType))
            {
               var strings = source.Unjoin("/s* ',' /s*; f");
               return makeArray(elementType, strings);
            }
            else if (_arraySetting.ValueOf(FromString(source)) is (true, var arraySetting))
            {
               Setting[] settings = [.. arraySetting.Settings().Select(t => t.setting)];
               return makeArray(elementType, settings);
            }
            else
            {
               return nil;
            }
         }
         else
         {
            return nil;
         }
      }
      else
      {
         return FromString(source).Map(setting => setting.Deserialize(type));
      }
   }

   protected static string toString(object? obj, Type type, bool encloseString)
   {
      static string encloseInQuotes(string text)
      {
         var escaped = text.ReplaceAll(("`", "``"), ("\t", "`t"), ("\r", "`r"), ("\n", "`n"));
         return $"\"{escaped}\"";
      }

      if (obj is null)
      {
         return "";
      }

      if (type == typeof(byte[]))
      {
         return ((byte[])obj).ToBase64();
      }
      else if (type.IsArray)
      {
         var array = (Array?)obj;
         List<string> list = [];
         if (array is not null)
         {
            foreach (var item in array)
            {
               if (item is not null && isBaseType(item.GetType()))
               {
                  list.Add(item.ToString() ?? "");
               }
            }
         }

         return list.ToString(", ");
      }
      else if (type == typeof(bool))
      {
         return (obj.ToString() ?? "").ToLower();
      }
      else if (type == typeof(string) || type == typeof(FileName) || type == typeof(FolderName))
      {
         var objToString = obj.ToString() ?? "";
         return encloseString ? encloseInQuotes(objToString) : objToString;
      }
      else if (type.IsEnum)
      {
         return obj.ToString() ?? "";
      }
      else
      {
         return obj.ToString() ?? "";
      }
   }

   protected static PropertyInfo[] getPropertyInfo(Type type, Func<PropertyInfo, bool> predicate)
   {
      var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.GetProperty)
         .Where(predicate);
      return [.. propertyInfos];
   }

   public static Result<Setting> Serialize<T>(T obj, bool encloseStrings, Func<PropertyInfo, bool> predicate, string name = ROOT_NAME)
      where T : class, new()
   {
      return tryTo(() => Serialize(typeof(T), obj, encloseStrings, predicate, name));
   }

   public static Result<Setting> Serialize<T>(T obj, bool encloseStrings, string name = ROOT_NAME) where T : class, new()
   {
      return tryTo(() => Serialize(typeof(T), obj, encloseStrings, _ => true, name));
   }

   public static Result<Setting> Serialize(Type type, object? obj, bool encloseStrings, Func<PropertyInfo, bool> predicate, string name = ROOT_NAME)
   {
      if (obj is null)
      {
         return fail("Object is null");
      }

      if (type.IsValueType)
      {
         return fail($"Type provided ({type.FullName}) is a value type. Only classes are allowed");
      }
      else
      {
         try
         {
            obj.Must().Not.BeNull().OrThrow();

            var setting = new Setting(name);

            var allPropertyInfo = getPropertyInfo(obj.GetType(), predicate);
            foreach (var propertyInfo in allPropertyInfo)
            {
               var propertyType = propertyInfo.PropertyType;
               var key = propertyInfo.Name.ToCamel();
               var value = propertyInfo.GetValue(obj);
               if (value is not null)
               {
                  if (isBaseType(propertyType))
                  {
                     setting.SetItem(key, new Item(key, toString(value, propertyType, encloseStrings)));
                  }
                  else if (value is Array array)
                  {
                     var elementType = propertyType.GetElementType();
                     if (elementType is not null)
                     {
                        if (isBaseType(elementType))
                        {
                           List<string> list = [];
                           for (var i = 0; i < array.Length; i++)
                           {
                              list.Add(toString(array.GetValue(i), elementType, encloseStrings));
                           }

                           var item = new Item(key, list.ToString(", "))
                           {
                              IsArray = true
                           };
                           setting.SetItem(key, item);
                        }
                        else
                        {
                           var arraySetting = new Setting(key);
                           for (var i = 0; i < array.Length; i++)
                           {
                              var generatedKey = Parser.GenerateKey();
                              var _elementSetting = Serialize(elementType, array.GetValue(i), false, predicate, generatedKey);
                              if (_elementSetting is (true, var elementSetting))
                              {
                                 arraySetting.SetItem(generatedKey, elementSetting);
                              }
                              else
                              {
                                 return _elementSetting.Exception;
                              }
                           }

                           setting.SetItem(key, arraySetting);
                        }
                     }
                  }
                  else
                  {
                     var _propertySetting = Serialize(propertyType, value, encloseStrings, predicate, key);
                     if (_propertySetting is (true, var propertySetting))
                     {
                        setting.SetItem(key, propertySetting);
                     }
                     else
                     {
                        return _propertySetting.Exception;
                     }
                  }
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

   public static Result<Setting> Serialize(Type type, object? obj, bool encloseStrings, string name = ROOT_NAME) =>
      Serialize(type, obj, encloseStrings, _ => true, name);

   protected object? getObject(Type type)
   {
      if (!type.IsArray)
      {
         return Activator.CreateInstance(type);
      }

      var elementType = type.GetElementType();
      Setting[] array = [.. Settings().Select(i => i.setting)];

      return makeArray(elementType, array).Required($"Couldn't make array of element type {elementType?.FullName ?? ""}");
   }

   public Result<object> Deserialize(Type type, Func<PropertyInfo, bool> predicate)
   {
      try
      {
         var obj = getObject(type);
         if (obj is null)
         {
            return fail("Object is null");
         }
         else
         {
            return fill(ref obj, type, predicate).Map(_ => obj);
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Result<object> Deserialize(Type type) => Deserialize(type, _ => true);

   public Result<T> Deserialize<T>(Func<PropertyInfo, bool> predicate) where T : class, new()
   {
      return
         from obj in tryTo(() => Deserialize(typeof(T), predicate))
         from cast in obj.Result().Cast<T>()
         select cast;
   }

   public Result<T> Deserialize<T>() where T : class, new() => Deserialize<T>(_ => true);

   protected Result<Unit> fill(ref object obj, Type type, Func<PropertyInfo, bool> predicate)
   {
      try
      {
         var allPropertyInfo = getPropertyInfo(type, predicate);
         foreach (var (key, value) in Items())
         {
            var name = key.ToPascal();
            var _propertyInfo = allPropertyInfo.FirstOrNone(p => p.Name.Same(name));
            if (_propertyInfo is (true, var propertyInfo))
            {
               var propertyType = propertyInfo.PropertyType;
               var _object = getConversion(propertyType, value);
               if (_object is (true, var @object))
               {
                  propertyInfo.SetValue(obj, @object);
               }
            }
         }

         foreach (var (key, setting) in Settings())
         {
            var name = key.ToPascal();
            var _propertyInfo = allPropertyInfo.FirstOrNone(p => p.Name.Same(name));
            if (_propertyInfo is (true, var propertyInfo))
            {
               var propertyType = propertyInfo.PropertyType;
               var _object = setting.Deserialize(propertyType);
               if (_object is (true, var @object))
               {
                  propertyInfo.SetValue(obj, @object);
               }
            }
         }

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Result<Unit> Fill(ref object? obj, Func<PropertyInfo, bool> predicate)
   {
      if (obj is null)
      {
         return fail("Object may not be null");
      }
      else
      {
         var type = obj.GetType();
         return fill(ref obj, type, predicate);
      }
   }

   public Result<Unit> Fill(ref object? obj) => Fill(ref obj, _ => true);

   public void Clear() => items.Clear();

   public void Fill(StringHash stringHash)
   {
      foreach (var (key, value) in stringHash)
      {
         this[key] = value;
      }
   }

   public void Fill(IEnumerable<string> enumerable)
   {
      var index = 0;
      foreach (var item in enumerable)
      {
         this[$"${index++}"] = item;
      }
   }

   public string[] Array
   {
      set
      {
         foreach (var (index, item) in value.Indexed())
         {
            this[$"${index}"] = item;
         }

         IsArray = true;
         IsHash = false;
      }
   }

   public StringHash Hash
   {
      set
      {
         foreach (var (key, item) in value)
         {
            this[key] = item;
         }

         IsHash = true;
         IsArray = false;
      }
   }

   public override Setting Clone()
   {
      var clone = new Setting(Key) { IsArray = IsArray, IsHash = IsHash };
      foreach (var (key, item) in items)
      {
         clone.SetItem(key, item.Clone());
      }

      return clone;
   }

   public override Setting Clone(string key)
   {
      var clone = new Setting(key) { IsArray = IsArray, IsHash = IsHash };
      foreach (var (itemKey, item) in items)
      {
         clone.SetItem(itemKey, item.Clone());
      }

      return clone;
   }

   public IEnumerable<ConfigurationItem> SelectItems(string path)
   {
      MaybeQueue<string> pathPartQueue = [.. path.Unjoin("'.'; f")];
      ConfigurationItem[] configurationItems = [.. items.Values];
      while (pathPartQueue.Dequeue() is (true, var pathPart))
      {
         configurationItems = [.. selectItems(configurationItems, pathPart)];
      }

      return configurationItems;
   }

   protected static IEnumerable<ConfigurationItem> selectItems(IEnumerable<ConfigurationItem> items, string pathPart)
   {
      if (getNameCount() is (true, var (name, count)))
      {
         return getCount(name, count);
      }
      else if (getSkipAndTake() is (true, var (name2, skip, take)))
      {
         return getSkipTake(name2, skip, take);
      }
      else if (pathPart.IsEmpty())
      {
         return getAll();
      }
      else
      {
         return getPart();
      }

      Func<ConfigurationItem, bool> getPredicate(string text)
      {
         if (getPattern(text) is (true, var (pattern, target)))
         {
            return target switch
            {
               'k' => i => i.Key.IsMatch(pattern),
               't' => i => i.Text.IsMatch(pattern),
               _ => _ => false
            };
         }
         else
         {
            return i => i.Key.Same(text);
         }
      }

      IEnumerable<ConfigurationItem> getPart()
      {
         var predicate = getPredicate(pathPart);

         foreach (var item in items.Where(predicate))
         {
            switch (item)
            {
               case Setting setting:
                  foreach (var (_, subItem) in setting.ConfigurationItems())
                  {
                     yield return subItem;
                  }

                  break;
               case Item configurationItem:
                  yield return configurationItem;

                  break;
            }
         }
      }

      IEnumerable<ConfigurationItem> getAll()
      {
         foreach (var item in items)
         {
            switch (item)
            {
               case Setting setting:
                  foreach (var (_, subItem) in setting.ConfigurationItems())
                  {
                     yield return subItem;
                  }

                  break;
               case Item configurationItem:
                  yield return configurationItem;

                  break;
            }
         }
      }

      Maybe<(string name, int count)> getNameCount()
      {
         return
            from matchResult in pathPart.Matches("^ /(-['[']+) '[' /(/d+) ']' $; f")
            let tuple = (name: matchResult.FirstGroup, count: matchResult.SecondGroup)
            from countAsInt in tuple.count.Maybe().Int32()
            select (tuple.name, count: countAsInt);
      }

      IEnumerable<ConfigurationItem> getCount(string name, int count)
      {
         var predicate = getPredicate(name);

         foreach (var item in items.Where(predicate).Take(count))
         {
            switch (item)
            {
               case Setting setting:
                  foreach (var (_, subItem) in setting.ConfigurationItems())
                  {
                     yield return subItem;
                  }

                  break;
               case Item configurationItem:
                  yield return configurationItem;

                  break;
            }
         }
      }

      Maybe<(string name, int skip, int take)> getSkipAndTake()
      {
         return
            from matchResult in pathPart.Matches("^ /(-['(']+) '(' /(/d+) ':' /(/d+) ')' $; f")
            let tuple = (name: matchResult.FirstGroup, skip: matchResult.SecondGroup, take: matchResult.ThirdGroup)
            from skipAsInt in tuple.skip.Maybe().Int32()
            from takeAsInt in tuple.take.Maybe().Int32()
            select (tuple.name, skipAsInt, takeAsInt);
      }

      IEnumerable<ConfigurationItem> getSkipTake(string name, int skip, int take)
      {
         var predicate = getPredicate(name);

         foreach (var item in items.Where(predicate).Skip(skip).Take(take))
         {
            switch (item)
            {
               case Setting setting:
                  foreach (var (_, subItem) in setting.ConfigurationItems())
                  {
                     yield return subItem;
                  }

                  break;
               case Item configurationItem:
                  yield return configurationItem;

                  break;
            }
         }
      }

      Maybe<(Pattern pattern, char target)> getPattern(string text)
      {
         if (text.Matches("^ /('k' | 't') '//' /(.+) '//'; f").Map(r => (r.FirstGroup, r.SecondGroup)) is (true, var (target, patternSource)))
         {
            return (patternSource, target[0]);
         }
         else
         {
            return nil;
         }
      }
   }

   public IEnumerable<Setting> SelectOnlySettings(string path) => SelectItems(path).OfType<Setting>();

   public IEnumerable<Item> SelectOnlyItems(string path) => SelectItems(path).OfType<Item>();

   public void Merge(Setting sourceSetting)
   {
      foreach (var (key, setting) in sourceSetting.Settings())
      {
         Set(key).Setting = setting;
      }

      foreach (var (key, item) in sourceSetting.Items())
      {
         this[key] = item;
      }
   }
}