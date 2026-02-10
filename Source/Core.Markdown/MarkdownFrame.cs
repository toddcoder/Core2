using Core.Computers;
using Core.Enumerables;
using Core.Markup.Html.Parser;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using Markdig;
using static Core.Monads.MonadFunctions;

namespace Core.Markdown;

public class MarkdownFrame(string styles, string markdown)
{
   public static Optional<MarkdownFrame> FromSource(string source)
   {
      try
      {
         List<string> styles = [];
         List<string> lines = [];
         var inStyle = true;
         Maybe<string> _continuing = nil;

         foreach (var line in source.Lines())
         {
            if (inStyle)
            {
               if (_continuing is (true, var continuing))
               {
                  if (line.IsMatch("']' $; f"))
                  {
                     styles.Add(continuing + line);
                     _continuing = nil;
                  }
                  else
                  {
                     _continuing = continuing + line;
                  }
               }
               else if (line.IsMatch("^ /s* -['[']+ '[' -[']']+ ']' $; f"))
               {
                  styles.Add(modifyStyleName(line));
               }
               else if (line.IsMatch("^ /s* -['[']+ '[' -[']']+ $; f"))
               {
                  _continuing = modifyStyleName(line);
               }
               else if (line.Matches("^ /s* '@' /(.+) $; f") is (true, var result))
               {
                  FileName file = result.FirstGroup;
                  importStyles(file);
               }
               else if (line.IsEmpty())
               {
               }
               else
               {
                  lines.Add(line);
                  inStyle = false;
               }
            }
            else
            {
               lines.Add(line);
            }
         }

         return new MarkdownFrame(styles.ToString("\r\n"), lines.ToString("\r\n"));

         void importStyles(FileName file)
         {
            foreach (var line in file.Lines)
            {
               if (_continuing is (true, var continuing))
               {
                  if (line.IsMatch("']' $; f"))
                  {
                     styles.Add(continuing + line);
                     _continuing = nil;
                  }
                  else
                  {
                     _continuing = continuing + line;
                  }
               }
               else if (line.IsMatch("^ /s* -['[']+ '[' -[']']+ ']' $; f"))
               {
                  styles.Add(modifyStyleName(line));
               }
               else if (line.IsMatch("^ /s* -['[']+ '[' -[']']+ $; f"))
               {
                  _continuing = modifyStyleName(line);
               }
               else if (line.IsEmpty())
               {
               }
            }
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected static string modifyStyleName(string line)
   {
      if (line.Matches("^ /(-['[']+) /('[' .+) $; f") is (true, var result))
      {
         var name = result.FirstGroup;
         var rest = result.SecondGroup;
         if (name.Matches("^ 'header' /(/d+)") is (true, var result2))
         {
            return $"h{result2.FirstGroup}{rest}".Trim();
         }

         var cssName = name switch
         {
            "table-header" => "th",
            "table-row" => "tr",
            "column" => "td",
            "ordered-list" => "ol",
            "list" => "ul",
            "para" => "p",
            _ => name
         };
         return $"{cssName}{rest}".Trim();
      }

      return line.Trim();
   }

   public Optional<string> ToHtml()
   {
      try
      {
         var stylesSource = $"style[{styles}]";
         var parser = new HtmlParser(stylesSource, true);
         var _html = parser.Parse();
         if (_html is (true, var html))
         {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);
            var rawHtml = document.ToHtml(pipeline);

            return html.Replace("<body />", $"<body>{rawHtml}</body>");
         }
         else
         {
            return _html.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}