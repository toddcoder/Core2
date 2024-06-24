using Core.Markup.Rtf;
using static Core.Markup.Rtf.RtfFunctions;
using static Core.Markup.Rtf.RtfStripperFunction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Monads.MonadFunctions;

namespace Core.Tests;

[TestClass]
public class RichTextFormatTests
{
   [TestMethod]
   public void BasicTest()
   {
      var document = new Document(PaperSize.A4, PaperOrientation.Landscape);
      var timesFont = document.Font("Times New Roman");
      var consolasFont = document.Font("Consolas");
      var goldColor = document.Color("gold");
      var tableHeaderColor = document.Color(0x76923C);
      var tableRowColor = document.Color(0xD6E3BC);
      var tableRowAltColor = document.Color(0xFFFFFF);

      document.DefaultCharFormat.Style = new Style() + timesFont + 12f;

      var header = document.Header;
      _ = header + "Test Document " + none + "Test.rtf" + italic;
      _ = header + "";

      var footer = document.Footer;
      _ = footer + "Page " + page + " of " + numPages;

      _ = document + "This is test line 1";
      _ = document + "This is test line 2";
      _ = document + "This is test line 3";

      _ = document + "";

      _ = document + "Item 1" + bullet;
      _ = document + "Item 2" + bullet;
      _ = document + "Item 3" + bullet;
      _ = document + "";

      _ = document + "Test 1: " + none + "italics" + italic;
      _ = document + "Test 2: " + none + "bold" + bold;
      _ = document + "Test 3: " + none + "underline" + underline;
      _ = document + "Test 4: " + none + "gold" + goldColor.Background;

      _ = document + "";

      var headerStyle = new Style() + center + bold;
      var rowStyle = new Style() + consolasFont + italic;

      var table = document.Table(12f);
      _ = table + "Full Name" + headerStyle + "Latin Letter" + headerStyle + "Greek Letter" + headerStyle;
      _ = table + "alpha" + rowStyle + goldColor.Background + "a" + "alpha" + rowStyle;
      _ = table + "bravo" + "b" + "beta";
      _ = table + "charlie" + "c" + "kappa";
      _ = table + "delta" + rowStyle + "d" + "delta" + rowStyle;
      _ = table + "echo" + "e" + "epsilon";
      _ = table + "foxtrot" + "f" + "phi";

      _ = table + marginBottom + 20f;
      _ = table + marginLeft + 20f;
      _ = table + innerBorder + BorderStyle.Dotted + 1f;
      _ = table + outerBorder + BorderStyle.Single + 1f;
      _ = table + headerBgColor + tableHeaderColor;
      _ = table + rowBgColor + tableRowColor;
      _ = table + rowAltBgColor + tableRowAltColor;

      _ = document + "Test 4: " + newPage + "Character Formatting" + consolasFont + 16f + goldColor.Background;

      _ = document + "";

      var paragraph = document + "Test 5: This is a footnote";
      var footnote = paragraph.Footnote();
      _ = footnote + "Footnote information" + bold;

      var boldStyle = new Style() + timesFont + 12f + bold;
      var url = "http://foobar.com/";

      _ = document + "Please click " + none + "foobar" + boldStyle + " with link " + url.Link() + " deployment of " +
         "foobar" + boldStyle;

      document.Save(@"C:\Temp\Basic.rtf");
      var rtf = document.Render();
      var _text = stripRichTextFormat(rtf);
      if (_text is (true, var text))
      {
         Console.WriteLine(text);
      }
      else
      {
         Console.WriteLine(_text.Exception);
      }
   }

   [TestMethod]
   public void TableDataTest()
   {
      var margins = (8f, nil, nil, nil);
      var colStyle1 = new Style() + bold + margins;
      var colStyle2 = new Style() + margins;
      var document = new Document();
      var table = document.Table(12);
      _ = table + "Pull Request" + colStyle1 + "" + "http://foobar".Link() + colStyle2;
      _ = table + "Deployment" + colStyle1 + "" + "http://deployment".Link() + colStyle2;
      _ = table + "Staging" + colStyle1 + "" + "http://staging".Link() + colStyle2;

      _ = table + outerBorder + BorderStyle.Double + 1f;
      _ = table + innerBorder + BorderStyle.Single + 1f;

      document.Save(@"C:\Temp\TableData.rtf");
   }

   [TestMethod]
   public void BulletTest()
   {
      var document = new Document();
      _ = document + "Top";
      _ = document + "Item 1" + bullet;
      _ = document + "Item 2" + bullet;
      _ = document + "Item 3" + bullet;
      _ = document + "Bottom";
      document.Line();
      _ = document + "Under the line";

      document.Save(@"C:\Temp\Bullet.rtf");
   }
}