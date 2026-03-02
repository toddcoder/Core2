using Core.Collections;

namespace Core.Markdown;

public class Replacements(string[] keys)
{
   protected List<string[]> replacements = [];

   public void FromDataLines(IEnumerable<string> dataLines)
   {
      foreach (var dataLine in dataLines)
      {
         /*var hash = keys.Zip(dataLine.Split(',').Select(s => s.Trim())).ToStringHash(t => t.First, t => t.Second);
         List<string> replacedLines = [];
         foreach (var templateLine in templateLines)
         {
            var replacedLine = templateLine;
            foreach (var (key, value) in hash)
            {
               replacedLine = replacedLine.Replace($"::{key}::", value);
            }

            replacedLines.Add(replacedLine);
         }

         replacements.Add([.. replacedLines]);*/
      }
   }

   public IEnumerable<string> ReplacedLines()
   {
      foreach (var replacement in replacements)
      {
         foreach (var line in replacement)
         {
            yield return line;
         }
      }
   }
}