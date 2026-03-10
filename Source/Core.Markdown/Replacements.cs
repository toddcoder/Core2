using Core.Collections;
using static Core.Markdown.ReplacementFunctions;

namespace Core.Markdown;

public class Replacements(params string[] keys)
{
   protected List<StringHash> replacements = [];
   protected StringHash hash = [];
   protected bool transacting;

   public string[] Keys => keys;

   public bool Transacting => transacting;

   public void Begin()
   {
      hash = [];
      transacting = true;
   }

   public void Commit()
   {
      if (hash.Keys.Count == keys.Length)
      {
         replacements.Add(hash);
      }

      hash = [];
      transacting = false;
   }

   public void RollBack()
   {
      hash.Clear();
      transacting = false;
   }

   public string this[string key]
   {
      set => hash[key] = value;
   }

   public IEnumerable<string> ReplacedLines(IEnumerable<string> templateLines)
   {
      string[] lines = [.. templateLines];
      foreach (var replacement in replacements)
      {
         foreach (var templateLine in lines)
         {
            var line = templateLine;
            line = replace(line, replacement);

            yield return line;
         }
      }
   }
}