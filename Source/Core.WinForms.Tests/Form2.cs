using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form2 : Form
{
   protected const string CAPTION = "The House of Delta Blues, Where We Play Jazz! " +
      "Intel Core i5 VPro Inside. Schema Change, Hotfix Eligible, Post Deploy, Fix, Backfill";

   protected UiActionContainer container1 = UiActionContainer.HorizontalContainer();
   protected UiActionContainer container2 = UiActionContainer.VerticalContainer();
   protected UiActionContainer container3 = UiActionContainer.HorizontalContainer();
   protected UiActionTextCanvas canvas = new();

   public Form2()
   {
      InitializeComponent();

      var builder = new Builder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 60 + 180 + 60 + 120 + 100f;
      builder.SetUp();

      (builder + container1).Row();
      (builder + container2).Row();
      (builder + container3).Row();
      (builder + canvas).Row();

      var uiAction = container1.Add("Lock Test");
      var uiAlfa = uiAction;
      uiAction.StatusFaded += (_, _) =>
      {
         uiAlfa.Locked = true;
         if (container1["Unlock Previous Button"] is (true, var uiBravo))
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

      uiAction = container1.Add("Unlock Previous Button");
      uiAction.Enabled = false;
      uiAction.Click += (_, _) =>
      {
         uiAlfa.Locked = false;
         if (container1["Unlock Previous Button"] is (true, var uiBravo))
         {
            uiBravo.Enabled = false;
         }
      };
      uiAction.ClickText = "Unlock previous button";

      uiAction = container1.Add("Disable Container");
      uiAction.Click += (_, _) => container1.Enabled = false;
      uiAction.ClickText = "Disable container";

      uiAction = container2.Add(CAPTION);
      var uiDelta = uiAction;

      uiAction.Click += (_, _) => uiDelta.ChooserGlyph = !uiDelta.ChooserGlyph;
      uiAction.ClickText = "Delta";

      uiAction = container2.Add("Echo");
      var uiEcho = uiAction;
      uiAction.ChooserGlyph = true;
      var subText = uiEcho.SubText("charlie").Set.MiniInverted(CardinalAlignment.NorthEast).SubText;
      subText = uiEcho.SubText("bravo").Set.MiniInverted().LeftOf(subText).SubText;
      _ = uiEcho.SubText("alfa").Set.MiniInverted().LeftOf(subText).SubText;
      uiEcho.Refresh();

      uiAction.Click += (_, _) => Text = "Echo";
      uiAction.ClickText = "Echo";

      uiAction = container2.Add("Foxtrot");
      uiAction.Click += (_, _) => Text = "Foxtrot";
      uiAction.ClickText = "Foxtrot";

      container3.Add("Edward", false);
      container3.Add("Louise", false);
      container3.Add("Celestine", false);
      container3.Add("Robespierre", false);

      canvas.BackColor = Color.Blue;
      canvas.WriteLine("Now is the time for[b:red]|all[i,f:black,b:yellow]|men to come to the aid of their party.");
      canvas.WriteLine("Air|quality[b]|alert.");
      canvas.Refresh();
   }
}