using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Markup.Quirk.Parser.RegexFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Quirk.Parser;

public partial class TagParser : BaseParser
{
   [GeneratedRegex(@$"^(\s*)({REGEX_TAG}) ")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens)
   {
      return unit;
   }
}