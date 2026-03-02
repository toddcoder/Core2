using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markdown;

public class MarkdownFrameGenerator(Replacer[] replacers, IMarkdownFrameOptions options)
{
   public Optional<(string source, string styles, bool tidy)> Generate()
   {
      try
      {
         List<Replacer> includedReplacers = [];
         var inclusions = options.Included;
         var includeLine = true;
         foreach (var replacer in replacers)
         {
            if (replacer is Replacer.Inclusion inclusion)
            {
               if (inclusions.Contains(inclusion.Key))
               {
                  includeLine = true;
                  includedReplacers.Add(replacer);
               }
               else
               {
                  includeLine = false;
               }
            }
            else if (includeLine)
            {
               includedReplacers.Add(replacer);
            }
         }

         List<string> lines = [];
         List<string> styles = [];
         Maybe<string> _key = nil;

         foreach (var replacer in includedReplacers)
         {
            switch (replacer)
            {
               case Replacer.LineSpecifier lineSpecifier when _key is (true, var key):
               {
                  if (options.MultipleReplacements.Maybe[key] is (true, var replacements))
                  {
                     lines.AddRange(replacements.ReplacedLines());
                  }

                  break;
               }
               case Replacer.LineSpecifier lineSpecifier:
               {
                  lines.Add(scalarReplacement(lineSpecifier.Line));
                  break;
               }
               case Replacer.MultiBegin multiBegin:
               {
                  _key = multiBegin.Key;
                  break;
               }
               case Replacer.MultiEnd:
                  _key = nil;
                  break;
               case Replacer.StyleSpecifier styleSpecifier:
               {
                  styles.Add(styleSpecifier.Style);
                  break;
               }
            }
         }

         return (lines.ToString(Environment.NewLine), styles.ToString(Environment.NewLine), options.Tidy);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected string scalarReplacement(string line)
   {
      if (line.Matches("^ /s* '::' /(-[':']+) '::' /s* $") is (true, var result))
      {
         var key = result.FirstGroup;
         return options.ScalarReplacements.Maybe[key] | line;
      }
      else
      {
         return line;
      }
   }
}