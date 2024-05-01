using System.Collections.Generic;
using Core.Collections;
using Core.Monads;

namespace Core.Applications.ConsoleProcessing;

public class ParameterValues(IEnumerable<ParameterValue> enumerable)
{
   protected StringHash<ParameterValue> parameterValues = enumerable.ToStringHash(p => p.ParameterName);

   public Maybe<ParameterValue> this[string parameterName]
   {
      get => parameterValues.Maybe[parameterName];
      set => parameterValues.Maybe[parameterName] = value;
   }
}