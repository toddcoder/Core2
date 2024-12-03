using Core.Strings;
using Core.WinForms.TableLayoutPanels;
using System.Diagnostics;

namespace Core.WinForms.Controls;

public partial class LabelUrl : UserControl, ILabelUiActionHost
{
   protected UiAction uiDivider = new();
   protected UiAction uiUrl = new() { UseEmojis = false, AutoSizeText = true };
   protected ExTextBox textBox = new() { Visible = false };
   protected LabelUiActionHost<UiAction> host;

   public event EventHandler<EventArgs>? UrlChanged;

   public LabelUrl(string label)
   {
      InitializeComponent();

      uiDivider.Divider(label);

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

                  uiDivider.Status = StatusType.Success;
               }
               else
               {
                  uiDivider.FailureStatus("empty url");
               }
            }
            catch (Exception exception)
            {
               uiDivider.ExceptionStatus(exception);
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
               break;
         }
      };
      textBox.LostFocus += (_, _) =>
      {
         textBox.Visible = false;
         uiUrl.Visible = true;
      };

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f + 160 + 160;
      _ = builder.Row * 2 * 50f;
      builder.SetUp();

      (builder + uiDivider + false).SpanCol(3).Row();
      (builder + uiUrl).SpanCol(3).Next();
      (builder + textBox + (0, 1)).SpanCol(3).Row();

      host = new LabelUiActionHost<UiAction>(tableLayoutPanel, uiDivider, uiUrl, b => b.Row * 2 * 50f);
   }

   public UiAction Label => uiDivider;

   public string Url
   {
      get => uiUrl.NonNullText;
      set
      {
         display(value);
         UrlChanged?.Invoke(this, EventArgs.Empty);
      }
   }

   public string Branch { get; set; } = "";

   protected void display(string text) => uiUrl.Display(text, Color.Blue, Color.White);

   public void Clear()
   {
      Branch = "";
      display("");
   }

   public void AddUiAction(UiAction action) => host.AddUiAction(action);

   public void AddUiActions(params UiAction[] actions) => host.AddUiActions(actions);

   public void ClearActions() => host.ClearActions();

   public bool ActionsVisible
   {
      get => host.ActionsVisible;
      set => host.ActionsVisible = value;
   }
}