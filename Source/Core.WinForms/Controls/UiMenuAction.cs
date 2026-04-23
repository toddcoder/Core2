using Core.Applications.Messaging;
using Core.Collections;
using Core.Monads;

namespace Core.WinForms.Controls;

public class UiMenuAction : UiAction
{
   protected List<UiMenuItemData> items = [];
   protected bool menuOpen;
   protected Lazy<UiMenu> menu;
   public readonly MessageEvent RequestMenuItems = new();
   public readonly MessageEvent MenuClosed = new();

   public UiMenuAction()
   {
      menu = new Lazy<UiMenu>(menuFactory);
      ChooserGlyph = true;
      ClickGlyph = true;
      ClickText = "Open menu";
   }

   public bool MenuEnabled { get; set; } = true;

   protected UiMenu menuFactory()
   {
      var uiMenu = new UiMenu(items);
      uiMenu.Closed += (_, _) =>
      {
         menuOpen = false;
         Invalidate();
      };
      return uiMenu;
   }

   public void Choose(IEnumerable<string> options, Action<string> onChoose)
   {
      foreach (var option in options)
      {
         TextItem(option, onChoose);
      }
   }

   public void Choose(IEnumerable<string> options, Action<string> onChoose, StringHash<Color> foreColors, StringHash<Color> backColors)
   {
      foreach (var option in options)
      {
         var item = TextItem(option, onChoose);
         if (foreColors.Maybe[option] is (true, var foreColor))
         {
            item.ForeColor = foreColor;
         }

         if (backColors.Maybe[option] is (true, var backColor))
         {
            item.BackColor = backColor;
         }
      }
   }

   public void Choose(IEnumerable<string> options, Action<string> onChoose, StringHash<Image> images)
   {
      foreach (var option in options)
      {
         var item = TextItem(option, onChoose);
         if (images.Maybe[option] is (true, var image))
         {
            item.Image = image;
         }
      }
   }

   public void Choose(IEnumerable<string> options, Action<string> onChoose, Func<string, Maybe<(Color foreColor, Color backColor)>> colorSelector)
   {
      foreach (var option in options)
      {
         var item = TextItem(option, onChoose);
         if (colorSelector(option) is (true, var colors))
         {
            item.BackColor = colors.backColor;
            item.ForeColor = colors.foreColor;
         }
      }
   }

   public UiMenuItemData TextItem(string text, Action<string> onClick)
   {
      var item = UiMenuItemData.TextItem(text, onClick);
      items.Add(item);

      return item;
   }

   public UiMenuItemData TextItemWithImage(string text, Image image, Action<string> onClick)
   {
      var item = UiMenuItemData.TextItemWithImage(text, image, onClick);
      items.Add(item);

      return item;
   }

   public UiMenuItemData Separator()
   {
      var item = UiMenuItemData.Separator();
      items.Add(item);

      return item;
   }

   protected override void OnClick(EventArgs e)
   {
      base.OnClick(e);
      if (MenuEnabled)
      {
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
   }

   protected override void OnLostFocus(EventArgs e)
   {
      base.OnLostFocus(e);
      this.Do(() =>
      {
         if (menuOpen && !menu.Value.ContainsFocus)
         {
            menu.Value.Close(ToolStripDropDownCloseReason.AppFocusChange);
            MenuClosed.Invoke();
         }
      });
   }

   public void ResetMenu() => menu = new Lazy<UiMenu>(menuFactory);
}