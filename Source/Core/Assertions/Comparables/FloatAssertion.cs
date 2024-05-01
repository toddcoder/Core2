﻿using System;
using System.ComponentModel;

namespace Core.Assertions.Comparables;

public class FloatAssertion : ComparableAssertion<float>
{
   protected static bool nearlyEqual(float f1, object obj, float epsilon)
   {
      var converter = TypeDescriptor.GetConverter(typeof(float));
      if (converter.CanConvertFrom(obj.GetType()))
      {
         var converted = converter.ConvertTo(obj, typeof(float));
         if (converted is not null)
         {
            var f2 = (float)converted;
            return nearlyEqual(f1, f2, epsilon);
         }
         else
         {
            return false;
         }
      }
      else
      {
         return false;
      }
   }

   protected static bool nearlyEqual(float f1, float f2, float epsilon) => Math.Abs(f1 - f2) < epsilon;

   public FloatAssertion(IComparable comparable) : base(comparable) { }

   public new FloatAssertion Not
   {
      get
      {
         not = true;
         return this;
      }
   }

   public FloatAssertion BeNearlyEqual(object obj, float epsilon = 0.00001f)
   {
      return (FloatAssertion)add(obj, _ => nearlyEqual(Comparable, obj, epsilon), $"{obj} must $not nearly be equal {comparable}");
   }
}