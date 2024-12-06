using Core.Monads;
using Core.Strings;
using Core.WinForms.TableLayoutPanels;
using System.Diagnostics;

namespace Core.WinForms.Controls;

public partial class LabelUrl : UserControl, ILabelUiActionHost
{
   protected UiAction uiLabel = new();
   protected UiAction uiUrl = new() { UseEmojis = false, AutoSizeText = true };
   protected ExTextBox textBox = new() { Visible = false };
   protected LabelUiActionHost<UiAction> host;
   protected bool isLocked;

   public new event EventHandler? TextChanged;
   public event EventHandler<EventArgs>? UrlChanged;
   public event EventHandler<LabelActionMessageArgs>? MessageReceived;

   public LabelUrl(string label)
   {
      InitializeComponent();

      uiLabel.Divider(label);

      uiUrl.Font = new Font("Consolas", 1f, FontStyle.Underline);
      uiUrl.DoubleClick += (_, _) =>
      {
         if (ModifierKeys is not Keys.Control)
         {
            textBox.Text = uiUrl.Text;
            textBox.BringToFront();
            uiUrl.Visible = false;
            textBox.Visible = true;
            textBox.Focus();
         }
      };
      uiUrl.Click += (_, _) =>
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

      Url = "";

      Controls.Add(textBox);

      textBox.KeyUp += (_, e) =>
      {
         switch (e.KeyCode)
         {
            case Keys.Escape:
               textBox.Visible = false;
               uiUrl.Visible = true;
               e.Handled = true;
               break;
            case Keys.Enter:
               textBox.Visible = false;
               uiUrl.Visible = true;
               display(textBox.Text);
               e.Handled = true;
               break;
         }
      };
      textBox.LostFocus += (_, _) =>
      {
         textBox.Visible = false;
         uiUrl.Visible = true;
      };
      textBox.MouseDown += (_, _) =>
      {
         if (!textBox.ClientRectangle.Contains(PointToClient(Cursor.Position)))
         {
            textBox.Visible = false;
            uiUrl.Visible = true;
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

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row * 2 * 50f;
      builder.SetUp();

      (builder + uiLabel + false).Row();
      (builder + uiUrl).Next();
      (builder + textBox + (0, 1)).Row();

      host = new LabelUiActionHost<UiAction>(tableLayoutPanel, uiLabel, uiUrl, b => b.Row * 2 * 50f);
   }

   public UiAction Label => uiLabel;

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

   protected void display(string text)
   {
      uiUrl.Display(text, Color.Blue, Color.White);
      if (!isLocked)
      {
         UrlChanged?.Invoke(this, EventArgs.Empty);
         uiUrl.IsDirty = CanDirty;
         uiUrl.Refresh();
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
      textBox.SetUpInTableLayoutPanel(tableLayoutPanel, 0, 1, actions.Length + 1);
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
}