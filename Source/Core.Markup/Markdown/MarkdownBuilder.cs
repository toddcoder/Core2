namespace Core.Markup.Markdown;

public class MarkdownBuilder
{
   protected MarkdownWriter writer = new();

   public MarkdownBuilder Break()
   {
      writer.LineBreak();
      return this;
   }

   public MarkdownBuilder TextLine(string text, string classRef = "")
   {
      writer.TextLine(text, classRef);
      return this;
   }

   public MarkdownBuilder Header(string text, int level, string classRef = "")
   {
      writer.Header(text, level, classRef);
      return this;
   }

   public MarkdownBuilder TableBegin(params string[] text)
   {
      writer.TableBegin(text);
      return this;
   }

   public MarkdownBuilder TableRow(params string[] text)
   {
      writer.TableRow(text);
      return this;
   }

   public MarkdownBuilder TableColumn(string text, bool end = false, string classRef = "")
   {
      writer.TableColumn(text, end, classRef);
      return this;
   }

   public MarkdownBuilder TableColumn(string linkDescription, string linkUrl, bool end = false, bool isImage = false, string classRef = "")
   {
      writer.TableColumn(linkDescription, linkUrl, end, isImage, classRef);
      return this;
   }

   public MarkdownBuilder TableColumnAs(string text, string @class, bool end = false)
   {
      writer.TableColumnAs(text, @class, end);
      return this;
   }

   public MarkdownBuilder TableMultilineRow(string column1, string[] lines)
   {
      writer.TableMultilineRow(column1, lines);
      return this;
   }

   public MarkdownBuilder TableEnd()
   {
      writer.TableEnd();
      return this;
   }

   public MarkdownBuilder Line(string text, string classRef = "")
   {
      writer.Line(text, classRef);
      return this;
   }

   public MarkdownBuilder Line(string text, string link, bool isImage = false, string classRef = "")
   {
      writer.Line(text, link, isImage, classRef);
      return this;
   }

   public MarkdownBuilder LineAs(string text, string @class)
   {
      writer.LineAs(text, @class);
      return this;
   }

   public MarkdownBuilder Line()
   {
      writer.Line();
      return this;
   }

   public MarkdownBuilder List(string text, int indent = 0)
   {
      writer.List(text, indent);
      return this;
   }

   public MarkdownBuilder List(string text, string link, int indent = 0, bool isImage = false)
   {
      writer.List(text, link, indent, isImage);
      return this;
   }

   public MarkdownBuilder OrderedList(string text, string prefix, int indent = 0)
   {
      writer.OrderedList(text, prefix, indent);
      return this;
   }

   public MarkdownBuilder OrderedList(string text, string link, string prefix, int indent = 0, bool isImage = false)
   {
      writer.OrderedList(text, link, prefix, indent, isImage);
      return this;
   }

   public MarkdownBuilder CheckList(string text, bool isChecked, int indent = 0)
   {
      writer.CheckList(text, isChecked, indent);
      return this;
   }

   public MarkdownBuilder CheckList(string text, string link, bool isChecked, int indent = 0, bool isImage = false)
   {
      writer.CheckList(text, link, isChecked, indent, isImage);
      return this;
   }

   public MarkdownBuilder Quote(string text)
   {
      writer.Quote(text);
      return this;
   }

   public MarkdownBuilder Quote(string text, string link, bool isImage = false)
   {
      writer.Quote(text, link, isImage);
      return this;
   }

   public MarkdownBuilder PageBreak()
   {
      writer.PageBreak();
      return this;
   }

   public MarkdownBuilder Ruler()
   {
      writer.Ruler();
      return this;
   }

   public MarkdownBuilder Html(string html)
   {
      writer.Html(html);
      return this;
   }

   public MarkdownBuilder Raw(string text)
   {
      writer.Raw(text);
      return this;
   }

   public MarkdownBuilder RawLine(string text)
   {
      writer.RawLine(text);
      return this;
   }

   public MarkdownBuilder RawLine()
   {
      writer.RawLine();
      return this;
   }

   public override string ToString() => writer.ToString();

   public string HtmlWrapper(string rawHtml)
   {
      return writer.HtmlWrapper(rawHtml);
   }
}