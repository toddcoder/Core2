using Core.Applications.Messaging;

namespace Core.WinForms.Controls;

public class UiMenuAction : UiAction
{
   protected List<UiMenuItemData> items = [];
   protected bool menuOpen;
   protected Lazy<UiMenu> menu;
   public readonly MessageEvent RequestMenuItems = new();

   public UiMenuAction()
   {
      menu = new Lazy<UiMenu>(menuFactory);
      ChooserGlyph = true;
   }

   protected UiMenu menuFactory()
   {
      var uiMenu = new UiMenu(items);
      //uiMenu.Width = ClientSize.Width;
      uiMenu.Closed += (_, _) =>
      {
         menuOpen = false;
         Invalidate();
      };
      return uiMenu;
   }

   public void TextItem(string text, Action<string> onClick)
   {
      items.Add(UiMenuItemData.TextItem(text, onClick));
   }

   public void TextItemWithImage(string text, Image image, Action<string> onClick)
   {
      items.Add(UiMenuItemData.TextItemWithImage(text, image, onClick));
   }

   public void Separator() => items.Add(UiMenuItemData.Separator());

   protected override void OnClick(EventArgs e)
   {
      base.OnClick(e);
      if (menuOpen)
      {
         menu.Value.Close();
      }
      else
      {
         RequestMenuItems.Invoke();
         menuOpen = true;
         var location = Cursor.Position;
         location = PointToClient(location);
         menu.Value.Show(this, location);
         Focus();
      }
   }

   protected override void OnLostFocus(EventArgs e)
   {
      base.OnLostFocus(e);
      this.Do(() =>
      {
         if (menuOpen && !menu.Value.ContainsFocus)
         {
            menu.Value.Close(ToolStripDropDownCloseReason.AppFocusChange);
         }
      });
   }

   public void ResetMenu() => menu = new Lazy<UiMenu>(menuFactory);
}