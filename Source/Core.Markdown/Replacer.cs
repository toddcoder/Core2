namespace Core.Markdown;

public abstract record Replacer
{
   public sealed record LineSpecifier(string Line) : Replacer;

   public sealed record StyleSpecifier(string Style) : Replacer;

   public sealed record MultiBegin(string Key) : Replacer;

   public sealed record Template(string Line) : Replacer;

   public sealed record MultiEnd : Replacer;

   public sealed record Inclusion(string Key) : Replacer;

   public sealed record InclusionEnd : Replacer;

   public sealed record RawMarkdown(string Markdown) : Replacer;
}