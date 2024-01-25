using System.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiStager : IList<UiAction>
{
   protected List<UiAction> uiActions = new();
   protected Maybe<UiAction> _lastUiAction = nil;
   protected int stageIndex = -1;

   public IEnumerator<UiAction> GetEnumerator() => uiActions.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Add(Control containerControl, UiAction uiAction, string text, UiActionType type = UiActionType.NoStatus)
   {
      containerControl.Controls.Add(uiAction);
      uiAction.AutoSizeText = true;
      uiAction.ShowMessage(text, type);
      Add(uiAction);
   }

   public void Add(UiAction item)
   {
      uiActions.Add(item);
      if (_lastUiAction is (true, var lastUiAction))
      {
         item.Location = lastUiAction.Location.OffsetX(lastUiAction.Size.Width);
         item.Height = lastUiAction.Height;
      }

      _lastUiAction = item;
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
         var text = uiAction.Text;
         uiAction.ShowMessage(text, type);
      }

      if (++stageIndex < uiActions.Count)
      {
         var uiAction = uiActions[stageIndex];
         var text = uiAction.Text;
         uiAction.Busy(text);
      }
   }

   public Maybe<UiAction> CurrentUiAction => maybe<UiAction>() & stageIndex < uiActions.Count & (() => uiActions[stageIndex]);
}