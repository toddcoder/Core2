﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Collections;
using Core.Matching.Parsers;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.GetHashCodeGenerator;
using RGroup = System.Text.RegularExpressions.Group;
using RRegex = System.Text.RegularExpressions.Regex;

namespace Core.Matching;

public partial class Pattern : IEquatable<Pattern>
{
   protected static bool isFriendly;

   public static implicit operator Pattern(string source)
   {
      var regex = MyRegex();
      var matches = regex.Matches(source);
      if (matches.Count > 0)
      {
         var match = matches[0];
         var group = match.Groups[0];
         var ignoreCase = false;
         var multiline = false;
         var options = group.Value;

         if (options.Contains('i'))
         {
            ignoreCase = true;
         }
         else if (options.Contains('c'))
         {
            ignoreCase = false;
         }

         if (options.Contains('m'))
         {
            multiline = true;
         }
         else if (options.Contains('s'))
         {
            multiline = false;
         }

         bool friendly;
         if (options.Contains('f'))
         {
            friendly = true;
         }
         else if (options.Contains('u'))
         {
            friendly = false;
         }
         else
         {
            friendly = isFriendly;
         }

         var pattern = source.Drop(-match.Length);
         Bits32<RegexOptions> regexOptions = RegexOptions.None;
         regexOptions[RegexOptions.IgnoreCase] = ignoreCase;
         regexOptions[RegexOptions.Multiline] = multiline;

         return new Pattern(pattern, regexOptions, friendly);
      }
      else
      {
         return new Pattern(source, RegexOptions.None, isFriendly);
      }
   }

   public static Pattern operator +(Pattern pattern1, Pattern pattern2) => new(pattern1.Regex + pattern2.Regex, pattern2.Options, pattern2.Friendly);

   public static Pattern operator +(Pattern pattern, string source)
   {
      Pattern pattern2 = source;
      return pattern + pattern2;
   }

   public static Maybe<MatchResult> operator &(Pattern pattern, string input) => input.Matches(pattern);

   protected static Memo<string, string> friendlyPatterns;
   protected static Memo<PatternKey, RRegex> compiledRegexes = new Memo<PatternKey, RRegex>.Function(pk => new RRegex(pk.Regex, pk.Options));

   static Pattern()
   {
      friendlyPatterns = new Memo<string, string>.Function(s => new Parser().Parse(s));
      isFriendly = true;
   }

   internal static string getRegex(string source) => friendlyPatterns[source];

   public static bool IsFriendly
   {
      get => isFriendly;
      set => isFriendly = value;
   }

   protected string regex;
   protected RegexOptions options;
   protected bool friendly;

   protected Pattern(string regex, RegexOptions options, bool friendly)
   {
      this.regex = friendly ? getRegex(regex) : regex;
      this.options = options;
      this.friendly = friendly;
   }

   public string Regex => regex;

   public RegexOptions Options => options;

   public bool Friendly => friendly;

   public Pattern Befriend() => new(regex, options, true);

   public Pattern Unfriend() => new(regex, options, false);

   protected RRegex getRegex() => compiledRegexes[new PatternKey(regex, options)];

   public Optional<MatchResult> MatchedBy(string input)
   {
      try
      {
         var rRegex = getRegex();
         var newMatches = rRegex.Matches(input)
            .Select((m, i) => new Match
            {
               Index = m.Index,
               Length = m.Length,
               Text = m.Value,
               Groups = [.. m.Groups.Cast<RGroup>().Select((g, j) => new Group { Index = g.Index, Length = g.Length, Text = g.Value, Which = j })],
               Which = i
            });
         Match[] matches = [.. newMatches];
         var slicer = new Slicer(input);
         Hash<int, string> indexesToNames = [];
         StringHash<int> namesToIndexes = [];
         foreach (var name in rRegex.GetGroupNames())
         {
            if (MyRegex1().IsMatch(name))
            {
               continue;
            }

            var index = rRegex.GroupNumberFromName(name);
            indexesToNames.Add(index, name);
            namesToIndexes.Add(name, index);
         }

         if (matches.Length > 0)
         {
            return new MatchResult(matches, indexesToNames, namesToIndexes, slicer, input, this);
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Pattern WithIgnoreCase(bool ignoreCase)
   {
      Bits32<RegexOptions> newOptions = options;
      newOptions[RegexOptions.IgnoreCase] = ignoreCase;

      return new Pattern(regex, newOptions, false);
   }

   public Pattern WithMultiline(bool multiline)
   {
      Bits32<RegexOptions> newOptions = options;
      newOptions[RegexOptions.Multiline] = multiline;

      return new Pattern(regex, newOptions, false);
   }

   public Pattern WithPattern(Func<string, string> patternFunc) => new(patternFunc(regex), options, false);

   public bool Equals(Pattern? other) => other is not null && regex == other.regex && options == other.options && friendly == other.friendly;

   public bool Equals(string input) => input.IsMatch(this);

   public override bool Equals(object? obj) => obj is Pattern other && Equals(other) || obj is string input && Equals(input);

   public override int GetHashCode() => hashCode() + regex + options + friendly;

   public static bool operator ==(Pattern left, Pattern right) => Equals(left, right);

   public static bool operator ==(Pattern pattern, string input) => pattern.Equals(input);

   public static bool operator !=(Pattern left, Pattern right) => !Equals(left, right);

   public static bool operator !=(Pattern pattern, string input) => !pattern.Equals(input);

   public string Replace(string input, string replacement) => getRegex().Replace(input, replacement);

   [GeneratedRegex(@"\s*;\s*([icmsfu]{1,3})$", RegexOptions.IgnoreCase, "en-US")]
   private static partial RRegex MyRegex();

   [GeneratedRegex(@"^[+-]?\d+$")]
   private static partial RRegex MyRegex1();
}