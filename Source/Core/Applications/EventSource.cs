using System;
using System.Collections.Generic;

namespace Core.Applications;

public class EventSource<T>(Func<IEnumerable<T>> function) where T : notnull
{
   public event EventHandler<EventSourceArgs<T>>? EventRaised;

   public void Evaluate()
   {
      foreach (var value in function())
      {
         EventRaised?.Invoke(this, new EventSourceArgs<T>(value));
      }
   }
}