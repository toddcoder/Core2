using Core.Matching;
using Core.WinForms.Controls;
using Core.WinForms.Drawing;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form9 : Form
{
   protected LabelUrl luUrl1 = new("url 1");
   protected LabelUrl luUrl2 = new("url 2");

   public Form9()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 60 + 60 + 100f;
      builder.SetUp();

      var uiSuccess = new UiAction();
      uiSuccess.ZeroOut();
      uiSuccess.Success("success");

      var uiFailure = new UiAction();
      uiFailure.ZeroOut();
      uiFailure.Failure("failure");

      luUrl1.Url = "http://google.com";
      luUrl1.UrlChanged += (_, e) =>
      {
         var url = e.Url;
         if (url.Matches(@"&view=\w+$; u") is (true, var result1))
         {
            result1.ZerothGroup = "";
            e.Url = result1.Text;
         }
         else if (url.Matches(@"\?_a=overview$; u") is (true, var result2))
         {
            result2.ZerothGroup = "";
            e.Url = result2.Text;
         }
      };
      luUrl2.Url = "http://google.com";
      luUrl2.AddUiActions(uiSuccess, uiFailure);

      var uiCanvas = new UiAction();
      uiCanvas.Painting += (_, e) =>
      {
         var size = new Size(150, 50);
         var referenceRectangle = new ReferenceRectangle(e.ClipRectangle, size);

         referenceRectangle.NorthWest();
         drawRectangle("northwest", referenceRectangle.Result);

         referenceRectangle.ReferencedSize = size;
         referenceRectangle.Center();
         drawRectangle("center", referenceRectangle.Result);

         referenceRectangle.ReferencedSize = size;
         referenceRectangle.SouthEast();
         drawRectangle("southeast", referenceRectangle.Result);

         referenceRectangle.ReferencedSize = size;
         referenceRectangle.North();
         drawRectangle("north", referenceRectangle.Result);

         referenceRectangle.ReferencedSize = size;
         referenceRectangle.South();
         drawRectangle("south", referenceRectangle.Result);

         referenceRectangle.ReferencedSize = size;
         referenceRectangle.West();
         drawRectangle("west", referenceRectangle.Result);

         referenceRectangle.ReferencedSize = size;
         referenceRectangle.East();
         drawRectangle("east", referenceRectangle.Result);

         referenceRectangle.ReferencedSize = size;
         referenceRectangle.SouthWest();
         drawRectangle("southwest", referenceRectangle.Result);

         referenceRectangle.ReferencedSize = size;
         referenceRectangle.NorthEast();
         drawRectangle("northeast", referenceRectangle.Result);

         referenceRectangle.ReferencedSize = new Size(16, 16);
         referenceRectangle.NorthEast();
         drawRectangleWithColor("?", referenceRectangle.Result, Color.White, Color.Blue);

         return;

         void drawRectangle(string text, Rectangle rectangle) => drawRectangleWithColor(text, rectangle, Color.White, Color.Green);

         void drawRectangleWithColor(string text, Rectangle rectangle, Color foreColor, Color backColor)
         {
            using var brush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(backColor, rectangle);
            var writer = new RectangleWriter(text, rectangle) { ForeColor = foreColor };
            writer.Write(e.Graphics);
         }
      };

      (builder + luUrl1).Row();
      (builder + luUrl2).Row();
      (builder + uiCanvas).Row();
   }
}