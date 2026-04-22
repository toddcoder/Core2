namespace Core.WinForms.Controls;

public class UiMenuAction : UiAction
{
   protected List<UiMenuItemData> items = [];
   protected bool menuOpen;
   protected Lazy<UiMenu> menu;

   public UiMenuAction()
   {
      menu = new Lazy<UiMenu>(() =>
      {
         var uiMenu = new UiMenu(items);
         uiMenu.Closed += (_, _) =>
         {
            menuOpen = false;
            Invalidate();
         };
         return uiMenu;
      });
      ChooserGlyph = true;
   }

   public void TextItem(string text, Action onClick)
   {
      items.Add(UiMenuItemData.TextItem(text, onClick));
   }

   public void TextItemWithImage(string text, Image image, Action onClick)
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
}