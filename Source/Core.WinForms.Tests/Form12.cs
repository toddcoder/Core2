using Core.Applications.Messaging;
using Core.Dates.DateIncrements;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using Timeout = Core.Dates.Timeout;

namespace Core.WinForms.Tests;

public partial class Form12 : Form
{
   protected LabelText ltLeft = new("left");
   protected LabelText ltRight = new("right");
   protected UiAction uiSend = new();
   protected LabelText ltResult = new("result");
   protected UiAction uiReceive = new();

   public Form12()
   {
      InitializeComponent();

      ltLeft.UpdateLong(100);
      ltRight.UpdateLong(53);

      var messageQueue = new MessageQueue<(long, long), long>
      {
         Waiting =
         {
            Handler = Application.DoEvents
         }
      };

      uiSend.Button("Send");
      uiSend.Click += (_, _) => uiSend.RunWorkerAsync();
      uiSend.DoWork += (_, e) =>
      {
         var left = ltLeft.Long;
         var right = ltRight.Long;

         e.Result = messageQueue.Send((left, right)).ToObject();
      };
      uiSend.RunWorkerCompleted += (_, e) =>
      {
         switch (e.Result)
         {
            case long result:
               ltResult.UpdateLong(result);
               break;
            case TimeoutException:
               uiSend.FailureStatus("Timeout");
               break;
            case Exception exception:
               uiSend.ExceptionStatus(exception);
               break;
            default:
               uiSend.FailureStatus("Unknown");
               break;
         }
      };
      uiSend.ClickText = "Send";

      uiReceive.Button("Receive");
      uiReceive.Click += (_, _) =>
      {
         var messageQueue = new MessageQueue<(long, long), long>();
         Timeout timeout = 5.Minutes();
         while (timeout.IsPending())
         {
            Application.DoEvents();
            while (messageQueue.Receive() is (true, var messageEnvelope))
            {
               var (id, (left, right)) = messageEnvelope;
               var result = left + right;
               messageQueue.SendBack(id, result);
            }
         }
      };
      uiReceive.ClickText = "Receive";

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 50f + 50f;
      _ = builder.Row * 4 * 25f;
      builder.SetUp();

      (builder + ltLeft).Next();
      (builder + ltRight).Row();

      (builder + uiSend).SpanCol(2).Row();

      (builder + ltResult).SpanCol(2).Row();

      (builder + uiReceive).SpanCol(2).Row();
   }
}