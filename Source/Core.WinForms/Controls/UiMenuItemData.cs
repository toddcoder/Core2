using Core.Applications.Messaging;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiMenuItemData
{
   public static UiMenuItemData TextItem(string text, Action onClick)
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

   public static UiMenuItemData TextItemWithImage(string text, Image image, Action onClick)
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

   public string Text { get; set; } = "";

   public Maybe<Image> Image { get; set; } = nil;

   public bool IsEnabled { get; set; } = true;

   public bool IsSeparator { get; set; }

   public readonly MessageEvent OnClick = new();
}