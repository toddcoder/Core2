using Core.Monads;
using Core.Objects;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Controls;

public partial class LabelText : UserControl, ILabelUiActionHost
{
   protected UiAction uiLabel = new() { TabStop = true };
   protected ExTextBox textBox = new() { BorderStyle = BorderStyle.None, Font = new Font("Consolas", 12f) };
   protected bool isLocked;
   protected LabelUiActionHost<ExTextBox> host;

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

      host = new LabelUiActionHost<ExTextBox>(tableLayoutPanel, uiLabel, textBox, b => b.Row * 2 * 50f);
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

   public void AddUiAction(UiAction action) => host.AddUiAction(action);

   public void AddUiActions(params UiAction[] actions) => host.AddUiActions(actions);


   public bool ActionsVisible
   {
      get => host.ActionsVisible;
      set => host.ActionsVisible = value;
   }

   public void ClearActions() => host.ClearActions();
}