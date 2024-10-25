using Core.Enumerables;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Controls;

public class LabelUiActionHost<TControl>(TableLayoutPanel tableLayoutPanel, UiAction uiLabel, TControl control,
   Func<TableLayoutBuilder, AxisSetup> arrangeRow) where TControl : Control
{
   protected List<UiAction> actions = [];

   public void AddUiAction(UiAction action)
   {
      action.SizeToText();
      actions.Add(action);

      rearrangeActions();
   }

   public void AddUiActions(params UiAction[] actions)
   {
      foreach (var uiAction in actions)
      {
         uiAction.SizeToText();
         this.actions.Add(uiAction);
      }

      rearrangeActions();
   }

   protected void rearrangeActions()
   {
      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      foreach (var uiAction in actions)
      {
         _ = builder.Col + uiAction.Width;
      }

      _ = arrangeRow(builder);
      builder.SetUp();

      (builder + uiLabel).Next();
      foreach (var uiAction in actions.Take(actions.Count - 1))
      {
         (builder + uiAction).Next();
      }

      (builder + actions[^1]).Row();
      (builder + control).SpanCol(actions.Count + 1).Row();
   }

   public bool ActionsVisible
   {
      get => actions.Count != 0 && actions.Select(a => a.Visible).Fold(bool (b, a) => b && a, false);
      set
      {
         foreach (var uiAction in actions)
         {
            uiAction.Visible = value;
         }
      }
   }

   public void ClearActions() => actions.Clear();
}