using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markdown;

public class MarkdownFrameReplacementsParser(string[] sourceLines, ScalarReplacements scalarReplacements, MultiReplacements multiReplacements)
{
   protected const string REGEX_SCALAR = @"^([a-z_][\w-]*)\s*:\s*(.+)$; u";
   protected const string REGEX_MULTI_BEGIN = @"^([a-z_][\w-]*)\s*\[(.+)$; u";
   protected const string REGEX_MULTI_END = @"^\]$; u";
   protected const string REGEX_RAW_MARKDOWN_BEGIN = "^([a-z_][\\w-]*)<$; u";
   protected const string REGEX_RAW_MARKDOWN_END = "^>$; u";

   public Optional<Unit> Parse()
   {
      try
      {
         Maybe<string> _rawMarkdown = nil;
         List<string> dataLines = [];

         foreach (var line in sourceLines)
         {
            if (line.Matches(REGEX_SCALAR) is (true, var scalarResult))
            {
               var key = scalarResult.FirstGroup;
               var value = scalarResult.SecondGroup;
               scalarReplacements[key] = value;
            }
            else if (line.Matches(REGEX_MULTI_BEGIN) is (true, var beginResult))
            {
               var key = beginResult.FirstGroup;
               string[] keyNames = [.. beginResult.SecondGroup.Split(',').Select(s => s.Trim())];
               multiReplacements.Begin(key, keyNames);
            }
            else if (line.Matches(REGEX_MULTI_END))
            {
               multiReplacements.Commit();
            }
            else if (line.Matches(REGEX_RAW_MARKDOWN_BEGIN) is (true, var rawMarkdownBeginResult))
            {
               _rawMarkdown = rawMarkdownBeginResult.FirstGroup;
               dataLines.Clear();
            }
            else if (line.Matches(REGEX_RAW_MARKDOWN_END))
            {
               if (_rawMarkdown is (true, var rawMarkdownKey))
               {
                  scalarReplacements[rawMarkdownKey] = dataLines.ToString(Environment.NewLine);
                  dataLines.Clear();
               }

               _rawMarkdown = nil;
            }
            else if (multiReplacements.Transacting)
            {
               multiReplacements.CurrentReplacements.Begin();
               foreach (var (key, value) in multiReplacements.CurrentReplacements.Keys.Zip(line.Split(',')))
               {
                  multiReplacements.CurrentReplacements[key] = value;
               }

               multiReplacements.CurrentReplacements.Commit();
            }
            else if (_rawMarkdown)
            {
               dataLines.Add(line);
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