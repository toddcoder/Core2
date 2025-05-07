using Core.Applications.Messaging;
using Core.Configurations;
using Core.WinForms.Controls;
using Core.WinForms.Forms;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form11 : Form
{
   protected enum Topic
   {
      User,
      State
   }

   protected const string STATE_INFO = "state-info";
   protected const string USER_INFO = "user-info";

   protected UiAction uiUserInfoPublish = new();
   protected UiAction uiUserInfoSubscribe = new();
   protected UiAction uiStatePublish = new();
   protected UiAction uiXPublish = new();
   protected LabelText ltStateSubscribe = new(STATE_INFO);
   protected Subscriber<Topic, string> userInfoSubscriber = new(USER_INFO);
   protected Subscriber<Topic, string> stateSubscriber = new(STATE_INFO);
   protected Publisher<Topic, string> userInfoPublisher = new(USER_INFO);
   protected Publisher<Topic, string> statePublisher = new(STATE_INFO);
   protected XPublisher xPublisher = new("state-info");
   protected SingletonForm<XSubscribing> xSubscribing = new(() => new XSubscribing());

   public Form11()
   {
      InitializeComponent();

      uiUserInfoPublish.Button("Publish");
      uiUserInfoPublish.Click += (_, _) => userInfoPublisher.Publish(Topic.User, Environment.UserName);
      uiUserInfoPublish.ClickText = "Publish";

      uiUserInfoSubscribe.Message("waiting");

      userInfoSubscriber[Topic.User] = p => { uiUserInfoSubscribe.Do(() => uiUserInfoSubscribe.Success(p.Payload)); };
      userInfoSubscriber.UnsubscribeOnClose(this);

      uiStatePublish.Button("Publish");
      uiStatePublish.Click += (_, _) => statePublisher.Publish(Topic.State, ltStateSubscribe.Text);
      uiStatePublish.ClickText = "Publish";

      uiXPublish.Button("X-Publish");
      uiXPublish.Click += async (_, _) =>
      {
         xSubscribing.Show();
         var setting = new Setting();
         setting.Set("payload").String = ltStateSubscribe.Text;
         await xPublisher.PublishAsync("state", setting);
      };
      uiXPublish.ClickText = "X-Publish";

      stateSubscriber[Topic.State] = p =>
      {
         listBox1.Do(() =>
         {
            listBox1.Items.Add(p.Payload);
            ltStateSubscribe.Text = "";
         });
      };
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

      (builder + uiXPublish).Next();
   }
}