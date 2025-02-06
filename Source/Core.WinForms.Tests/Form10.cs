using Core.DataStructures;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form10 : Form
{
   protected ControlContainer<UiAction> container = ControlContainer<UiAction>.ReadingContainer();
   protected UiAction uiRemove = new();
   protected UiAction uiAdd = new();
   protected MaybeStack<UiAction> stack = [];

   public Form10()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f + 100 + 100;
      _ = builder.Row + 100f + 40;
      builder.SetUp();

      uiRemove.Button("/minus");
      uiRemove.Click += (_, _) =>
      {
         if (stack.Pop() is (true, var uiLast))
         {
            container.Remove(uiLast);
         }
      };

      uiAdd.Button("/plus");
      uiAdd.Click += (_, _) =>
      {
         var uiAction = new UiAction();
         uiAction.Message(container.Count.ToString());
         uiAction.ZeroOut();
         container.Add(uiAction);
         stack.Push(uiAction);
      };

      (builder + container).SpanCol(2).Row();
      (builder.SkipCol() + uiRemove).Next();
      (builder + uiAdd).Row();
   }
}