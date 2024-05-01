namespace Core.Applications.ConsoleProcessing;

public class FloatParameterValue(string parameterName, float value) : ParameterValue(parameterName)
{
   public float Value => value;
}