using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Quirk.Parser;

public abstract class BaseParser
{
   public static Token[] GetTokens(ParseState state, Match match)
   {
      return [.. match.AllGroups().Select(g => new Token(state.Index + g.Index, g.Length, g.Value))];
   }

   public abstract Regex Regex();

   public virtual Optional<Unit> Scan(ParseState state)
   {
      var matches = Regex().Matches(state.Line);
      if (matches.Count > 0)
      {
         var match = matches[0];
         var _parsed = Parse(state, GetTokens(state, match));
         if (_parsed)
         {
            state.Advance(match.Length);
         }

         return _parsed;
      }
      else
      {
         return nil;
      }
   }

   public abstract Optional<Unit> Parse(ParseState state, Token[] tokens);
}