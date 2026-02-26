using Core.Collections;

namespace Core.Markdown;

public readonly record struct Replacement(StringHash Replacements)
{
   public string Replace(string text)
   {
      foreach (var (key, value) in Replacements)
      {
         text = text.Replace($"::{key}::", value);
      }

      return text;
   }
}