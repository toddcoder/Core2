using Core.Collections;
using Core.Matching;
using Core.Objects;
using Core.Strings;
using static Core.Markdown.ReplacementFunctions;

namespace Core.Markdown;

public class ScalarReplacements : IHash<string, string>
{
   protected readonly StringHash replacements = [];

   public string this[string key]
   {
      get => replacements[key];
      set => replacements[key] = value;
   }

   public bool ContainsKey(string key) => replacements.ContainsKey(key);

   public Hash<string, string> GetHash() => replacements;

   public HashInterfaceMaybe<string, string> Items => replacements.Items;

   public HashMaybe<string, string> Maybe => new(replacements);

   public string Replace(string line) => replace(line, replacements);

   public string RawReplace(string key) => replacements.Maybe[key] | "";

   public void Merge(Hash<string, string> other) => replacements.Merge(other);
}