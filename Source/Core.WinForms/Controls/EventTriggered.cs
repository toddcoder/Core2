using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public abstract class EventTriggered
{
   public class TextChanged(string text) : EventTriggered
   {
      public string Text => text;
   }

   public class MessageShown(MessageShownArgs args) : EventTriggered
   {
      public string Text => args.Text;

      public UiActionType Type => args.Type;
   }

   public class ListViewSelectedIndexChanged(Maybe<(ListViewItem item, int index)> selectedItemWithIndex) : EventTriggered
   {
      public Maybe<ListViewItem> Item => selectedItemWithIndex.Map(s => s.item);

      public Maybe<int> Index = selectedItemWithIndex.Map(s => s.index);
   }

   public class ListBoxSelectedIndexChanged(Maybe<int> index) : EventTriggered
   {
      public Maybe<int> Index => index;
   }
}