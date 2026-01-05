namespace Core.Markup.Quirk.Parser;

public static class RegexFunctions
{
   public const string REGEX_BREAK = @"(\r\n|\r|\n){2}|(?<!\\)(;\r\n|\r|\n)|$";
   public const string REGEX_TAG = "[A-Za-z][A-Za-z0-9_-]*";
}