using Core.Applications.Loggers;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Json;

public class JsonLogger : Logger
{
   protected JsonWriter jsonWriter;
   protected Memo<string, int> counts = new Memo<string, int>.Value(0);

   public JsonLogger()
   {
      jsonWriter = new JsonWriter();
      jsonWriter.BeginObject();
   }

   public override void WriteRaw(char prefix, string message)
   {
      base.WriteRaw(prefix, message);

      Maybe<string> _type = prefix switch
      {
         '.' => "m",
         '!' => "s",
         '?' => "f",
         '*' => "e",
         '~' => "l",
         _ => nil
      };

      if (_type is (true, var type))
      {
         var index = counts[type]++;
         var key = $"{type}{index}";
         jsonWriter.BeginObject(key);
         jsonWriter.Write("time", now);
         jsonWriter.Write("msg", message);
         jsonWriter.EndObject();
      }
   }
}