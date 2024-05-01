namespace Core.Applications.ConsoleProcessing;

public class BoolParameterValue(string parameterName, bool value) : ParameterValue(parameterName)
{
   public bool Value => value;
}