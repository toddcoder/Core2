﻿using System.Collections;
using System.Data.SqlClient;
using Core.Assertions;
using Core.Collections;
using Core.Data.Configurations;
using Core.Data.Setups;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using Microsoft.Data.SqlClient;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Data;

public class Adapters<T> : IEnumerable<Adapter<T>> where T : class
{
   protected static StringHash<Func<DataSettings, string, ISetup>> setups;

   static Adapters()
   {
      setups = new StringHash<Func<DataSettings, string, ISetup>>
      {
         ["sql"] = (dataGroups, adapterName) =>
         {
            var sqlSetup = SqlSetup.FromDataGroups(dataGroups, adapterName).ForceValue();
            sqlSetup.Handler = Handler;

            return sqlSetup;
         }
      };
   }

   public static Maybe<SqlInfoMessageEventHandler> Handler { get; set; } = nil;

   public static void RegisterSetup(string name, Func<DataSettings, string, ISetup> func) => setups[name] = func;

   public static Result<Func<DataSettings, string, ISetup>> Setup(string setupType) => setups.Require(setupType);

   protected DataSettings dataSettings;
   protected StringHash<Adapter<T>> adapters;
   protected StringSet validAdapters;
   protected Predicate<string> isValidAdapterName;

   public Adapters(DataSettings dataSettings, params string[] validAdapterNames)
   {
      this.dataSettings = dataSettings;
      adapters = [];
      validAdapters = [];
      if (validAdapterNames.Length == 0)
      {
         isValidAdapterName = _ => true;
      }
      else
      {
         isValidAdapterName = adapterName => adapterName.IsNotEmpty() && validAdapters.Contains(adapterName);
         validAdapters.AddRange(validAdapterNames);
      }
   }

   protected Adapters(DataSettings dataSettings, StringHash<Adapter<T>> adapters, StringSet validAdapters, Predicate<string> isValidAdapterName)
   {
      this.dataSettings = dataSettings;
      this.adapters = new StringHash<Adapter<T>>(adapters);
      this.validAdapters = [.. validAdapters];
      this.isValidAdapterName = isValidAdapterName;
   }

   protected Result<string> validAdapterName(string adapterName)
   {
      return isValidAdapterName(adapterName).Result(() => adapterName, $"Adapter name {adapterName} is invalid");
   }

   protected Result<string> adapterExists(string adapterName)
   {
      return
         from adaptersSetting in dataSettings.AdaptersSetting.Result("Adapters setting not created")
         from adapterSetting in adaptersSetting.Result.String(adapterName)
         select adapterName;
   }

   public Result<Adapter<T>> Adapter(string adapterName, T entity, string setupType = "sql")
   {
      if (adapters.Maybe[adapterName] is (true, var adapter))
      {
         adapter.Entity = entity;
         return adapter;
      }

      return
         from name in validAdapterName(adapterName)
         from childName in adapterExists(name)
         from setup in Setup(setupType)
         from adapter1 in getAdapter(entity, childName, setup)
         select adapter1;
   }

   protected Result<Adapter<T>> getAdapter(T entity, string child, Func<DataSettings, string, ISetup> setup) => tryTo(() =>
   {
      var adapter = adapters.Find(child, an => new Adapter<T>(entity, setup(dataSettings, an)), true);
      adapter.Entity = entity;

      return adapter;
   });

   protected Result<Adapter<T>> getAdapter(Func<T> alwaysUse, string child, Func<DataSettings, string, ISetup> setup)
   {
      return tryTo(() => adapters.Find(child, an => new Adapter<T>(alwaysUse(), setup(dataSettings, an)), true));
   }

   public Result<TResult> Execute<TResult>(string adapterName, T entity, Func<T, TResult> map, string setupType = "sql") where TResult : notnull
   {
      if (adapters.Maybe[adapterName] is (true, var adapter))
      {
         adapter.Entity = entity;
         return adapter.TryTo.Execute().Map(map);
      }

      return
         from name in validAdapterName(adapterName)
         from childName in adapterExists(name)
         from adapter1 in Adapter(childName, entity, setupType)
         from obj in adapter1.TryTo.Execute()
         from result in map(obj).Success()
         select result;
   }

