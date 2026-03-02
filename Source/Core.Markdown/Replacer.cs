using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using static Core.Strings.StringFunctions;

namespace Core.Markdown;

public abstract record Replacer
{
   public sealed record LineSpecifier(string Line) : Replacer;

   public sealed record StyleSpecifier(string Style) : Replacer;

   public sealed record MultiBegin(string Key) : Replacer;

   public sealed record MultiEnd() : Replacer;

   public sealed record Inclusion(string Key) : Replacer;

   protected const string REGEX_USE_STYLE = "'[{' /(-['}']+) '}'/(-[']']+)']'; f";

   protected const string REGEX_REPLACEMENT_SPECIFIERS = "'::' /(-[':']+) '::'; f";

   protected static List<string> generatedStyles = [];

   public static string ModifyLine(string line, StringHash variables)
   {
      if (line.Matches(REGEX_USE_STYLE) is (true, var styleResult))
      {
         var linePortion = styleResult.FirstGroup;
         var style = styleResult.SecondGroup;
         if (style.Matches("/(-['(']+ '(' -[')']+ ')') /s*; f") is (true, var styleLiteralResult))
         {
            var classId = $"class-{shortUniqueId()}";
            var specifiers = styleLiteralResult.Matches.Select(m => m.FirstGroup).ToString(" ");
            generatedStyles.Add($".{classId}[{specifiers}]");
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

      if (line.Matches(REGEX_REPLACEMENT_SPECIFIERS) is (true, var replacementResult))
      {
         foreach (var match in replacementResult)
         {
            match.Text = variables.Maybe[match.FirstGroup] | "";
         }

         line = replacementResult.Text;
      }

      return line;
   }
}