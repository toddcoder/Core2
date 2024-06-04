using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.WinForms.TableLayoutPanels.BuilderFunctions;

namespace Core.WinForms.Tests;

public partial class Form2 : Form
{
   protected const string CAPTION = "The House of Delta Blues, Where We Play Jazz! " +
      "Intel Core i5 VPro Inside. Schema Change, Hotfix Eligible, Post Deploy, Fix, Backfill";

   protected UiActionContainer container1 = UiActionContainer.HorizontalContainer();
   protected UiActionContainer container2 = UiActionContainer.VerticalContainer();

   public Form2()
   {
      InitializeComponent();

      var builder = new Builder(tableLayoutPanel);
      _ = builder + 100.ColPercent();
      _ = builder + 60.RowPixels() + 180.RowPixels() + 100.RowPercent() + setup;

      _ = builder + container1 + (0, 0) + row;
      _ = builder + container2 + control;

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
   }
}