using Core.Applications.Loggers;
using Core.Computers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Monads.MonadFunctions;

namespace Core.Tests;

[TestClass]
public class LoggerTests
{
   [TestMethod]
   public void FileLoggerTest()
   {
      FileName file = @"C:\Temp\log.log";
      file.Delete();
      FileLogger logger;
      using (logger = file)
      {
         logger.WriteMessage("This is a message");
         logger.WriteFailure("This is a failure");
         logger.WriteSuccess("This is a success");
         logger.WriteException(fail("This is an exception"));

         logger.Location = "SetUpDocumentCompleted";
         logger.WriteSuccess("Done");
         logger.MaxLocationLength = 22;
         logger.WriteMessage("Location length set to 22");
         logger.Flush();
      }

      Console.Write(file.Text);
   }
}