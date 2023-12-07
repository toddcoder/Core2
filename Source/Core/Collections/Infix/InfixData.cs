using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections.Infix;

public class InfixData<TValue, TInfix> : IInfixData<TValue, TInfix> where TInfix : notnull
{
   public InfixData(TValue value, Maybe<TInfix> _infix)
   {
      Value = value;
      Infix = _infix;
   }

   public InfixData(TValue value, TInfix infix) : this(value, infix.Some())
   {
   }

   public InfixData(TValue value) : this(value, nil)
   {
   }

   public TValue Value { get; }

   public Maybe<TInfix> Infix { get; }

   public override string ToString() => Value + (Infix.Map(i => " " + i) | "");

   public void Deconstruct(out TValue value, out Maybe<TInfix> _infix)
   {
      value = Value;
      _infix = Infix;
   }
}