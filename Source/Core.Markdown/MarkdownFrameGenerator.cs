using Core.Enumerables;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Markdown;

public class MarkdownFrameGenerator(Replacer[] replacers, IMarkdownFrameOptions options)
{
   public Optional<(string source, string styles, bool tidy)> Generate()
   {
      try
      {
         List<Replacer> includedReplacers = [];
         var scalarReplacements = options.ScalarReplacements;
         var multiReplacements = options.MultipleReplacements;
         var includeLine = true;
         foreach (var replacer in replacers)
         {
            switch (replacer)
            {
               case Replacer.Inclusion inclusion when keyExists(inclusion.Key) is (true, var key):
               {
                  includeLine = valueExists(key);
                  break;
               }
               case Replacer.Inclusion:
                  includeLine = false;
                  break;
               case Replacer.InclusionNegative inclusionNegative when !keyExists(inclusionNegative.Key):
               {
                  includeLine = true;
                  break;
               }
               case Replacer.InclusionNegative inclusionNegative when keyExists(inclusionNegative.Key) is (true, var key):
               {
                  includeLine = valueExists(key);
                  break;
               }
               case Replacer.InclusionEnd:
                  includeLine = true;
                  break;
               default:
               {
                  if (replacer is Replacer.StyleSpecifier || includeLine)
                  {
                     includedReplacers.Add(replacer);
                  }

                  break;
               }
            }
         }

         List<string> lines = [];
         List<string> styles = [];
         List<string> templateLines = [];
         Maybe<string> _key = nil;

         foreach (var replacer in includedReplacers)
         {
            switch (replacer)
            {
               case Replacer.LineSpecifier lineSpecifier:
               {
                  lines.Add(scalarReplacements.Replace(lineSpecifier.Line));
                  break;
               }
               case Replacer.MultiBegin multiBegin:
               {
                  _key = multiBegin.Key;
                  break;
               }
               case Replacer.MultiEnd:
               {
                  if (_key is (true, var key))
                  {
                     if (options.MultipleReplacements.Maybe[key] is (true, var replacements))
                     {
                        lines.AddRange(replacements.ReplacedLines(templateLines).Select(l => scalarReplacements.Replace(l)));
                     }
                  }

                  templateLines.Clear();
                  _key = nil;
                  break;
               }
               case Replacer.StyleSpecifier styleSpecifier:
               {
                  styles.Add(styleSpecifier.Style);
                  break;
               }
               case Replacer.RawMarkdown rawMarkdown:
               {
                  var rawMarkdownSource = scalarReplacements.RawReplace(rawMarkdown.Markdown);
                  foreach (var line in rawMarkdownSource.Lines())
                  {
                     lines.Add(scalarReplacements.Replace(line));
                  }

                  break;
               }
               case Replacer.Template template:
               {
                  templateLines.Add(template.Line);
                  break;
               }
            }
         }

         foreach (var (key, value) in options.Variables)
         {
            options.Variables[key] = scalarReplacements.Replace(value);
         }

         return (lines.ToString(Environment.NewLine), styles.ToString(Environment.NewLine), options.Tidy);

         Maybe<string> keyExists(string key)
         {
            if (scalarReplacements.Maybe[key] || multiReplacements.Maybe[key])
            {
               return key;
            }
            else
            {
               return nil;
            }
         }

         bool valueExists(string key)
         {
            if (scalarReplacements.Maybe[key] is (true, var scalarValue))
            {
               return scalarValue.IsNotEmpty();
            }
            else if (multiReplacements.Maybe[key])
            {
               return true;
            }
            else
            {
               return false;
            }
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}