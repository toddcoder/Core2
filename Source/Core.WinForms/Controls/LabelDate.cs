using Core.Monads;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Controls;

public partial class LabelDate : UserControl, ILabelUiActionHost
{
   protected UiAction uiLabel = new();
   protected bool isLocked;
   protected LabelUiActionHost<CoreDateTimePicker> host;

   public event EventHandler? ValueChanged;
   public event EventHandler<LabelActionMessageArgs>? MessageReceived;

   public LabelDate(string label)
   {
      InitializeComponent();

      uiLabel.Divider(label);

      dateTimePicker.Format = DateTimePickerFormat.Short;
      dateTimePicker.Value = DateTime.Now;
      dateTimePicker.ValueChanged += (_, _) =>
      {
         if (!isLocked)
         {
            ValueChanged?.Invoke(this, EventArgs.Empty);
            uiLabel.IsDirty = CanDirty;
         }
      };

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row * 2 * 50f;
      builder.SetUp();

      (builder + uiLabel + false).Row();
      (builder + dateTimePicker).Row();

      host = new LabelUiActionHost<CoreDateTimePicker>(tableLayoutPanel, uiLabel, dateTimePicker, b => b.Row * 2 * 50f);
   }

   public DateTime Value
   {
      get => dateTimePicker.Value;
      set => dateTimePicker.Value = value == DateTime.MinValue ? DateTime.Today : value;
   }

   public void UpdateValue(DateTime dateTime)
   {
      try
      {
         isLocked = true;
         Value = dateTime;
      }
      finally
      {
         isLocked = false;
      }
   }

   public UiAction Label => uiLabel;

   public string LabelString
   {
      get => uiLabel.NonNullText;
      set => uiLabel.Divider(value);
   }

   public DateTimePicker DateTimePicker => dateTimePicker;

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

   public void AddUiAction(UiAction action) => host.AddUiAction(action);

   public void AddUiActions(params UiAction[] actions) => host.AddUiActions(actions);

   public void HookMessageReceived()
   {
      foreach (var uiAction in host)
      {
         uiAction.MessageReceived += (_, e) => MessageReceived?.Invoke(this, new LabelActionMessageArgs(uiAction, e.Message, e.Cargo));
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