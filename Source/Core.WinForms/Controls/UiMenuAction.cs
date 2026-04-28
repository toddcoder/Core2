using Core.Applications.Messaging;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiMenuAction : UiAction
{
   protected List<UiMenuItemData> items = [];
   protected bool menuOpen;
   protected Lazy<UiMenu> menu;
   protected Maybe<string[]> _textItems = nil;
   protected Maybe<Hash<string, string>> _hash = nil;
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

   public UiMenuAction Choose(params string[] textItems)
   {
      _textItems = textItems;
      return this;
   }

   public UiMenuAction Choose(params (string key, string value)[] hashItems)
   {
      _hash = hashItems.ToHash();
      return this;
   }

   public void Choose(IEnumerable<string> options, Action<string> onChoose)
   {
      foreach (var option in options)
      {
         TextItem(option, onChoose);
      }
   }

   public void Choose(Hash<string, string> hash, Action<string, string> onChoose)
   {
      foreach (var (key, selectedValue) in hash)
      {
         TextItem(key, _ => onChoose(key, selectedValue));
      }
   }

   public void Then(Action<string> onChoose)
   {
      if (_textItems is (true, var textItems))
      {
         foreach (var textItem in textItems)
         {
            TextItem(textItem, onChoose);
         }
      }
   }

   public void Then(Action<string> onChoose, Action<UiMenuItemData> setter)
   {
      if (_textItems is (true, var textItems))
      {
         foreach (var textItem in textItems)
         {
            var item = TextItem(textItem, onChoose);
            setter(item);
         }
      }
   }

   public void Then(Action<string, string> onChoose)
   {
      if (_hash is (true, var hash))
      {
         foreach (var (key, selectedValue) in hash)
         {
            TextItem(key, _ => onChoose(key, selectedValue));
         }
      }
   }

   public void Then(Action<string, string> onChoose, Action<UiMenuItemData> setter)
   {
      if (_hash is (true, var hash))
      {
         foreach (var (key, selectedValue) in hash)
         {
            var item = TextItem(key, _ => onChoose(key, selectedValue));
            setter(item);
         }
      }
   }

   public void Choose(IEnumerable<string> options, Action<string> onChoose, Action<UiMenuItemData> setter)
   {
      foreach (var option in options)
      {
         var item = TextItem(option, onChoose);
         setter(item);
      }
   }

   public void Choose(Hash<string, string> hash, Action<string, string> onChoose, Action<UiMenuItemData> setter)
   {
      foreach (var (key, selectedValue) in hash)
      {
         var item = TextItem(key, _ => onChoose(key, selectedValue));
         setter(item);
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