using Core.Monads;
using Core.WinForms.TableLayoutPanels;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public partial class LabelRichText : UserControl, ILabelUiActionHost
{
   protected UiAction uiLabel = new() { TabStop = true };
   protected ExRichTextBox textBox = new() { BorderStyle = BorderStyle.None, Font = new Font("Consolas", 12f) };
   protected UiAction uiCopy = new();
   protected UiAction uiPaste = new();
   protected UiAction uiSelectAll = new();
   protected UiAction uiItalic = new();
   protected UiAction uiBold = new();
   protected UiAction uiUnderline = new();
   protected bool isLocked;
   protected Maybe<(int start, int length)> _selection = nil;
   protected LabelUiActionHost<ExRichTextBox> host;

   public new event EventHandler? TextChanged;
   public event EventHandler<UiActionMessageArgs>? MessageReceived;

   public LabelRichText(string label)
   {
      InitializeComponent();

      uiLabel.Divider(label);

      textBox.Text = "";
      textBox.TextChanged += (_, _) =>
      {
         if (!isLocked)
         {
            TextChanged?.Invoke(this, EventArgs.Empty);
            uiLabel.IsDirty = CanDirty;
         }
      };
      textBox.LostFocus += (_, _) => _selection = textBox.Selection;
      textBox.GotFocus += (_, _) =>
      {
         if (_selection is (true, var selection))
         {
            textBox.Selection = selection;
         }
      };

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 40 + 100f;
      builder.SetUp();

      (builder + uiLabel + false).Row();
      (builder + textBox).Row();

      host = new LabelUiActionHost<ExRichTextBox>(tableLayoutPanel, uiLabel, textBox, b => b.Row + 40 + 100f);
   }

   public new string Text
   {
      get => textBox.Text;
      set => textBox.Text = value;
   }

   public string Rtf
   {
      get => textBox.Rtf ?? "";
      set => textBox.Rtf = value;
   }

   public string[] Lines
   {
      get => textBox.Lines;
      set => textBox.Lines = value;
   }

   public void UpdateText(string text)
   {
      try
      {
         isLocked = true;
         textBox.Text = text;
      }
      finally
      {
         isLocked = false;
      }
   }

   public void UpdateRtf(string rtf)
   {
      try
      {
         isLocked = true;
         textBox.Rtf = rtf;
      }
      finally
      {
         isLocked = false;
      }
   }

   public void UpdateLines(string[] lines)
   {
      try
      {
         isLocked = true;
         textBox.Lines = lines;
      }
      finally
      {
         isLocked = false;
      }
   }

   public UiAction Label => uiLabel;

   public ExRichTextBox TextBox => textBox;

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

   public bool CanDirty { get; set; } = true;

   public void AddUiAction(UiAction action) => host.AddUiAction(action);

   public void AddUiActions(params UiAction[] actions) => host.AddUiActions(actions);

   public void HookMessageReceived()
   {
      foreach (var uiAction in host)
      {
         uiAction.MessageReceived += (_, e) => MessageReceived?.Invoke(this, e);
      }
   }

   public bool ActionsVisible
   {
      get => host.ActionsVisible;
      set => host.ActionsVisible = value;
   }

   public void ClearActions() => host.ClearActions();

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