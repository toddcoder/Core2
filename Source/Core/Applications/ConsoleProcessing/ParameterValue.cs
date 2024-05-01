using Core.Computers;
using Core.Objects;
using Core.Strings;

namespace Core.Applications.ConsoleProcessing;

public abstract class ParameterValue(string parameterName)
{
   public static ParameterValue FromValue(string name, string value)
   {
      if (value.IsEmpty())
      {
         return new MissingParameterValue(name);
      }
      else if (value.Maybe().Single() is (true, var singleValue))
      {
         return new FloatParameterValue(name, singleValue);
      }
      else if (value.Maybe().Int32() is (true, var intValue))
      {
         return new IntParameterValue(name, intValue);
      }
      else if (value.Maybe().Boolean() is (true, var boolValue))
      {
         return new BoolParameterValue(name, boolValue);
      }
      else if (value.Maybe().DateTime() is (true, var dateValue))
      {
         return new DateTimeParameterValue(name, dateValue);
      }
      else if (FileName.ValidateFileName(value) is (true, var file) && file)
      {
         return new FileParameterValue(name, file);
      }
      else if (FolderName.ValidateFolderName(value) is (true, var folder) && folder)
      {
         return new FolderParameterValue(name, folder);
      }
      else
      {
         return new StringParameterValue(name, value);
      }
   }

   public string ParameterName => parameterName;
}