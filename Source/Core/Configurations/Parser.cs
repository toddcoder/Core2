﻿using System.Collections.Generic;
using System.Text;
using Core.DataStructures;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Configurations;

internal class Parser
{
   protected const string REGEX_KEY = "/(['$@']? [/w '?'] [/w '-']*)";

   protected string source;

   internal Parser(string source)
   {
      this.source = source;
   }

   public Result<Setting> Parse()
   {
      var rootSetting = new Setting();
      MaybeStack<ConfigurationItem> stack = [];
      stack.Push(rootSetting);

      Maybe<Setting> peekSetting()
      {
         return
            from parentItem in stack.Peek()
            from parentGroup in parentItem.IfCast<Setting>()
            select parentGroup;
      }

      Maybe<Setting> popSetting()
      {
         return
            from parentItem in stack.Pop()
            from parentGroup in parentItem.IfCast<Setting>()
            select parentGroup;
      }

      Result<(string, string, bool)> getLinesAsArray(string source)
      {
         List<string> lines = [];
         while (source.Length > 0)
         {
            LazyMaybe<int> _length = nil;
            LazyMaybe<MatchResult> _result = nil;

            if (_length.ValueOf(source.Matches("^ /s* '}' ([/r /n]+ | $); f").Map(r => r.Length)))
            {
               return (source.Drop(_length), lines.ToString(","), true);
            }
            else if (_result.ValueOf(source.Matches("^ /s* /(-[/r /n]*) ('/r/n')?; f")) is (true, var result))
            {
               var _stringInfo = getString(result.FirstGroup);
               if (_stringInfo is (true, var (_, @string, _)))
               {
                  source = source.Drop(result.Length);
                  lines.Add(@string);
               }
               else
               {
                  return _stringInfo.Exception;
               }
            }
         }

         return fail("No terminating }");
      }

      Result<(string newSource, string str, bool isArray)> getString(string source)
      {
         LazyMaybe<(string, int)> _quote = nil;
         LazyMaybe<MatchResult> _openBrace = nil;
         LazyMaybe<MatchResult> _endOfLine = nil;

         if (_quote.ValueOf(source.Matches("^ /s* /[quote]; f").Map(result => result.FirstGroupAndLength)) is (true, var (group, length)))
         {
            var quote = group[0];
            return getQuotedString(source.Drop(length), quote);
         }
         else if (_openBrace.ValueOf(source.Matches("^ /s* '{' [/r /n]+; f")) is (true, var openBrace))
         {
            var newSource = source.Drop(openBrace.Length);
            return getLinesAsArray(newSource);
         }
         else if (_endOfLine.ValueOf(source.Matches("^ /s* /(-[/r /n]*) ('/r/n')?; f")))
         {
            var foundReturn = false;
            var builder = new StringBuilder();
            for (var i = 0; i < source.Length; i++)
            {
               var current = source[i];
               switch (current)
               {
                  case ';':
                     return (source.Drop(i + 1), builder.ToString(), false);
                  case ']' or '#':
                     return (source.Drop(i), builder.ToString(), false);
                  case '\r' or '\n':
                     foundReturn = true;
                     break;
                  default:
                     if (foundReturn)
                     {
                        return (source.Drop(i - 1), builder.ToString(), false);
                     }
                     else
                     {
                        builder.Append(current);
                     }

                     break;
               }
            }

            return (string.Empty, builder.ToString(), false);
         }
         else
         {
            return fail("Couldn't determine string");
         }
      }

      static Result<(string newSource, string str, bool isArray)> getQuotedString(string source, char quote)
      {
         var escaped = false;
         var builder = new StringBuilder();

         for (var i = 0; i < source.Length; i++)
         {
            var current = source[i];
            switch (current)
            {
               case '`':
                  if (escaped)
                  {
                     builder.Append("`");
                     escaped = false;
                  }
                  else
                  {
                     escaped = true;
                  }

                  break;
               case 't':
                  if (escaped)
                  {
                     builder.Append("\t");
                     escaped = false;
                  }
                  else
                  {
                     builder.Append('t');
                  }

                  break;
               case 'r':
                  if (escaped)
                  {
                     builder.Append("\r");
                     escaped = false;
                  }
                  else
                  {
                     builder.Append('r');
                  }

                  break;
               case 'n':
                  if (escaped)
                  {
                     builder.Append("\n");
                     escaped = false;
                  }
                  else
                  {
                     builder.Append('n');
                  }

                  break;
               default:
                  if (current == quote)
                  {
                     if (escaped)
                     {
                        builder.Append(current);
                        escaped = false;
                     }
                     else
                     {
                        var newSource = source.Drop(i + 1);
                        var str = builder.ToString();
                        return (newSource, str, false);
                     }
                  }
                  else
                  {
                     builder.Append(current);
                     escaped = false;
                  }

                  break;
            }
         }

         return fail("Open string");
      }

      while (source.Length > 0)
      {
         LazyMaybe<int> _openSetting = nil;
         LazyMaybe<(string, int)> _settingKey = nil;
         LazyMaybe<int> _closeSetting = nil;
         LazyMaybe<(string, int)> _oneLineKey = nil;
         LazyMaybe<int> _comment = nil;
         LazyMaybe<(string, int)> _key = nil;
         LazyResult<(string, string, bool)> _string = nil;

         if (_openSetting.ValueOf(source.Matches("^ /s* '['; f").Map(r => r.Length)) is (true, var openSetting))
         {
            var key = GetKey("?");
            var setting = new Setting(key);
            var _parentSetting = peekSetting();
            if (_parentSetting is (true, var parentSetting))
            {
               parentSetting.SetItem(key, setting);
            }
            else
            {
               return fail("No parent setting found");
            }

            stack.Push(setting);

            source = source.Drop(openSetting);
         }
         else if (_settingKey.ValueOf(source.Matches($"^ /s* {REGEX_KEY} /s* '['; f").Map(r => r.FirstGroupAndLength)) is
                  (true, var (settingKey, settingLength)))
         {
            var key = GetKey(settingKey);
            var setting = new Setting(key);
            var _parentSetting = peekSetting();
            if (_parentSetting is (true, var parentSetting))
            {
               parentSetting.SetItem(key, setting);
            }
            else
            {
               return fail("No parent setting found");
            }

            stack.Push(setting);

            source = source.Drop(settingLength);
         }
         else if (_closeSetting.ValueOf(source.Matches("^ /s* ']'; f").Map(r => r.Length)) is (true, var closeSetting))
         {
            var _setting = popSetting();
            if (_setting is (true, var setting))
            {
               var _parentSetting = peekSetting();
               if (_parentSetting is (true, var parentSetting))
               {
                  parentSetting.SetItem(setting.Key, setting);
               }
               else
               {
                  return fail("No parent setting found");
               }
            }
            else
            {
               return fail("Not closing on setting");
            }

            source = source.Drop(closeSetting);
         }
         else if (_oneLineKey.ValueOf(source.Matches($"^ /s* {REGEX_KEY} '.'; f").Map(r => r.FirstGroupAndLength)) is
                  (true, var (oneLineKey, oneLineLength)))
         {
            var key = GetKey(oneLineKey);
            var setting = new Setting(key);
            var _parentSetting = peekSetting();
            if (_parentSetting is (true, var parentSetting))
            {
               parentSetting.SetItem(key, setting);
            }
            else
            {
               return fail("No parent setting found");
            }

            source = source.Drop(oneLineLength);
         }
         else if (_comment.ValueOf(source.Matches("^ /s* '#' -[/r /n]*; f").Map(r => r.Length)) is (true, var commentLength))
         {
            var key = GenerateKey();
            var remainder = source.Drop(commentLength);
            var _stringTuple = getString(remainder);
            if (_stringTuple is (true, var (aSource, value, isArray)))
            {
               source = aSource;
               var item = new Item(key, value)
               {
                  IsArray = isArray
               };
               var _setting = peekSetting();
               if (_setting is (true, var setting))
               {
                  setting.SetItem(item.Key, item);
               }
            }
            else if (source.IsMatch("^ /s+ $; f"))
            {
               break;
            }
            else
            {
               return fail($"Didn't understand value {remainder}");
            }
         }
         else if (_key.ValueOf(source.Matches($"^ /s* {REGEX_KEY} ':' /s*; f").Map(r => r.FirstGroupAndLength)) is (true, var (key, length)))
         {
            key = GetKey(key);
            var remainder = source.Drop(length);
            var _tupleString = getString(remainder);
            if (_tupleString is (true, var (aSource, value, isArray)))
            {
               source = aSource;
               var item = new Item(key, value)
               {
                  IsArray = isArray
               };
               var _setting = peekSetting();
               if (_setting is (true, var setting))
               {
                  setting.SetItem(item.Key, item);
               }
            }
            else if (source.IsMatch("^ /s+ $; f"))
            {
               break;
            }
            else
            {
               return fail($"Didn't understand value {remainder}");
            }
         }
         else if (source.IsMatch("^ /s+ $; f"))
         {
            break;
         }
         else if (_string.ValueOf(getString(source.TrimLeft())) is (true, var (aSource, value, isArray)))
         {
            source = aSource;
            var stringKey = GenerateKey();
            var item = new Item(stringKey, value)
            {
               IsArray = isArray
            };
            var _setting = peekSetting();
            if (_setting is (true, var setting))
            {
               setting.SetItem(item.Key, item);
            }
         }
         else
         {
            return fail($"Didn't understand {source.KeepUntil("\r\n")}");
         }
      }

      while (popSetting() is (true, var setting))
      {
         if (popSetting() is (true, var parentSetting))
         {
            parentSetting.SetItem(setting.Key, setting);
         }
         else
         {
            break;
         }
      }

      return rootSetting;
   }

   public static string GenerateKey() => $"__$key_{uniqueID()}";

   public static string GetKey(string keySource) => keySource == "?" ? GenerateKey() : keySource;
}