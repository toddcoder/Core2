namespace Core.Markup.Markdown;

public class MarkdownBuilder
{
   protected MarkdownWriter writer = new();

   public MarkdownBuilder Break()
   {
      writer.WriteLineBreak();
      return this;
   }

   public MarkdownBuilder TextLine(string text, string classRef = "")
   {
      writer.WriteTextLine(text, classRef);
      return this;
   }

   public MarkdownBuilder Header(string text, int level, string classRef = "")
   {
      writer.WriteHeader(text, level, classRef);
      return this;
   }

   public MarkdownBuilder TableBegin(params string[] text)
   {
      writer.WriteTableBegin(text);
      return this;
   }

   public MarkdownBuilder TableRow(params string[] text)
   {
      writer.WriteTableRow(text);
      return this;
   }

   public MarkdownBuilder TableColumn(string text, bool end = false, string classRef = "")
   {
      writer.WriteTableColumn(text, end, classRef);
      return this;
   }

   public MarkdownBuilder TableColumn(string linkDescription, string linkUrl, bool end = false, bool isImage = false, string classRef = "")
   {
      writer.WriteTableColumn(linkDescription, linkUrl, end, isImage, classRef);
      return this;
   }

   public MarkdownBuilder TableColumnAs(string text, string @class, bool end = false)
   {
      writer.WriteTableColumnAs(text, @class, end);
      return this;
   }

   public MarkdownBuilder TableMultilineRow(string column1, string[] lines)
   {
      writer.WriteTableMultilineRow(column1, lines);
      return this;
   }

   public MarkdownBuilder TableEnd()
   {
      writer.WriteTableEnd();
      return this;
   }

   public MarkdownBuilder Line(string text, string classRef = "")
   {
      writer.WriteLine(text, classRef);
      return this;
   }

   public MarkdownBuilder Line(string text, string link, bool isImage = false, string classRef = "")
   {
      writer.WriteLine(text, link, isImage, classRef);
      return this;
   }

   public MarkdownBuilder LineAs(string text, string @class)
   {
      writer.WriteLineAs(text, @class);
      return this;
   }

   public MarkdownBuilder Line()
   {
      writer.WriteLine();
      return this;
   }

   public MarkdownBuilder List(string text, int indent = 0)
   {
      writer.WriteList(text, indent);
      return this;
   }

   public MarkdownBuilder List(string text, string link, int indent = 0, bool isImage = false)
   {
      writer.WriteList(text, link, indent, isImage);
      return this;
   }

   public MarkdownBuilder OrderedList(string text, string prefix, int indent = 0)
   {
      writer.WriteOrderedList(text, prefix, indent);
      return this;
   }

   public MarkdownBuilder OrderedList(string text, string link, string prefix, int indent = 0, bool isImage = false)
   {
      writer.WriteOrderedList(text, link, prefix, indent, isImage);
      return this;
   }

   public MarkdownBuilder CheckList(string text, bool isChecked, int indent = 0)
   {
      writer.WriteCheckList(text, isChecked, indent);
      return this;
   }

   public MarkdownBuilder CheckList(string text, string link, bool isChecked, int indent = 0, bool isImage = false)
   {
      writer.WriteCheckList(text, link, isChecked, indent, isImage);
      return this;
   }

   public MarkdownBuilder Quote(string text)
   {
      writer.WriteQuote(text);
      return this;
   }

   public MarkdownBuilder Quote(string text, string link, bool isImage = false)
   {
      writer.WriteQuote(text, link, isImage);
      return this;
   }

   public MarkdownBuilder PageBreak()
   {
      writer.WritePageBreak();
      return this;
   }

   public MarkdownBuilder Ruler()
   {
      writer.WriteRuler();
      return this;
   }

   public MarkdownBuilder Html(string html)
   {
      writer.WriteHtml(html);
      return this;
   }

   public MarkdownBuilder Raw(string text)
   {
      writer.WriteRaw(text);
      return this;
   }

   public MarkdownBuilder RawLine(string text)
   {
      writer.WriteRawLine(text);
      return this;
   }

   public MarkdownBuilder RawLine()
   {
      writer.WriteRawLine();
      return this;
   }

   public override string ToString() => writer.ToString();

   public string HtmlWrapper(string rawHtml)
   {
      return writer.HtmlWrapper(rawHtml);
   }
}