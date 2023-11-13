using Core.Data.Configurations;
using Core.Data.ConnectionStrings;
using Core.Data.DataSources;
using Core.Monads;

namespace Core.Data.Setups;

public class TextSetup : ISetup
{
   public static Result<TextSetup> FromDataSettings(DataSettings dataSettings, string adapterName, string fileName)
   {
      return
         from adaptersSetting in dataSettings.AdaptersSetting.Result("Adapters setting not created")
         from adapterSetting in adaptersSetting.Result.Setting(adapterName)
         from connectionName in adapterSetting.Result.String("connection")
         from connectionsSetting in dataSettings.ConnectionsSetting.Result("Connections setting not created")
         from connectionSetting in connectionsSetting.Result.Setting(connectionName)
         let commandName = adaptersSetting.Maybe.String("command") | adapterName
         from commandsSetting in dataSettings.CommandsSetting.Result("Commands setting not created")
         from commandSetting in commandsSetting.Result.Setting(commandName)
         let connection = new Connection(connectionSetting) { ["file"] = fileName }
         from connectionString in SqlConnectionString.FromConnection(connection)
         from command in Command.FromSetting(commandsSetting)
         from parameters in Data.Parameters.Parameters.FromSetting(adapterSetting.Maybe.Setting("parameters"))
         from fields in Data.Fields.Fields.FromSetting(adapterSetting.Maybe.Setting("fields"))
         select new TextSetup(connectionString, command.Text, fields, parameters, command.CommandTimeout);
   }

   protected TextSetup(IConnectionString connectionString, string commandText, Fields.Fields fields, Parameters.Parameters parameters,
      TimeSpan commandTimeout)
   {
      ConnectionString = connectionString;
      CommandText = commandText;
      Fields = fields;
      Parameters = parameters;
      CommandTimeout = commandTimeout;
   }

   public DataSource DataSource => new TextDataSource(ConnectionString.ConnectionString, CommandTimeout);

   public IConnectionString ConnectionString { get; }

   public string CommandText { get; }

   public Fields.Fields Fields { get; }

   public Parameters.Parameters Parameters { get; }

   public TimeSpan CommandTimeout { get; }
}