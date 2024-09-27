using System.Text;
using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Strings;

namespace Core.Markup.Markdown;

public class MarkdownWriter
{
   protected StringWriter writer = new();
   // ReSharper disable once CollectionNeverUpdated.Global
   protected AutoStringHash<List<string>> styles = new(_ => [], true);
   protected bool userStyles;

   protected void loadBaseStyles()
   {
      userStyles = true;
      style("h1", "font-family", "arial");
      style("h1", "font-size", "28pt");
      style("h2", "font-family", "arial");
      style("h2", "font-size", "16pt");
      style("h3", "font-family", "arial");
      style("h3", "font-size", "12pt");
      style("table", "font-family", "Times New Roman");
      style("table", "font-size", "12pt");
      style("td", "padding", "4px");
      style("td", "margin", "0");
      style("td", "border", "0");
      style("td", "border-bottom", "1px solid black");
      style("td", "border-collapse", "collapse");
      style("th", "border", "0");
      style("tbody tr:nth-child(even)", "background", "#f0f0f2");
      style("p", "font-family", "Times New Roman");
      style("p", "font-size", "12pt");
   }

   protected static string flattenString(string text) => text.ToNonNullString().Substitute("'/r/n' | '/r' | '/n'; f", " ");

   public static string FixString(string text)
   {
      var flatText = flattenString(text);
      var linkText = flatText.Substitute("-(< ['[(']) /b /('https'? ':////' -/s+); f", "[$1]($1)");
      var escapedPipes = linkText.Substitute(@"-(< '\') '\|'; f", @"\|");
      var taggified = Tagify(escapedPipes);

      return taggified;
   }

   public static string FixNumberUrl(string url)
   {
      var flatText = flattenString(url);
      var _description = flatText.Matches("/(/d+) -/d* $; f").Map(r => r.FirstGroup);

      return $"[{_description | url}]({url})";
   }

   public static string Tagify(string text)
   {
      if (text.IsEmpty())
      {
         return text;
      }
      else if (text.Matches("'.'? /(/([/w '-']+)(/['!?'] /([/w '-']+))? '>') /(.*) '.' /2; f") is (true, var result))
      {
         var tagName = result.SecondGroup;
         var style = result.FourthGroup;
         var styleType = result.ThirdGroup;
         var innerText = result.FifthGroup;
         var remainder = text.Drop(result.Length);

         var builder = new StringBuilder($"<{tagName}");
         if (style.IsNotEmpty())
         {
            switch (styleType)
            {
               case "!":
                  builder.Append($" class=\"{style}\">");
                  break;
               case "?":
                  builder.Append($" style=\"{style}\">");
                  break;
            }
         }
         else
         {
            builder.Append('>');
         }

         builder.Append(Tagify(innerText));
         builder.Append($"</{tagName}>");
         builder.Append(Tagify(remainder));

         return builder.ToString();
      }
      else
      {
         return text;
      }
   }

   protected void writeLine(string text)
   {
      writer.WriteLine(FixString(text));
      writer.WriteLine();
   }

   public void WriteHeader(string text, int level) => writeLine($"{"#".Repeat(level)} {text}");

   public void WriteTableBegin(params string[] text)
   {
      var columnHeaders = text.Select(i => $"|{FixString(i)}").ToString("");
      writer.WriteLine();
      writer.WriteLine(columnHeaders);
      writer.WriteLine("|");
   }

   public void WriteRow(params string[] text)
   {
      if (text.Length > 1 && text[1].IsNotEmpty())
      {
         writer.WriteLine(text.Select(i => $"|{FixString(i)}").ToString(""));
      }
   }

   public void WriteColumn(string text, bool end = false)
   {
      writer.Write($"| {FixString(text)}");
      if (end)
      {
         writer.WriteLine();
      }
   }

   public void WriteColumn(string text, string link, bool end = false, bool isImage = false)
   {
      WriteColumn(isImage ? ImageLink(text, link) : Link(text, link), end);
   }

   public void WriteColumnAs(string text, string @class, bool end = false) => WriteColumn($"<span class='{@class}'>{text}</span>", end);

   public void WriteMultilineRow(string column1, string[] lines)
   {
      switch (lines.Length)
      {
         case 0:
            break;
         case 1 when lines[0].IsNotEmpty():
            WriteRow(column1, lines[0]);
            break;
         default:
            WriteRow(column1, lines[0]);
            foreach (var line in lines.Skip(1))
            {
               WriteRow("", line);
            }

            break;
      }
   }

   public void WriteTableEnd() => writer.WriteLine();

   public void WriteLine(string line) => writeLine(line);

   public void WriteLine(string text, string link, bool isImage = false)
   {
      writer.WriteLine(isImage ? ImageLink(text, link) : Link(text, link));
      writer.WriteLine();
   }

   public void WriteLineAs(string text, string @class) => writeLine($"<p class='{@class}'>{text}</p>");

   public void WriteLine() => writeLine("");

   public void WriteHtml(string html) => writer.WriteLine(html);

   public static string Link(string text, string link) => $"[{text}]({link})";

   public static string ImageLink(string text, string link) => $"![{text}]({link})";

   public void WriteRuler()
   {
      writer.WriteLine();
      writeLine("---");
   }

   protected void style(string className, string key, string value) => styles[className].Add($"{key}: {value}");

   public void Style(string className, string key, string value)
   {
      if (!userStyles)
      {
         loadBaseStyles();
      }

      style(className, key, value);
   }

   public string HtmlWrapper(string rawHtml)
   {
      using var stringWriter = new StringWriter();

      stringWriter.WriteLine("<html>");
      stringWriter.WriteLine("<head>");
      stringWriter.WriteLine("<meta http-equiv='X-UA-Compatible' content='IE=edge' />");
      stringWriter.WriteLine("<style>");

      loadBaseStyles();

      foreach (var (className, list) in styles)
      {
         var selectors = "{" + list.ToString("; ") + "}";
         stringWriter.Write($"{className} {selectors}");
      }

      stringWriter.WriteLine("</style>");
      stringWriter.WriteLine("</head>");
      stringWriter.WriteLine("<body>");
      stringWriter.WriteLine(rawHtml);
      stringWriter.WriteLine("</body>");
      stringWriter.WriteLine("</html>");

      return stringWriter.ToString();
   }

   public void WriteList(string text, int indent = 0) => writer.WriteLine($"{"    ".Repeat(indent)}- {FixString(text)}");

   public void WriteList(string text, string link, int indent = 0, bool isImage = false)
   {
      WriteList(isImage ? ImageLink(text, link) : Link(text, link), indent);
   }

   public void WritePageBreak()
   {
      writer.WriteLine("<div style='page-break-after: always'></div>");
      writer.WriteLine();
   }

   public override string ToString() => writer.ToString();
}