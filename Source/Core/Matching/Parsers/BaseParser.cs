using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using RRegex = System.Text.RegularExpressions.Regex;

namespace Core.Matching.Parsers;

using MMatch = System.Text.RegularExpressions.Match;
using GGroup = System.Text.RegularExpressions.Group;

public abstract class BaseParser
{
   public const string REGEX_IDENTIFIER = "[a-zA-Z_][a-zA-Z_0-9]*";
   public const string REGEX_BAL_IDENTIFIER = "[a-zA-Z_][a-zA-Z_0-9-]*";

   protected string[] tokens;

   public BaseParser()
   {
      tokens = [];
   }

   public abstract string Pattern { get; }

   protected static string escape(string text) => RRegex.Escape(text).Replace("]", @"\]");

   public Maybe<string> Scan(string source, ref int index)
   {
      static IEnumerable<GGroup> getGroups(MMatch match)
      {
         foreach (GGroup group in match.Groups)
         {
            yield return group;
         }
      }

      var options = RegexOptions.Compiled;
      var regex = new RRegex(Pattern, options);

      var input = source.Drop(index);
      var matches = regex.Matches(input);
      if (matches.Count > 0)
      {
         var oldIndex = index;

         index += matches[0].Length;
         tokens = [.. getGroups(matches[0]).Select(group => group.Value)];
         var _result = Parse(source, ref index);
         if (!_result)
         {
            index = oldIndex;
         }

         return _result;
      }
      else
      {
         return nil;
      }
   }

   public abstract Maybe<string> Parse(string source, ref int index);
}