using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.TableLayoutPanels;

public class AxisSetup
{
   public static AxisSetup operator +(AxisSetup axisSetup, int pixels)
   {
      axisSetup.values.Add(pixels);
      return axisSetup;
   }

   public static AxisSetup operator +(AxisSetup axisSetup, float percent)
   {
      axisSetup.values.Add(percent);
      return axisSetup;
   }

   public static AxisSetup operator *(AxisSetup axisSetup, int value)
   {
      if (axisSetup._repeat is (true, var repeat))
      {
         for (var i = 0; i < repeat; i++)
         {
            axisSetup.values.Add(value);
         }

         axisSetup._repeat = nil;
      }
      else
      {
         axisSetup._repeat = value;
      }

      return axisSetup;
   }

   public static AxisSetup operator *(AxisSetup axisSetup, float value)
   {
      if (axisSetup._repeat is (true, var repeat))
      {
         for (var i = 0; i < repeat; i++)
         {
            axisSetup.values.Add(value);
         }

         axisSetup._repeat = nil;
      }
      else
      {
         axisSetup._repeat = (int)value;
      }

      return axisSetup;
   }

   protected List<Either<int, float>> values = [];

   protected Maybe<int> _repeat = nil;

   public List<Either<int, float>> Values => values;

   public AxisSetup AutoSize()
   {
      values.Add(-1);
      return this;
   }
}