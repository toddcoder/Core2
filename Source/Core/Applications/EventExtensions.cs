using Core.Monads;
using System.ComponentModel;
using System.Reflection;
using System;
using static Core.Monads.MonadFunctions;

namespace Core.Applications;

public static class EventExtensions
{
   public static Maybe<Delegate> ClearEvent(this object obj, string eventName)
   {
      try
      {
         var type = obj.GetType();
         var fieldInfo = type.BaseType!.GetField(eventName, BindingFlags.Static | BindingFlags.NonPublic);
         var propertyValue = fieldInfo!.GetValue(obj);
         var propertyInfo = type.GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
         var eventHandlerList = (EventHandlerList)propertyInfo!.GetValue(obj, null)!;
         var eventHandler = eventHandlerList[propertyInfo];
         if (eventHandler is not null)
         {
            eventHandlerList.RemoveHandler(propertyValue!, eventHandler);
            return eventHandler;
         }
         else
         {
            return nil;
         }
      }
      catch
      {
         return nil;
      }
   }
}