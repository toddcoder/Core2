using Core.Applications.Messaging;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiMenuItemData
{
   public static UiMenuItemData TextItem(string text, Action<string> onClick)
   {
      var item = new UiMenuItemData
      {
         Text = text,
         OnClick =
         {
            Handler = onClick
         }
      };
      return item;
   }

   public static UiMenuItemData TextItemWithImage(string text, Image image, Action<string> onClick)
   {
      var item = new UiMenuItemData
      {
         Text = text,
         Image = image,
         OnClick =
         {
            Handler = onClick
         }
      };
      return item;
   }

   public static UiMenuItemData Separator() => new()
   {
      IsSeparator = true
   };

   public static UiMenuItemData TextWithSubmenu(string text)
   {
      var item = new UiMenuItemData
      {
         Text = text,
         HasSubmenu = true
      };
      return item;
   }

   public static UiMenuItemData TextWithImageAndSubmenu(string text, Image image)
   {
      var item = new UiMenuItemData
      {
         Text = text,
         Image = image,
         HasSubmenu = true
      };
      return item;
   }

   public string Text { get; set; } = "";

   public Maybe<Image> Image { get; set; } = nil;

   public bool IsEnabled { get; set; } = true;

   public bool IsSeparator { get; set; }

   public readonly MessageEvent<string> OnClick = new();

   public Maybe<Color> ForeColor { get; set; } = nil;

   public Maybe<Color> BackColor { get; set; } = nil;

   public bool UsesDefaultColors => !ForeColor && !BackColor;

   public UiMenuItemData Colors(Color foreColor, Color backColor)
   {
      ForeColor = foreColor;
      BackColor = backColor;

      return this;
   }

   public bool IsBold { get; set; }

   public bool IsItalic { get; set; }

   public bool HasSubmenu { get; set; }
}