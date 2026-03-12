using Core.Matching;
using Core.Strings;

namespace Core.Markdown;

public static class MarkdownExtensions
{
   private const string REGEX_OPERATOR = @"([\\`*_{}\[\]()#+\-.!>|]); u";

   extension(string text)
   {
      public string EscapeMarkdown()
      {
         if (text.IsEmpty())
         {
            return text;
         }

         return text.Substitute(REGEX_OPERATOR, "$1");
      }
   }
}