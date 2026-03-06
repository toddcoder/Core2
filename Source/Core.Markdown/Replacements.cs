using Core.Collections;

namespace Core.Markdown;

public class Replacements(params string[] keys)
{
   protected List<StringHash> replacements = [];

   public int KeyCount => keys.Length;

   public void Add(StringHash replacement) => replacements.Add(replacement);

   /*public void FromDataLines(IEnumerable<string> dataLines)
   {
      foreach (var dataLine in dataLines)
      {
         var hash = keys.Zip(dataLine.Split(',').Select(s => s.Trim())).ToStringHash(t => t.First, t => t.Second);
         replacements.Add(hash);
      }
   }*/

   public IEnumerable<string> ReplacedLines(IEnumerable<string> templateLines)
   {
      string[] lines = [.. templateLines];
      foreach (var replacement in replacements)
      {
         foreach (var templateLine in lines)
         {
            var line = templateLine;
            foreach (var (key, value) in replacement)
            {
               line = line.Replace($"::{key}::", value);
            }

            yield return line;
         }
      }
   }
}