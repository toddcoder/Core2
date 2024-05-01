using System;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.ConsoleProcessing;

public class Command(string commandName, Func<CommandValue, Optional<Unit>> function, Maybe<string> _help) : IHash<string, Parameter>
{
   public string CommandName => commandName;

   public Func<CommandValue, Optional<Unit>> Function => function;

   public Maybe<string> Help => _help;

   protected StringHash<Parameter> parameters = [];

   public Parameter this[string parameterName]
   {
      get => parameters[parameterName];
      set => parameters[parameterName] = value;
   }

   public bool ContainsKey(string key) => parameters.ContainsKey(key);

   public Hash<string, Parameter> GetHash() => parameters;

   public HashInterfaceMaybe<string, Parameter> Items => parameters.Items;

   public Optional<Parameter> MatchesShortcut(char shortcut)
   {
      foreach (var value in parameters.Values)
      {
         if (value.Shortcut is (true, var valueShortcut) && valueShortcut == shortcut)
         {
            return value;
         }
      }

      return nil;
   }
}