using Core.Enumerables;
using Core.Monads;
using Core.Objects;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Controls;

public partial class LabelText : UserControl
{
   protected const int PADDING = 2;

   protected UiAction uiLabel = new() { TabStop = true };
   protected ExTextBox textBox = new() { BorderStyle = BorderStyle.None, Font = new Font("Consolas", 12f) };
   protected bool isLocked;
   protected List<UiAction> actions = [];

   public new event EventHandler? TextChanged;

   public LabelText(string label)
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
      textBox.GotFocus += (_, _) => textBox.SelectAll();
      textBox.Click += (_, _) => textBox.SelectAll();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row * 2 * 50f;
      builder.SetUp();

      (builder + uiLabel + false).Row();
      (builder + textBox).Row();
   }

   public new string Text
   {
      get => textBox.Get(() => textBox.Text);
      set => textBox.Do(() => textBox.Text = value);
   }

   public long Long
   {
      get => textBox.Get(() => textBox.Text.Maybe().Int64() | 0L);
      set => textBox.Do(() => textBox.Text = value.ToString());
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

   public void UpdateLong(long value)
   {
      try
      {
         isLocked = true;
         textBox.Text = value.ToString();
      }
      finally
      {
         isLocked = false;
      }
   }

   public UiAction Label => uiLabel;

   public ExTextBox TextBox => textBox;

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

   public void AddUiAction(UiAction action)
   {
      action.SizeToText();
      actions.Add(action);

      rearrangeActions();
   }

   public void AddUiActions(params UiAction[] actions)
   {
      foreach (var uiAction in actions)
      {
         uiAction.SizeToText();
         this.actions.Add(uiAction);
      }

      rearrangeActions();
   }

   protected void rearrangeActions()
   {
      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      foreach (var uiAction in actions)
      {
         _ = builder.Col + uiAction.Width;
      }

      _ = builder.Row * 2 * 50f;
      builder.SetUp();

      (builder + uiLabel).Next();
      foreach (var uiAction in actions.Take(actions.Count - 1))
      {
         (builder + uiAction).Next();
      }

      (builder + actions[^1]).Row();
      (builder + textBox).SpanCol(actions.Count + 1).Row();
   }

   public bool ActionsVisible
   {
      get => actions.Count != 0 && actions.Select(a => a.Visible).Fold(bool (b, a) => b | false && a) | false;
      set
      {
         foreach (var uiAction in actions)
         {
            uiAction.Visible = value;
         }
      }
   }

   public void ClearActions() => actions.Clear();
}