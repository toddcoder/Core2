using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Markdown;

public class MarkdownFrameParser(string[] sourceLines)
{
   protected const string REGEX_STYLE = "^ /(-['[']+) '[' /(-[']']+) ']' $; f";
   protected const string REGEX_USE_STYLE = "'[{' /(-['}']+) '}'/(-[']']+)']'; f";
   protected const string REGEX_RAW_STYLE = "/(-['(']+ '(' -[')']+ ')') /s*; f";
   protected const string REGEX_INCLUSION = "^ /s* '[:' /(-[':']+) ':]' /s* $; f";
   protected const string REGEX_BEGIN = "^ /s* '(:' /(-[':']+) ':)' /s* $; f";
   protected const string REGEX_END = "^ /s* '(' [':']+ ')' /s* $; f";
   protected const string REGEX_VARIABLE = "^ '//' /([/w '.-']+) /s* ':' /s* /(.+) $";
   protected const string REGEX_HEADER = "^ 'header' /(/d+); f";

   protected List<string> generatedStyles = [];

   public IEnumerable<string> GeneratedStyles => generatedStyles;

   public Optional<Replacer[]> Parse(IMarkdownFrameOptions options)
   {
      List<Replacer> replacers = [];
      Maybe<string> _key = nil;
      foreach (var sourceLine in sourceLines)
      {
         var line = sourceLine.Trim();
         try
         {
            if (line.Matches(REGEX_STYLE) is (true, var styleResult))
            {
               var name = styleResult.FirstGroup;
               var rest = styleResult.SecondGroup;

               if (name.Matches(REGEX_HEADER) is (true, var result2))
               {
                  replacers.Add(new Replacer.StyleSpecifier($"h{result2.FirstGroup}[{rest}]".Trim()));
                  continue;
               }

               var cssName = name switch
               {
                  "table-header" => "th",
                  "table-row" => "tr",
                  "table-col" => "td",
                  "ordered" => "ol",
                  "unordered" => "ul",
                  "para" => "p",
                  "link" => "a",
                  _ => name
               };
               replacers.Add(new Replacer.StyleSpecifier($"{cssName}[{rest}]".Trim()));
            }
            else
            {
               if (line.Matches(REGEX_USE_STYLE) is (true, var useStyleResult))
               {
                  Slicer slicer = line;
                  foreach (var match in useStyleResult)
                  {
                     var linePortion = match.FirstGroup;
                     var style = match.SecondGroup;
                     if (style.Matches(REGEX_RAW_STYLE) is (true, var rawResult))
                     {
                        var classId = $"class-{shortUniqueId()}";
                        var specifiers = rawResult.Matches.Select(m => m.FirstGroup).ToString(" ");
                        generatedStyles.Add($".{classId}[{specifiers}]");
                        slicer[rawResult.Index, rawResult.Length] = $"<span class=\"{classId}\" {specifiers}>{linePortion}</span>";
                     }
                     else
                     {
                        slicer[match.Index, match.Length] = $"<span class=\"{style}\">{linePortion}</span>";
                     }
                  }

                  line = slicer.ToString();
               }

               if (line.Matches(REGEX_INCLUSION) is (true, var inclusionResult))
               {
                  var key = inclusionResult.FirstGroup;
                  replacers.Add(new Replacer.Inclusion(key));
               }
               else if (line.Matches(REGEX_BEGIN) is (true, var beginResult))
               {
                  var key = beginResult.FirstGroup;
                  replacers.Add(new Replacer.MultiBegin(key));
                  _key = key;
               }
               else if (line.Matches(REGEX_END))
               {
                  replacers.Add(new Replacer.MultiEnd());
                  _key = nil;
               }
               else if (line.Matches(REGEX_VARIABLE) is (true, var variableResult))
               {
                  options.Variables[variableResult.FirstGroup] = variableResult.SecondGroup;
               }
               else if (_key)
               {
                  replacers.Add(new Replacer.Template(line));
               }
               else
               {
                  replacers.Add(new Replacer.LineSpecifier(line));
               }
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      return (Replacer[])[.. replacers];
   }
}