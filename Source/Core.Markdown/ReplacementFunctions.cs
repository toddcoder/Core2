using Core.Collections;
using Core.Matching;
using Core.Objects;
using Core.Strings;

namespace Core.Markdown;

public static class ReplacementFunctions
{
   public static string replace(string input, Hash<string, string> hash)
   {
      if (input.Matches("'::' /(-[':']+) '::' ('[' /(-[']']+) ']')?; f") is (true, var result))
      {
         Slicer slicer = input;
         foreach (var match in result)
         {
            var key = match.FirstGroup;
            var replacement = hash.Maybe[key] | "";
            var specifier = match.SecondGroup;
            if (specifier.IsNotEmpty())
            {
               if (specifier.Matches("^ /(.+) '||' /(.*) $; f") is (true, var patternResult))
               {
                  var pattern = patternResult.FirstGroup;
                  var substitution = patternResult.SecondGroup;
                  replacement = replacement.Substitute(pattern, substitution);
               }
               else if (specifier.Matches("^ /('l'|'u'|'t') 'c'; f") is (true, var casingResult))
               {
                  replacement = casingResult.FirstGroup switch
                  {
                     "l" => replacement.ToLower(),
                     "u" => replacement.ToUpper(),
                     "t" => replacement.ToTitleCase(),
                     _ => replacement
                  };
               }
               else
               {
                  if (replacement.ToObject() is (true, var obj))
                  {
                     replacement = obj.FormatObject(specifier) | replacement;
                  }
                  else if (specifier.StartsWith('s'))
                  {
                     replacement = replacement.FormatAs(specifier);
                  }
               }
            }

            slicer[match.Index, match.Length] = replacement;
         }

         return slicer.ToString();
      }
      else
      {
         return input;
      }
   }
}