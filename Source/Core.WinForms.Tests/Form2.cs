using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form2 : Form
{
   protected const string CAPTION = "The House of Delta Blues, Where We Play Jazz! " +
      "Intel Core i5 VPro Inside. Schema Change, Hotfix Eligible, Post Deploy, Fix, Backfill";

   protected ControlContainer<UiAction> container1 = ControlContainer<UiAction>.HorizontalContainer();
   protected ControlContainer<UiAction> container2 = ControlContainer<UiAction>.VerticalContainer();
   protected ControlContainer<UiAction> container3 = ControlContainer<UiAction>.HorizontalContainer();
   protected UiActionTextCanvas canvas = new();

   public Form2()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 60 + 180 + 60 + 120 + 100f;
      builder.SetUp();

      (builder + container1).Row();
      (builder + container2).Row();
      (builder + container3).Row();
      (builder + canvas).Row();

      var uiAction = new UiAction();
      uiAction.Ghost("Lock Test");
      var uiAlfa = uiAction;
      uiAction.StatusFaded += (_, _) =>
      {
         uiAlfa.Locked = true;
         if (container1[0] is (true, var uiBravo))
         {
            uiBravo.Enabled = true;
         }
      };
      uiAction.Click += (_, _) =>
      {
         if (uiAlfa.Locked)
         {
            MessageBox.Show(this, "You should not see this");
         }
         else
         {
            uiAlfa.Status = StatusType.Success;
         }
      };
      uiAction.ClickText = "Locking = disabling";
      container1.Add(uiAction);

      uiAction = new UiAction();
      uiAction.Button("Unlock Previous Button");
      uiAction.Enabled = false;
      uiAction.Click += (_, _) =>
      {
         uiAlfa.Locked = false;
         if (container1[1] is (true, var uiBravo))
         {
            uiBravo.Enabled = false;
         }
      };
      uiAction.ClickText = "Unlock previous button";
      container1.Add(uiAction);

      uiAction = new UiAction();
      uiAction.Button("Disable Container");
      uiAction.Click += (_, _) => container1.Enabled = false;
      uiAction.ClickText = "Disable container";
      container1.Add(uiAction);

      uiAction = new UiAction();
      uiAction.Button(CAPTION);
      var uiDelta = uiAction;
      uiAction.Click += (_, _) => uiDelta.ChooserGlyph = !uiDelta.ChooserGlyph;
      uiAction.ClickText = "Delta";
      container2.Add(uiAction);

      uiAction = new UiAction();
      uiAction.Button("Echo");
      var uiEcho = uiAction;
      uiAction.ChooserGlyph = true;
      var subText = uiEcho.SubText("charlie").Set.MiniInverted(CardinalAlignment.NorthEast).SubText;
      subText = uiEcho.SubText("bravo").Set.MiniInverted().LeftOf(subText).SubText;
      _ = uiEcho.SubText("alfa").Set.MiniInverted().LeftOf(subText).SubText;
      uiEcho.Refresh();
      uiAction.Click += (_, _) => Text = "Echo";
      uiAction.ClickText = "Echo";
      container2.Add(uiAction);

      uiAction = new UiAction();
      uiAction.Button("Foxtrot");
      uiAction.Click += (_, _) => Text = "Foxtrot";
      uiAction.ClickText = "Foxtrot";
      container2.Add(uiAction);
      uiAction.Click += (_, _) => Text = "Foxtrot";
      uiAction.ClickText = "Foxtrot";

      uiAction = new UiAction();
      uiAction.Button("Edward");
      container3.Add(uiAction);
      uiAction = new UiAction();
      uiAction.Button("Louise");
      container3.Add(uiAction);
      uiAction= new UiAction();
      uiAction.Button("Celestine");
      container3.Add(uiAction);
      uiAction = new UiAction();
      uiAction.Button("Robespierre");
      container3.Add(uiAction);

      canvas.BackColor = Color.Blue;
      canvas.WriteLine("Now is the time for[b:red]|all[i,f:black,b:yellow]|men to come to the aid of their party.");
      canvas.WriteLine("Air|quality[b]|alert.");
      canvas.Refresh();

      this.Tuck(canvas);
   }
}