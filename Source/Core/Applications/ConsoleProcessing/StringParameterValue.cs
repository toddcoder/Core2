namespace Core.Applications.ConsoleProcessing;

public class StringParameterValue(string parameterName, string value) : ParameterValue(parameterName)
{
   public string Value => value;
}