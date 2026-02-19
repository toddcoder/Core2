using Core.Markdown;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class MarkdownFrameTester : Form
{
   protected ExRichTextBox textSource = new() { BorderStyle = BorderStyle.None };
   protected ExRichTextBox textModifiedMarkdown = new() { BorderStyle = BorderStyle.None };
   protected ExRichTextBox textHtml = new() { BorderStyle = BorderStyle.None };
   protected UiAction uiRefresh = new();
   protected UiAction uiCopy = new();

   public MarkdownFrameTester()
   {
      InitializeComponent();

      textSource.TextChanged += (_, _) => refresh();

      uiRefresh.Button("Refresh");
      uiRefresh.Click += (_, _) => refresh();
      uiCopy.ClickText = "Refresh HTML from markdown";

      uiCopy.Button("Copy");
      uiCopy.Click += (_, _) => Clipboard.SetText(textHtml.Text.IsNotEmpty() ? textHtml.Text : "");
      uiCopy.ClickText = "Copy HTML to clipboard";

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 50f + 50f;
      _ = builder.Row + 50f + 50f + 60;
      builder.SetUp();

      (builder + textSource).Next();
      (builder + textHtml).Row();
      (builder + textModifiedMarkdown).SpanCol(2).Row();
      (builder + uiRefresh).Next();
      (builder + uiCopy).Row();

      return;

      void refresh()
      {
         var _html =
            from frame in MarkdownFrame.FromSource(textSource.Text)
            from result in frame.ToHtml()
            select (frame, result);
         if (_html is (true, var (markdownFrame, html)))
         {
            textHtml.Text = html;
            textModifiedMarkdown.Text = markdownFrame.Markdown;
         }
         else if (_html.Exception is (true, var exception))
         {
            uiRefresh.Exception(exception);
         }
         else
         {
            uiRefresh.Failure("failed");
         }
      }
   }
}