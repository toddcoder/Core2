using System;

namespace Core.Applications.ConsoleProcessing;

public class DateTimeParameterValue(string parameterName, DateTime value) : ParameterValue(parameterName)
{
   public DateTime Value => value;
}