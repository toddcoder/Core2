using Core.Computers;

namespace Core.Applications.ConsoleProcessing;

public class FolderParameterValue(string parameterName, FolderName value) : ParameterValue(parameterName)
{
   public FolderName Value => value;
}