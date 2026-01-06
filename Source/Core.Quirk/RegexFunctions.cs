namespace Core.Quirk;

public static class RegexFunctions
{
   public const string REGEX_TAG = "^ /(['a-zA-Z_@'] [/w '-']*) ' '; f";
   public const string REGEX_ATTRIBUTE = "^ /s* /(['a-zA-Z_'] [/w '-']*) '(' /(-[')']+) ')'; f";
   public const string REGEX_BOLD = @"-(< '\') /'*' /(.*?) -(< '\') /'*'; f";
   public const string REGEX_ITALIC = @"-(< '\') /'_' /(.*?) -(< '\') /'_'; f";
   public const string REGEX_RAW = @"-(< '\') /'``' /(.*?) -(< '\') /'``'; f";
}