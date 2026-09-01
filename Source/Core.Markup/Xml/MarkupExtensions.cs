using System.Text;
using System.Xml;
using Core.Assertions;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Xml;

public static class MarkupExtensions
{
   private const string PATTERN_EMPTY_ELEMENT = "'<' /(-['//!'] -['>']+ -['//']) '><//' /(-['>']+) '>'; f";
   private const string TEXT_EMPTY_ELEMENT = "<$1/>";
   private const string PATTERN_HEADER = "/s* '<?' -['?']+ '?>'; mf";

   private static Result<string> fromStream(Stream stream, Encoding encoding) => tryTo(() =>
   {
      stream.Position = 0;
      using var reader = new StreamReader(stream, encoding);

      return reader.ReadToEnd();
   });

   extension(string markup)
   {
      public string Tidy(Encoding encoding, bool includeHeader = true, char quoteChar = '"')
      {
         markup.Must().Not.BeNullOrEmpty().OrThrow();
         encoding.Must().Not.BeNull().OrThrow();

         var document = new XmlDocument();
         document.LoadXml(markup);
         document.LoadXml(document.OuterXml.Substitute(PATTERN_EMPTY_ELEMENT, TEXT_EMPTY_ELEMENT));

         using var stream = new MemoryStream();
         using var writer = new XmlTextWriter(stream, encoding);
         writer.Formatting = Formatting.Indented;
         writer.Indentation = 3;
         writer.QuoteChar = quoteChar;

         document.Save(writer);

         var _text = fromStream(stream, encoding);
         if (_text is (true, var text))
         {
            return includeHeader ? text : text.Substitute(PATTERN_HEADER, string.Empty).Trim();
         }
         else
         {
            return string.Empty;
         }
      }

      public Optional<string> TidyXml(Encoding encoding, bool includeHeader = true, char quoteChar = '"')
      {
         try
         {
            if (markup.IsEmpty())
            {
               return nil;
            }

            var document = new XmlDocument();
            document.LoadXml(markup);
            document.LoadXml(document.OuterXml.Substitute(PATTERN_EMPTY_ELEMENT, TEXT_EMPTY_ELEMENT));

            using var stream = new MemoryStream();
            using var writer = new XmlTextWriter(stream, encoding);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 3;
            writer.QuoteChar = quoteChar;

            document.Save(writer);

            var _text = fromStream(stream, encoding);
            if (_text is (true, var text))
            {
               return includeHeader ? text : text.Substitute(PATTERN_HEADER, string.Empty).Trim();
            }
            else
            {
               return _text.Exception;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public string Tidy(bool includeHeader) => Tidy(markup, Encoding.UTF8, includeHeader);

      public Optional<string> TidyXml(bool includeHeader) => markup.TidyXml(Encoding.UTF8, includeHeader);

      public string ToMarkup()
      {
         markup.Must().Not.BeNullOrEmpty().OrThrow();

         markup = markup.Substitute("'&' -(> ('amp' | 'lt' | 'gt' | 'quot' | 'apos') ';'); f", "&amp;");
         markup = markup.Substitute("'<'; f", "&lt;");
         markup = markup.Substitute("'>'; f", "&gt;");
         markup = markup.Substitute("[dquote]; f", "&quot;");
         markup = markup.Substitute("[squote]; f", "&apos;");

         return markup;
      }

      public string FromMarkup()
      {
         markup.Must().Not.BeNullOrEmpty().OrThrow();

         markup = markup.Substitute("'&apos;'; f", "'");
         markup = markup.Substitute("'&quot;'; f", "\"");
         markup = markup.Substitute("'&gt;'; f", ">");
         markup = markup.Substitute("'&lt;'; f", "<");
         markup = markup.Substitute("'&amp'; f", "&");

         return markup;
      }

      public string Simplify()
      {
         markup.Must().Not.BeNullOrEmpty().OrThrow();

         return markup
            .Substitute("/s+ /w+ ':' /w '=' [dquote] -[dquote]+ [dquote]; f", "")
            .Substitute("/s+ 'xmlns=' [dquote] -[dquote]+ [dquote]; f", "");
      }
   }
}