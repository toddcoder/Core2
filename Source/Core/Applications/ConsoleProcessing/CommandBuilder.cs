using System;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.ConsoleProcessing;

public class CommandBuilder(string commandName, ConsoleProcessor consoleProcessor)
{
   public static CommandBuilder operator +(CommandBuilder builder, Func<CommandValue, Optional<Unit>> function)
   {
      builder.Function = function;
      return builder;
   }

   public static Command operator +(CommandBuilder builder, CommandTerminal _) => builder.Command();

   public static ParameterBuilder operator +(CommandBuilder builder, string parameterName)
   {
      return new ParameterBuilder(parameterName, builder);
   }

   public string CommandName => commandName;

   public Maybe<Func<CommandValue, Optional<Unit>>> Function { get; set; } = nil;

   public StringHash<Parameter> Parameters { get; init; } = [];

   public Maybe<string> Help { get; set; } = nil;

   public Command Command()
   {
      var action = Function | (_ => unit);
      var command = new Command(commandName, action, Help);
      foreach (var (key, parameter) in Parameters)
      {
         command[key] = parameter;
      }

      consoleProcessor[commandName] = command;

      return command;
   }
}