using System;
using System.Collections.Generic;
using System.Text;
using Core.Assertions;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.ConsoleProcessing;

public class CommandLineParser(string commandLine, string parameterPrefix, string shortcutPrefix, StringHash<Command> commands)
{
   protected const string COMMAND_PATTERN = "^ /([/w '-']+) /b /(.+) $; f";

   public Optional<CommandValue> Parse()
   {
      try
      {
         var _result = parseCommandValue(commandLine);
         if (_result is (true, var (commandName, remainder)) && commands.Maybe[commandName] is (true, var command))
         {
            List<ParameterValue> parameterValues = [];

            while (parseParameterValue(remainder, command) is (true, var (parameterName, value, newRemainder)))
            {
               var parameterValue = ParameterValue.FromValue(parameterName, value);
               parameterValues.Add(parameterValue);
               remainder = newRemainder;
            }

            return new CommandValue(commandName, parameterValues);
         }
         else if (_result.Exception is (true, var exception))
         {
            return exception;
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

   protected Optional<(string commandName, string remainder)> parseCommandValue(string input)
   {
      try
      {
         if (input.Matches(COMMAND_PATTERN).Map(r => (r.FirstGroup, r.SecondGroup)) is (true, var (commandName, remainder)))
         {
            return (commandName, remainder.Trim());
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

   protected Optional<(string helpTopic, string remainder)> parseHelpValue(string input)
   {
      try
      {
         if (input.PrefixOf("help", trim: true) is (true, var (_, remainder)))
         {
         }

         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Optional<(string parameterName, string value, string remainder)> parseParameterValue(string input, Command command)
   {
      try
      {
         if (input.StartsWith(parameterPrefix) && input.Length > parameterPrefix.Length + 1)
         {
            input = input.Drop(parameterPrefix.Length);
            return
               from tuple1 in parseCommandValue(input)
               from tuple2 in parseValue(tuple1.remainder)
               from _ in command.GetHash().Must().HaveKeyOf(tuple1.commandName).OrNone()
               select (tuple1.commandName, tuple2.value, tuple2.remainder);
         }
         else if (input.StartsWith(shortcutPrefix) && input.Length >= shortcutPrefix.Length + 1)
         {
            input = input.Drop(shortcutPrefix.Length);
            var shortcut = input[0];
            var remainder = input.Drop(1);
            return
               from parameterShortCut in command.MatchesShortcut(shortcut)
               from tuple1 in parseValue(remainder)
               select (parameterShortCut.Name, tuple1.value, tuple1.remainder);
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

   protected Optional<(string value, string remainder)> parseValue(string input)
   {
      try
      {
         if (input.IsEmpty())
         {
            return ("true", "");
         }
         else if (input.Matches("^ /('true' | 'false') /b /(.*) $; f") is (true, var result))
         {
            var (value, remainder) = result;
            return (value, remainder.Trim());
         }
         else if (input.Trim().StartsWith(parameterPrefix) || input.Trim().StartsWith(shortcutPrefix))
         {
            return ("true", input.Trim());
         }
         else
         {
            var firstChar = input[0];
            return firstChar switch
            {
               '"' => parseDoubleQuotedString(input.Drop(1)),
               '\'' => parseSingleQuotedString(input.Drop(1)),
               _ => parseNonSpaceString(input)
            };
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Optional<(string value, string remainder)> parseDoubleQuotedString(string input)
   {
      try
      {
         var escaped = false;
         var builder = new StringBuilder();
         for (var i = 0; i < input.Length; i++)
         {
            var ch = input[i];
            switch (ch)
            {
               case '\\' when escaped:
                  builder.Append("\\");
                  escaped = false;
                  break;
               case '\\':
                  escaped = true;
                  break;
               case '"' when escaped:
                  builder.Append("\"");
                  escaped = false;
                  break;
               case '"':
               {
                  var value = builder.ToString();
                  var remainder = input.Drop(i);
                  return (value, remainder.Trim());
               }
               default:
                  builder.Append(ch);
                  break;
            }
         }

         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Optional<(string value, string remainder)> parseSingleQuotedString(string input)
   {
      try
      {
         var escaped = false;
         var builder = new StringBuilder();
         for (var i = 0; i < input.Length; i++)
         {
            var ch = input[i];
            switch (ch)
            {
               case '\\' when escaped:
                  builder.Append("\\");
                  escaped = false;
                  break;
               case '\\':
                  escaped = true;
                  break;
               case '\'' when escaped:
                  builder.Append("'");
                  escaped = false;
                  break;
               case '\'':
               {
                  var value = builder.ToString();
                  var remainder = input.Drop(i);
                  return (value, remainder.Trim());
               }
               default:
                  builder.Append(ch);
                  break;
            }
         }

         return nil;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Optional<(string value, string remainder)> parseNonSpaceString(string input)
   {
      try
      {
         var builder = new StringBuilder();
         for (var i = 0; i < input.Length; i++)
         {
            var ch = input[i];
            if (char.IsWhiteSpace(ch))
            {
               var value = builder.ToString();
               var remainder = input.Drop(i);
               return (value, remainder.Trim());
            }
            else
            {
               builder.Append(ch);
            }
         }

         return (builder.ToString(), "");
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}