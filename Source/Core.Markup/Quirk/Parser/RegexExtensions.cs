using Core.Monads;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Quirk.Parser;

public static class RegexExtensions
{
   extension(string input)
   {
      public Maybe<MatchCollection> MatchOf(string pattern)
      {
         var regex = new Regex(pattern);
         return input.MatchOf(regex);
      }

      public Maybe<MatchCollection> MatchOf(Regex regex)
      {
         var collection = regex.Matches(input);
         return collection.Count > 0 ? collection : nil;
      }
   }

   extension(Match match)
   {
      public IEnumerable<Group> AllGroups()
      {
         foreach (Group group in match.Groups)
         {
            yield return group;
         }
      }
   }
}