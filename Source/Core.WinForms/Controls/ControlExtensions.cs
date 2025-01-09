using Core.Applications;

namespace Core.WinForms.Controls;

public static class ControlExtensions
{
   private static void setUpFont(Control control, string fontName, float fontSize, FontStyle fontStyle)
   {
      var font = new Font(fontName, fontSize, fontStyle);
      if (control is UiAction uiAction)
      {
         uiAction.Font = font;
      }
      else
      {
         control.Font = font;
      }
   }

   private static void setUpDimensions(Control control, int x, int y, int width, int height, string fontName, float fontSize, FontStyle fontStyle)
   {
      control.AutoSize = false;
      control.Location = new Point(x, y);
      control.Size = new Size(width, height);
      setUpFont(control, fontName, fontSize, fontStyle);
   }

   public static void SetUp(this Control control, int x, int y, int width, int height, AnchorStyles anchor, string fontName = "Consolas",
      float fontSize = 12f, FontStyle fontStyle = FontStyle.Regular)
   {
      setUpDimensions(control, x, y, width, height, fontName, fontSize, fontStyle);
      control.Anchor = anchor;
   }

   public static void SetUp(this Control control, int x, int y, int width, int height, DockStyle dock, string fontName = "Consolas",
      float fontSize = 12f, FontStyle fontStyle = FontStyle.Regular)
   {
      setUpDimensions(control, x, y, width, height, fontName, fontSize, fontStyle);
      control.Dock = dock;
   }

   public static void SetUpInTableLayoutPanel(this Control control, TableLayoutPanel tableLayoutPanel, int column, int row, int columnSpan = 1,
      int rowSpan = 1, string fontName = "Consolas", float fontSize = 12f, FontStyle fontStyle = FontStyle.Regular,
      DockStyle dockStyle = DockStyle.Fill)
   {
      control.Dock = dockStyle;
      tableLayoutPanel.Controls.Add(control, column, row);

      if (columnSpan > 1)
      {
         tableLayoutPanel.SetColumnSpan(control, columnSpan);
      }

      if (rowSpan > 1)
      {
         tableLayoutPanel.SetRowSpan(control, rowSpan);
      }

      setUpFont(control, fontName, fontSize, fontStyle);
   }

   public static void SetUpInPanel(this Control control, Panel panel, string fontName = "Consolas", float fontSize = 12f,
      FontStyle fontStyle = FontStyle.Regular, DockStyle dockStyle = DockStyle.Fill)
   {
      control.Dock = dockStyle;
      panel.Controls.Add(control);
      setUpFont(control, fontName, fontSize, fontStyle);
   }

   public static void SetUp(this Control control, int x, int y, int width, int height, string fontName = "Consolas", float fontSize = 12f,
      FontStyle fontStyle = FontStyle.Regular)
   {
      setUpDimensions(control, x, y, width, height, fontName, fontSize, fontStyle);
   }

   public static void SetupInFlowLayoutPanel(this Control control, FlowLayoutPanel flowLayoutPanel, string fontName = "Consolas",
      float fontSize = 12f, FontStyle fontStyle = FontStyle.Regular, DockStyle dockStyle = DockStyle.Fill)
   {
      control.Dock = dockStyle;
      flowLayoutPanel.Controls.Add(control);
      setUpFont(control, fontName, fontSize, fontStyle);
   }

   public static Point CursorPosition(this Control control) => control.Get(() => control.PointToClient(Cursor.Position));

   public static void MoveTo(this Control control, Point location) => control.Location = location;

   public static void MoveTo(this Control control, int x, int y) => control.MoveTo(new Point(x, y));

   public static void MoveTo(this Control control, Rectangle rectangle)
   {
      control.MoveTo(rectangle.Location);
      control.Size = rectangle.Size;
   }

   public static void MoveTo(this Control control, Point location, Size size) => control.MoveTo(new Rectangle(location, size));

   public static void MoveTo(this Control control, int x, int y, Size size) => control.MoveTo(new Point(x, y), size);

   public static void MoveTo(this Control control, Point location, int width, int height) => control.MoveTo(location, new Size(width, height));

   public static void ZeroOut(this Control control)
   {
      control.Padding = new Padding(0);
      control.Margin = new Padding(0);
   }

   public static Image Image<T>(this Resources<T> resources, string name)
   {
      using var stream = resources.Stream(name);
      return System.Drawing.Image.FromStream(stream);
   }

   public static Resources<TControl> Resources<TControl>(this TControl _) where TControl : Control => new();

   public static Resources<TControl> Resources<TControl>(this TControl _, string path) where TControl : Control => new(path);
}