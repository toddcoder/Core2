﻿using Core.Collections;
using Core.Configurations;
using Core.Dates.DateIncrements;
using Core.Objects;

namespace Core.Data;

public class Connection : IHash<string, string>
{
   protected StringHash data;

   public Connection(Setting connectionSetting)
   {
      data = [];
      Name = connectionSetting.Key;
      Type = connectionSetting.Maybe.String("type") | "sql";
      Timeout = connectionSetting.Maybe.String("timeout").Map(t => t.Maybe().TimeSpan()) | (() => 30.Seconds());
      ReadOnly = connectionSetting.Maybe.Boolean("read-only") | false;
      foreach (var (key, value) in connectionSetting.Items())
      {
         data[key] = value;
      }
   }

   public string Name { get; set; }

   public string this[string name]
   {
      get => data[name];
      set => data[name] = value;
   }

   public bool ContainsKey(string key) => data.ContainsKey(key);

   public Hash<string, string> GetHash() => data.AsHash;

   public HashInterfaceMaybe<string, string> Items => new(this);

   public string Type { get; set; }

   public TimeSpan Timeout { get; set; }

   public bool ReadOnly { get; set; }
}