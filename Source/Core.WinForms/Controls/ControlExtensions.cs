using Core.Applications;
using Core.Applications.Messaging;

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

   extension(Control control)
   {
      public void SetUp(int x, int y, int width, int height, AnchorStyles anchor, string fontName = "Consolas",
         float fontSize = 12f, FontStyle fontStyle = FontStyle.Regular)
      {
         setUpDimensions(control, x, y, width, height, fontName, fontSize, fontStyle);
         control.Anchor = anchor;
      }

      public void SetUp(int x, int y, int width, int height, DockStyle dock, string fontName = "Consolas",
         float fontSize = 12f, FontStyle fontStyle = FontStyle.Regular)
      {
         setUpDimensions(control, x, y, width, height, fontName, fontSize, fontStyle);
         control.Dock = dock;
      }

      public void SetUpInTableLayoutPanel(TableLayoutPanel tableLayoutPanel, int column, int row, int columnSpan = 1,
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

      public void SetUpInPanel(Panel panel, string fontName = "Consolas", float fontSize = 12f,
         FontStyle fontStyle = FontStyle.Regular, DockStyle dockStyle = DockStyle.Fill)
      {
         control.Dock = dockStyle;
         panel.Controls.Add(control);
         setUpFont(control, fontName, fontSize, fontStyle);
      }

      public void SetUp(int x, int y, int width, int height, string fontName = "Consolas", float fontSize = 12f,
         FontStyle fontStyle = FontStyle.Regular)
      {
         setUpDimensions(control, x, y, width, height, fontName, fontSize, fontStyle);
      }

      public void SetupInFlowLayoutPanel(FlowLayoutPanel flowLayoutPanel, string fontName = "Consolas",
         float fontSize = 12f, FontStyle fontStyle = FontStyle.Regular, DockStyle dockStyle = DockStyle.Fill)
      {
         control.Dock = dockStyle;
         flowLayoutPanel.Controls.Add(control);
         setUpFont(control, fontName, fontSize, fontStyle);
      }

      public Point CursorPosition() => control.Get(() => control.PointToClient(Cursor.Position));
      public void MoveTo(Point location) => control.Location = location;
      public void MoveTo(int x, int y) => control.MoveTo(new Point(x, y));

      public void MoveTo(Rectangle rectangle)
      {
         control.MoveTo(rectangle.Location);
         control.Size = rectangle.Size;
      }

      public void MoveTo(Point location, Size size) => control.MoveTo(new Rectangle(location, size));
      public void MoveTo(int x, int y, Size size) => control.MoveTo(new Point(x, y), size);
      public void MoveTo(Point location, int width, int height) => control.MoveTo(location, new Size(width, height));

      public void ZeroOut()
      {
         control.Padding = new Padding(0);
         control.Margin = new Padding(0);
      }
   }

   public static Image Image<T>(this Resources<T> resources, string name)
   {
      using var stream = resources.Stream(name);
      return System.Drawing.Image.FromStream(stream);
   }

   public static Resources<TControl> Resources<TControl>(this TControl _) where TControl : Control => new();

   public static Resources<TControl> Resources<TControl>(this TControl _, string path) where TControl : Control => new(path);

   public static void UnsubscribeOnDisposed<TPayload>(this Subscriber<TPayload> subscriber, Control control) where TPayload : notnull
   {
      control.Disposed += (_, _) => subscriber.Unsubscribe();
   }

   public static void UnsubscribeOnRemoved<TPayload>(this Subscriber<TPayload> subscriber, Control control) where TPayload : notnull
   {
      control.ControlRemoved += (_, _) => subscriber.Unsubscribe();
   }

   public static void UnsubscribeOnDisposed<TTopic, TPayload>(this Subscriber<TTopic, TPayload> subscriber, Control control) where TTopic : notnull
      where TPayload : notnull
   {
      control.Disposed += (_, _) => subscriber.Unsubscribe();
   }

   public static void UnsubscribeOnRemoved<TTopic, TPayload>(this Subscriber<TTopic, TPayload> subscriber, Control control) where TTopic : notnull
      where TPayload : notnull
   {
      control.ControlRemoved += (_, _) => subscriber.Unsubscribe();
   }

   public static void UnsubscribeOnDisposed(this Subscriber subscriber, Control control)
   {
      control.Disposed += (_, _) => subscriber.Unsubscribe();
   }

   public static void UnsubscribeOnRemoved(this Subscriber subscriber, Control control)
   {
      control.ControlRemoved += (_, _) => subscriber.Unsubscribe();
   }

   public static void UnsubscribeOnDisposed<TPayload>(this SubscriberServer<TPayload> subscriber, Control control) where TPayload : notnull
   {
      control.Disposed += (_, _) => subscriber.Dispose();
   }

   public static void UnsubscribeOnRemoved<TPayload>(this SubscriberServer<TPayload> subscriber, Control control) where TPayload : notnull
   {
      control.ControlRemoved += (_, _) => subscriber.Dispose();
   }

   public static void UnsubscribeOnDisposed(this SubscriberServer subscriber, Control control)
   {
      control.Disposed += (_, _) => subscriber.Dispose();
   }

   public static void UnsubscribeOnRemoved(this SubscriberServer subscriber, Control control)
   {
      control.ControlRemoved += (_, _) => subscriber.Dispose();
   }

   public static DragDrop<TabControl> DragDrop(this TabControl tabControl) => new(tabControl);
}