using Core.Computers;
using Core.Configurations;
using Core.Dates.DateIncrements;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.Data;

public class Command
{
   public static Result<Command> FromSetting(Setting commandSetting)
   {
      var name = commandSetting.Key;
      return
         from values in getValues(commandSetting)
         select new Command { Name = name, Text = values.text, CommandTimeout = values.timeout };
   }

   protected static Result<(string text, TimeSpan timeout)> getValues(Setting commandSetting)
   {
      try
      {
         string command;
         LazyMaybe<string> _text = nil;
         LazyMaybe<string> _fileName = nil;
         if (_text.ValueOf(commandSetting.Maybe.String("text")))
         {
            command = _text;
         }
         else if (_fileName.ValueOf(commandSetting.Maybe.String("file")) is (true, var fileName))
         {
            FileName file = fileName;
            command = file.Text;
         }
         else
         {
            return fail("Require 'text' or 'file' values");
         }

         var timeout = commandSetting.Maybe.String("timeout").Map(t => t.Maybe().TimeSpan()) | (() => 30.Seconds());

         return (command, timeout);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Command(Setting commandSetting)
   {
      Name = commandSetting.Key;
      LazyMaybe<string> _text = nil;
      LazyMaybe<string> _fileName = nil;
      if (_text.ValueOf(commandSetting.Maybe.String("text")) is (true, var text))
      {
         Text = text;
      }
      else if (_fileName.ValueOf(commandSetting.Maybe.String("file")) is (true, var fileName))
      {
         FileName file = fileName;
         Text = file.Text;
      }
      else
      {
         throw fail("Require 'text' or 'file' values");
      }

      CommandTimeout = commandSetting.Maybe.String("timeout").Map(t => t.Maybe().TimeSpan()) | (() => 30.Seconds());
   }

   internal Command()
   {
      Name = string.Empty;
      CommandTimeout = 30.Seconds();
      Text = string.Empty;
   }

   public string Name { get; set; }

   public TimeSpan CommandTimeout { get; init; }

   public string Text { get; init; }
}