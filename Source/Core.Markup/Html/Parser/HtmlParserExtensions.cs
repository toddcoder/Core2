namespace Core.Markup.Html.Parser;

public static class HtmlParserExtensions
{
   public static string Slash(this string source) =>
      source.Replace("/", "//").Replace("[", "/[").Replace("]", "/]").Replace(">", "/>").Replace("%", "/%");
}