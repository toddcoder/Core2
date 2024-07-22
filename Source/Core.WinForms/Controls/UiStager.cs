using System.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiStager(Control container, string fontName = "Consolas", float fontSize = 12f, UiActionType defaultActionType = UiActionType.NoStatus)
   : IList<UiAction>
{
   protected List<UiAction> uiActions = [];
   protected int stageIndex = -1;

   public IEnumerator<UiAction> GetEnumerator() => uiActions.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Add(UiAction uiAction, string text, UiActionType type = UiActionType.NoStatus)
   {
      if (uiActions.Count == 0)
      {
         container.Resize += (_, _) => rearrange();
      }

      uiAction.AutoSize = false;
      uiAction.Font = new Font(fontName, fontSize);
      uiAction.Anchor = AnchorStyles.Left | AnchorStyles.Top;
      container.Controls.Add(uiAction);
      uiAction.AutoSizeText = true;
      uiAction.ShowMessage(text, type);
      Add(uiAction);
   }

   public void Add(string text, UiActionType type = UiActionType.NoStatus)
   {
      var uiAction = new UiAction();
      container.Controls.Add(uiAction);
      Add(uiAction, text, type);
   }

   public void AddRange(params string[] texts)
   {
      foreach (var text in texts)
      {
         Add(text, defaultActionType);
      }
   }

   protected void rearrange()
   {
      if (uiActions.Count > 0)
      {
         var width = container.ClientSize.Width / uiActions.Count;
         var left = 0;
         var height = container.ClientSize.Height;
         foreach (var uiAction in uiActions)
         {
            uiAction.Location = new Point(left, 0);
            uiAction.Size = new Size(width, height);

            left += width;
         }
      }
   }

   public void Add(UiAction item)
   {
      item.PaintingBackground += (_, e) =>
      {
         using var pen = new Pen(item.GetForeColor(item.Type));
         e.Graphics.DrawRectangle(pen, item.ClientRectangle.Reposition(1, 1).Resize(-2, -2));
      };
      uiActions.Add(item);
      rearrange();
   }

   public void Clear() => uiActions.Clear();

   public bool Contains(UiAction item) => uiActions.Contains(item);

   public void CopyTo(UiAction[] array, int arrayIndex) => uiActions.CopyTo(array, arrayIndex);

   public bool Remove(UiAction item) => uiActions.Remove(item);

   public int Count => uiActions.Count;

   public bool IsReadOnly => false;

   public int IndexOf(UiAction item) => uiActions.IndexOf(item);

   public void Insert(int index, UiAction item) => uiActions.Insert(index, item);

   public void RemoveAt(int index) => uiActions.RemoveAt(index);

   public UiAction this[int index]
   {
      get => uiActions[index];
      set => uiActions[index] = value;
   }

   public void NextStage(UiActionType type)
   {
      if (stageIndex > -1 && stageIndex < uiActions.Count)
      {
         var uiAction = uiActions[stageIndex];
         var text = uiAction.NonNullText;
         uiAction.ShowMessage(text, type);
      }

      if (++stageIndex < uiActions.Count)
      {
         var uiAction = uiActions[stageIndex];
         var text = uiAction.NonNullText;
         uiAction.Busy(text);
      }
   }

   public Maybe<UiAction> CurrentUiAction => maybe<UiAction>() & stageIndex < uiActions.Count & (() => uiActions[stageIndex]);

   public void Reset()
   {
      rearrange();
      foreach (var uiAction in uiActions)
      {
         var text = uiAction.NonNullText;
         uiAction.Busy(text);
      }

      stageIndex = -1;
   }
}