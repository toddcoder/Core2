using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;
using Core.Monads;

namespace Core.Data.Setups;

public class OleDbSetup : ISetup
{
   protected static StringHash<Func<IConnectionString>> registeredTypes;

   static OleDbSetup() => registeredTypes = [];

   public static void RegisterType(string type, Func<IConnectionString> func) => registeredTypes[type] = func;

   public static Result<OleDbSetup> FromDataGroups(DataSettings dataSettings, string adapterName, Maybe<FileName> file)
   {
      var _result =
         from adaptersSetting in dataSettings.AdaptersSetting.Result("Adapters setting not created")
         from adapterSetting in adaptersSetting.Result.Setting(adapterName)
         let _parameters = new Parameters.Parameters(adapterSetting.Maybe.Setting("parameters"))
         let _fields = new Fields.Fields(adapterSetting.Maybe.Setting("fields"))
         from connectionName in adapterSetting.Result.String("connection")
         let commandName = adapterSetting.Maybe.String("command") | adapterName
         from connectionsSetting in dataSettings.ConnectionsSetting.Result("Connections setting not created")
         from connectionSetting in connectionsSetting.Result.Setting(connectionName)
         from commandsSetting in dataSettings.CommandsSetting.Result("Commands setting not created")
         from commandSetting in commandsSetting.Result.Setting(commandName)
         let _command = new Command(commandSetting)
         let _connection = new Connection(connectionSetting)
         let type = _connection.Type.ToLower()
         select (_parameters, _fields, _command, _connection);
      if (_result is (true, var (parameters, fields, command, connection)))
      {
         var type = connection.Type.ToLower();
         var _connectionString = type switch
         {
            "access" => new AccessConnectionString(),
            "excel" => new ExcelConnectionString(),
            "csv" => new CSVConnectionString(),
            _ => registeredTypes.Items[type].Map(f => f())
         };

         return _connectionString.Map(connectionString => new OleDbSetup(file)
         {
            ConnectionString = connectionString, CommandTimeout = command.CommandTimeout, CommandText = command.Text, Parameters = parameters,
            Fields = fields
         }).Result($"Didn't understand type {type}");
      }
      else
      {
         return _result.Exception;
      }
   }

   protected Maybe<FileName> file;

   public OleDbSetup(Maybe<FileName> file)
   {
      this.file = file;

      ConnectionString = null!;
      CommandText = string.Empty;
      Fields = [];
      Parameters = new Parameters.Parameters();
   }

   public DataSource DataSource
   {
      get
      {
         ConnectionString.Must().Not.BeNull().OrThrow();
         return new OleDbDataSource(ConnectionString.ConnectionString, file);
      }
   }

   public IConnectionString ConnectionString { get; init; }

   public string CommandText { get; init; }

   public Fields.Fields Fields { get; init; }

   public Parameters.Parameters Parameters { get; init; }

   public TimeSpan CommandTimeout { get; init; }
}