﻿using System.Text;
using Core.Applications.Messaging;
using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Documents;

public class Document
{
   protected const string PATTERN_CRLF = "/r /n | /r | /n; f";

   public static string GetWindowsText(string text) => SetWindowsText(text).ToString("\r\n");

   public static string[] SetWindowsText(string text) => text.Unjoin(PATTERN_CRLF);

   public static Maybe<string> ClipboardText()
   {
      return maybe<string>() & Clipboard.ContainsText(TextDataFormat.Text) & (() => Clipboard.GetText(TextDataFormat.Text));
   }

   protected Form form;
   protected RichTextBox textBox;
   protected string extension;
   protected string documentName;
   protected string formName;
   protected Maybe<FileName> _file = nil;
   protected bool isDirty;
   protected OpenFileDialog openFileDialog;
   protected SaveFileDialog saveFileDialog;
   protected Menus menus;
   protected string fontName;
   protected float fontSize;
   protected bool displayFileName;
   protected Maybe<Colorizer> _colorizer = nil;
   protected string filter;
   protected bool keepClean;

   public MessageEvent OKButtonClicked = new();
   public MessageEvent CancelButtonClicked = new();
   public MessageEvent YesButtonClicked = new();
   public MessageEvent NoButtonClicked = new();

   public Document(Form form, RichTextBox textBox, string extension, string documentName, string fontName = "Consolas",
      float fontSize = 12f, bool displayFileName = true, string filter = "")
   {
      this.form = form;
      this.textBox = textBox;
      this.extension = extension.Substitute("^ '.'; f", "");
      this.documentName = documentName;
      this.fontName = fontName;
      this.fontSize = fontSize;
      this.displayFileName = displayFileName;
      this.filter = filter.IsEmpty() ? $"{documentName} files (*.{this.extension})|*.{this.extension}|All files (*.*)|*.*" : filter;

      formName = this.form.Text;

      this.textBox.TextChanged += (_, _) =>
      {
         if (!keepClean)
         {
            Dirty();
         }

         if (_colorizer is (true, var colorizer))
         {
            colorizer.Colorize(this.textBox);
         }
      };

      this.form.FormClosing += (_, e) => Close(e);

      openFileDialog = new OpenFileDialog
      {
         AddExtension = true,
         CheckFileExists = true,
         CheckPathExists = true,
         DefaultExt = this.extension,
         Filter = this.filter,
         RestoreDirectory = true,
         Title = $"Open {this.documentName} file"
      };
      saveFileDialog = new SaveFileDialog
      {
         AddExtension = true,
         CheckPathExists = true,
         DefaultExt = this.extension,
         Filter = openFileDialog.Filter,
         RestoreDirectory = true,
         Title = $"Save {this.documentName} file"
      };

      menus = new Menus();

      initializeTextBox();

      DisplayFileName();
   }

   public bool KeepClean
   {
      get => keepClean;
      set => keepClean = value;
   }

   public Maybe<Colorizer> Colorizer
   {
      get => _colorizer;
      set => _colorizer = value;
   }

   public void StandardFileMenu()
   {
      menus.Menu("&File");
      menus.Menu("File", "New...", (_, _) => New(), "^N");
      standardItems();
   }

   public void StandardFileMenu(EventHandler handler)
   {
      menus.Menu("&File");
      menus.Menu("File", "New", handler, "^N");
      standardItems();
   }

   protected void standardItems()
   {
      menus.Menu("File", "Open...", (_, _) => Open(), "^O");
      menus.Menu("File", "Save", (_, _) => Save(), "^S");
      menus.Menu("File", "Save As...", (_, _) => SaveAs());
      menus.Separator("File");
      menus.Menu("File", "Exit", (_, _) => form.Close(), "%F4");
   }

   public void StandardEditMenu()
   {
      menus.Menu("&Edit");
      menus.Menu("Edit", "Undo", (_, _) => Undo(), "^Z");
      menus.Menu("Edit", "Redo", (_, _) => Redo());
      menus.Separator("Edit");
      menus.Menu("Edit", "Cut", (_, _) => Cut(), "^X");
      menus.Menu("Edit", "Copy", (_, _) => Copy(), "^C");
      menus.Menu("Edit", "Paste", (_, _) => Paste(), "^V");
      menus.Menu("Edit", "Delete", (_, _) => Delete());
      menus.Separator("Edit");
      menus.Menu("Edit", "Select All", (_, _) => SelectAll(), "^A");
   }

   public void StandardMenus()
   {
      StandardFileMenu();
      StandardEditMenu();
   }

   public void StandardMenus(EventHandler fileNewHandler)
   {
      StandardFileMenu(fileNewHandler);
      StandardEditMenu();
   }

   public void StandardContextEdit() => menus.StandardContextEdit(this);

   public Menus Menus => menus;

   public void RenderMainMenu() => menus.CreateMainMenu(form);

   public void RenderContextMenu() => RenderContextMenu(textBox);

   public void RenderContextMenu(Control control) => menus.CreateContextMenu(control);

   protected void initializeTextBox()
   {
      textBox.AcceptsTab = true;
      textBox.DetectUrls = false;
      textBox.Dock = DockStyle.Fill;
      textBox.Font = new Font(fontName, fontSize, FontStyle.Regular);
      textBox.HideSelection = false;
      textBox.ShowSelectionMargin = true;
      textBox.WordWrap = false;
      textBox.ScrollBars = RichTextBoxScrollBars.Both;
   }

   public Maybe<string> FileName => _file.Map(f => f.ToString());

   public bool IsDirty => isDirty;

