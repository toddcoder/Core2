using System.Collections;
using System.Data;
using Core.Data.DataSources;
using Core.Monads;
using Core.Monads.Lazy;
using static Core.Monads.MonadFunctions;

namespace Core.Data;

public class Reader<T> : IDisposable, IEnumerable<T> where T : notnull
{
   protected DataSource dataSource;
   protected Func<T> entityFunc;

   public Reader(DataSource dataSource, T entity, Func<T> entityFunc, string command, Parameters.Parameters parameters, Fields.Fields fields)
   {
      this.dataSource = dataSource;
      dataSource.BeginReading(entity, command, parameters, fields);
      Entity = entity;
      this.entityFunc = entityFunc;

      var outputParameters = new Parameters.Parameters();
      foreach (var dataParameter in dataSource.Command.Required("Command hasn't be set").Parameters.Cast<IDataParameter>()
                  .Where(p => p.Direction == ParameterDirection.Output))
      {
         outputParameters[dataParameter.ParameterName] = parameters[dataParameter.ParameterName];
      }
   }

   public object Entity { get; set; }

   public Maybe<T> Next() => dataSource.NextReading(entityFunc()).Map(obj => (T)obj);

   void IDisposable.Dispose()
   {
      dispose();
      GC.SuppressFinalize(this);
   }

   protected void dispose()
   {
      dataSource.EndReading();
   }

   ~Reader() => dispose();

   public IEnumerator<T> GetEnumerator()
   {
      try
      {
         LazyMaybe<T> _entity = nil;
         while (_entity.ValueOf(Next(), true) is (true, var entity))
         {
            yield return entity;
         }
      }
      finally
      {
         dispose();
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}