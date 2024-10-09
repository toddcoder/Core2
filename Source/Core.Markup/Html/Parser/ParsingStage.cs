namespace Core.Markup.Html.Parser;

public enum ParsingStage
{
   Name,
   Style,
   StyleName,
   StyleKey,
   Tag,
   Attribute,
   Text,
   Raw
}