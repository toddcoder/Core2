using Core.Data.ConnectionStrings;
using Core.Data.DataSources;

namespace Core.Data.Setups;

public interface ISetup
{
   DataSource DataSource { get; }

   IConnectionString ConnectionString { get; }

   string CommandText { get; }

   Fields.Fields Fields { get; }

   Parameters.Parameters Parameters { get; }

   TimeSpan CommandTimeout { get; }
}