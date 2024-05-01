using System.Collections.Generic;
using Core.Monads;

namespace Core.Applications.ConsoleProcessing;

public class CommandValue(string commandName, IEnumerable<ParameterValue> parameterValues)
{
   protected ParameterValues parameterValues = new(parameterValues);

   public string CommandName => commandName;

  public Maybe<ParameterValue> this[string parameterName] => parameterValues[parameterName];
}