using Core.Monads;
using Microsoft.Data.SqlClient;

namespace Core.Data.Setups;

public interface ISetupWithInfo
{
   Maybe<SqlInfoMessageEventHandler> Handler { get; }
}