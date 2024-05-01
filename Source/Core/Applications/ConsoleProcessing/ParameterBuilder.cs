using System;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.ConsoleProcessing;

public class ParameterBuilder(string name, CommandBuilder commandBuilder)
{
   public static ParameterBuilder operator +(ParameterBuilder builder, Type type)
   {
      builder._type = type;
      return builder;
   }

   public static ParameterBuilder operator +(ParameterBuilder builder, string help)
   {
      builder._help = help;
      return builder;
   }

   public static ParameterBuilder operator +(ParameterBuilder builder, char shortcut)
   {
      builder._shortcut = shortcut;
      return builder;
   }

   public static ParameterBuilder operator +(ParameterBuilder builder, bool optional)
   {
      builder.optional = optional;
      return builder;
   }

   public static Parameter operator +(ParameterBuilder builder, ParameterTerminal _) => builder.Parameter();

   public string Name => name;

   protected Maybe<Type> _type = nil;

   protected Maybe<string> _help = nil;

   protected Maybe<char> _shortcut = nil;

   protected bool optional;

   public CommandBuilder CommandBuilder => commandBuilder;

   public Parameter Parameter()
   {
      var type = _type | typeof(string);
      var help = _help | name;

      var parameter = new Parameter(name, type, help, _shortcut, optional);
      commandBuilder.Parameters[name] = parameter;

      return parameter;
   }
}