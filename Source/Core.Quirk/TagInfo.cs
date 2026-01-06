using Core.Markup.Xml;
using Core.Matching;
using static Core.Quirk.RegexFunctions;

namespace Core.Quirk;

public record TagInfo(string Tag, string Text)
{
   public TagInfo EmptyTag() => new("", $"{Tag} {Text}");

   public Element GetElement()
   {
      var element = new Element
      {
         Name = Tag
      };

      string text;
      var startIndex = 0;
      if (Text.Matches(REGEX_ATTRIBUTE) is (true, var result))
      {
         foreach (var match in result)
         {
            element.Attributes.Add(match.SecondGroup, match.ThirdGroup);
            startIndex = match.Index + match.Length;
         }

         text = Text[startIndex..].Trim();
      }
      else
      {
         text = Text;
      }

      text = text.Substitute(REGEX_BOLD, "<strong>$1</strong>");
      text= text.Substitute(REGEX_ITALIC, "<em>$1</em>");
      text = text.Substitute(REGEX_RAW, "<code>$1</code>");
      element.Text = text;

      return element;
   }
}