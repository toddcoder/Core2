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
   protected AutoStringHash<StringHash> styles = new(_ => [], true);

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
      else if (text.Matches("'.'? /(/([/w '-']+)(/['!?'] /([/w ':; -']+))? '>') /(.*?) ('.' /2 | $); f") is (true, var result))
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

   public void WriteLineBreak() => writer.Write("<br/>");

   protected static string getClassRef(string classRef) => classRef.IsNotEmpty() ? "{" + classRef + "}" : "";

   protected void writeMarkdownLine(string text, string classRef)
   {
      writer.WriteLine(FixString(text) + getClassRef(classRef));
   }

   public void WriteTextLine(string text, string classRef = "")
   {
      writeMarkdownLine(text, classRef);
      writer.WriteLine("<br/>");
   }

   public void WriteHeader(string text, int level, string classRef = "") => writeMarkdownLine($"{"#".Repeat(level)} {text}", classRef);

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

   public void WriteColumn(string text, bool end = false, string classRef = "")
   {
      writer.Write($"| {FixString(text)}" + getClassRef(classRef));
      if (end)
      {
         writer.WriteLine();
      }
   }

   public void WriteColumn(string text, string link, bool end = false, bool isImage = false, string classRef = "")
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

   public void WriteLine(string line, string classRef = "") => writeMarkdownLine(line, classRef);

   public void WriteLine(string text, string link, bool isImage = false, string classRef = "") =>
      writeMarkdownLine(isImage ? ImageLink(text, link) : Link(text, link), classRef);

   public void WriteLineAs(string text, string @class) => writeMarkdownLine($"<p class='{@class}'>{text}</p>", "");

   public void WriteLine() => writeMarkdownLine("", "");

   public void WriteHtml(string html) => writer.WriteLine(html);

   public static string Link(string text, string link) => $"[{text}]({link})";

   public static string ImageLink(string text, string link) => $"![{text}]({link})";

   public void WriteRuler()
   {
      writer.WriteLine();
      writeMarkdownLine("---", "");
   }

   public void Write(string text) => writer.Write(FixString(text));

   public void Write(string text, string link, bool isImage = false) => Write(isImage ? ImageLink(text, link) : Link(text, link));

   protected void style(string className, string key, string value) => styles[className][key] = value;

   public void Style(string className, string key, string value) => style(className, key, value);

   public string HtmlWrapper(string rawHtml)
   {
      using var stringWriter = new StringWriter();

      stringWriter.WriteLine("<html>");
      stringWriter.WriteLine("<head>");
      stringWriter.WriteLine("<meta http-equiv='X-UA-Compatible' content='IE=edge' />");
      stringWriter.WriteLine("<style>");

      foreach (var (className, list) in styles)
      {
         var selectors = "{" + list.Tuples().Select(t => $"{t.key}: {t.value}").ToString("; ") + "}";
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

   public void WriteOrderedList(string text, string prefix, int indent = 0) =>
      writer.WriteLine($"{"    ".Repeat(indent)}{prefix} {FixString(text)}");

   public void WriteOrderedList(string text, string link, string prefix, int indent = 0, bool isImage = false)
   {
      WriteOrderedList(isImage ? ImageLink(text, link) : Link(text, link), prefix, indent);
   }

   protected string getCheckListPrefix(bool isChecked) => isChecked ? "[x]" : "[ ]";

   public void WriteCheckList(string text, bool isChecked, int indent = 0)
   {
      writer.WriteLine($"{"    ".Repeat(indent)}{getCheckListPrefix(isChecked)} {FixString(text)}");
   }

   public void WriteCheckList(string text, string link, bool isChecked, int indent = 0, bool isImage = false)
   {
      WriteCheckList(isImage ? ImageLink(text, link) : Link(text, link), isChecked, indent);
   }

   public void WriteQuote(string text) => writer.WriteLine($"> {FixString(text)}");

   public void WriteQuote(string text, string link, bool isImage = false) =>
      writer.WriteLine($"> {(isImage ? Link(text, link) : ImageLink(text, link))}");

   public void WritePageBreak()
   {
      writer.WriteLine("<div style='page-break-after: always'></div>");
      writer.WriteLine();
   }

   public void WriteListItem() => writer.Write("- ");

   public void WriteCheckItem(bool isChecked)
   {
      writer.Write("[");
      writer.Write(isChecked ? "X" : " ");
      writer.Write("] ");
   }

   public void WriteLink(string description, string url)
   {
      writer.Write($"[{description}]({url})");
   }

   public void WriteLink(string url) => WriteLink(url, url);

   public void WriteRaw(string text) => writer.Write(text);

   public void WriteRawLine(string text) => writer.WriteLine(text);

   public void WriteRawLine() => writer.WriteLine();

   public override string ToString() => writer.ToString();
}