namespace Core.Markdown;

public abstract record ReplacementResult
{
   public sealed record LineOnly(string Line) : ReplacementResult;

   public sealed record LineAndStyles(string Line, IEnumerable<string> Styles) : ReplacementResult;

   public sealed record LinesOnly(IEnumerable<string> Lines) : ReplacementResult;

   public sealed record LinesAndStyles(IEnumerable<string> Lines, IEnumerable<string> Styles) : ReplacementResult;
}