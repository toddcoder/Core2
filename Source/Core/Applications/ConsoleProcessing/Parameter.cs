using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.ConsoleProcessing;

public class Parameter(string name, Type type, string help, Maybe<char> _shortcut, bool optional)
{
   public string Name => name;

   public Type Type => type;

   public string Help => help;

   public Maybe<char> Shortcut => _shortcut;

   public bool Optional => optional;

   public Maybe<ParameterValue> Matches(IEnumerable<ParameterValue> parameterValues) => parameterValues.FirstOrNone(p => p.ParameterName == name);

   public override string ToString() => $"{name}: {type}{(optional ? "?" : "")}{_shortcut.Map(s => $" ({s})") | ""}";
}