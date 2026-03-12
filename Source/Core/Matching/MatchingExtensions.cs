using System;
using System.Collections.Generic;
using System.Text;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using RRegex = System.Text.RegularExpressions.Regex;

namespace Core.Matching;

public static class MatchingExtensions
{
   extension(string input)
   {
      public bool IsMatch(Pattern pattern) => pattern.MatchedBy(input);

      public string Substitute(Pattern pattern, string replacement) => RRegex.Replace(input, pattern.Regex, replacement, pattern.Options);

      public string Substitute(Pattern pattern, string replacement, int count)
      {
         var regex = new RRegex(pattern.Regex, pattern.Options);
         return regex.Replace(input, replacement, count);
      }

      public string Replace(Pattern pattern, Action<MatchResult> replacer)
      {
         var _response = pattern.MatchedBy(input);
         if (_response is (true, var response))
         {
            replacer(response);
            return response.ToString();
         }
         else
         {
            return input;
         }
      }

      public string[] Unjoin(Pattern pattern) => RRegex.Split(input, pattern.Regex, pattern.Options);

      public IEnumerable<Slice> UnjoinIntoSlices(Pattern pattern)
      {
         var _matches = input.Matches(pattern);
         if (_matches is (true, var matches))
         {
            var index = 0;
            int length;
            string text;
            foreach (var (_, matchIndex, matchLength) in matches)
            {
               length = matchIndex - index;
               text = input.Drop(index).Keep(length);
               yield return new Slice(text, index, length);

               index = matchIndex + matchLength;
            }

            length = input.Length - index;
            text = input.Drop(index);
            yield return new Slice(text, index, length);
         }
      }

      public (string, string) Unjoin2(Pattern pattern)
      {
         var result = input.Unjoin(pattern);
         return result.Length == 1 ? (result[0], "") : (result[0], result[1]);
      }

      public (string, string, string) Unjoin3(Pattern pattern)
      {
         var result = input.Unjoin(pattern);
         return result.Length switch
         {
            1 => (result[0], "", ""),
            2 => (result[0], result[1], ""),
            _ => (result[0], result[1], result[2])
         };
      }

      public (string, string, string, string) Unjoin4(Pattern pattern)
      {
         var result = input.Unjoin(pattern);
         return result.Length switch
         {
            1 => (result[0], "", "", ""),
            2 => (result[0], result[1], "", ""),
            3 => (result[0], result[1], result[2], ""),
            _ => (result[0], result[1], result[2], result[3])
         };
      }

      public (string group1, string group2) MatchGroup2(Pattern pattern)
      {
         var _result = pattern.MatchedBy(input);
         if (_result is (true, var (group1, group2)))
         {
            return (group1, group2);
         }
         else
         {
            return ("", "");
         }
      }

      public (string group1, string group2, string group3) MatchGroup3(Pattern pattern)
      {
         var _result = pattern.MatchedBy(input);
         if (_result is (true, var (group1, group2, group3)))
         {
            return (group1, group2, group3);
         }
         else
         {
            return ("", "", "");
         }
      }

      public (string group1, string group2, string group3, string group4) MatchGroup4(Pattern pattern)
      {
         var _result = pattern.MatchedBy(input);
         if (_result is (true, var (group1, group2, group3, group4)))
         {
            return (group1, group2, group3, group4);
         }
         else
         {
            return ("", "", "", "");
         }
      }

      public string Retain(Pattern pattern)
      {
         var _matches = pattern.MatchedBy(input);
         if (_matches is (true, var matches))
         {
            var builder = new StringBuilder();
            foreach (var match in matches)
            {
               builder.Append(match.Text);
            }

            return builder.ToString();
         }
         else
         {
            return string.Empty;
         }
      }

      public string Scrub(Pattern pattern)
      {
         var _matches = pattern.MatchedBy(input);
         if (_matches is (true, var matches))
         {
            foreach (var match in matches)
            {
               match.Text = string.Empty;
            }

            return matches.ToString();
         }
         else
         {
            return input;
         }
      }

      public Optional<MatchResult> MatchedBy(string input1) => ((Pattern)input).MatchedBy(input1);
      public Maybe<MatchResult> Matches(Pattern pattern) => pattern.MatchedBy(input).Maybe();

      public Pattern Pattern(bool ignoreCase, bool multiline, bool friendly)
      {
         if (!ignoreCase && !multiline && Matching.Pattern.IsFriendly)
         {
            return input;
         }

         var strIgnoreCase = ignoreCase ? "i" : "c";
         var strMultiline = multiline ? "m" : "s";
         var strFriendly = friendly ? "f" : "u";

         return $"{input}; {strIgnoreCase}{strMultiline}{strFriendly}";
      }

      public string Escape()
      {
         return Matching.Pattern.IsFriendly ? input.Replace("/", "//") : RRegex.Escape(input).Replace("]", @"\]");
      }

      public IEnumerable<Match> AllMatches(Pattern pattern)
      {
         var _matches = input.Matches(pattern);
         if (_matches is (true, var matches))
         {
            foreach (var match in matches)
            {
               yield return match;
            }
         }
      }
   }

   public static Maybe<MatchResult> FirstMatch(this IEnumerable<Pattern> patterns, string input)
   {
      foreach (var pattern in patterns)
      {
         var _result = pattern.MatchedBy(input);
         if (_result is (true, var result))
         {
            return result;
         }
      }

      return nil;
   }

   public static IEnumerable<Either<Slice, Match>> LeadingMatches(this IEnumerable<Match> matches, string input, bool includeRemainder = false)
   {
      var index = 0;
      foreach (var match in matches)
      {
         var text = input.Drop(index).Keep(match.Index - index);
         var length = text.Length;
         var slice = new Slice(text, index, length);
         yield return slice;

         yield return match;

         index = match.Index + match.Length;
      }

      if (includeRemainder)
      {
         var remainder = input.Drop(index);
         if (remainder.IsNotEmpty())
         {
            yield return new Slice(remainder, index, remainder.Length);
         }
      }
   }

   public static Maybe<string> ExtractGroup(this string input, Pattern pattern, int groupIndex = 1)
   {
      if (input.Matches(pattern) is (true, var result) && groupIndex.Between(0).Until(result.GroupCount(0)))
      {
         return result.Groups(0)[groupIndex];
      }
      else
      {
         return nil;
      }
   }
}