using System.Text;
using Core.Collections;
using Core.Computers;
using Core.Markdown;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Tests;

public partial class MarkdownFrameTester : Form
{
   protected const string REGEX_SCALAR = @"^([a-z_][\w-]*)\s*:\s*(.+)$; u";
   protected const string REGEX_MULTI_BEGIN = @"^([a-z_][\w-]*)\s*\[(.+)$; u";
   protected const string REGEX_MULTI_END = @"^\]$; u";
   protected const string REGEX_INCLUDE = @"^([a-z_][\w-]*)([+-])$; u";

   protected ExRichTextBox textSource = new() { BorderStyle = BorderStyle.None };
   protected ExRichTextBox textModifiedMarkdown = new() { BorderStyle = BorderStyle.None };
   protected ExRichTextBox textHtml = new() { BorderStyle = BorderStyle.None };
   protected ExRichTextBox textReplacements = new() { BorderStyle = BorderStyle.None };
   protected UiAction uiRefresh = new();
   protected UiAction uiCopy = new();
   protected UiAction uiOpen = new();
   protected FileName codeFile = @"C:\Temp\code.md";
   protected FileName replacementsFile = @"C:\Temp\replacements.txt";
   protected FileName testFile = @"C:\Temp\test.html";

   public MarkdownFrameTester()
   {
      InitializeComponent();

      textSource.TextChanged += (_, _) => refresh();

      uiRefresh.Button("Refresh");
      uiRefresh.Click += (_, _) => refresh();
      uiCopy.ClickText = "Refresh HTML from markdown";

      uiCopy.Button("Copy");
      uiCopy.Click += (_, _) =>
      {
         Clipboard.SetText(textHtml.Text.IsNotEmpty() ? textHtml.Text : "");
         saveText();
      };
      uiCopy.ClickText = "Copy HTML to clipboard";

      uiOpen.Button("Open");
      uiOpen.Click += (_, _) =>
      {
         var html = textHtml.Text;
         if (html.IsNotEmpty())
         {
            testFile.TryTo.SetText(html, Encoding.UTF8);
            testFile.Open();
         }
      };

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 50f + 25f + 25f;
      _ = builder.Row + 50f + 50f + 60;
      builder.SetUp();

      (builder + textSource).Next();
      (builder + textModifiedMarkdown).SpanCol(2).Row();
      (builder + textReplacements).Next();
      (builder + textHtml).SpanCol(2).Row();
      (builder + uiRefresh).Next();
      (builder + uiCopy).Next();
      (builder + uiOpen).Row();

      if (codeFile)
      {
         textSource.Text = codeFile.TryTo.Text | "";
      }

      if (replacementsFile)
      {
         textReplacements.Text = replacementsFile.TryTo.Text | "";
      }

      return;

      void refresh()
      {
         StringHash scalarReplacements = [];
         StringHash<Replacements> multipleReplacements = [];
         StringSet included = [];
         updateReplacements(scalarReplacements, multipleReplacements, included);
         var options = new MarkdownFrameTestOptions(textSource.Text, true, scalarReplacements, multipleReplacements, included);

         var _html =
            from frame in MarkdownFrame.FromSource(options)
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

      void updateReplacements(StringHash scalarReplacements, StringHash<Replacements> multiLineReplacements, StringSet included)
      {
         var key = "";
         Maybe<Replacements> _replacements = nil;

         foreach (var line in textReplacements.Lines)
         {
            if (line.Matches(REGEX_SCALAR) is (true, var scalarResult))
            {
               key = scalarResult.FirstGroup;
               var value = scalarResult.SecondGroup;
               scalarReplacements[key] = value;
            }
            else if (line.Matches(REGEX_MULTI_BEGIN) is (true, var beginResult))
            {
               key = beginResult.FirstGroup;
               string[] keyNames = [.. beginResult.SecondGroup.Split(',').Select(s => s.Trim())];
               _replacements = new Replacements(keyNames);
            }
            else if (line.Matches(REGEX_MULTI_END))
            {
               multiLineReplacements.Maybe[key] = _replacements;
            }
            else if (line.Matches(REGEX_INCLUDE) is (true, var includeResult))
            {
               key = includeResult.FirstGroup;
               var include = includeResult.SecondGroup == "+";
               if (include)
               {
                  included.Add(key);
               }
            }
            else if (_replacements is (true, var replacements))
            {
               //replacements.AddTemplateLine(line);
            }
         }
      }
   }

   protected void MarkdownFrameTester_FormClosing(object sender, FormClosingEventArgs e) => saveText();

   protected void saveText()
   {
      codeFile.TryTo.SetText(textSource.Text, Encoding.UTF8);
      replacementsFile.TryTo.SetText(textReplacements.Text, Encoding.UTF8);
      testFile.TryTo.SetText(textHtml.Text, Encoding.UTF8);
   }
}