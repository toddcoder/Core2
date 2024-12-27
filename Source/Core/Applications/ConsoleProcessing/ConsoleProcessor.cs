using System;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.ConsoleProcessing;

public class ConsoleProcessor(string parameterPrefix = "--", string shortcutPrefix = "-") : IHash<string, Command>
{
   public static CommandBuilder operator +(ConsoleProcessor consoleProcessor, string commandName) => new(commandName, consoleProcessor);

   protected StringHash<Command> commands = [];

   public Command this[string key]
   {
      get => commands[key];
      set => commands[key] = value;
   }

   public bool ContainsKey(string key) => commands.ContainsKey(key);

   public Hash<string, Command> GetHash() => commands;

   public HashInterfaceMaybe<string, Command> Items => commands.Items;

   public Optional<Unit> Execute(string commandline)
   {
      try
      {
         commands["help"] = generateHelpCommand();

         var parser = new CommandLineParser(commandline, parameterPrefix, shortcutPrefix, commands);
         var _result = parser.Parse();
         if (_result is (true, var commandValue))
         {
            if (commands.Maybe[commandValue.CommandName] is (true, var command))
            {
               return command.Function(commandValue);
            }
            else
            {
               return fail($"Didn't understand command {commandValue.CommandName}");
            }
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

   protected Command generateHelpCommand()
   {
      var helpCommand = this + "help" + help;

      foreach (var (_, command) in commands)
      {
         var _commandHelp = command.Help;
         if (_commandHelp is (true, var commandHelp))
         {
            _ = helpCommand + command.CommandName + typeof(string) + true;
         }
      }

      return helpCommand.Command();
   }

   protected Optional<Unit> help(CommandValue commandValue)
   {
      try
      {
         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}