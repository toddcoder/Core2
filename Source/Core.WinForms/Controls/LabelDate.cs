using Core.Monads;
using Core.WinForms.TableLayoutPanels;
using System.ComponentModel;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public partial class LabelDate : UserControl, ILabelUiActionHost, IHasObjectId
{
   protected UiMenuAction uiLabel = new() { TabStop = true };
   protected bool isLocked;
   protected LabelUiActionHost<CoreDateTimePicker> host;
   protected Maybe<DateTimePickerFormat> _lastFormat = nil;

   public event EventHandler? ValueChanged;
   public event EventHandler<LabelActionMessageArgs>? MessageReceived;

   public LabelDate()
   {
      InitializeComponent();

      uiLabel.Divider(Label);

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
      CustomFormat = nil;
   }

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
   public string Label
   {
      get => uiLabel.NonNullText;
      set => uiLabel.Divider(value);
   }

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
   public DateTime Value
   {
      get => dateTimePicker.Value;
      set => dateTimePicker.Value = value == DateTime.MinValue ? DateTime.Today : value;
   }

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
   public Maybe<string> CustomFormat
   {
      get;
      set
      {
         field = value;
         if (field is (true, var customFormat))
         {
            _lastFormat = dateTimePicker.Format;
            dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.CustomFormat = customFormat;
         }
         else if (_lastFormat is (true, var lastFormat))
         {
            dateTimePicker.Format = lastFormat;
            _lastFormat = nil;
         }
      }
   }

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
   public bool ShowUpDown
   {
      get => dateTimePicker.ShowUpDown;
      set => dateTimePicker.ShowUpDown = value;
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

   public UiMenuAction UiLabel => uiLabel;

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
   public string LabelString
   {
      get => uiLabel.NonNullText;
      set => uiLabel.Divider(value);
   }

   public DateTimePicker DateTimePicker => dateTimePicker;

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
   public StatusType Status
   {
      get => uiLabel.Status;
      set => uiLabel.Status = value;
   }

   public void ExceptionStatus(Exception exception) => uiLabel.ExceptionStatus(exception);

   public void FailureStatus(string message) => uiLabel.FailureStatus(message);

   public void ShowStatus(Optional<Unit> _optional, Either<string, Func<string>> failureFunc) => uiLabel.ShowStatus(_optional, failureFunc);

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
   public bool IsDirty
   {
      get => uiLabel.IsDirty;
      set => uiLabel.IsDirty = value;
   }

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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

   [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
   public long ObjectId { get; set; }
}