using System.Text.RegularExpressions;
using Core.Monads;

namespace Core.Markup.Quirk.Parser;

public abstract class BaseParser
{
   public static Token[] GetTokens(ParseState state, Match match) => [.. match.Groups]
   public abstract Regex Regex();

   public virtual Optional<Unit> Scan(ParseState state)
   {
      var matches = Regex().Matches(state.CurrentSource);
      if (matches.Count > 0)
      {
         var match = matches[0];
         var index = state.Index;
         var _parsed = Parse(state,)
      }
   }

   public abstract Optional<Unit> Parse(ParseState state, string[] tokens);
}