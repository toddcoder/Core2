using System.Collections;
using Core.Collections;
using Core.Monads;

namespace Core.Markdown;

public class Replacements : IEnumerable<(string key, Replacement replacement)>
{
   protected StringHash<Replacement> replacements = [];

   public Maybe<Replacement> this[string key]
   {
      get => replacements.Maybe[key];
      set => replacements[key] = value;
   }

   public IEnumerator<(string key, Replacement replacement)> GetEnumerator()
   {
      return (IEnumerator<(string key, Replacement replacement)>)replacements.ItemsArray().GetEnumerator();
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}