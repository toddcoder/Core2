using Core.Strings;

namespace Core.WinForms.Controls;

public sealed class UiMenu : ToolStripDropDown
{
   private Color backgroundColor = SystemColors.Menu;
   private Color hoverColor = SystemColors.MenuHighlight;
   private Color textColor = SystemColors.MenuText;
   private Color disabledColor = SystemColors.GrayText;
   private Color separatorColor = SystemColors.ControlDark;
   private Color borderColor = SystemColors.ControlDark;
   private Font itemFont = new("Consolas", 12f);
   private int itemHeight = 36;
   private int separatorHeight = 9;
   private int iconSize = 18;
   private int iconPadding = 10;
   private int textPadding = 32;
   private int textRightPadding = 16;
   private int minMenuWidth = 120;
   private int maxMenuWidth = 800;

   public UiMenu(IEnumerable<UiMenuItemData> items)
   {
      List<UiMenuItemData> items1 = [.. items];
      var menuWidth = calculateMenuWidth(items1);
      AutoSize = true;
      DropShadowEnabled = true;
      BackColor = backgroundColor;

      foreach (var item in items1)
      {
         var height = item.IsSeparator ? separatorHeight : itemHeight;
         var background = item.BackColor | backgroundColor;
         var foreColor = item.ForeColor | textColor;
         var arguments = new UiMenuArguments(item, height, menuWidth, background, hoverColor, foreColor, disabledColor, separatorColor, itemFont,
            iconSize, iconPadding, textPadding);

         var host = new UiMenuItem(arguments);
         host.ItemClicked.Handler = () =>
         {
            Close(ToolStripDropDownCloseReason.ItemClicked);
            item.OnClick.Invoke(item.Text);
         };

         Items.Add(host);
      }

      Paint += (_, e) =>
      {
         using var pen = new Pen(borderColor, 1);
         e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
      };
   }

   private int calculateMenuWidth(IEnumerable<UiMenuItemData> items)
   {
      using var bitmap = new Bitmap(1, 1);
      using var g = Graphics.FromImage(bitmap);

      var maxTextWidth = items.Where(i => !i.IsSeparator && i.Text.IsNotEmpty())
         .Select(i => (int)Math.Ceiling(g.MeasureString(i.Text, itemFont).Width)).DefaultIfEmpty(0).Max();
      var ideal = textPadding + maxTextWidth + textRightPadding;

      return Math.Clamp(ideal, minMenuWidth, maxMenuWidth);
   }

   protected override void OnPaintBackground(PaintEventArgs e)
   {
      base.OnPaintBackground(e);
      e.Graphics.Clear(backgroundColor);
   }
}