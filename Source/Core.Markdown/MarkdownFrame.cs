using Core.Computers;
using Core.Enumerables;
using Core.Markup.Html.Parser;
using Core.Markup.Xml;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using Markdig;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Markdown;

public class MarkdownFrame(string styles, string markdown, bool tidy = true)
{
   public string Styles => styles;

   public string Markdown => markdown;

   public static Optional<MarkdownFrame> FromSource(IMarkdownFrameOptions options)
   {
      var tidy = options.Tidy;
      try
      {
         List<string> styles = [];
         List<string> lines = [];
         var inStyle = true;
         Maybe<string> _continuing = nil;
         Maybe<Inclusion> _inclusion = nil;

         var sourceLines = options.Source.Lines();
         if (sourceLines[0].Matches("^ /(['+-']) /('tidy') $; f") is (true, var result))
         {
            tidy = result.FirstGroup == "+";
            sourceLines = [.. sourceLines.Skip(1)];
         }

         foreach (var line in sourceLines)
         {
            if (line.Matches("'[:' /s* /(-[': ']+) /s* ':]'; f") is (true, var result2))
            {
               var key = result2.FirstGroup;
               var inclusion = new Inclusion(key, options.Included.Contains(key));
               _inclusion = inclusion;
               continue;
            }

            if (_inclusion is (true, { Include: false }))
            {
               continue;
            }

            if (inStyle)
            {
               importStyle(line);
            }
            else
            {
               addLineAndPossibleStyle(line, styles, lines, options);
            }
         }

         return new MarkdownFrame(styles.ToString("\r\n"), lines.ToString("\r\n"), tidy);

         void importStyle(string line)
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
               addLineAndPossibleStyle(line, styles, lines, options);
               inStyle = false;
            }
         }

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

   protected static void addLineAndPossibleStyle(string line, List<string> styleList, List<string> lines, IMarkdownFrameOptions options)
   {
      if (line.Matches("'[{' /(-['}']+) '}'/(-[']']+)']'; f") is (true, var result))
      {
         var linePortion = result.FirstGroup;
         var style = result.SecondGroup;
         if (style.Matches("/(-['(']+ '(' -[')']+ ')') /s*; f") is (true, var results))
         {
            var classId = $"class-{shortUniqueId()}";
            var specifiers = results.Matches.Select(m => m.FirstGroup).ToString(" ");
            styleList.Add($".{classId}[{specifiers}]");
            result.ZerothGroup = $"<span class=\"{classId}\">{linePortion}</span>";
            addLine(result.Text, lines, options);
         }
         else
         {
            var replacement = $"<span class=\"{style}\">{linePortion}</span>";
            result.ZerothGroup = replacement;
            addLine(result.Text, lines, options);
         }
      }
      else
      {
         addLine(line, lines, options);
      }
   }

   protected static void addLine(string line, List<string> lines, IMarkdownFrameOptions options)
   {
      if (line.Matches("/['(:'] ':' /(-[':']+) ':' /[':)']; f") is (true, var result))
      {
         switch (result)
         {
            case { FirstGroup: ":", ThirdGroup: ":" }:
            {
               foreach (var match in result)
               {
                  var key = match.SecondGroup;
                  match.ZerothGroup = options.ScalarReplacements.Maybe[key] | "";
               }

               lines.Add(result.Text);
               break;
            }
            case { FirstGroup: "(", ThirdGroup: ")" }:
            {
               var key = result.SecondGroup;
               var enumerable = options.MultipleReplacements.Maybe[key] | [];
               var index = result.Index;
               var length = result.Length;
               foreach (var value in enumerable)
               {
                  Slicer slicer = line;
                  slicer[index, length] = value;
                  lines.Add(slicer.ToString());
               }

               break;
            }
         }
      }
      else if (line == "!br")
      {
         lines.Add("<br/>");
      }
      else
      {
         lines.Add(line);
      }
   }

   protected static string modifyStyleName(string line)
   {
      if (line.Matches("^ /(-['[']+) /('[' .+) $; f") is (true, var result))
      {
         var name = result.FirstGroup;
         var rest = result.SecondGroup;
         if (name.Matches("^ 'header' /(/d+); f") is (true, var result2))
         {
            return $"h{result2.FirstGroup}{rest}".Trim();
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
         return $"{cssName}{rest}".Trim();
      }

      return line.Trim();
   }

   public Optional<string> ToHtml()
   {
      try
      {
         if (styles.IsEmpty())
         {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);
            var rawHtml = document.ToHtml(pipeline);

            var newHtml = $"<html><body>{rawHtml}</body></html>";
            return tidy ? newHtml.Tidy(true) : newHtml;
         }

         var stylesSource = $"style[{styles}]";
         var parser = new HtmlParser(stylesSource, tidy);
         var _html = parser.Parse();
         if (_html is (true, var html))
         {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);
            var rawHtml = document.ToHtml(pipeline);

            var newHtml = html.Replace("<body />", $"<body>{rawHtml}</body>");
            return tidy ? newHtml.Tidy(true) : newHtml;
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