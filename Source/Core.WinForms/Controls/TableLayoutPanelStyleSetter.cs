namespace Core.WinForms.Controls;

public class TableLayoutPanelStyleSetter
{
   protected TableLayoutPanelStyle style;

   public TableLayoutPanelStyleSetter(TableLayoutPanelStyle style)
   {
      this.style = style;
   }

   public TableLayoutPanelStyleSetter AutoSize
   {
      get
      {
         style.SetAutoSize();
         return this;
      }
   }

   public TableLayoutPanelStyleSetter Absolute
   {
      get
      {
         style.SetAbsolute();
         return this;
      }
   }

   public TableLayoutPanelStyleSetter Percent
   {
      get
      {
         style.SetPercent();
         return this;
      }
   }

   public TableLayoutPanelStyleSetter Value(float value)
   {
      style.SetValue(value);
      return this;
   }

   public TableLayoutPanelStyle Set => style;
}