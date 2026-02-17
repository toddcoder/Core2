using Core.Markdown;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class MarkdownFrameTester : Form
{
   protected ExRichTextBox textSource = new();
   protected ExRichTextBox textHtml = new();
   protected UiAction uiCopy = new();

   public MarkdownFrameTester()
   {
      InitializeComponent();

      textSource.TextChanged += (_, _) => { textHtml.Text = MarkdownFrame.FromSource(textSource.Text).Map(m => m.ToHtml()) | ""; };

      uiCopy.Button("Copy");
      uiCopy.Click += (_, _) => Clipboard.SetText(textHtml.Text.IsNotEmpty() ? textHtml.Text : "");
      uiCopy.ClickText = "Copy HTML to clipboard";

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 50f + 50f;
      _ = builder.Row + 100f + 100;
      builder.SetUp();

      (builder + textSource).Next();
      (builder + textHtml).Row();
      (builder.SkipCol() + uiCopy).Row();
   }
}