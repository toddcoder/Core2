using System.Text;
using Core.Computers;
using Core.Dates.DateIncrements;
using Core.Enumerables;
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
   protected const string REGEX_RAW_MARKDOWN_BEGIN = "^([a-z_][\\w-]*)<$; u";
   protected const string REGEX_RAW_MARKDOWN_END = "^>$; u";

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
      uiRefresh.ClickText = "Refresh HTML from markdown";
      uiRefresh.Tick += (_, _) => uiRefresh.Button("Refresh");

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
         var scalarReplacements = new ScalarReplacements();
         MultiReplacements multipleReplacements = new();
         updateReplacements(scalarReplacements, multipleReplacements);
         var options = new MarkdownFrameTestOptions(textSource.Text, true, scalarReplacements, multipleReplacements, []);

         var _html =
            from frame in MarkdownFrame.Create(options)
            from result in frame.ToHtml()
            select (frame, result, frame.Variables);
         if (_html is (true, var (markdownFrame, html, variables)))
         {
            textHtml.Text = html;
            textModifiedMarkdown.Text = markdownFrame.Markdown;
            Text = variables.Select(i => $"{i.Key}: {i.Value}").ToString(", ");
         }
         else if (_html.Exception is (true, var exception))
         {
            uiRefresh.Exception(exception);
            uiRefresh.StartTimer(10.Seconds(), true);
         }
         else
         {
            uiRefresh.Failure("failed");
         }
      }

      void updateReplacements(ScalarReplacements scalarReplacements, MultiReplacements multiLineReplacements)
      {
         Maybe<string> _rawMarkdown = nil;
         List<string> dataLines = [];

         foreach (var line in textReplacements.Lines)
         {
            if (line.Matches(REGEX_SCALAR) is (true, var scalarResult))
            {
               var key = scalarResult.FirstGroup;
               var value = scalarResult.SecondGroup;
               scalarReplacements[key] = value;
            }
            else if (line.Matches(REGEX_MULTI_BEGIN) is (true, var beginResult))
            {
               var key = beginResult.FirstGroup;
               string[] keyNames = [.. beginResult.SecondGroup.Split(',').Select(s => s.Trim())];
               multiLineReplacements.Begin(key, keyNames);
            }
            else if (line.Matches(REGEX_MULTI_END))
            {
               multiLineReplacements.Commit();
            }
            else if (line.Matches(REGEX_RAW_MARKDOWN_BEGIN) is (true, var rawMarkdownBeginResult))
            {
               _rawMarkdown = rawMarkdownBeginResult.FirstGroup;
               dataLines.Clear();
            }
            else if (line.Matches(REGEX_RAW_MARKDOWN_END))
            {
               if (_rawMarkdown is (true, var rawMarkdownKey))
               {
                  scalarReplacements[rawMarkdownKey] = dataLines.ToString(Environment.NewLine);
                  dataLines.Clear();
               }

               _rawMarkdown = nil;
            }
            else if (multiLineReplacements.Transacting)
            {
               multiLineReplacements.CurrentReplacements.Begin();
               foreach (var (key, value) in multiLineReplacements.CurrentReplacements.Keys.Zip(line.Split(',')))
               {
                  multiLineReplacements.CurrentReplacements[key] = value;
               }

               multiLineReplacements.CurrentReplacements.Commit();
            }
            else if (_rawMarkdown)
            {
               dataLines.Add(line);
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