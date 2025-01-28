using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form10 : Form
{
   protected ControlContainer<UiAction> container = ControlContainer<UiAction>.ReadingContainer();
   protected UiAction uiAdd = new();

   public Form10()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f + 100;
      _ = builder.Row + 100f + 40;
      builder.SetUp();

      uiAdd.Button("/plus");
      uiAdd.Click += (_, _) =>
      {
         var uiAction = new UiAction();
         uiAction.Message(container.Count.ToString());
         uiAction.ZeroOut();
         container.Add(uiAction);
      };

      (builder + container).SpanCol(2).Row();
      (builder.SkipCol() + uiAdd).Row();
   }
}