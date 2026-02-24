using Core.Enumerables;
using Core.Matching;
using Core.Strings;
using static Core.Strings.StringFunctions;

namespace Core.Markdown;

public static class ReplacementFunctions
{
   private const string REGEX_USE_STYLE = "'[{' /(-['}']+) '}'/(-[']']+)']'; f";
   private const string REGEX_REPLACEMENT_SPECIFIERS = "/'::' /(-[':']+) /'::'; f";
   private const string REGEX_MULTIPLE_REPLACEMENT_SPECIFIERS = "/'(:' /(-[':']+) /':)'; f";

   public static ReplacementResult forLine(string line, IMarkdownFrameOptions options)
   {
      List<string> styles = [];

      if (line.Matches(REGEX_USE_STYLE) is (true, var styleResult))
      {
         var linePortion = styleResult.FirstGroup;
         var style = styleResult.SecondGroup;
         if (style.Matches("/(-['(']+ '(' -[')']+ ')') /s*; f") is (true, var styleLiteralResult))
         {
            var classId = $"class-{shortUniqueId()}";
            var specifiers = styleLiteralResult.Matches.Select(m => m.FirstGroup).ToString(" ");
            styles.Add($".{classId}[{specifiers}]");
            styleResult.ZerothGroup = $"<span class=\"{classId}\">{linePortion}</span>";
            line = styleResult.Text;
         }
         else
         {
            var replacement = $"<span class=\"{style}\">{linePortion}</span>";
            styleResult.ZerothGroup = replacement;
            line = styleResult.Text;
         }
      }

      if (line.Matches(REGEX_REPLACEMENT_SPECIFIERS) is (true, var specifierResult))
      {
         switch (specifierResult)
         {
            case { FirstGroup: "::", ThirdGroup: "::" }:
            {
               foreach (var match in specifierResult)
               {
                  var key = match.SecondGroup;
                  match.ZerothGroup = key switch
                  {
                     "br" => "<br/>",
                     _ => options.ScalarReplacements.Maybe[key] | ""
                  };
               }

               line = specifierResult.Text;
               break;
            }
         }
      }

      if (line.Matches(REGEX_MULTIPLE_REPLACEMENT_SPECIFIERS) is (true, var multipleResult))
      {
         switch (multipleResult)
         {
            case { FirstGroup: "(:", ThirdGroup: ":)" }:
            {
               var key = multipleResult.SecondGroup;
               var enumerable = options.MultipleReplacements.Maybe[key] | [];
               var index = multipleResult.Index;
               var length = multipleResult.Length;
               List<string> lines = [];
               foreach (var value in enumerable)
               {
                  Slicer slicer = line;
                  slicer[index, length] = value;
                  var singleResult = forLine(slicer.ToString(), options);
                  switch (singleResult)
                  {
                     case ReplacementResult.LineAndStyles lineAndStyles:
                     {
                        lines.Add(lineAndStyles.Line);
                        styles.AddRange(lineAndStyles.Styles);
                        break;
                     }
                     case ReplacementResult.LineOnly lineOnly:
                     {
                        lines.Add(lineOnly.Line);
                        break;
                     }
                     case ReplacementResult.LinesAndStyles linesAndStyles:
                     {
                        lines.AddRange(linesAndStyles.Lines);
                        styles.AddRange(linesAndStyles.Styles);
                        break;
                     }
                     case ReplacementResult.LinesOnly linesOnly:
                     {
                        lines.AddRange(linesOnly.Lines);
                        break;
                     }
                  }
               }

               return styles.Count == 0 ? new ReplacementResult.LinesOnly(lines) : new ReplacementResult.LinesAndStyles(lines, styles);
            }
         }
      }

      return styles.Count == 0 ? new ReplacementResult.LineOnly(line) : new ReplacementResult.LineAndStyles(line, styles);
   }
}