   public Result<T> Execute(string adapterName, T entity, string setupType = "sql")
   {
      if (adapters.Maybe[adapterName] is (true, var adapter))
      {
         adapter.Entity = entity;
         return adapter.TryTo.Execute();
      }

      return
         from name in validAdapterName(adapterName)
         from childName in adapterExists(name)
         from adapter1 in Adapter(childName, entity, setupType)
         from obj in adapter1.TryTo.Execute()
         select obj;
   }

   public Result<Adapter<T>> Adapter(string adapterName, Func<T> entityFunc, string setupType = "sql")
   {
      if (adapters.Maybe[adapterName] is (true, var adapter))
      {
         adapter.Entity = entityFunc();
         return adapter;
      }

      return
         from name in validAdapterName(adapterName)
         from childName in adapterExists(name)
         from setup in Setup(setupType)
         from adapter1 in getAdapter(entityFunc(), childName, setup)
         select adapter1;
   }

   public Result<T> Execute(string adapterName, Func<T> entityFunc, string setupType = "sql")
   {
      if (adapters.Maybe[adapterName] is (true, var adapter))
      {
         adapter.Entity = entityFunc();
         return adapter.TryTo.Execute();
      }

      return
         from name in validAdapterName(adapterName)
         from childName in adapterExists(name)
         from adapter1 in Adapter(childName, entityFunc(), setupType)
         from obj in adapter1.TryTo.Execute()
         select obj;
   }

   public IEnumerator<Adapter<T>> GetEnumerator() => adapters.Values.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public IEnumerable<Adapter<T>> AllAdapters(Func<string, T> func, string setupType = "sql")
   {
      return validAdapters.Select(name => Adapter(name, func(name), setupType)).SuccessfulValue();
   }

   public void Add(string adapterName)
   {
      adapterName.Must().Not.BeNullOrEmpty().OrThrow();

      validAdapters.Add(adapterName);
   }

   public Result<IBulkCopyTarget> BulkCopy(string sourceAdapterName, string targetAdapterName, Func<T> entityFunc,
      string sourceSetupType = "sql")
   {
      return
         from sourceAdapter in Adapter(sourceAdapterName, entityFunc, sourceSetupType)
         from targetAdapter in Adapter(targetAdapterName, entityFunc)
         from target in targetAdapter.TryTo.BulkCopy(sourceAdapter)
         select target;
   }

   public Result<IBulkCopyTarget> BulkCopy(string sourceAdapterName, string targetAdapterName, T entity,
      string sourceSetupType = "sql")
   {
      return
         from sourceAdapter in Adapter(sourceAdapterName, entity, sourceSetupType)
         from targetAdapter in Adapter(targetAdapterName, entity)
         from target in targetAdapter.TryTo.BulkCopy(sourceAdapter)
         select target;
   }

   public IEnumerable<Result<T>> ExecuteAll(T entity, string setupType = "sql")
   {
      return validAdapters.Select(key => Execute(key, entity, setupType));
   }

   public IEnumerable<Result<T>> ExecuteAll(Func<string, T> map, string setupType = "sql")
   {
      return validAdapters
         .Select(name => new { Name = name, Entity = tryTo(() => map(name)) })
         .Select(result => result.Entity.Map(entity => Execute(result.Name, entity, setupType)));
   }

   public string[] Names => adapters.KeyArray();

   public Adapters<T> Clone() => new(dataSettings, adapters, validAdapters, isValidAdapterName);
}

public class Adapters : Adapters<DataContainer>
{
   public Adapters(DataSettings dataSettings, params string[] validAdapterNames) : base(dataSettings, validAdapterNames)
   {
   }
}