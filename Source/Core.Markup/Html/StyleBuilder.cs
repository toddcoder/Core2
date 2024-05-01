using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Html;

[Obsolete("Use HtmlBuilder")]
public class StyleBuilder
{
   // ReSharper disable once CollectionNeverUpdated.Global
   protected AutoStringHash<List<string>> styles;
   protected Maybe<string> _key;

   public StyleBuilder()
   {
      styles = new AutoStringHash<List<string>>(_ => [], true);
      _key = nil;
   }

   protected void add(string key, string style, string code) => styles[key].Add($"{style}: {code}");

   public void Add(string key, string style, string code)
   {
      _key = key;
      add(key, style, code);
   }

   public void Add(string style, string code)
   {
      if (_key is (true, var key))
      {
         add(key, style, code);
      }
   }

   public override string ToString() => styles.Select(i => $"{i.Key} {{{i.Value.ToString("; ")}}}").ToString(" ");
}