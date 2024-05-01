using Core.Computers;

namespace Core.Applications.ConsoleProcessing;

public class FileParameterValue(string parameterName, FileName value) : ParameterValue(parameterName)
{
   public FileName Value => value;
}