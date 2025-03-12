using Core.Computers;
using System;
using Core.Strings;

namespace Core.Applications.Loggers;

public class FileLogger(FileName file, int indentation = 0, int maxLocationLength = 20) : Logger(indentation)
{
   protected int maxLocationLength = maxLocationLength;

   public static implicit operator FileLogger(FileName file) => new(file);

   public static implicit operator FileLogger(string file) => new(file);

   protected FileAppender appender = new(file);

   public string Location { get; set; } = "main";

   public int MaxLocationLength
   {
      get => maxLocationLength;
      set
      {
         if (value > 0)
         {
            maxLocationLength = value;
         }
         else
         {
            maxLocationLength = 20;
         }
      }
   }

   public override void WriteRaw(char prefix, string message)
   {
      now = DateTime.Now;
      setMinDateTime(now);
      setMaxDateTime(now);

      var location = Location.EnsureLength(maxLocationLength);

      appender.WriteLine($"{now:O} | {location} | {prefix} | {indentation}{message}");
   }

   public override void WriteRule() => appender.WriteLine(RULE);

   public void Flush() => appender.Flush();

   public override void Dispose()
   {
      appender.Dispose();
      base.Dispose();
      GC.SuppressFinalize(this);
   }
}