using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using SysColor = System.Drawing.Color;

namespace Core.Markup.Rtf.Markup.Parsers;

public class ColorDefinitionParser : LineParser
{
   public override Pattern Pattern => $"^ '//color' /s+ /({WORD}) /s* /([/w+] | {HEX} | '(' {HEX} /s* ',' /s* {HEX} /s* ',' /s* {HEX} ')' |) $; f";

   public override Optional<Line> Parse(string[] groups, ParsingState state)
   {
      var name = groups[0];
      var value = groups[1];
      ColorDescriptor colorDescriptor;

      if (value.StartsWith("0x"))
      {
         if (value.Drop(2).FromHex() is (true, var intValue))
         {
            colorDescriptor = state.Document.Color(intValue);
         }
         else
         {
            return nil;
         }
      }
      else if (getRgb(value) is (true, var (red, green, blue)))
      {
         colorDescriptor = state.Document.Color(red, green, blue);
      }
      else if (value.IsNotEmpty())
      {
         var _color = getColor(value);
         if (_color is (true, var color))
         {
            colorDescriptor = state.Document.Color(color);
         }
         else
         {
            return _color.Exception;
         }
      }
      else
      {
         return nil;
      }

      state.Definitions[name] = new Definition.Color(colorDescriptor);

      return new NullLine();
   }

   protected static Result<SysColor> getColor(string colorName)
   {
      try
      {
         var color = SysColor.FromName(colorName);
         return color.IsEmpty ? fail($"Color {colorName} is unknown") : color;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected static Maybe<(byte, byte, byte)> getRgb(string source)
   {
      return
         from result in source.Matches($"^ '(' {HEX} /s* ',' /s* {HEX} /s* ',' /s* {HEX} ')' $; f")
         from red in result.FirstGroup.FromHex()
         from redByte in red.Maybe().Cast<byte>()
         from green in result.SecondGroup.FromHex()
         from greenByte in green.Maybe().Cast<byte>()
         from blue in result.ThirdGroup.FromHex()
         from blueByte in blue.Maybe().Cast<byte>()
         select (redByte, greenByte, blueByte);
   }
}