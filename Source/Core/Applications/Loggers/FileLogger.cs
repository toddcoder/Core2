using Core.Computers;
using System;

namespace Core.Applications.Loggers;

public class FileLogger(FileName file, int indentation = 0) : Logger(indentation)
{
   public static implicit operator FileLogger(FileName file) => new(file);

   public static implicit operator FileLogger(string file) => new(file);

   protected FileAppender appender = new(file);

   public override void WriteRaw(char prefix, string message)
   {
      now = DateTime.Now;
      setMinDateTime(now);
      setMaxDateTime(now);

      appender.WriteLine($"{now:O} |{prefix}| {indentation}{message}");
   }

   public void Flush() => appender.Flush();

   public override void Dispose()
   {
      appender.Dispose();
      base.Dispose();
      GC.SuppressFinalize(this);
   }
}