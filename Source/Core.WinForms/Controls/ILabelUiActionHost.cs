namespace Core.WinForms.Controls;

public interface ILabelUiActionHost
{
   void AddUiAction(UiAction action);

   void AddUiActions(params UiAction[] actions);

   public bool ActionsVisible { get; set; }

   public void ClearActions();
}