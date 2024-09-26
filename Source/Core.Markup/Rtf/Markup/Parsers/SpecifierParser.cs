using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf.Markup.Parsers;

public class SpecifierParser
{
   public Optional<Specifiers> Parse(string source, ParsingState state)
   {
      try
      {
         var length = 0;
         var currentSource = source;
         var specifiers = new Specifiers();
         Pattern pattern = $"^ /('$' | '^') /({LineParser.WORD}+) '.'?";
         while (currentSource.Matches(pattern) is (true, var result))
         {
            var prefix = result.FirstGroup;
            var name = result.SecondGroup;
            length += result.Length;

            if (prefix == "$")
            {
               var suffix = "";
               if (name.EndsWith(":bg"))
               {
                  suffix = "bg";
                  name = name.Drop(-3);
               }
               else if (name.EndsWith(":fg"))
               {
                  suffix = "fg";
                  name = name.Drop(-3);
               }

               if (state.Definitions.Maybe[name] is (true, var definition))
               {
                  switch (definition)
                  {
                     case Definition.Color colorDefinition when suffix == "bg":
                     {
                        var colorDescriptor = colorDefinition.ColorDescriptor;
                        var specifier = new Specifier.BackgroundColor(colorDescriptor);
                        specifiers.Add(specifier);
                        break;
                     }
                     case Definition.Color colorDefinition when suffix == "fg":
                     {
                        var colorDescriptor = colorDefinition.ColorDescriptor;
                        var specifier = new Specifier.Color(colorDescriptor);
                        specifiers.Add(specifier);
                        break;
                     }
                     case Definition.Color colorDefinition:
                     {
                        var colorDescriptor = colorDefinition.ColorDescriptor;
                        var specifier = new Specifier.Color(colorDescriptor);
                        specifiers.Add(specifier);
                        break;
                     }
                     case Definition.Font fontDefinition:
                     {
                        var fontDescriptor = fontDefinition.FontDescriptor;
                        var specifier = new Specifier.Font(fontDescriptor);
                        specifiers.Add(specifier);
                        break;
                     }
                     case Definition.Style styleDefinition:
                     {
                        var styleSpecifiers = styleDefinition.Specifiers;
                        var specifier = new Specifier.Style(styleSpecifiers);
                        specifiers.Add(specifier);
                        break;
                     }
                     default:
                        return nil;
                  }
               }
            }
            else
            {
               switch (name)
               {
                  case "none":
                     specifiers.Add(new Specifier.Feature.None());
                     break;
                  case "bold":
                     specifiers.Add(new Specifier.Feature.Bold());
                     break;
                  case "italic":
                     specifiers.Add(new Specifier.Feature.Italic());
                     break;
                  case "underline":
                     specifiers.Add(new Specifier.Feature.Underline());
                     break;
                  case "bullet":
                     specifiers.Add(new Specifier.Feature.Bullet());
                     break;
                  case "new-page":
                     specifiers.Add(new Specifier.Feature.NewPage());
                     break;
                  case "new-page-after":
                     specifiers.Add(new Specifier.Feature.NewPageAfter());
                     break;
                  case "left":
                     specifiers.Add(new Specifier.Alignment.Left());
                     break;
                  case "right":
                     specifiers.Add(new Specifier.Alignment.Right());
                     break;
                  case "center":
                     specifiers.Add(new Specifier.Alignment.Center());
                     break;
                  case "distributed":
                     specifiers.Add(new Specifier.Alignment.Distributed());
                     break;
                  case "justify":
                     specifiers.Add(new Specifier.Alignment.FullyJustify());
                     break;
                  case "page":
                     specifiers.Add(new Specifier.FieldType.Page());
                     break;
                  case "num-pages":
                     specifiers.Add(new Specifier.FieldType.NumPages());
                     break;
                  case "date":
                     specifiers.Add(new Specifier.FieldType.Date());
                     break;
                  case "time":
                     specifiers.Add(new Specifier.FieldType.Time());
                     break;
                  default:
                     return nil;
               }
            }
         }

         if (length > 0)
         {
            return specifiers;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}