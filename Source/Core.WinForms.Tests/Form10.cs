using Core.Applications;
using Core.DataStructures;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Tests;

public partial class Form10 : Form
{
   protected ControlContainer<UiAction> container = ControlContainer<UiAction>.ReadingContainer();
   protected UiAction uiDirection = new() { ChooserGlyph = true };
   protected UiAction uiRemove = new();
   protected UiAction uiAdd = new();
   protected MaybeStack<UiAction> stack = [];
   protected Idle idle = new(10) { MaximumSeconds = 50 };
   protected UiAction uiIdle = new();

   public Form10()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f + 200 + 200 + 100 + 100;
      _ = builder.Row + 100f + 40;
      builder.SetUp();

      uiIdle.NoStatus("idle");

      uiDirection.Success("Reading");
      uiDirection.Click += (_, _) =>
      {
         var _chosen = uiDirection.Choose("Direction").Choices("Reading", "Horizontal", "Vertical", "Fixed")
            .ModifyTitle(false).Choose();
         if (_chosen is (true, var chosen))
         {
            container.Clear();
            container.Direction = chosen.Value switch
            {
               "Reading" => ControlDirection.Reading,
               "Horizontal" => ControlDirection.Horizontal,
               "Vertical" => ControlDirection.Vertical,
               "Fixed" => ControlDirection.Reading,
               _ => container.Direction
            };
            if (chosen.Value == "Fixed")
            {
               container.ControlWidth = 200;
               container.ControlHeight = 60;
            }
            else
            {
               container.ControlWidth = nil;
               container.ControlHeight = nil;
            }

            uiDirection.Success(chosen.Value);
         }
      };
      uiDirection.ClickText = "Select direction";

      uiRemove.Button("/minus");
      uiRemove.Click += (_, _) =>
      {
         if (stack.Pop() is (true, var uiLast))
         {
            container.Remove(uiLast);
         }
      };
      uiRemove.ClickText = "Remove item";

      uiAdd.Button("/plus");
      uiAdd.Click += (_, _) =>
      {
         var uiAction = new UiAction();
         uiAction.Message(container.Count.ToString());
         uiAction.ZeroOut();
         container.Add(uiAction);
         stack.Push(uiAction);
      };
      uiAdd.ClickText = "Add item";

      (builder + container).SpanCol(5).Row();
      (builder.SkipCol() + uiIdle).Next();
      (builder + uiDirection).Next();
      (builder + uiRemove).Next();
      (builder + uiAdd).Row();

      idle.UserIdle.Handler = p =>
      {
         uiIdle.Success("Idle");
         uiIdle.Count = p;
      };
      idle.InputResumed.Handler = () =>
      {
         uiIdle.Message("Resumed");
         uiIdle.Count = nil;
      };
      idle.MaximumExceeded.Handler=p =>
      {
         uiIdle.Failure("maximum exceeded");
         uiIdle.Count = p;
      };
   }

   protected void timer1_Tick(object sender, EventArgs e)
   {
      idle.CheckIdleTime();
   }
}