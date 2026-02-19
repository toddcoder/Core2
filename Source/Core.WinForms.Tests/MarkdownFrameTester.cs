using Core.Collections;
using Core.Markdown;
using Core.Matching;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class MarkdownFrameTester : Form
{
   protected ExRichTextBox textSource = new() { BorderStyle = BorderStyle.None };
   protected ExRichTextBox textModifiedMarkdown = new() { BorderStyle = BorderStyle.None };
   protected ExRichTextBox textHtml = new() { BorderStyle = BorderStyle.None };
   protected ExRichTextBox textReplacements = new() { BorderStyle = BorderStyle.None };
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
      (builder + textModifiedMarkdown).Row();
      (builder + textReplacements).Next();
      (builder + textHtml).Row();
      (builder + uiRefresh).Next();
      (builder + uiCopy).Row();

      return;

      void refresh()
      {
         StringHash scalarReplacements = [];
         StringHash<IEnumerable<string>> multipleReplacements = [];
         MarkdownFrame.ScalarReplacement.Handler = arg => arg.Value = scalarReplacements.Maybe[arg.Key] | "";
         MarkdownFrame.MultipleReplacements.Handler = arg => arg.Values = multipleReplacements.Maybe[arg.Key] | [];
         updateReplacements(scalarReplacements, multipleReplacements);

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

      void updateReplacements(StringHash scalarReplacements, StringHash<IEnumerable<string>> multipleReplacements)
      {
         foreach (var line in textReplacements.Lines)
         {
            if (line.Matches("^ /(['a-z_'][/w '-']*) /s* '->' /s* /(.+) $; f") is (true, var result))
            {
               var key = result.FirstGroup;
               var value = result.SecondGroup;
               if (value.StartsWith('[') && value.EndsWith("]"))
               {
                  var values = value.Drop(1).Drop(-1).Unjoin("/s* ',' /s*; f");
                  multipleReplacements[key] = values;
               }
               else
               {
                  scalarReplacements[key] = value;
               }
            }
         }
      }
   }
}