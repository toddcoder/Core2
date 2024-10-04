namespace Core.Markup.Html.Parser;

public enum ParsingStage
{
   Name,
   Style,
   StyleName,
   StyleKey,
   StyleValue,
   Body,
   Tag,
   Waiting,
   Attribute,
   AttributeValue,
   Text,
   EndTag
}