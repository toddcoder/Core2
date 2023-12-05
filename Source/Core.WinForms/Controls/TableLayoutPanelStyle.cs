using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class TableLayoutPanelStyle
{
   protected TableLayoutPanelBuilder builder;
   protected Maybe<SizeType> _sizeType;
   protected Maybe<float> _value;

   public TableLayoutPanelStyle(TableLayoutPanelBuilder builder)
   {
      this.builder = builder;

      _sizeType = nil;
      _value = nil;
   }

   public TableLayoutPanelStyleSetter Set => new(this);

   public void SetAutoSize() => _sizeType = SizeType.AutoSize;

   public void SetAbsolute() => _sizeType = SizeType.Absolute;

   public void SetPercent() => _sizeType = SizeType.Percent;

   public void SetValue(float value) => _value = value;

   public ColumnStyle ColumnStyle()
   {
      if (_sizeType is (true, var sizeType))
      {
         return new ColumnStyle(sizeType, _value | 0f);
      }
      else
      {
         return new ColumnStyle();
      }
   }

   public RowStyle RowStyle()
   {
      if (_sizeType is (true, var sizeType))
      {
         return new RowStyle(sizeType, _value | 0f);
      }
      else
      {
         return new RowStyle();
      }
   }
}