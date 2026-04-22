using Core.Applications.Messaging;

namespace Core.WinForms.Controls;

internal sealed class UiMenuItem : ToolStripControlHost
{
   public readonly MessageEvent ItemClicked = new();
   private readonly UiMenuArguments arguments;
   private bool hovered;
   private Panel panel;

   public UiMenuItem(UiMenuArguments arguments) : base(new Panel
   {
      Width = arguments.Width, Height = arguments.Height, BackColor = arguments.BackgroundColor
   })
   {
      this.arguments = arguments;
      AutoSize = false;
      Margin = Padding.Empty;
      Padding = Padding.Empty;
      Size = new Size(arguments.Width, arguments.Height);

      panel = (Panel)Control;
      panel.Paint += (_, e) => panelPaint(e);
      panel.MouseEnter += (_, _) =>
      {
         hovered = true;
         panel.Invalidate();
      };
      panel.MouseLeave += (_, _) =>
      {
         hovered = false;
         panel.Invalidate();
      };
      panel.MouseClick += (_, e) =>
      {
         if (e.Button == MouseButtons.Left && arguments.Data is { IsEnabled: true, IsSeparator: false })
         {
            ItemClicked.Invoke();
         }
      };
   }

   public UiMenuItem(UiMenuArguments arguments, string name) : this(arguments)
   {
      Name = name;
   }

   private void panelPaint(PaintEventArgs e)
   {
      var g = e.Graphics;
      var bounds = panel.ClientRectangle;

      if (arguments.Data.IsSeparator)
      {
         var y = bounds.Height / 2;
         using var pen = new Pen(arguments.SeparatorColor, 1);
         g.DrawLine(pen, arguments.IconPadding, y, bounds.Width - arguments.IconPadding, y);
         return;
      }

      g.Clear(hovered && arguments.Data.IsEnabled ? arguments.HoverColor : arguments.BackgroundColor);

      if (arguments.Data.Image is (true, var image))
      {
         var rectangle = new Rectangle(arguments.IconPadding, (bounds.Height - image.Height) / 2, image.Width, image.Height);
         g.DrawImage(image, rectangle);
      }

      var foregroundColor = arguments.Data.IsEnabled ? arguments.TextColor : arguments.DisabledColor;
      using var brush = new SolidBrush(foregroundColor);
      var textBounds = new Rectangle(arguments.TextPadding, 0, bounds.Width - arguments.TextPadding - 4, bounds.Height);
      var stringFormat = new StringFormat
      {
         Alignment = StringAlignment.Near,
         LineAlignment = StringAlignment.Center,
         Trimming = StringTrimming.EllipsisCharacter
      };
      g.DrawString(arguments.Data.Text, arguments.Font, brush, textBounds, stringFormat);
   }
}