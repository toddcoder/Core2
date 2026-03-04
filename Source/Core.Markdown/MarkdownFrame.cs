using Core.Markup.Html.Parser;
using Core.Markup.Xml;
using Core.Monads;
using Core.Strings;
using Markdig;

namespace Core.Markdown;

public class MarkdownFrame(string styles, string markdown, bool tidy)
{
   public string Styles => styles;

   public string Markdown => markdown;

   public static Optional<MarkdownFrame> Create(IMarkdownFrameOptions options)
   {
      try
      {
         var sourceLines = options.Source.Lines();
         var parser = new MarkdownFrameParser(sourceLines);
         return
            from replacers in parser.Parse()
            let generator = new MarkdownFrameGenerator(replacers, options)
            from generated in generator.Generate()
            select new MarkdownFrame(generated.styles, generated.source, generated.tidy);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<string> ToHtml()
   {
      try
      {
         if (styles.IsEmpty())
         {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);
            var rawHtml = document.ToHtml(pipeline);

            var newHtml = $"<html><body>{rawHtml}</body></html>";
            return tidy ? newHtml.Tidy(true) : newHtml;
         }

         var parser = new StylesParser(styles);
         var _html = parser.Parse();
         if (_html is (true, var html))
         {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var document = Markdig.Markdown.Parse(markdown, pipeline);
            var rawHtml = document.ToHtml(pipeline);

            var newHtml = html.Replace("<body />", $"<body>{rawHtml}</body>");
            return tidy ? newHtml.Tidy(true) : newHtml;
         }
         else
         {
            return _html.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}