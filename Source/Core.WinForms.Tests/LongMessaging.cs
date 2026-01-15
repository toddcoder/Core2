using Core.Applications.LongMessaging;
using Core.WinForms.Controls;
using Core.WinForms.Messaging;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Tests;

public partial class LongMessaging : Form
{
   protected UiAction uiMessage = new();
   protected UiAction uiSendMessage = new();
   protected NamedPipeServerBackground background;

   public LongMessaging()
   {
      background = new NamedPipeServerBackground("test153")
      {
         Initialized =
         {
            Handler = () => uiMessage.Busy(true)
         },
         MessageSent =
         {
            Handler = msg => uiMessage.Message(msg)
         },
         Finalized =
         {
            Handler = () => uiMessage.Busy(false)
         }
      };

      InitializeComponent();

      Controls.Add(uiMessage);
      Controls.Add(uiSendMessage);

      uiMessage.Button("Click to start");
      uiMessage.Click += (_, _) => background.RunWorkerAsync();
      uiMessage.ClickText = "Click to start the IPC server";
      uiMessage.Location = new Point(10, 10);
      uiMessage.Size = new Size(200, 50);

      uiSendMessage.Button("Send Message");
      uiSendMessage.Click += async (_, _) => await NamedPipeIpc.SendAsync("test153", "Test!", nil);
      uiSendMessage.ClickText = "Send message";
      uiSendMessage.Location = new Point(10, 70);
      uiSendMessage.Size = new Size(200, 50);
   }
}