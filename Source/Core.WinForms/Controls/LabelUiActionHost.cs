using System.Collections;
using Core.Enumerables;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Controls;

public class LabelUiActionHost<TControl>(TableLayoutPanel tableLayoutPanel, UiAction uiLabel, TControl control,
   Func<TableLayoutBuilder, AxisSetup> arrangeRow) : IEnumerable<UiAction> where TControl : Control
{
   protected List<UiAction> actions = [];

   public void AddUiAction(UiAction action)
   {
      if (action.HostCanSizeToText)
      {
         action.SizeToText(action.HostCanSizeToTextPadding);
      }

      actions.Add(action);

      RearrangeActions();
   }

   public void AddUiActions(params UiAction[] actions)
   {
      foreach (var uiAction in actions)
      {
         if (uiAction.HostCanSizeToText)
         {
            uiAction.SizeToText(uiAction.HostCanSizeToTextPadding);
         }

         this.actions.Add(uiAction);
      }

      RearrangeActions();
   }

   public void RearrangeActions()
   {
      if (actions.Count > 0)
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

   public void ClearActions()
   {
      foreach (var uiAction in actions)
      {
         tableLayoutPanel.Controls.Remove(uiAction);
      }

      actions.Clear();
   }

   public IEnumerator<UiAction> GetEnumerator() => actions.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}