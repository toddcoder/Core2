﻿using System;
using Core.Collections;
using Core.Monads;

namespace Core.Objects;

public class PropertyInterface
{
   public static IEvaluator GetEvaluator(object obj) => obj is DataContainer dc ? new DataContainerEvaluator(dc) : new PropertyEvaluator(obj);

   protected IEvaluator evaluator = null!;
   protected bool isConvertible;

   public PropertyInterface(string name, string signature)
   {
      Name = name;
      Signature = signature;
   }

   public string Name { get; set; }

   public string Signature { get; set; }

   public virtual Type PropertyType { get; private set; } = null!;

   protected static bool getIsConvertible(Type? type) => type?.GetInterface("IConvertible") is not null;

   public void DeterminePropertyType(object entity)
   {
      evaluator = GetEvaluator(entity);
      PropertyType = evaluator.Contains(Signature) ? evaluator.Type(Signature) : typeof(string);
      isConvertible = getIsConvertible(PropertyType);
   }

   public Maybe<object> GetValue(object entity)
   {
      evaluator = GetEvaluator(entity);
      return ((IHash<string, object>)evaluator).Items[Signature];
   }

   public void SetValue(object entity, object? value)
   {
      evaluator = GetEvaluator(entity);
      if (isConvertible && getIsConvertible(value?.GetType()))
      {
         value = Convert.ChangeType(value, PropertyType);
      }

      if (value is not null)
      {
         evaluator[Signature] = value;
      }
   }
}