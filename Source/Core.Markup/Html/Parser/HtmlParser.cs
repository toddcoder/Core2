using System.Text;
using Core.Collections;
using Core.Enumerables;
using Core.Markup.Xml;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Html.Parser;

public class HtmlParser(string source, bool tidy)
{
   public static implicit operator HtmlParser(string source) => new(source, true);

   protected Maybe<bool> _tidy = nil;

   public Optional<string> Parse()
   {
      try
      {
         string localSource;
         if (source.Matches("'#' /('tidy' | 'untidy') /s+; f") is (true, var result))
         {
            _tidy = result.FirstGroup == "tidy";
            localSource = source.Drop(result.Length);
         }
         else
         {
            localSource = source;
         }

         var gatherer = new HtmlGatherer();

         foreach (var character in localSource)
         {
            gatherer.Gather();

            switch (gatherer.Stage)
            {
               case ParsingStage.Name:
               {
                  switch (character)
                  {
                     case '[' when gatherer.Escaped:
                        gatherer.GatherCharacter(character);
                        break;
                     case '/':
                        gatherer.Escaped = true;
                        break;
                     case '[' when gatherer.Gathered == "style":
                        gatherer.BeginStyle();
                        break;
                     case '[':
                        gatherer.BeginTag();
                        break;
                     case '.':
                        gatherer.ClosedTag();
                        break;
                     default:
                        gatherer.GatherCharacter(character, true);
                        break;
                  }

                  break;
               }
               case ParsingStage.Style:
               {
                  switch (character)
                  {
                     case '[' or ']' when gatherer.Escaped:
                        gatherer.GatherCharacter(character);
                        break;
                     case '/':
                        gatherer.Escaped = true;
                        break;
                     case '[':
                        gatherer.BeginStyleName();
                        break;
                     case ']':
                        gatherer.EndStyle();
                        break;
                     default:
                        gatherer.GatherCharacter(character, true);
                        break;
                  }

                  break;
               }
               case ParsingStage.StyleName:
               {
                  switch (character)
                  {
                     case '(' or ']' when gatherer.Escaped:
                        gatherer.GatherCharacter(character);
                        break;
                     case '/':
                        gatherer.Escaped = true;
                        break;
                     case '(':
                        gatherer.BeginStyleKey();
                        break;
                     case ']':
                        gatherer.EndStyleName();
                        break;
                     default:
                        gatherer.GatherCharacter(character, true);
                        break;
                  }

                  break;
               }
               case ParsingStage.StyleKey:
               {
                  switch (character)
                  {
                     case ')' when gatherer.Escaped:
                        gatherer.GatherCharacter(character);
                        break;
                     case '/':
                        gatherer.Escaped = true;
                        break;
                     case ')':
                        gatherer.BeginStyleValue();
                        break;
                     default:
                        gatherer.GatherCharacter(character, true);
                        break;
                  }

                  break;
               }
               case ParsingStage.Tag:
               {
                  switch (character)
                  {
                     case '(' or '[' or '`' or ']' or '.' when gatherer.Escaped:
                        gatherer.GatherCharacter(character);
                        break;
                     case '/':
                        gatherer.Escaped = true;
                        break;
                     case '[' when gatherer.Gathered == "style":
                        gatherer.BeginStyle();
                        break;
                     case '[':
                        gatherer.BeginTag();
                        break;
                     case '(':
                        gatherer.BeginAttribute();
                        break;
                     case '`':
                        gatherer.BeginText();
                        break;
                     case ']':
                        gatherer.EndTag();
                        break;
                     case '.':
                        gatherer.ClosedTag();
                        break;
                     default:
                        gatherer.GatherCharacter(character, true);
                        break;
                  }

                  break;
               }
               case ParsingStage.Attribute:
               {
                  switch (character)
                  {
                     case ')' when gatherer.Escaped:
                        gatherer.GatherCharacter(character);
                        break;
                     case '/':
                        gatherer.Escaped = true;
                        break;
                     case ')':
                        gatherer.EndAttribute();
                        break;
                     default:
                        gatherer.GatherCharacter(character, true);
                        break;
                  }

                  break;
               }
               case ParsingStage.Text:
               {
                  switch (character)
                  {
                     case '`' when gatherer.Escaped:
                        gatherer.GatherCharacter(character);
                        break;
                     case '/':
                        gatherer.Escaped = true;
                        break;
                     case '`':
                        gatherer.EndText();
                        break;
                     default:
                        gatherer.GatherCharacter(character, true);
                        break;
                  }

                  break;
               }
            }
         }

         gatherer.EndAll();

         return Render(gatherer.Styles, gatherer.Body);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected Optional<string> Render(AutoStringHash<Set<StyleKeyValue>> styles, StringBuilder body)
   {
      try
      {
         using var writer = new StringWriter();
         writer.WriteLine("<html>");
         writer.WriteLine("<head>");
         writer.WriteLine("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />");
         writer.WriteLine("<style>");

         foreach (var (styleName, styleKeyValues) in styles.Where(t => t.Key != "body"))
         {
            writer.WriteLine($"  {styleName} {{");
            foreach (var (key, value) in styleKeyValues)
            {
               writer.WriteLine($"   {key}: {value};");
            }

            writer.WriteLine("  }");
         }

         writer.WriteLine(" </style>");
         writer.WriteLine("</head>");
         writer.Write(" <body");
         if (styles.Maybe["body"] is (true, var bodyStyleKeyValues))
         {
            writer.Write(" style=");
            writer.Write('"');
            writer.Write(bodyStyleKeyValues.Select(kv => $"{kv.Key}: {kv.Value}").ToString("; "));
            writer.Write('"');
         }

         writer.WriteLine('>');

         writer.WriteLine(body);
         writer.WriteLine(" </body>");
         writer.WriteLine("</html>");
         var html = writer.ToString();

         if (_tidy is (true, var overriddenTidy))
         {
         }
         else
         {
            overriddenTidy = tidy;
         }

         return overriddenTidy ? html.TidyXml(true) : html;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}