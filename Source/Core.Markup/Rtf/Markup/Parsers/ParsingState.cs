using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf.Markup.Parsers;

public class ParsingState(Document document)
{
   protected StringHash<Definition> definitions = [];
   protected Context context = new Context.DocumentContext();
   protected List<Line> lines = [];

   public Document Document => document;

   public Context Context
   {
      get => context;
      set => context = value;
   }

   public StringHash<Definition> Definitions => definitions;

   public void AddLine(Line line) => lines.Add(line);

   public Result<Unit> Render()
   {
      try
      {
         foreach (var line in lines)
         {
            var _result = line.Render(this);
            if (!_result)
            {
               return _result;
            }
         }

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}