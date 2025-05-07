using Core.Applications.Messaging;

namespace Core.WinForms.Tests;

public partial class XSubscribing : Form
{
   protected XSubscriber xSubscriber = new("state-info");

   public XSubscribing()
   {
      InitializeComponent();

      xSubscriber["state"] = s =>
      {
         var payload = s.Value.String("payload");
         listBox1.Do(() => listBox1.Items.Add(payload));
      };
      xSubscriber.ExceptionRaised.Handler = exception => Text = exception.Message;
      xSubscriber.Waiting.Handler = Application.DoEvents;
      xSubscriber.StopListeningOnClose(this);
   }

   protected async void XSubscribing_Load(object sender, EventArgs e)
   {
      Show();
      Application.DoEvents();

      await xSubscriber.StartListeningAsync();
   }
}