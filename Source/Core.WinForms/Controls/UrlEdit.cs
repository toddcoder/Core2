using Core.Applications;
using Core.Strings;
using Core.WinForms.TableLayoutPanels;
using System.Diagnostics;

namespace Core.WinForms.Controls;

public partial class UrlEdit : UserControl, IHasObjectId
{
   protected UiAction uiUrl = new() { UseEmojis = false, AutoSizeText = true };
   protected TableLayoutPanel textLayoutPanel = new() { Visible = false };
   protected ExTextBox textBox = new() { TextAlign = HorizontalAlignment.Center, ForeColor = Color.Blue };
   protected UiAction uiCopy = new() { ClickGlyph = false };
   protected UiAction uiPaste = new() { ClickGlyph = false };
   protected UiAction uiOk = new() { ClickGlyph = false };
   protected UiAction uiCancel = new() { ClickGlyph = false };
   protected bool isLocked;

   public new event EventHandler? TextChanged;
   public event EventHandler<UrlChangedArgs>? UrlChanged;
   public event EventHandler? Activated;
   public event EventHandler? Accepted;
   public event EventHandler? Cancelled;

   public UrlEdit()
   {
      var resources = new Resources<LabelUrl>();
      var copyImage = resources.Image("Copy.png");
      var pasteImage = resources.Image("Paste.png");
      var okImage = resources.Image("CheckBoxChecked.png");
      var cancelImage = resources.Image("Cancel.png");

      InitializeComponent();

      uiUrl.Font = new Font("Consolas", 1f, FontStyle.Underline);
      uiUrl.DoubleClick += (_, _) =>
      {
         if (ModifierKeys is Keys.Control)
         {
            openUrl(uiUrl.NonNullText);
         }
         else if (uiUrl.NonNullText.IsNotEmpty())
         {
            Clipboard.SetText(uiUrl.NonNullText);
            uiUrl.Status = StatusType.Done;
         }

         return;

         void openUrl(string url)
         {
            try
            {
               if (url.IsNotEmpty())
               {
                  using var process = new Process();
                  process.StartInfo.UseShellExecute = true;
                  process.StartInfo.FileName = url;
                  process.Start();
               }
            }
            catch
            {
            }
         }
      };
      uiUrl.Click += (_, _) =>
      {
         if (ModifierKeys is Keys.Control)
         {
            openUrl(uiUrl.NonNullText);
         }
         else
         {
            Activated?.Invoke(this, EventArgs.Empty);

            textBox.Text = uiUrl.Text;
            textBox.BringToFront();
            uiUrl.Visible = false;
            textLayoutPanel.Visible = true;
            textBox.Focus();
         }

         return;

         void openUrl(string url)
         {
            try
            {
               if (url.IsNotEmpty())
               {
                  using var process = new Process();
                  process.StartInfo.UseShellExecute = true;
                  process.StartInfo.FileName = url;
                  process.Start();
               }
            }
            catch
            {
            }
         }
      };

      textBox.ZeroOut();
      textBox.KeyUp += (_, e) =>
      {
         switch (e.KeyCode)
         {
            case Keys.Escape:
               textLayoutPanel.Visible = false;
               uiUrl.Visible = true;
               e.Handled = true;
               Cancelled?.Invoke(this, EventArgs.Empty);
               break;
            case Keys.Enter:
               textLayoutPanel.Visible = false;
               uiUrl.Visible = true;
               display(textBox.Text);
               e.Handled = true;
               Accepted?.Invoke(this, EventArgs.Empty);
               break;
            case Keys.C when e.Control && uiUrl.NonNullText.IsNotEmpty():
               Clipboard.SetText(uiUrl.NonNullText);
               uiUrl.Status = StatusType.Done;
               break;
            case Keys.V when e.Control && Clipboard.ContainsText():
               display(Clipboard.GetText());
               uiUrl.Status = StatusType.Done;
               break;
         }
      };
      textBox.TextChanged += (_, _) =>
      {
         if (!isLocked)
         {
            TextChanged?.Invoke(this, EventArgs.Empty);
         }
      };

      uiCopy.Button("");
      uiCopy.Image = copyImage;
      uiCopy.ZeroOut();
      uiCopy.Click += (_, _) =>
      {
         if (textBox.Text.IsNotEmpty())
         {
            Clipboard.SetText(textBox.Text);
            uiUrl.Status = StatusType.Done;

            textLayoutPanel.Visible = false;
            uiUrl.Visible = true;
         }
      };
      uiCopy.ClickText = "Copy text";

      uiPaste.Button("");
      uiPaste.Image = pasteImage;
      uiPaste.ZeroOut();
      uiPaste.Click += (_, _) =>
      {
         if (Clipboard.ContainsText())
         {
            display(Clipboard.GetText());
            uiUrl.Status = StatusType.Done;

            textLayoutPanel.Visible = false;
            uiUrl.Visible = true;
         }
      };
      uiPaste.ClickText = "Paste text";

      uiOk.Button("");
      uiOk.Image = okImage;
      uiOk.ZeroOut();
      uiOk.Click += (_, _) =>
      {
         textLayoutPanel.Visible = false;
         uiUrl.Visible = true;
         display(textBox.Text);
         Accepted?.Invoke(this, EventArgs.Empty);
      };
      uiOk.ClickText = "Accept URL changes";

      uiCancel.Button("");
      uiCancel.Image = cancelImage;
      uiCancel.ZeroOut();
      uiCancel.Click += (_, _) =>
      {
         textLayoutPanel.Visible = false;
         uiUrl.Visible = true;
         Cancelled?.Invoke(this, EventArgs.Empty);
      };
      uiCancel.ClickText = "Cancel URL changes";

      var builder = new TableLayoutBuilder(textLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Col * 4 * 40;
      _ = builder.Row + 100f;
      builder.SetUp();

      (builder + textBox).Next();
      (builder + uiCopy).Next();
      (builder + uiPaste).Next();
      (builder + uiOk).Next();
      (builder + uiCancel).Row();

      builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 100f;
      builder.SetUp();

      (builder + uiUrl).Next();
      (builder + textLayoutPanel + (0, 0)).Row();
   }

   public long ObjectId { get; set; }

   public string Url
   {
      get => uiUrl.NonNullText;
      set => display(value);
   }

   public void UpdateUrl(string url)
   {
      try
      {
         isLocked = true;
         display(url);
      }
      finally
      {
         isLocked = false;
      }
   }

   public string Branch { get; set; } = "";

   protected void display(string url)
   {
      var args = new UrlChangedArgs(url);
      UrlChanged?.Invoke(this, args);
      if (!args.Cancel)
      {
         url = args.Url;

         uiUrl.Display(url, Color.Blue, Color.White);
         textBox.Text = url;
         if (!isLocked)
         {
            uiUrl.Refresh();
         }
      }
   }

   public void Clear()
   {
      Branch = "";
      UpdateUrl("");
   }
}