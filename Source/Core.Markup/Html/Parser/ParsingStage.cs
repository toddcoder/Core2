namespace Core.Markup.Html.Parser;

public enum ParsingStage
{
   Starting,
   Style,
   StyleName,
   StyleKey,
   StyleValue,
   Body,
   Tag,
   Attribute,
   AttributeValue,
   Text,
   EndTag
}