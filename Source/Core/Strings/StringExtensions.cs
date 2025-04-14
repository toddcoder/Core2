using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Core.Assertions;
using Core.Dates.Now;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;
using static Core.Strings.StringStreamFunctions;

namespace Core.Strings;

public static class StringExtensions
{
   private enum StageType
   {
      LeftNotFound,
      LeftFound
   }

   private const string PAIRS = "()[]{}<>";

   public static string Repeat(this string source, int count)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var result = new StringBuilder();
      for (var i = 0; i < count; i++)
      {
         result.Append(source);
      }

      return result.ToString();
   }

   public static string Repeat(this string source, int count, int maxLength)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var result = source.Repeat(count);
      return result.Length <= maxLength ? result : result[..maxLength];
   }

   public static string Repeat(this string source, int count, string connector)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      List<string> result = [];
      for (var i = 0; i < count; i++)
      {
         result.Add(source);
      }

      return result.ToString(connector.ToNonNullString());
   }

   public static string Repeat(this string source, int count, int maxLength, string connector)
   {
      var result = source.Repeat(count, connector);
      return result.Length <= maxLength ? result : result[..maxLength];
   }

   public static string[] Lines(this string source, SplitType split)
   {
      return source.IsEmpty() ? [] : source.Unjoin(splitPattern(split));
   }

   public static string[] Lines(this string source) => source.Unjoin("/r/n | /r | /n; f");

   public static string Slice(this string source, int startIndex, int stopIndex)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      if (startIndex < 0)
      {
         startIndex = 0;
      }

      if (stopIndex > source.Length - 1)
      {
         stopIndex = source.Length - 1;
      }

      var length = stopIndex - startIndex + 1;
      return source.Drop(startIndex).Keep(length);
   }

   public static string Slice(this string source, Maybe<int> startIndex, Maybe<int> stopIndex)
   {
      return source.Slice(startIndex | 0, stopIndex | (() => source.Length - 1));
   }

   public static string Truncate(this string source, int limit, bool addEllipses = true)
   {
      if (source.IsEmpty() || limit <= 0)
      {
         return "";
      }
      else if (source.Length <= limit)
      {
         return source;
      }
      else if (addEllipses)
      {
         return source.Keep(limit - 1) + "…";
      }
      else
      {
         return source.Keep(limit);
      }
   }

   public static string EnsureLength(this string source, int length, char paddingCharacter = ' ')
   {
      if (source.IsEmpty())
      {
         return paddingCharacter.ToString().Repeat(length);
      }
      else if (source.Length < length)
      {
         return source.PadRight(length, paddingCharacter);
      }
      else if (source.Length > length)
      {
         return source.Keep(length - 3) + "...";
      }
      else
      {
         return source;
      }
   }

   public static string Elliptical(this string source, int limit, char upTo, bool pad = false, string ellipses = "…")
   {
      if (source.IsEmpty() || limit <= 0)
      {
         return "";
      }
      else if (source.Length > limit)
      {
         var index = source.LastIndexOf(upTo);
         if (index == -1)
         {
            return source.Truncate(limit);
         }
         else
         {
            var ellipsesLength = ellipses.Length;
            var suffix = source.Keep(-(source.Length - index));
            var prefix = source.Keep(limit - (1 + suffix.Length));
            if (!source.StartsWith(prefix))
            {
               prefix = $"{ellipses}{prefix.Drop(ellipsesLength)}";
            }

            var result = $"{prefix}{ellipses}{suffix}";
            if (result.Length > limit)
            {
               result = $"{result.Keep(limit - ellipsesLength)}{ellipses}";
            }

            return result;
         }
      }
      else if (source.Length < limit && pad)
      {
         return source.LeftJustify(limit);
      }
      else
      {
         return source;
      }
   }

   public static string Elliptical(this string source, int limit, string upTo, bool pad = false, string ellipses = "…")
   {
      char[] upToChars = [.. upTo];
      if (source.IsEmpty() || limit <= 0)
      {
         return "";
      }
      else if (source.Length > limit)
      {
         var index = source.LastIndexOfAny(upToChars);
         if (index == -1)
         {
            return source.Truncate(limit);
         }
         else
         {
            var ellipsesLength = ellipses.Length;
            var suffix = source.Keep(-(source.Length - index));
            var prefix = source.Keep(limit - (1 + suffix.Length));
            if (!source.StartsWith(prefix))
            {
               prefix = $"{ellipses}{prefix.Drop(ellipsesLength)}";
            }

            var result = $"{prefix}{ellipses}{suffix}";
            if (result.Length > limit)
            {
               result = $"{result.Keep(limit - ellipsesLength)}{ellipses}";
            }

            return result;
         }
      }
      else if (source.Length < limit && pad)
      {
         return source.LeftJustify(limit);
      }
      else
      {
         return source;
      }
   }

   private static string replaceWhitespace(string source, string replacement)
   {
      return source.Map(s => string.Join(replacement, s.Unjoin("/s+; f")));
   }

   public static string NormalizeWhitespace(this string source) => replaceWhitespace(source, " ");

   public static string RemoveWhitespace(this string source) => replaceWhitespace(source, "");

   public static string ToTitleCase(this string source)
   {
      return source.Map(s => new CultureInfo("en-US").TextInfo.ToTitleCase(s.ToLower()));
   }

   public static string CamelToLowerWithSeparator(this string source, string separator, string quantitative = "+")
   {
      if (source.IsEmpty())
      {
         return "";
      }

      if (source.IsMatch("^ {A-Z} $; f"))
      {
         return source.ToLower();
      }
      else
      {
         var _matches = source.Matches($"-/b /(['A-Z']{quantitative}); f");
         if (_matches is (true, var matches))
         {
            foreach (var match in matches)
            {
               match.FirstGroup = separator + match.FirstGroup;
            }

            return matches.ToString().ToLower();
         }
         else
         {
            return source.ToLower();
         }
      }
   }

   public static string CamelToSnakeCase(this string source, string quantitative = "+")
   {
      return source.Map(s => s.CamelToLowerWithSeparator("_", quantitative));
   }

   public static string CamelToObjectGraphCase(this string source, string quantitative = "+")
   {
      return source.Map(s => s.CamelToLowerWithSeparator("-", quantitative));
   }

   public static string LowerToCamelCase(this string source, string separator, bool upperCase)
   {
      if (source.IsEmpty())
      {
         return "";
      }
      else
      {
         var pattern = separator.Escape().SingleQuotify() + "/(['a-z']); f";
         var _matches = source.Matches(pattern);
         if (_matches is (true, var matches))
         {
            foreach (var match in matches)
            {
               match.Text = match.FirstGroup.ToUpper();
            }

            return upperCase ? matches.ToString().ToUpper1() : matches.ToString().ToLower1();
         }
         else
         {
            return upperCase ? source.ToUpper1() : source.ToLower1();
         }
      }
   }

   public static string SnakeToCamelCase(this string source, bool upperCase) => source.LowerToCamelCase("_", upperCase);

   public static string Abbreviate(this string source)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var text = source;
      if (text.Has("_"))
      {
         text = text.SnakeToCamelCase(true);
      }

      var _matches = text.Matches("['A-Z'] ['a-z']*; f");
      if (_matches is (true, var matches))
      {
         var numeric = "";
         var _numeric = text.Matches("/d+ $; f").Map(r => r.Text);
         if (_numeric)
         {
            numeric = _numeric;
         }

         var builder = new StringBuilder();
         foreach (var match in matches)
         {
            var _subMatches = match.ZerothGroup.Matches("^ /uc? /lc* [/lv 'y' /uv 'Y']+ /(/lc?) /1? ['h']? ('e' $)?; f");
            if (_subMatches is (true, var subMatches))
            {
               foreach (var subMatch in subMatches)
               {
                  builder.Append(fixMatch(match.ZerothGroup, subMatch.ZerothGroup));
               }
            }
            else
            {
               builder.Append(matches[0, 0]);
            }

            if (matches[0, 0].EndsWith("s"))
            {
               builder.Append('s');
            }
         }

         return builder + numeric;
      }
      else
      {
         return text;
      }
   }

   private static string fixMatch(string match, string subMatch)
   {
      if (match.IsMatch("^ 'assign' /w+; f"))
      {
         return "Assign";
      }
      else if (match.IsMatch("^ 'institut' /w+; f"))
      {
         return "Inst";
      }
      else if (match.IsMatch("^ 'cust'; f"))
      {
         return "Cust";
      }
      else if (match.IsMatch("'id' $; f"))
      {
         return "ID";
      }
      else if (match.IsMatch("/b 'image' /b; f"))
      {
         return "Img";
      }
      else if (match.IsMatch("'company'; f"))
      {
         return "Cpny";
      }
      else if (match.IsMatch("'user'; f"))
      {
         return "Usr";
      }
      else if (match.IsMatch("'order'; f"))
      {
         return "Ord";
      }
      else if (match.IsMatch("'history'; f"))
      {
         return "Hist";
      }
      else if (match.IsMatch("'account'; f"))
      {
         return "Acct";
      }
      else if (match.IsMatch("^ 'stag' ('e' | 'ing'); f"))
      {
         return "Stg";
      }
      else if (match.IsMatch("'import'; f"))
      {
         return "Imp";
      }
      else if (match.IsMatch("'message' 's'?; f"))
      {
         return "Msg";
      }
      else
      {
         return subMatch;
      }
   }

   public static string Append(this string target, string text) => string.IsNullOrEmpty(target) ? text : target + text;

   public static string Passwordify(this string source)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      source = source.ToLower().Replace("a", "@");
      source = source.Replace("b", "6");
      source = source.Replace("c", "(");
      source = source.Replace("e", "3");
      source = source.Replace("g", "8");
      source = source.Replace("h", "4");
      source = source.Replace("i", "1");
      source = source.Replace("l", "7");
      source = source.Replace("o", "0");
      source = source.Replace("q", "9");
      source = source.Replace("s", "$");
      source = source.Replace("t", "+");

      return source.Replace("z", "2");
   }

   public static string Depasswordify(this string source)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      source = source.Replace("@", "a");
      source = source.Replace("6", "b");
      source = source.Replace("(", "c");
      source = source.Replace("3", "e");
      source = source.Replace("8", "g");
      source = source.Replace("4", "h");
      source = source.Replace("1", "i");
      source = source.Replace("7", "l");
      source = source.Replace("0", "o");
      source = source.Replace("9", "q");
      source = source.Replace("$", "s");
      source = source.Replace("+", "t");

      return source.Replace("2", "z");
   }

   [Obsolete("Use new string")]
   public static string Copy(this string source) => new(source);

   public static string RemoveCComments(this string source)
   {
      return source.Map(s => s.Substitute("'/*' .*? '*/'; f", ""));
   }

   public static string RemoveCOneLineComments(this string source)
   {
      return source.Map(s => s.Substitute("'//' .*? /crlf; fm", ""));
   }

   public static string RemoveSQLOneLineComments(this string source)
   {
      return source.Map(s => s.Substitute("'--' .*? /crlf; fm", ""));
   }

   public static string AllowOnly(this string source, string allowed)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      if (!allowed.IsMatch("'[' -[']']+ ']'; f"))
      {
         allowed = $"['{allowed}']; f";
      }

      var input = allowed;
      var _matches = input.Matches(source);
      if (_matches is (true, var matches))
      {
         var builder = new StringBuilder();
         foreach (var match in matches)
         {
            builder.Append(match.ZerothGroup);
         }

         return builder.ToString();
      }
      else
      {
         return "";
      }
   }

   public static string AllowOnly(this string source, params char[] allowed)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var characters = new StringBuilder();
      foreach (var character in allowed)
      {
         characters.Append(character);
      }

      return source.AllowOnly(characters.ToString());
   }

   public static string Quotify(this string source, string quote) => source.Map(s => $"\"{s.Replace("\"", quote)}\"");

   public static string SingleQuotify(this string source, string quote) => source.Map(s => $"'{s.Replace("'", quote)}'");

   public static string Quotify(this string source) => source.Quotify(@"\""");

   public static string SingleQuotify(this string source) => source.SingleQuotify(@"\'");

   public static string QuotifyIf(this string source, string escapedQuote)
   {
      return source.IsNumeric() || source.Same("true") || source.Same("false") ? source : source.Quotify(escapedQuote);
   }

   public static string SingleQuotifyIf(this string source, string escapedQuote)
   {
      return source.IsNumeric() || source.Same("true") || source.Same("false") ? source : source.SingleQuotify(escapedQuote);
   }

   public static string QuotifyIf(this string source) => source.QuotifyIf(@"\""");

   public static string SingleQuotifyIf(this string source) => source.SingleQuotifyIf("\'");

   public static string Unquotify(this string source)
   {
      return source.Map(s => s.Matches("^ `quote /(.*) `quote $; f").Map(result => result.FirstGroup) | s);
   }

   public static string SingleUnquotify(this string source)
   {
      return source.Map(s => source.Matches("^ `apos /(.*) `apos $; f").Map(result => result.FirstGroup) | s);
   }

   public static string Guillemetify(this string source) => source.Map(s => $"«{s}»");

   public static string VisibleWhitespace(this string source, bool spaceVisible)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      if (spaceVisible)
      {
         source = source.Substitute("' '; f", "•");
      }

      source = source.Substitute("/t; f", "¬");
      source = source.Substitute("/n; f", "¶");
      source = source.Substitute("/r; f", "¤");

      return source;
   }

   public static string VisibleTabs(this string source) => source.Map(s => s.Substitute("/t; f", "¬"));

   public static string PadCenter(this string source, int length, char paddingCharacter = ' ')
   {
      if (source.IsEmpty())
      {
         return paddingCharacter.ToString().Repeat(length);
      }
      else
      {
         var sourceLength = source.Length;
         if (sourceLength < length)
         {
            var difference = length - sourceLength;
            var half = difference / 2;
            var padRight = difference.IsEven() ? half : half + 1;
            var paddingString = paddingCharacter.ToString();

            return paddingString.Repeat(half) + source + paddingString.Repeat(padRight);
         }
         else
         {
            return source;
         }
      }
   }

   public static string Pad(this string source, PadType padType, int length, char paddingCharacter = ' ') => padType switch
   {
      PadType.Center => source.PadCenter(length, paddingCharacter),
      PadType.Left => source.PadLeft(length, paddingCharacter),
      PadType.Right => source.PadRight(length, paddingCharacter),
      _ => source
   };

   public static string Pad(this string source, int width, char paddingCharacter = ' ')
   {
      return source.Map(s => width > 0 ? s.PadLeft(width, paddingCharacter) : s.PadRight(-width, paddingCharacter));
   }

   public static string Justify(this object source, Justification justification, int width, char paddingCharacter = ' ')
   {
      var asString = source.ToNonNullString();

      if (asString.IsEmpty())
      {
         return "";
      }

      PadType padType;
      switch (justification)
      {
         case Justification.Left:
            padType = PadType.Right;
            break;
         case Justification.Right:
            padType = PadType.Left;
            break;
         case Justification.Center:
            padType = PadType.Center;
            break;
         default:
            return asString;
      }

      return asString.Pad(padType, width, paddingCharacter);
   }

   public static string LeftJustify(this object source, int width, char paddingChar = ' ')
   {
      return source.Justify(Justification.Left, width, paddingChar);
   }

   public static string RightJustify(this object source, int width, char paddingChar = ' ')
   {
      return source.Justify(Justification.Right, width, paddingChar);
   }

   public static string Center(this string source, int width, char paddingChar = ' ')
   {
      return source.Justify(Justification.Center, width, paddingChar);
   }

   public static string Debyteify(this byte[] source)
   {
      var result = new StringBuilder();
      var crLFEmitted = false;

      foreach (var character in source)
      {
         if (character == 0 || character.Between((byte)32).And(126))
         {
            result.Append(Convert.ToChar(character));
            crLFEmitted = false;
         }
         else if (!crLFEmitted)
         {
            result.Append("\r\n");
            crLFEmitted = true;
         }
      }

      return result.ToString();
   }

   public static string Debyteify(this string source, Encoding encoding) => source.Map(s => encoding.GetBytes(s).Debyteify());

   public static string Debyteify(this string source) => source.Debyteify(Encoding.ASCII);

   public static string DefaultTo(this string source, string defaultValue) => source.IsEmpty() ? defaultValue : source;

   public static string TrimLeft(this string source) => source.Map(s => s.TrimStart());

   public static string TrimRight(this string source) => source.Map(s => s.TrimEnd());

   public static string TrimAll(this string source) => source.Map(s => s.Trim());

   public static string ToUpper1(this string source) => source.Map(s => s.Keep(1).ToUpper() + s.Drop(1));

   public static string ToLower1(this string source) => source.Map(s => s.Keep(1).ToLower() + s.Drop(1));

   [Obsolete("Use int.Plural(string)")]
   public static string Plural(this string source, int number, bool omitNumber = false)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      Pattern pattern = "'(' /(/w+) (',' /(/w+))? ')'; fi";
      var _matches = source.Matches(pattern);
      if (_matches is (true, var matches1))
      {
         if (number == 1)
         {
            foreach (var match in matches1)
            {
               if (match.SecondGroup.IsNotEmpty())
               {
                  match.Text = match.FirstGroup;
               }
               else
               {
                  match.Text = "";
               }
            }
         }
         else
         {
            foreach (var match in matches1)
            {
               if (match.SecondGroup.IsNotEmpty())
               {
                  match.Text = match.SecondGroup;
               }
               else
               {
                  match.Text = match.FirstGroup;
               }
            }
         }

         var matcherText = matches1.ToString();
         var numberAccountedFor = false;
         pattern = @"-(< '\') /'#'; f";
         _matches = matcherText.Matches(pattern);
         if (_matches)
         {
            numberAccountedFor = true;
            foreach (var match in matches1)
            {
               match.FirstGroup = number.ToString();
            }

            matcherText = matches1.ToString();
         }

         pattern = @"/('\#'); f";
         _matches = matcherText.Matches(pattern);
         if (_matches is (true, var matches2))
         {
            foreach (var match in matches2)
            {
               match.FirstGroup = "#";
            }

            matcherText = matches2.ToString();
         }

         if (omitNumber || numberAccountedFor)
         {
            return matcherText;
         }
         else
         {
            return $"{number} {matcherText}";
         }
      }
      else
      {
         return source;
      }
   }

   public static string Reverse(this string source) => source.Replace(new string([.. source.Select(c => c).Reverse()]));

   public static string Succ(this string source)
   {
      if (source.IsNotEmpty())
      {
         var builder = new StringBuilder(source);
         return succ(builder, builder.Length - 1).ToString();
      }
      else
      {
         return source;
      }
   }

   private static StringBuilder succ(StringBuilder builder, int index)
   {
      while (true)
      {
         if (builder.Length != 0)
         {
            if (index >= 0)
            {
               var ch = builder[index];
               switch (ch)
               {
                  case '9':
                     builder[index] = '0';
                     if (index == 0)
                     {
                        builder.Insert(0, '0');
                        return builder;
                     }

                     index--;
                     continue;
                  case 'Z':
                     builder[index] = 'A';
                     if (index == 0)
                     {
                        builder.Insert(0, 'A');
                        return builder;
                     }

                     index--;
                     continue;
                  case 'z':
                     builder[index] = 'a';
                     if (index == 0)
                     {
                        builder.Insert(0, 'a');
                        return builder;
                     }

                     index--;
                     continue;
               }

               if (ch.ToString().IsMatch("[alnum]; f"))
               {
                  builder[index] = (char)(builder[index] + 1);
                  return builder;
               }

               index--;
            }
            else
            {
               return new StringBuilder();
            }
         }
         else
         {
            return builder;
         }
      }
   }

   public static string Pred(this string source)
   {
      if (source.IsNotEmpty())
      {
         var builder = new StringBuilder(source);
         return pred(builder, builder.Length - 1).ToString();
      }
      else
      {
         return source;
      }
   }

   private static StringBuilder pred(StringBuilder builder, int index)
   {
      while (true)
      {
         if (builder.Length != 0)
         {
            if (index >= 0)
            {
               var ch = builder[index];
               switch (ch)
               {
                  case '0':
                     builder[index] = '9';
                     index--;
                     continue;
                  case 'A':
                     builder[index] = 'Z';
                     index--;
                     continue;
                  case 'a':
                     builder[index] = 'z';
                     index--;
                     continue;
               }

               if (ch.ToString().IsMatch("[alnum]; f"))
               {
                  builder[index] = (char)(builder[index] - 1);
                  return builder;
               }

               index--;
            }
            else
            {
               builder.Remove(0, 1);
               return builder;
            }
         }
         else
         {
            return builder;
         }
      }
   }

   public static bool IsNotEmpty(this string source) => !string.IsNullOrEmpty(source);

   public static bool IsEmpty(this string source) => string.IsNullOrEmpty(source);

   public static bool IsNotWhiteSpace(this string source) => !string.IsNullOrWhiteSpace(source);

   public static bool IsWhiteSpace(this string source) => string.IsNullOrWhiteSpace(source);

   public static bool Has(this string source, string substring, bool ignoreCase = false)
   {
      if (source.IsNotEmpty() && substring.IsNotEmpty())
      {
         return ignoreCase ? source.ToLower().Contains(substring.ToLower()) : source.Contains(substring);
      }
      else
      {
         return false;
      }
   }

   public static bool IsWhitespace(this string source) => source.IsEmpty() || source.IsMatch("^ /s* $; f");

   public static bool IsQuoted(this string source) => !source.IsEmpty() && source.IsMatch("^ [quote] .*? [quote] $; f");

   public static bool Same(this string source, string comparison)
   {
      if (source.IsEmpty())
      {
         source = source.ToNonNullString();
      }

      if (comparison.IsEmpty())
      {
         comparison = comparison.ToNonNullString();
      }

      return string.Compare(source, comparison, StringComparison.OrdinalIgnoreCase) == 0;
   }

   public static bool AnySame(this string source, params string[] comparisons) => comparisons.Any(source.Same);

   public static bool IsGUID(this string source)
   {
      return !source.IsEmpty() && source.IsMatch("^ '{'? [/l /d]8 '-' [/l /d]4 '-' [/l /d]4 '-' [/l /d]4 '-' [/l /d]12 '}'? $; f");
   }

   public static bool In(this string source, params string[] comparisons) => !source.IsEmpty() && comparisons.Any(source.Same);

   public static bool IsConvertibleTo<T>(this string source)
   {
      try
      {
         if (source.IsNotEmpty())
         {
            _ = Convert.ChangeType(source, typeof(T));
            return true;
         }
         else
         {
            return false;
         }
      }
      catch
      {
         return false;
      }
   }

   public static bool IsConvertibleTo(this string source, Type type)
   {
      try
      {
         if (source.IsNotEmpty())
         {
            _ = Convert.ChangeType(source, type);
            return true;
         }
         else
         {
            return false;
         }
      }
      catch
      {
         return false;
      }
   }

   public static string ToBase64(this string source, Encoding encoding)
   {
      return Convert.ToBase64String(encoding.GetBytes(source));
   }

   public static string ToBase64(this string source) => source.ToBase64(Encoding.ASCII);

   public static Enum ToBaseEnumeration(this string value, Type enumerationType, bool ignoreCase = true)
   {
      return (Enum)Enum.Parse(enumerationType, value, ignoreCase);
   }

   public static Enum ToBaseEnumeration(this string value, Type enumerationType, bool ignoreCase, Enum defaultValue)
   {
      try
      {
         return (Enum)Enum.Parse(enumerationType, value, ignoreCase);
      }
      catch
      {
         return defaultValue;
      }
   }

   public static string ExtractFromQuotes(this string source)
   {
      return source.Map(s => s.Matches("^ [quote] /(.*?) [quote] $; f").Map(result => result.FirstGroup) | s);
   }

   public static Maybe<object> ToObject(this string value)
   {
      if (value.IsQuoted())
      {
         return value.ExtractFromQuotes();
      }
      else if (value.IsIntegral())
      {
         var newValue = value.Value().Int64();
         return newValue is >= int.MinValue and <= int.MaxValue ? value.Maybe().Int32() : value;
      }

      if (value.IsSingle())
      {
         Pattern pattern = "^ /(.+) ['fF'] $; f";
         var _fSuffix = value.Matches(pattern).Map(r => r.FirstGroup);

         return _fSuffix.Map(f => f.Maybe().Single());
      }

      if (value.IsDouble())
      {
         Pattern pattern = "^ /(.+) ['dD'] $; f";
         var _dSuffix = value.Matches(pattern).Map(r => r.FirstGroup);

         return _dSuffix.Map(d => d.Maybe().Double());
      }

      if (value.IsDecimal())
      {
         Pattern pattern = "^ /(.+) ['mM'] $; f";
         var _mSuffix = value.Matches(pattern).Map(r => r.FirstGroup);

         return _mSuffix.Map(m => m.Maybe().Decimal());
      }

      if (value.IsGUID())
      {
         return new Guid(value);
      }
      else if (value.IsDate())
      {
         return value.Maybe().DateTime();
      }
      else if (value.Same("false") || value.Same("true"))
      {
         return value.Maybe().Boolean();
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<Type> ToType(this string value)
   {
      if (value.IsEmpty())
      {
         return nil;
      }
      else if (value.IsQuoted())
      {
         return typeof(string);
      }
      else if (value.IsIntegral())
      {
         return typeof(int);
      }
      else if (value.IsSingle())
      {
         return typeof(float);
      }
      else if (value.IsDouble())
      {
         return typeof(double);
      }
      else if (value.IsDecimal())
      {
         return typeof(decimal);
      }
      else if (value.IsGUID())
      {
         return typeof(Guid).Some();
      }
      else if (value.IsDate())
      {
         return typeof(DateTime).Some();
      }
      else if (value.Same("false") || value.Same("true"))
      {
         return typeof(bool);
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<object> ToParsed(this string value, Type type)
   {
      if (value.IsEmpty())
      {
         return nil;
      }
      else if (type == typeof(string))
      {
         return value.ExtractFromQuotes();
      }
      else if (type == typeof(int))
      {
         return value.Maybe().Int32().CastAs<object>();
      }
      else if (type == typeof(float))
      {
         return value.Maybe().Single().CastAs<object>();
      }
      else if (type == typeof(double))
      {
         return value.Maybe().Double().CastAs<object>();
      }
      else if (type == typeof(decimal))
      {
         return value.Maybe().Decimal().CastAs<object>();
      }
      else if (type == typeof(Guid))
      {
         return value.Maybe().Guid().CastAs<object>();
      }
      else if (type == typeof(DateTime))
      {
         return value.Maybe().DateTime().CastAs<object>();
      }
      else if (type == typeof(bool))
      {
         return value.Maybe().Boolean().CastAs<object>();
      }
      else
      {
         return nil;
      }
   }

   public static Result<object> AsObject(this string value)
   {
      if (value.IsEmpty())
      {
         return fail("value is null");
      }
      else if (value.IsQuoted())
      {
         return value.ExtractFromQuotes();
      }
      else if (value.IsIntegral())
      {
         return
            from newValue in value.Result().Int64()
            from assertion in newValue.Must().BeBetween(int.MinValue).And(int.MaxValue).OrFailure()
            select (object)(int)assertion;
      }

      if (value.IsSingle())
      {
         return
            from result in value.Matches("^ /(.+) ['fF'] $; f").Result("Single in invalid format")
            from floated in result.FirstGroup.Result().Single()
            select (object)floated;
      }

      if (value.IsDouble())
      {
         return
            from result in value.Matches("^ /(.+) ['dD'] $; f").Result("Double in invalid format")
            from doubled in result.FirstGroup.Result().Double()
            select (object)doubled;
      }

      if (value.IsDecimal())
      {
         return
            from result in value.Matches("^ /(.+) ['mM'] $; f").Result("Decimal in invalid format")
            from asDecimal in result.FirstGroup.Result().Decimal()
            select (object)asDecimal;
      }

      if (value.IsGUID())
      {
         return value.Result().Guid().Map(g => g.CastAs<object>());
      }
      else if (value.IsDate())
      {
         return value.Result().DateTime().Map(dt => dt.Result().Cast<object>());
      }
      else if (value.Same("false") || value.Same("true"))
      {
         return value.Result().Boolean().Map(b => b.Result().Cast<object>());
      }
      else
      {
         return fail($"Couldn't determine type of {value}");
      }
   }

   public static Result<Type> Type(this string value)
   {
      if (value.IsEmpty())
      {
         return fail("value is null");
      }
      else if (value.IsQuoted())
      {
         return typeof(string);
      }
      else if (value.IsIntegral())
      {
         return typeof(int);
      }
      else if (value.IsSingle())
      {
         return typeof(float);
      }
      else if (value.IsDouble())
      {
         return typeof(double);
      }
      else if (value.IsDecimal())
      {
         return typeof(decimal);
      }
      else if (value.IsGUID())
      {
         return typeof(Guid);
      }
      else if (value.IsDate())
      {
         return typeof(DateTime);
      }
      else if (value.Same("false") || value.Same("true"))
      {
         return typeof(bool);
      }
      else
      {
         return fail($"Couldn't determine type of {value}");
      }
   }

   public static Result<object> Parsed(this string value, Type type)
   {
      if (value.IsEmpty())
      {
         return fail("value is null");
      }
      else if (type == typeof(string))
      {
         return value.ExtractFromQuotes().Success().CastAs<object>();
      }
      else if (type == typeof(int))
      {
         return value.Result().Int32().CastAs<object>();
      }
      else if (type == typeof(float))
      {
         return value.Result().Single().CastAs<object>();
      }
      else if (type == typeof(double))
      {
         return value.Result().Double().CastAs<object>();
      }
      else if (type == typeof(decimal))
      {
         return value.Result().Decimal().CastAs<object>();
      }
      else if (type == typeof(Guid))
      {
         return value.Result().Guid().CastAs<object>();
      }
      else if (type == typeof(DateTime))
      {
         return value.Result().DateTime().CastAs<object>();
      }
      else if (type == typeof(bool))
      {
         return value.Result().Boolean().CastAs<object>();
      }
      else
      {
         return fail($"Couldn't determine type of {value}");
      }
   }

   public static string ToNonNullString(this object? value) => value?.ToString() ?? "";

   public static Maybe<string> ToMaybeString(this object? value)
   {
      if (value is not null)
      {
         return value.ToString() ?? "";
      }
      else
      {
         return nil;
      }
   }

   public static string ToLiteral(this object value)
   {
      if (value.IsFloat())
      {
         var result = value.ToString() ?? "";
         if (!result.Has("."))
         {
            result += ".0";
         }

         if (value.IsDouble())
         {
            return result + "d";
         }
         else if (value.IsSingle())
         {
            return result + "f";
         }
         else if (value.IsDecimal())
         {
            return result + "m";
         }
         else
         {
            return result;
         }
      }
      else
      {
         return value.ToString() ?? "";
      }
   }

   public static Maybe<int> ExtractInt(this string source)
   {
      return maybe<int>() & source.IsNotEmpty() & (() => source.Matches("/(['+-']? /d+); f").Map(result => result[0, 1].Maybe().Int32()));
   }

   public static Maybe<double> ExtractDouble(this string source)
   {
      return maybe<double>() & source.IsNotEmpty() & (() => source.Matches("/(['+-']? /d* '.' /d* (['eE'] ['-+']? /d+)?); f")
         .Map(result => result[0, 1].Value().Double()));
   }

   public static Maybe<char> First(this string source) => maybe<char>() & source.IsNotEmpty() & (() => source[0]);

   public static Maybe<char> Last(this string source) => maybe<char>() & source.IsNotEmpty() & (() => source[^1]);

   public static Maybe<string> Left(this string source, int length)
   {
      var minLength = length.MinOf(source.Length);
      return maybe<string>() & minLength > 0 & (() => source.Keep(minLength));
   }

   public static Maybe<string> Right(this string source, int length)
   {
      var minLength = Math.Min(length, source.Length);
      return maybe<string>() & (source.IsNotEmpty() && minLength > 0) & (() => source.Drop(source.Length - minLength).Keep(minLength));
   }

   public static Maybe<string> Sub(this string source, int index, int length)
   {
      return maybe<string>() & (source.IsNotEmpty() && length > 0 && index >= 0 && index + length - 1 < source.Length) &
         (() => source.Drop(index).Keep(length));
   }

   public static Maybe<string> Sub(this string source, int index)
   {
      return maybe<string>() & (source.IsNotEmpty() && index >= 0 && index < source.Length) & (() => source.Drop(index));
   }

   public static bool IsDate(this string date) => DateTime.TryParse(date, out _);

   public static string FromBase64(this string source, Encoding encoding)
   {
      return encoding.GetString(Convert.FromBase64String(source));
   }

   public static byte[] FromBase64(this string source) => Convert.FromBase64String(source);

   public static string Head(this string source) => source.Keep(1);

   public static string Tail(this string source) => source.Drop(1);

   public static string Foot(this string source) => source.Keep(-1);

   public static string Front(this string source) => source.Drop(-1);

   public static IEnumerable<string> Enumerable(this string source) => source.Select(ch => ch.ToString());

   public static StringSegment Balanced(this string source, char left)
   {
      if (source.IsNotEmpty())
      {
         var delimitedText = DelimitedText.BothQuotes();

         var leftOfPairIndex = PAIRS.IndexOf(left);
         if (leftOfPairIndex != -1 && leftOfPairIndex.IsEven())
         {
            var right = PAIRS[leftOfPairIndex + 1];
            var parsed = delimitedText.Destringify(source, true);
            var count = 0;
            var escaped = false;
            var type = StageType.LeftNotFound;
            var index = -1;

            for (var i = 0; i < parsed.Length; i++)
            {
               var ch = parsed[i];
               switch (type)
               {
                  case StageType.LeftNotFound:
                     if (ch == left)
                     {
                        count++;
                        if (!escaped)
                        {
                           type = StageType.LeftFound;
                           index = i;
                        }
                     }
                     else if (ch == '/')
                     {
                        escaped = true;
                        continue;
                     }

                     break;
                  case StageType.LeftFound:
                     if (ch == left)
                     {
                        count++;
                     }
                     else if (ch == right)
                     {
                        count--;
                        if (count == 0)
                        {
                           var segment = parsed.Slice(index, i);
                           segment = delimitedText.Restringify(segment, RestringifyQuotes.None);
                           return new StringSegment(segment, index, i);
                        }
                     }

                     break;
               }

               escaped = false;
            }

            return new StringSegment();
         }
         else
         {
            return new StringSegment();
         }
      }
      else
      {
         return new StringSegment();
      }
   }

   public static string Drop(this string source, int count)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      switch (count)
      {
         case > 0:
            count = count.MinOf(source.Length);
            return source.Substring(count);
         case 0:
            return source;
         default:
            count = (-count).MinOf(source.Length);
            return source.Substring(0, source.Length - count);
      }
   }

   public static string Drop(this string source, Pattern pattern, int group = 0)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var _result = source.Matches(pattern);
      if (_result is (true, var result))
      {
         var (_, index, length) = result.GetGroup(0, group);
         var count = index + length;

         return source.Drop(count);
      }
      else
      {
         return source;
      }
   }

   public static string DropWhile(this string source, Predicate<string> predicate)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      for (var i = 0; i < source.Length; i++)
      {
         var ch = source.Substring(i, 1);
         if (!predicate(ch))
         {
            return source.Slice(i, source.Length - 1);
         }
      }

      return source;
   }

   public static string DropWhile(this string source, string searchString, StringComparison comparisonType = StringComparison.CurrentCulture)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var index = source.LastIndexOf(searchString, comparisonType);
      return index > -1 ? Drop(source, index + searchString.Length) : source;
   }

   public static string DropWhile(this string source, params char[] chars)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      for (var i = 0; i < source.Length; i++)
      {
         if (!chars.Contains(source[i]))
         {
            return source.Drop(i);
         }
      }

      return source;
   }

   public static string DropUntil(this string source, Predicate<string> predicate)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      for (var i = 0; i < source.Length; i++)
      {
         var ch = source.Substring(i, 1);
         if (predicate(ch))
         {
            return source.Slice(i, source.Length - 1);
         }
      }

      return source;
   }

   public static string DropUntil(this string source, string searchString, StringComparison comparisonType = StringComparison.CurrentCulture)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var index = source.IndexOf(searchString, comparisonType);
      return index > -1 ? Drop(source, index) : "";
   }

   public static string DropUntil(this string source, params char[] chars)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      for (var i = 0; i < source.Length; i++)
      {
         if (chars.Contains(source[i]))
         {
            return source.Drop(i);
         }
      }

      return source;
   }

   public static string Drop(this string source, CharacterType type)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      for (var i = 0; i < source.Length; i++)
      {
         if (!isMatch(source[i], type))
         {
            return source.Drop(i);
         }
      }

      return source;
   }

   public static string Keep(this string source, int count)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      switch (count)
      {
         case > 0:
            count = count.MinOf(source.Length);
            return source.Substring(0, count);
         case 0:
            return "";
         default:
            count = (-count).MinOf(source.Length);
            return source.Substring(source.Length - count);
      }
   }

   public static string Keep(this string source, Pattern pattern, int group = 0)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var _result = source.Matches(pattern);
      if (_result is (true, var result))
      {
         var (_, index, length) = result.GetGroup(0, group);
         var count = index + length;

         return source.Keep(count);
      }
      else
      {
         return source;
      }
   }

   public static string KeepWhile(this string source, Predicate<string> predicate)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      for (var i = 0; i < source.Length; i++)
      {
         var ch = source.Substring(i, 1);
         if (!predicate(ch))
         {
            return source.Slice(0, i - 1);
         }
      }

      return source;
   }

   public static string KeepWhile(this string source, string searchString, StringComparison comparisonType = StringComparison.CurrentCulture)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var index = source.LastIndexOf(searchString, comparisonType);
      return index > -1 ? Keep(source, index + searchString.Length) : "";
   }

   public static string KeepWhile(this string source, params char[] chars)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      for (var i = 0; i < source.Length; i++)
      {
         if (!chars.Contains(source[i]))
         {
            return source.Keep(i);
         }
      }

      return source;
   }

   public static string KeepUntil(this string source, Predicate<string> predicate)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      for (var i = 0; i < source.Length; i++)
      {
         var ch = source.Substring(i, 1);
         if (predicate(ch))
         {
            return source.Slice(0, i - 1);
         }
      }

      return source;
   }

   public static string KeepUntil(this string source, string searchString, StringComparison comparisonType = StringComparison.CurrentCulture)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var index = source.IndexOf(searchString, comparisonType);
      return index > -1 ? Keep(source, index) : source;
   }

   public static string KeepUntil(this string source, params char[] chars)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      for (var i = 0; i < source.Length; i++)
      {
         if (chars.Contains(source[i]))
         {
            return source.Keep(i);
         }
      }

      return source;
   }

   private static bool isMatch(char character, CharacterType characterType) => characterType switch
   {
      CharacterType.Letter when char.IsLetter(character) => true,
      CharacterType.UpperCaseLetter when char.IsUpper(character) => true,
      CharacterType.LowerCaseLetter when char.IsLower(character) => true,
      CharacterType.Numeric when char.IsDigit(character) => true,
      CharacterType.AlphaNumeric when char.IsLetterOrDigit(character) => true,
      CharacterType.Whitespace when char.IsWhiteSpace(character) => true,
      CharacterType.Punctuation when char.IsPunctuation(character) => true,
      _ => false
   };

   public static string Keep(this string source, CharacterType type)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      for (var i = 0; i < source.Length; i++)
      {
         if (!isMatch(source[i], type))
         {
            return source.Keep(i);
         }
      }

      return source;
   }

   public static string DropKeep(this string source, Slice slice) => source.Drop(slice.Index).Keep(slice.Length);

   public static string Map(this string source, string replacement) => source.IsNotEmpty() ? replacement : "";

   public static string If(this string source, Predicate<string> predicate)
   {
      var builder = new StringBuilder();
      for (var i = 0; i < source.Length; i++)
      {
         var ch = source.Substring(i);
         if (predicate(ch))
         {
            builder.Append(ch);
         }
      }

      return builder.ToString();
   }

   public static string Unless(this string source, Predicate<string> predicate)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var builder = new StringBuilder();
      for (var i = 0; i < source.Length; i++)
      {
         var ch = source.Substring(i);
         if (!predicate(ch))
         {
            builder.Append(ch);
         }
      }

      return builder.ToString();
   }

   public static Maybe<int> FromHex(this string source)
   {
      Maybe<int> matches()
      {
         return source.Matches("^ ('0x')? /(['0-9a-fA-F']+) $; f").Map(m => int.Parse(m.FirstGroup, NumberStyles.HexNumber));
      }

      return maybe<int>() & source.IsNotEmpty() & matches;
   }

   public static Maybe<string> GetSignature(this string parameterName)
   {
      return maybe<string>() & parameterName.IsNotEmpty() &
         (() => parameterName.Matches("^ '@' /(.*) $; f").Map(m => m.FirstGroup.SnakeToCamelCase(true)));
   }

   public static IEnumerable<Slice> SlicesOf(this string source, string value, StringComparison comparison = StringComparison.CurrentCulture)
   {
      if (source.IsEmpty())
      {
         yield break;
      }

      var index = 0;
      List<int> list = [0];

      while (index > -1)
      {
         index = source.IndexOf(value, index, comparison);
         if (index > -1)
         {
            list.Add(index);
            index += value.Length + 1;
         }
      }

      for (var i = 0; i < list.Count; i++)
      {
         var start = list[i];
         var length = i + 1 < list.Count ? list[i + 1] - start : source.Length - start;
         var text = source.Drop(start).Keep(length);

         yield return new Slice(text, start, length);
      }
   }

   public static string Obscure(this string source, char character) => character.Repeat(source.Length);

   public static string Obscure(this string source, string characters, bool random = false)
   {
      if (source.IsNotEmpty())
      {
         var length = characters.Length;
         var str = stream();

         if (random)
         {
            var rand = new Random(NowServer.Now.Millisecond);
            for (var i = 0; i < source.Length; i++)
            {
               str /= characters[rand.Next(length)];
            }
         }
         else
         {
            var index = 0;
            for (var i = 0; i < source.Length; i++)
            {
               str /= characters[index++];
               index %= length;
            }
         }

         return str;
      }
      else
      {
         return "";
      }
   }

   public static string Replace(this string source, params (string, string)[] replacements)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var result = source;
      foreach (var replacement in replacements)
      {
         var (oldString, newString) = replacement;
         result = result.Replace(oldString, newString);
      }

      return result;
   }

   public static string ReplaceAll(this string source, params (string, string)[] replacements)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var builder = new StringBuilder(source);
      foreach (var (find, replace) in replacements)
      {
         builder.Replace(find, replace);
      }

      return builder.ToString();
   }

   public static Result<long> ByteSize(this string source)
   {
      if (source.IsEmpty())
      {
         return fail("Source is empty");
      }
      else
      {
         var _suffixed = source.Matches("^ /(/d+) /['kmg']? $; f");
         if (_suffixed is (true, var (valueSource, suffix)))
         {
            var _value = valueSource.Maybe().Int64();
            if (_value is (true, var value))
            {
               if (suffix.IsEmpty())
               {
                  return value;
               }

               value *= 1028;
               if (suffix == "k")
               {
                  return value;
               }

               value *= 1028;
               return suffix == "m" ? value : value * 1028;
            }
            else
            {
               return fail($"{valueSource} can't be converted to a long");
            }
         }
         else
         {
            return fail("Badly formatted source");
         }
      }
   }

   public static Maybe<long> AsByteSize(this string source) => source.ByteSize().Map(l => l.Some()).Recover(_ => nil);

   public static long ToByteSize(this string source, long defaultValue = 0)
   {
      return source.AsByteSize() | defaultValue;
   }

   public static string Partition(this string source, int allowedLength, string splitPattern = @"-(< '\')','; f", int padding = 1)
   {
      if (source.IsEmpty())
      {
         return "";
      }

      var array = source.Unjoin(splitPattern);
      var result = array.Select(e => $"{e}{" ".Repeat(padding)}").ToString("").Trim();

      return result.Center(allowedLength).Elliptical(allowedLength, ' ');
   }

   public static Maybe<int> Find(this string source, string substring, int startIndex = 0, bool ignoreCase = false)
   {
      if (source.IsNotEmpty() && substring.IsNotEmpty())
      {
         var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
         var index = source.IndexOf(substring, startIndex, comparison);

         return maybe<int>() & index != -1 & (() => index);
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<int> FindBackward(this string source, string substring, int startIndex = -1, bool ignoreCase = false)
   {
      if (source.IsNotEmpty() && substring.IsNotEmpty())
      {
         if (startIndex == -1)
         {
            startIndex = source.Length - 1;
         }

         var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
         var index = source.LastIndexOf(substring, startIndex, comparison);

         return maybe<int>() & index != -1 & (() => index);
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<Slice> FindByRegex(this string source, Pattern pattern)
   {
      var _result = source.Matches(pattern);
      if (_result is (true, var result))
      {
         var (text, index, length) = result.GetMatch(0);
         return new Slice(text, index, length);
      }
      else
      {
         return nil;
      }
   }

   public static IEnumerable<int> FindAll(this string source, string substring, bool ignoreCase = false)
   {
      var _index = source.Find(substring, 0, ignoreCase);
      while (true)
      {
         if (_index)
         {
            yield return _index;
         }
         else
         {
            break;
         }

         _index = source.Find(substring, _index + substring.Length, ignoreCase);
      }
   }

   public static IEnumerable<Slice> FindAllByRegex(this string source, Pattern pattern)
   {
      var _matches = source.Matches(pattern);
      if (_matches is (true, var matches))
      {
         foreach (var (text, index, length) in matches)
         {
            yield return new Slice(text, index, length);
         }
      }
   }

   public static string ToCamel(this string source)
   {
      var pascal = source.ToPascal();
      return pascal.Keep(1).ToLower() + pascal.Drop(1);
   }

   public static string ToPascal(this string source)
   {
      static string allPascalCase(string word) => word.Keep(1).ToUpper() + word.Drop(1).ToLower();

      static IEnumerable<string> split(string whole)
      {
         if (whole.IsMatch("^ ['A-Z']+ $; f"))
         {
            whole = allPascalCase(whole);
         }
         else if (whole.IsMatch("-['A-Za-z0-9 ']; f"))
         {
            whole = whole.Unjoin("-['A-Za-z0-9 ']+; f").Select(s => s.ToPascal()).ToString("");
         }

         var part = new StringBuilder();
         foreach (var ch in whole)
         {
            if (ch == '_')
            {
               if (part.Length > 0)
               {
                  yield return allPascalCase(part.ToString());
               }

               part.Clear();
            }
            else if (char.IsUpper(ch) || char.IsDigit(ch))
            {
               if (part.Length > 0)
               {
                  yield return part.ToString();
               }

               part.Clear();
               part.Append(ch);
            }
            else if (part.Length == 0)
            {
               part.Append(char.ToUpper(ch));
            }
            else
            {
               part.Append(char.ToLower(ch));
            }
         }

         if (part.Length > 0)
         {
            yield return part.ToString();
         }
      }

      if (source.IsEmpty())
      {
         return "";
      }
      else if (source.IsMatch("^ ['0-9']+ $; f"))
      {
         return source;
      }

      return split(source.Substitute("/s+; f", "_")).ToString("");
   }

   [Obsolete("Use EqualTo")]
   public static Maybe<bool> IsExactlyEqualTo(this string left, string right)
   {
      if (left == right)
      {
         return true;
      }
      else if (left.Same(right))
      {
         return false;
      }
      else
      {
         return nil;
      }
   }

   public static EqualTo EqualTo(this string left, string right)
   {
      if (left == right)
      {
         return Strings.EqualTo.ExactlyEqual;
      }
      else if (left.Same(right))
      {
         return Strings.EqualTo.Same;
      }
      else
      {
         return Strings.EqualTo.NotEqual;
      }
   }

   public static string Exactly(this string source, int length, bool addEllipsis = true, bool leftJustify = true, bool normalizeWhitespace = true)
   {
      if (source.IsEmpty())
      {
         return " ".Repeat(length);
      }
      else
      {
         var normalized = normalizeWhitespace ? source.NormalizeWhitespace() : source;
         var sourceLength = normalized.Length;
         if (sourceLength <= length)
         {
            return leftJustify ? normalized.LeftJustify(length) : normalized.RightJustify(length);
         }
         else
         {
            var keepCount = length - (addEllipsis ? 3 : 0);
            var keep = normalized.Keep(keepCount);

            var ellipsis = addEllipsis ? "..." : "";

            return $"{keep}{ellipsis}";
         }
      }
   }

   public static string IndentedLines(this string source, int indentation = 3)
   {
      var indent = " ".Repeat(indentation);
      return source.Lines().Select(line => $"{indent}{line}").ToString("\r\n");
   }

   public static IEnumerable<string> IndentedLines(this IEnumerable<string> lines, int indentation = 3)
   {
      var indent = " ".Repeat(indentation);
      foreach (var line in lines)
      {
         yield return $"{indent}{line}";
      }
   }

   public static IEnumerable<string> Words(this string source)
   {
      var builder = new StringBuilder();
      var anyLower = false;

      foreach (var ch in source)
      {
         if (char.IsUpper(ch))
         {
            if (anyLower && builder.Length > 0)
            {
               yield return builder.ToString();

               builder.Clear();
            }

            builder.Append(ch);
            anyLower = false;
         }
         else if (char.IsLower(ch) || char.IsDigit(ch))
         {
            anyLower = true;
            builder.Append(ch);
         }
         else if (ch == '_' || char.IsPunctuation(ch) || char.IsWhiteSpace(ch))
         {
            if (builder.Length > 0)
            {
               yield return builder.ToString();

               builder.Clear();
            }

            anyLower = false;
         }
      }

      if (builder.Length > 0)
      {
         yield return builder.ToString();
      }
   }

   private static StringComparison getStringComparison(bool ignoreCase) => ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

   public static Maybe<(string result, string remainder)> PrefixOf(this string haystack, string needle, bool ignoreCase = false, bool trim = false)
   {
      var stringComparison = getStringComparison(ignoreCase);
      if (haystack.StartsWith(needle, stringComparison))
      {
         var result = haystack.Keep(needle.Length);
         var remainder = haystack.Drop(needle.Length);

         if (trim)
         {
            remainder = remainder.Trim();
         }

         return (result, remainder);
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<(string remainder, string result)> SuffixOf(this string haystack, string needle, bool ignoreCase = false, bool trim = false)
   {
      var stringComparison = getStringComparison(ignoreCase);
      if (haystack.EndsWith(needle, stringComparison))
      {
         var result = haystack.Keep(-needle.Length);
         var remainder = haystack.Drop(-needle.Length);

         if (trim)
         {
            remainder = remainder.Trim();
         }

         return (remainder, result);
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<(string left, string result, string right)> InfixOf(this string haystack, string needle, bool ignoreCase = false,
      bool trimLeft = false, bool trimRight = false)
   {
      if (haystack.Find(needle, ignoreCase: ignoreCase) is (true, var index))
      {
         var left = haystack.Keep(index);
         var result = haystack.Drop(index).Keep(needle.Length);
         var right = haystack.Drop(index + needle.Length);

         if (trimLeft)
         {
            left = left.Trim();
         }

         if (trimRight)
         {
            right = right.Trim();
         }

         return (left, result, right);
      }
      else
      {
         return nil;
      }
   }

   public static bool IsBase64(this string input)
   {
      if (input.IsEmpty() || input.Length % 4 != 0)
      {
         return false;
      }
      else if (!input.IsMatch("^ ['a-zA-Z0-9+//']* '='0%2 $; f"))
      {
         return false;
      }
      else
      {
         try
         {
            input.FromBase64();
            return true;
         }
         catch (FormatException)
         {
            return false;
         }
      }
   }

   public static StringAppender Prefix(this string source, string prefix) => new StringAppender(source).Prefix(prefix);

   public static StringAppender Suffix(this string source, string suffix) => new StringAppender(source).Suffix(suffix);

   public static StringAppender Map(this string source, Func<string, string> mappingFunc) => new StringAppender(source).Map(mappingFunc);

   public static StringAppender Replace(this string source, string replacement) => new StringAppender(source).Replace(replacement);
}