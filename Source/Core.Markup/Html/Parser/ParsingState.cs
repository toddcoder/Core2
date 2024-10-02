using Core.Matching;
using System.Text;

namespace Core.Markup.Html.Parser;

public class ParsingState(string source)
{
   protected SourceLines sourceLines = new(source);
   protected StringBuilder styles = new();
   protected StringBuilder body = new();
}