using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public class Signature
{
   public const string REGEX_FORMAT = "^ /(/w+) ('[' /(/d*) ']')? $; f";

   public Signature(string signature)
   {
      var _openIndex = signature.Find("[");
      if (_openIndex)
      {
         Name = signature.Keep(_openIndex);
         Index = signature.Drop(_openIndex + 1).KeepUntil("]").Maybe().Int32();
      }
      else
      {
         Name = signature;
         Index = nil;
      }
   }

   public string Name { get; set; }

   public Maybe<int> Index { get; set; }

   public override string ToString() => Index.Map(i => $"{Name}[{i}]") | Name;
}