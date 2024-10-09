using Core.Markup.Html;
using Core.Markup.Html.Parser;
using Core.Markup.Xml;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class MarkupTests
{
   [TestMethod]
   public void StyleBuildTest()
   {
      var builder = new HtmlBuilder();

      var tableThTd = builder + "table, th, td {";
      _ = tableThTd + "border: collapse";
      _ = tableThTd + "padding: 5px";
      _ = tableThTd + "front-family: Verdana";

      var trNthChildEven = builder + "tr:nth-child(even) {";
      _ = trNthChildEven + "color: white";
      _ = trNthChildEven + "background-color: salmon";

      Console.WriteLine(builder);
   }

   [TestMethod]
   public void ImplicitAttributeTest()
   {
      var builder = new MarkupBuilder("alpha");
      var alpha = builder.Root;
      alpha += "@bar=txt";
      alpha += "baz=txt2";
      Console.WriteLine(alpha);
   }

   [TestMethod]
   public void ImplicitElementTest()
   {
      var builder = new MarkupBuilder("alpha");
      var alpha = builder.Root;
      alpha *= "bar>txt";
      alpha *= "baz>' txt2 ";
      Console.WriteLine(alpha);
   }

   [TestMethod]
   public void PlusElementTest()
   {
      var builder = new MarkupBuilder("alpha");
      var alpha = builder.Root;
      var bar = alpha + "bar>";
      _ = bar + "baz>none";

      Console.WriteLine(builder);
   }

   [TestMethod]
   public void HtmlTest()
   {
      var builder = new HtmlBuilder();
      var tableThTd = builder + "table, th, td { border: 1px solid black";
      tableThTd += "border-collapse: collapse";
      tableThTd += "padding: 5px";
      _ = tableThTd + "font-family: Verdana";

      var nthChild = builder + "tr:nth-child(even) {";
      nthChild += "color: white";
      _ = nthChild + "background-color: salmon";

      var table = builder.Body + "table>";
      var tr = table + "tr>";
      tr *= "th>Alpha";
      tr *= "th>Bravo";
      _ = tr * "th>Charlie";

      tr = table + "tr>";
      tr *= "td>alpha";
      tr *= "td>beta";
      _ = tr * "td>kappa";

      tr = table + "tr>";
      tr *= "td>ah";
      tr *= "td>bo";
      _ = tr * "td>tso";

      Console.WriteLine(builder);
   }

   [TestMethod]
   public void HtmlTest2()
   {
      var builder = new HtmlBuilder();

      var defaultStyle = builder + ".header{";
      _ = defaultStyle + "color: white";
      _ = defaultStyle + "background-color: blue";

      var titleStyle = builder + ".title{";
      _ = titleStyle + "font-weight: bold";
      _ = titleStyle + "font-size: 16px";

      var boldStyle = builder + ".bold{";
      _ = boldStyle + "font-weight: bold";
      _ = boldStyle + "font-size: 14px";

      var body = builder.Body;
      _ = body + "@style='font-family: Verdana; font-size: 11px'";

      _ = body + "p>Merged Branches" + "@class=title";

      var table = body + "table>" + "@border=1px black solid";
      _ = table + "th>" + "@class=header" + "b>Branch";

      _ = table + "tr>" + "td>Alpha";
      _ = table + "tr>" + "td>Bravo";
      _ = table + "tr>" + "td>Charlie";

      _ = body + "hr>";
      _ = body + "p>Conflicted Branches" + "@class=title";

      foreach (var branch in (string[]) ["branch1", "branch2", "branch3"])
      {
         _ = body + "hr>";

         table = body + "table>" + "@border=1px black solid";
         _ = table + $"th>{branch}" + "@class=header";
         _ = table + "th>File" + "@class=header";

         foreach (var file in (string[]) ["file1", "file2", "file3", "file4"])
         {
            _ = table + "tr>" + "td>" + $"td>{file}";
         }
      }

      Console.WriteLine(builder);
   }

   [TestMethod]
   public void LinkTest()
   {
      var builder = new HtmlBuilder();
      var body = builder.Body;
      _ = body + "link|https://www.w3schools.com/html/html_links.asp|HTML Links";
      Console.WriteLine(builder);
   }

   [TestMethod]
   public void HtmlParserTest()
   {
      var lines = new LineAccumulator();
      lines += "style[";
      lines += "p[ font-size(11pt) font-family(Consolas) ]";
      lines += ".fb-p[ color(blue) ]";
      lines += "]";
      lines += "p[";
      lines += "id(remedy) class(fb-p)";
      lines += "`Build for Release `";
      lines += "em[";
      lines += "`1.4.0 [PSA]`";
      lines += "]";
      lines += "]";
      var parser = new HtmlParser(lines.ToString(), true);
      var _html = parser.Parse();
      if (_html is (true, var html))
      {
         Console.WriteLine(html);
      }
      else if (_html.Exception is (true, var exception))
      {
         Console.WriteLine(exception.Message);
      }
   }

   [TestMethod]
   public void HtmlParserTest1()
   {
      var lines = new LineAccumulator();

      lines += "style[";
      lines += "table, th, td[";
      lines += "border(1px solid black)";
      lines += "border-collapse(collapse)";
      lines += "padding(5px)";
      lines += "font-family(Verdana)";
      lines += "]";
      lines += "tr:nth-child(even)[";
      lines += "color(white)";
      lines += "background-color(salmon)";
      lines += "]";
      lines += "]";

      lines += "table[";
      lines += "tr[";
      lines += "th[`Alfa`]";
      lines += "th[`Bravo`]";
      lines += "th[`Charlie`]";
      lines += "]";
      lines += "tr[";
      lines += "td[`alpha`]";
      lines += "td[`beta`]";
      lines += "td[`kappa`]";
      lines += "]";
      lines += "tr[";
      lines += "td[`ah`]";
      lines += "td[`bo`]";
      lines += "td[`tso`]";
      lines += "]";
      lines += "]";

      var parser = new HtmlParser(lines.ToString(), true);
      var _html = parser.Parse();
      if (_html is (true, var html))
      {
         Console.WriteLine(html);
      }
      else if (_html.Exception is (true, var exception))
      {
         Console.WriteLine(exception.Message);
      }
   }

   [TestMethod]
   public void HtmlParserTest2()
   {
      var lines = new LineAccumulator();
      lines += "style[";
      lines += ".header[color(white) background-color(blue)]";
      lines += ".title[font-weight(bold) font-size(16px)]";
      lines += ".bold[font-weight(bold) font-size(14px)]";
      lines += "body[font-family(Verdana) font-size(11px)]";
      lines += "]";

      lines += "p[class(title) `Merged Branches`]";
      lines += "table[border(1px black solid)";
      lines += "th[class(header) b[`Branch`]]";
      lines += "tr[td[`Alpha`]]";
      lines += "tr[td[`Bravo`]]";
      lines += "tr[td[`Charlie`]]";
      lines += "]";
      lines += "br.";
      lines += "p[class(title) `Conflicted Branches/Files`]";
      lines += "table[border(1px black solid)";
      lines += "th[class(header) b[`branch1`]]";
      lines += "th[class(header) b[`file`]]";
      lines += "tr[td[`file1`]]";
      lines += "tr[td[`file2`]]";
      lines += "tr[td[`file3`]]";
      lines += "]";
      lines += "style[";
      lines += "p[margin(0)]";
      _ = lines + "]";

      var parser = new HtmlParser(lines.ToString(), true);
      var _html = parser.Parse();
      if (_html is (true, var html))
      {
         Console.WriteLine(html);
      }
      else if (_html.Exception is (true, var exception))
      {
         Console.WriteLine(exception.Message);
      }
   }

   [TestMethod]
   public void BrTest()
   {
      var lines = new LineAccumulator();

      lines += "#tidy";
      lines += "br.";
      lines += "p[class(standard) `Mortimer J. Snerd`]";
      lines += "p[class(italic) `Code~Serf`]";
      lines += "p[class(bold) `Computer Lab of Chaos`]";
      lines += "br.";
      lines += "p[class(fixed) `###-###-#### Office`]";
      lines += "p[class(fixed) `###-###-#### Cell`]";
      lines += "{<div style='page-break-after: always'></div>}";
      _ = lines + "a[class(fixed) href(mailto:mjsnerd@clc.com) `Mortimer J. Snerd`]";

      var parser = new HtmlParser(lines.ToString(), false);
      var _html = parser.Parse();
      if (_html is (true, var html))
      {
         Console.WriteLine(html);
      }
      else if (_html.Exception is (true, var exception))
      {
         Console.WriteLine(exception.Message);
      }
   }

   [TestMethod]
   public void RawHtmlTest()
   {
      var lines = new LineAccumulator();

      lines += "#untidy";
      lines += "br.";
      lines += "{<div style='page-break-after: always'><//div>}";
      _ = lines + "br.";

      var parser = new HtmlParser(lines.ToString(), false);
      var _html = parser.Parse();
      if (_html is (true, var html))
      {
         Console.WriteLine(html);
      }
      else if (_html.Exception is (true, var exception))
      {
         Console.WriteLine(exception.Message);
      }
   }
}