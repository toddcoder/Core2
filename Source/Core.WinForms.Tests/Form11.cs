using Core.Applications.Messaging;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form11 : Form
{
   protected const string STATE_INFO = "state-info";
   protected const string USER_INFO = "user-info";
   protected const string TOPIC_USER = "user";
   protected const string TOPIC_STATE = "state";

   protected UiAction uiUserInfoPublish = new();
   protected UiAction uiUserInfoSubscribe = new();
   protected UiAction uiStatePublish = new();
   protected LabelText ltStateSubscribe = new(STATE_INFO);
   protected Subscriber<string> userInfoSubscriber = new(USER_INFO);
   protected Subscriber<string> stateSubscriber = new(STATE_INFO);
   protected Publisher<string> userInfoPublisher = new(USER_INFO);
   protected Publisher<string> statePublisher = new(STATE_INFO);

   public Form11()
   {
      InitializeComponent();

      uiUserInfoPublish.Button("Publish");
      uiUserInfoPublish.Click += (_, _) => userInfoPublisher.Publish(TOPIC_USER, Environment.UserName);
      uiUserInfoPublish.ClickText = "Publish";

      uiUserInfoSubscribe.Message("waiting");

      /*userInfoSubscriber.Received.Handler = p =>
      {
         if (p.Topic == TOPIC_USER)
         {
            uiUserInfoSubscribe.Do(() => uiUserInfoSubscribe.Success(p.Payload));
         }
      };*/
      userInfoSubscriber[TOPIC_USER] = p =>
      {
         uiUserInfoSubscribe.Do(() => uiUserInfoSubscribe.Success(p.Payload));
      };
      userInfoSubscriber.UnsubscribeOnClose(this);

      uiStatePublish.Button("Publish");
      uiStatePublish.Click += (_, _) => statePublisher.Publish(TOPIC_STATE, ltStateSubscribe.Text);
      uiStatePublish.ClickText = "Publish";

      stateSubscriber[TOPIC_STATE] = p =>
      {
         listBox1.Do(() =>
         {
            listBox1.Items.Add(p.Payload);
            ltStateSubscribe.Text = "";
         });
      };
      /*stateSubscriber.Received.Handler = p =>
      {
         if (p.Topic == TOPIC_STATE)
         {
            listBox1.Do(() =>
            {
               listBox1.Items.Add(p.Payload);
               ltStateSubscribe.Text = "";
            });
         }
      };*/
      stateSubscriber.UnsubscribeOnClose(this);

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col * 4 * 25f;
      _ = builder.Row + 60 + 60 + 100f;
      builder.SetUp();

      (builder + uiUserInfoPublish).SpanCol(1).Next();
      (builder + uiUserInfoSubscribe).SpanCol(3).Row();

      (builder + uiStatePublish).Next();
      (builder + ltStateSubscribe).Next();
      (builder + listBox1).SpanCol(2).SpanRow(2).Row();
   }
}