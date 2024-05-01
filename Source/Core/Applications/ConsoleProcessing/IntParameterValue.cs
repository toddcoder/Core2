namespace Core.Applications.ConsoleProcessing;

public class IntParameterValue(string parameterName, int value) : ParameterValue(parameterName)
{
   public int Value => value;
}