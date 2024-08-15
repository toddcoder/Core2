﻿using System.Collections;
using System.Data;
using Core.Assertions;
using Core.Data.DataSources;
using Core.Data.Setups;
using Core.Monads;
using Core.Objects;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Data;

public class Adapter<T> : IEnumerable<T> where T : class
{
   public static Result<Adapter<T>> FromSetup(ISetup setup, T entity) => tryTo(() => new Adapter<T>(entity, setup));

   public static Result<Adapter<T>> FromSetupObject(T entity)
   {
      if (entity is ISetupObject setupObject)
      {
         return
            from setup in tryTo(() => setupObject.Setup())
            from adapter in FromSetup(setup, entity)
            select adapter;
      }
      else
      {
         return fail("Entity must support ISetupObject interface");
      }
   }

   protected T entity;
   protected Func<T> newFunc = null!;

   public Adapter(T entity, ISetup setup)
   {
      DataSource = setup.DataSource;
      if (setup is ISetupWithInfo setupWithInfo && setupWithInfo.Handler)
      {
         DataSource.SetMessageHandler(setupWithInfo.Handler);
      }

      Command = setup.CommandText;
      Parameters = new Parameters.Parameters(setup.Parameters);
      Fields = new Fields.Fields(setup.Fields);

      this.entity = entity.Must().Not.BeNull().Force<T>();
      setEntityType();
      setNewFunc();
   }

   internal Adapter(Adapter<T> other, string command)
   {
      DataSource = other.DataSource;
      Command = command;
      Parameters = other.Parameters;
      Fields = other.Fields;
      entity = other.entity;

      setEntityType();
      setNewFunc();
   }

   protected void setNewFunc()
   {
      var entityType = entity.GetType();
      if (entityType.GetConstructors().Any(c => c.GetParameters().Length == 0))
      {
         newFunc = () => (T)Activator.CreateInstance(entityType)!;
      }
      else
      {
         newFunc = () => entity;
      }
   }

   public T Entity
   {
      get => entity;
      set
      {
         entity = value.MustOfType().Not.BeNull().Force<T>();
         setEntityType();
      }
   }

   public Func<T> NewFunc
   {
      get => newFunc;
      set => newFunc = value;
   }

   public DataSource DataSource { get; set; }

   public string Command { get; set; }

   public Parameters.Parameters Parameters { get; set; }

   public Fields.Fields Fields { get; set; }

   public int RecordsAffected { get; set; }

   public string ConnectionString
   {
      get => DataSource.ConnectionString;
      set => DataSource.ConnectionString = value;
   }

   public bool HasRows { get; set; }

   protected void setEntityType()
   {
      Parameters.DeterminePropertyTypes(entity);
      Fields.DeterminePropertyTypes(entity);
   }

   public T Execute()
   {
      RecordsAffected = DataSource.Execute(entity, Command, Parameters, Fields);
      HasRows = DataSource.HasRows;

      return entity;
   }

   public Optional<T> ExecuteOptional()
   {
      try
      {
         RecordsAffected = DataSource.Execute(entity, Command, Parameters, Fields);
         HasRows = DataSource.HasRows;

         return HasRows ? entity : nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Maybe<T> ExecuteMaybe()
   {
      try
      {
         RecordsAffected = DataSource.Execute(entity, Command, Parameters, Fields);
         HasRows = DataSource.HasRows;

         return maybe<T>() & HasRows & (() => entity);
      }
      catch
      {
         return nil;
      }
   }

   public IDataReader ExecuteReader() => DataSource.ExecuteReader(entity, Command, Parameters);

   public Optional<int> ExecuteNonQuery()
   {
      try
      {
         return DataSource.ExecuteNonQuery(entity, Command, Parameters);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public IBulkCopyTarget BulkCopy<TSource>(Adapter<TSource> sourceAdapter) where TSource : class
   {
      if (DataSource is IBulkCopyTarget bulkCopy)
      {
         bulkCopy.TableName = Command;
         bulkCopy.Copy(sourceAdapter);

         return bulkCopy;
      }
      else
      {
         throw fail("This data source doesn't support a bulk copy");
      }
   }

   public IBulkCopyTarget BulkCopy(IDataReader reader, TimeSpan timeout)
   {
      if (DataSource is IBulkCopyTarget bulkCopy)
      {
         bulkCopy.TableName = Command;
         bulkCopy.Copy(reader, timeout);

         return bulkCopy;
      }
      else
      {
         throw fail("This data source doesn't support a bulk copy");
      }
   }

   public IDbConnection NativeConnection() => DataSource.GetConnection();

   public Reader<T> Reader() => new(DataSource, Entity, newFunc, Command, Parameters, Fields);

   public DataSet DataSet()
   {
      if (DataSource is SqlDataSource ds)
      {
         return ds.DataSet(entity, Command, Parameters);
      }
      else
      {
         throw fail("You may only use a SQLDataSource for this function.");
      }
   }

   public IEnumerator<T> GetEnumerator() => Reader().GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public AdapterTrying<T> TryTo => new(this);

   public Adapter<T> WithNewCommand(string newCommand) => new(this, newCommand);
}

public class Adapter : Adapter<DataContainer>
{
   public Adapter(ISetup setup) : base(new DataContainer(), setup)
   {
   }

   internal Adapter(Adapter other, string command) : base(other, command)
   {
   }
}