using Core.Monads;
using Core.Strings;
using Core.WinForms.TableLayoutPanels;
using System.Diagnostics;
using Core.Applications;

namespace Core.WinForms.Controls;

public partial class LabelUrl : UserControl, ILabelUiActionHost, IHasObjectId
{
   protected UiAction uiLabel = new();
   protected UiAction uiUrl = new() { UseEmojis = false, AutoSizeText = true, AllowDrop = true };
   protected TableLayoutPanel textLayoutPanel = new() { Visible = false };
   protected ExTextBox textBox = new() { TextAlign = HorizontalAlignment.Center, ForeColor = Color.Blue };
   protected UiAction uiCopy = new() { ClickGlyph = false };
   protected UiAction uiPaste = new() { ClickGlyph = false };
   protected UiAction uiOk = new() { ClickGlyph = false };
   protected UiAction uiCancel = new() { ClickGlyph = false };
   protected LabelUiActionHost<UiAction> host;
   protected bool isLocked;

   public new event EventHandler? TextChanged;
   public event EventHandler<UrlChangedArgs>? UrlChanged;
   public event EventHandler<LabelActionMessageArgs>? MessageReceived;
   public event EventHandler? Activated;
   public event EventHandler? Accepted;
   public event EventHandler? Cancelled;

   public LabelUrl(string label)
   {
      var resources = new Resources<LabelUrl>();
      var copyImage = resources.Image("Copy.png");
      var pasteImage = resources.Image("Paste.png");
      var okImage = resources.Image("CheckBoxChecked.png");
      var cancelImage = resources.Image("Cancel.png");

      InitializeComponent();

      uiLabel.Divider(label);

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

                  uiLabel.Status = StatusType.Success;
               }
               else
               {
                  uiLabel.FailureStatus("empty url");
               }
            }
            catch (Exception exception)
            {
               uiLabel.ExceptionStatus(exception);
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

                  uiLabel.Status = StatusType.Success;
               }
               else
               {
                  uiLabel.FailureStatus("empty url");
               }
            }
            catch (Exception exception)
            {
               uiLabel.ExceptionStatus(exception);
            }
         }
      };
      uiUrl.DragOver += (_, e) =>
      {
         if (e.Data is not null)
         {
            if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
               e.Effect = DragDropEffects.Copy;
            }
            else
            {
               e.Effect = DragDropEffects.None;
            }
         }
         else
         {
            e.Effect = DragDropEffects.None;
         }
      };
      uiUrl.DragDrop += (_, e) =>
      {
         if (e.Data is not null)
         {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
               var url = e.Data.GetData(DataFormats.Text)?.ToString() ?? "";
               if (url.IsNotEmpty())
               {
                  Url = url;
               }
            }
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
               var url = e.Data.GetData(DataFormats.UnicodeText)?.ToString() ?? "";
               if (url.IsNotEmpty())
               {
                  Url = url;
               }
            }
         }
      };

      Url = "";

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
            uiLabel.IsDirty = CanDirty;
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
      _ = builder.Row * 2 * 50f;
      builder.SetUp();

      (builder + uiLabel + false).Row();
      (builder + uiUrl).Next();
      (builder + textLayoutPanel + (0, 1)).Row();

      host = new LabelUiActionHost<UiAction>(tableLayoutPanel, uiLabel, uiUrl, b => b.Row * 2 * 50f);
   }

   public UiAction Label => uiLabel;

   public string LabelString
   {
      get => uiLabel.NonNullText;
      set => uiLabel.Divider(value);
   }

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
            uiUrl.IsDirty = CanDirty;
            uiUrl.Refresh();
         }
      }
   }

   public void Clear()
   {
      Branch = "";
      UpdateUrl("");
   }

   public void AddUiAction(UiAction action) => host.AddUiAction(action);

   public void AddUiActions(params UiAction[] actions)
   {
      host.AddUiActions(actions);
      textLayoutPanel.SetUpInTableLayoutPanel(tableLayoutPanel, 0, 1, actions.Length + 1);
   }

   public void ClearActions() => host.ClearActions();

   public bool ActionsVisible
   {
      get => host.ActionsVisible;
      set => host.ActionsVisible = value;
   }

   public StatusType Status
   {
      get => uiLabel.Status;
      set => uiLabel.Status = value;
   }

   public void ExceptionStatus(Exception exception) => uiLabel.ExceptionStatus(exception);

   public void FailureStatus(string message) => uiLabel.FailureStatus(message);

   public void ShowStatus(Optional<Unit> _optional, Either<string, Func<string>> failureFunc) => uiLabel.ShowStatus(_optional, failureFunc);

   public bool IsDirty
   {
      get => uiLabel.IsDirty;
      set => uiLabel.IsDirty = value;
   }

   public bool CanDirty { get; set; }

   public void HookMessageReceived()
   {
      foreach (var uiAction in host)
      {
         uiAction.MessageReceived += (_, e) => MessageReceived?.Invoke(this, new LabelActionMessageArgs(uiAction, e.Message, e.Cargo));
      }
   }

   public void SendMessage(string message, object cargo)
   {
      foreach (var uiAction in host)
      {
         uiAction.SendMessage(message, cargo);
      }
   }

   public void SendMessage(string message)
   {
      foreach (var uiAction in host)
      {
         uiAction.SendMessage(message);
      }
   }

   public void RegisterMessage(string message)
   {
      foreach (var uiAction in host)
      {
         uiAction.RegisterMessage(message);
      }
   }

   public long ObjectId { get; set; }
}