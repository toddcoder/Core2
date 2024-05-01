using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Applications;

public class ParallelProcessor<T>
{
   protected SlicedList<T>[] listSlices;
   protected int numberOfThreads;
   protected Action<T> action;
   protected ManualResetEvent[] manualResetEvents;

   public ParallelProcessor(int numberOfThreads, Action<T> action)
   {
      this.numberOfThreads = numberOfThreads;
      listSlices = new SlicedList<T>[this.numberOfThreads];
      this.action = action;
      manualResetEvents = new ManualResetEvent[this.numberOfThreads];

      for (var i = 0; i < this.numberOfThreads; i++)
      {
         manualResetEvents[i] = new ManualResetEvent(false);
         listSlices[i] = new SlicedList<T>(manualResetEvents[i]);
      }
   }

   public void ForEach(IEnumerable<T> items)
   {
      prepareListSlices(items);
      for (var i = 0; i < numberOfThreads; i++)
      {
         manualResetEvents[i].Reset();
         ThreadPool.QueueUserWorkItem(doWork, listSlices[i]);
      }

      WaitHandle.WaitAll(manualResetEvents);
   }

   protected void prepareListSlices(IEnumerable<T> items)
   {
      T[] array = [.. items];
      for (var i = 0; i < numberOfThreads; i++)
      {
         listSlices[i].Items = array;
         listSlices[i].Indexes.Clear();
      }

      for (var i = 0; i < array.Length; i++)
      {
         listSlices[i % numberOfThreads].Indexes.AddLast(i);
      }
   }

   protected void doWork(object? o)
   {
      if (o is not null)
      {
         var slicedList = (SlicedList<T>)o;

         foreach (var i in slicedList.Indexes)
         {
            action(slicedList.Items[i]);
         }

         slicedList.ManualResetEvent.Set();
      }
   }
}

public class SlicedList<T>
{
   public SlicedList(ManualResetEvent manualResetEvent)
   {
      ManualResetEvent = manualResetEvent;

      Items = [];
      Indexes = new LinkedList<int>();
   }

   public T[] Items { get; set; }

   public LinkedList<int> Indexes { get; init; }

   public ManualResetEvent ManualResetEvent { get; init; }
}