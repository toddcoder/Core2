using System.Text;
using Core.Collections;
using Core.DataStructures;
using Core.Enumerables;
using Core.Markup.Xml;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Html.Parser;

public class HtmlParser(string source, bool tidy)
{
   public static implicit operator HtmlParser(string source) => new(source, true);
   protected int index;

   public Optional<string> Parse()
   {
      try
      {
         var styles = new AutoStringHash<Set<StyleKeyValue>>(_ => [], true);
         var body = new StringBuilder();
         var gathering = new StringBuilder();
         var stage = ParsingStage.Starting;
         var tagStack = new MaybeStack<string>();
         var escaped = false;
         var styleName = "";
         var styleKey = "";
         var attribute = "";

         foreach (var character in source)
         {
            switch (character)
            {
               case '>' or '`' or '[' or ']' when escaped:
                  gathering.Append(character);
                  escaped = false;
                  break;
               case '>' or '`' or '[' or ']':
               {
                  var gathered = gathering.ToString();
                  gathering.Clear();
                  var gatheredIsEmpty = gathered.IsEmpty() || gathered.IsWhiteSpace();

                  switch (stage)
                  {
                     case ParsingStage.Starting when gathered == "style":
                        stage = ParsingStage.Style;
                        break;
                     case ParsingStage.Starting:
                        goto case ParsingStage.Tag;
                     case ParsingStage.Style when gatheredIsEmpty:
                        stage = ParsingStage.Tag;
                        break;
                     case ParsingStage.Style:
                        stage = ParsingStage.StyleName;
                        styleName = gathered;
                        break;
                     case ParsingStage.StyleName when gatheredIsEmpty:
                        stage = ParsingStage.Style;
                        break;
                     case ParsingStage.StyleName:
                        stage = ParsingStage.StyleKey;
                        styleKey = gathered;
                        break;
                     case ParsingStage.StyleKey:
                        stage = ParsingStage.StyleName;
                        styles[styleName].Add(new StyleKeyValue(styleKey, gathered));
                        break;
                     case ParsingStage.Tag when gatheredIsEmpty:
                     {
                        if (tagStack.Pop() is (true, var tag))
                        {
                           body.Append(tag);
                        }

                        break;
                     }

                     case ParsingStage.Tag when gathered.EndsWith('.'):
                        body.Append($"<{gathered.Drop(-1)} />");
                        break;
                     case ParsingStage.Tag:
                        body.Append($"<{gathered}");
                        tagStack.Push($"</{gathered}>");
                        stage = ParsingStage.Attribute;
                        break;
                     case ParsingStage.Attribute when gatheredIsEmpty:
                        body.Append('>');
                        stage = ParsingStage.Text;
                        break;
                     case ParsingStage.Attribute when gathered.StartsWith('@'):
                        stage = ParsingStage.AttributeValue;
                        attribute = gathered.Drop(1);
                        break;
                     case ParsingStage.Attribute:
                        body.Append('>');
                        goto case ParsingStage.Tag;
                     case ParsingStage.AttributeValue:
                        stage = ParsingStage.Attribute;
                        body.Append($" {attribute}=\"{gathered}\"");
                        break;
                     case ParsingStage.Text when gatheredIsEmpty:
                     {
                        if (tagStack.Pop() is (true, var tag))
                        {
                           body.Append(tag);
                        }
                        else
                        {
                           return fail($"Unbalanced tags at {index}");
                        }

                        break;
                     }
                     case ParsingStage.Text:
                        stage = ParsingStage.Tag;
                        body.Append(MarkupTextHolder.Markupify(gathered, QuoteType.Double));
                        break;
                  }

                  break;
               }
               case '/' when escaped:
                  gathering.Append(character);
                  escaped = false;
                  break;
               case '/':
                  escaped = true;
                  break;
               case ' ' when stage is ParsingStage.AttributeValue or ParsingStage.StyleKey:
                  gathering.Append(character);
                  break;
               case ' ' or '\t' or '\r' or '\n' when stage is not ParsingStage.Text:
               {
                  break;
               }
               case ' ' or '\t' or '\r' or '\n':
                  gathering.Append(character);

                  break;
               default:
                  gathering.Append(character);
                  break;
            }

            index++;
         }

         if (stage is ParsingStage.Text && gathering.Length > 0)
         {
            body.Append(MarkupTextHolder.Markupify(gathering.ToString()));
         }

         while (tagStack.Pop() is (true, var tag))
         {
            body.Append(tag);
         }

         return Render(styles, body);
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

         return tidy ? html.TidyXml(true) : html;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}