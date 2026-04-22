namespace Core.WinForms.Controls;

public sealed class UiMenu : ToolStripDropDown
{
   private Color backgroundColor = Color.FromArgb(30, 30, 30);
   private Color hoverColor = Color.FromArgb(60, 120, 215);
   private Color textColor = Color.White;
   private Color disabledColor = Color.Gray;
   private Color separatorColor = Color.FromArgb(80, 80, 80);
   private Color borderColor = Color.FromArgb(80, 80, 80);
   private Font itemFont = new("Consolas", 12f);
   private int itemHeight = 36;
   private int separatorHeight = 9;
   private int menuWidth = 220;
   private int iconSize = 18;
   private int iconPadding = 10;
   private int textPadding = 42;

   public UiMenu(IEnumerable<UiMenuItemData> items)
   {
      List<UiMenuItemData> items1 = [.. items];
      AutoSize = true;
      DropShadowEnabled = true;
      BackColor = backgroundColor;

      foreach (var item in items1)
      {
         var height = item.IsSeparator ? separatorHeight : itemHeight;
         var arguments = new UiMenuArguments(item, height, menuWidth, backgroundColor, hoverColor, textColor, disabledColor, separatorColor, itemFont,
            iconSize, iconPadding, textPadding);

         var host = new UiMenuItem(arguments);
         host.ItemClicked.Handler = () =>
         {
            Close(ToolStripDropDownCloseReason.ItemClicked);
            item.OnClick.Invoke();
         };

         Items.Add(host);
      }

      Paint += (_, e) =>
      {
         using var pen = new Pen(borderColor, 1);
         e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
      };
   }

   protected override void OnPaintBackground(PaintEventArgs e)
   {
      base.OnPaintBackground(e);
      e.Graphics.Clear(backgroundColor);
   }
}