   public void Dirty()
   {
      isDirty = true;
      DisplayFileName();
   }

   public void Clean()
   {
      isDirty = false;
      DisplayFileName();
   }

   public virtual void New()
   {
      if (isDirty)
      {
         var result = getSaveResponse();
         switch (result)
         {
            case DialogResult.Yes:
               YesButtonClicked.Invoke();
               Save();
               break;
            case DialogResult.No:
               NoButtonClicked.Invoke();
               textBox.Clear();
               Clean();
               break;
            default:
               return;
         }
      }

      textBox.Clear();
      Clean();
      _file = nil;
      DisplayFileName();
   }

   public virtual void Open()
   {
      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
         Open(openFileDialog.FileName);
      }
   }

   public virtual bool OpenIf(out DialogResult dialogResult)
   {
      dialogResult = openFileDialog.ShowDialog();
      if (dialogResult == DialogResult.OK)
      {
         OKButtonClicked.Invoke();
         Open(openFileDialog.FileName);
         return true;
      }
      else
      {
         CancelButtonClicked.Invoke();
         return false;
      }
   }

   public virtual void Open(FileName fileName)
   {
      _file = fileName;
      if (_file is (true, var file))
      {
         textBox.Text = file.Lines.ToString("\r\n");
      }

      Clean();
   }

   public void DisplayFileName()
   {
      if (displayFileName)
      {
         var title = new StringBuilder();
         if (_file is (true, var file))
         {
            title.Append(file);
            title.Append(" - ");
            title.Append(formName);
            if (IsDirty)
            {
               title.Append(" *");
            }

            form.Text = title.ToString();
         }
         else
         {
            form.Text = formName;
         }
      }
   }

   protected string getText() => GetWindowsText(textBox.Text);

   protected void setText(string text) => textBox.Lines = SetWindowsText(text);

   public virtual void Save()
   {
      if (IsDirty)
      {
         if (_file)
         {
            save();
         }
         else
         {
            SaveAs();
         }
      }
   }

   protected virtual void save()
   {
      if (_file is (true, var file))
      {
         if (file.Exists())
         {
            file.Delete();
         }

         file.Encoding = Encoding.UTF8;
         file.Text = getText();
      }

      Clean();
   }

   public virtual void SaveAs()
   {
      if (saveFileDialog.ShowDialog() == DialogResult.OK)
      {
         OKButtonClicked.Invoke();
         _file = (FileName)saveFileDialog.FileName;
         save();
      }
      else
      {
         CancelButtonClicked.Invoke();
      }
   }

   public virtual void Close(FormClosingEventArgs e)
   {
      if (IsDirty)
      {
         var result = getSaveResponse();
         switch (result)
         {
            case DialogResult.Yes:
               YesButtonClicked.Invoke();
               Save();
               break;
            case DialogResult.Cancel:
               CancelButtonClicked.Invoke();
               e.Cancel = true;
               break;
         }
      }
   }

   protected DialogResult getSaveResponse()
   {
      var message = _file.Map(f => $"File {f} not saved") | "File not saved";
      var text = $"{documentName} File Not Saved";

      return MessageBox.Show(message, text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
   }

   public virtual void Undo() => textBox.Undo();

   public virtual void Redo()
   {
      if (textBox.CanRedo)
      {
         textBox.Redo();
      }
   }

   public virtual void Cut() => textBox.Cut();

   public virtual void Copy() => textBox.Copy();

   public virtual void Paste()
   {
      var clipFormat = DataFormats.GetFormat(DataFormats.Text);
      if (textBox.CanPaste(clipFormat))
      {
         textBox.Paste(clipFormat);
      }
   }

   public virtual void Delete()
   {
      if (textBox.SelectionLength > 0)
      {
         textBox.SelectedText = "";
      }
   }

   public virtual void SelectAll() => textBox.SelectAll();

   public void CopyTextBoxSettings(RichTextBox otherTextBox)
   {
      otherTextBox.AcceptsTab = textBox.AcceptsTab;
      otherTextBox.DetectUrls = textBox.DetectUrls;
      otherTextBox.Font = new Font(textBox.Font, FontStyle.Regular);
      otherTextBox.HideSelection = textBox.HideSelection;
      otherTextBox.ShowSelectionMargin = textBox.ShowSelectionMargin;
      otherTextBox.WordWrap = textBox.WordWrap;
      otherTextBox.ScrollBars = textBox.ScrollBars;
   }

   public void CopyTextBoxSettings(TextBox otherTextBox)
   {
      otherTextBox.AcceptsTab = textBox.AcceptsTab;
      otherTextBox.Font = new Font(textBox.Font, FontStyle.Regular);
      otherTextBox.HideSelection = textBox.HideSelection;
      otherTextBox.WordWrap = textBox.WordWrap;
      otherTextBox.ScrollBars = textBox.ScrollBars switch
      {
         RichTextBoxScrollBars.None => ScrollBars.None,
         RichTextBoxScrollBars.Horizontal or RichTextBoxScrollBars.ForcedHorizontal => ScrollBars.Horizontal,
         RichTextBoxScrollBars.Vertical or RichTextBoxScrollBars.ForcedVertical => ScrollBars.Vertical,
         RichTextBoxScrollBars.Both or RichTextBoxScrollBars.ForcedBoth => ScrollBars.Both,
         _ => otherTextBox.ScrollBars
      };
   }

   public string Text
   {
      get => getText();
      set => setText(value);
   }

   public string[] Lines
   {
      get => textBox.Lines;
      set => textBox.Lines = value;
   }

   public DocumentTrying TryTo => new(this);
}