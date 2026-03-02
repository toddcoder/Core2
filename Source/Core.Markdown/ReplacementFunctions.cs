namespace Core.Markdown;

public static class ReplacementFunctions
{
   private const string REGEX_USE_STYLE = "'[{' /(-['}']+) '}'/(-[']']+)']'; f";
   private const string REGEX_REPLACEMENT_SPECIFIERS = "/'::' /(-[':']+) /'::'; f";
   private const string REGEX_MULTIPLE_REPLACEMENT_SPECIFIERS_BEGIN = "/'(:' /(-[':']+) /':)'; f";
   private const string REGEX_MULTIPLE_REPLACEMENT_SPECIFIERS_END = "'(' ':'+ ')'; f";

   public static ReplacementResult forLine(string line, IMarkdownFrameOptions options)
   {
      return new ReplacementResult.LineOnly("");
   }
}