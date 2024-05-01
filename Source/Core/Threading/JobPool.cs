﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Core.DataStructures;
using Core.Enumerables;
using Core.Monads.Lazy;
using static Core.Monads.MonadFunctions;

namespace Core.Threading;

public class JobPool
{
   protected bool multiThreaded;
   protected int refillThreshold;
   protected int processorCount;
   protected Job[] jobs;
   protected ManualResetEvent[] manualResetEvents;
   protected object locker;
   protected JobQueue queue;

   public event EventHandler<JobExceptionArgs>? JobException;
   public event EventHandler<CompletedArgs>? Completed;

   public JobPool(bool multiThreaded = true, int refillThreshold = 5)
   {
      this.multiThreaded = multiThreaded;
      this.refillThreshold = refillThreshold;

      processorCount = Environment.ProcessorCount;
      manualResetEvents = [.. Enumerable.Range(0, processorCount).Select(_ => new ManualResetEvent(false))];
      locker = new object();
      jobs = [.. Enumerable.Range(0, processorCount).Select(i => new Job(i, manualResetEvents[i], locker))];
      queue = new JobQueue(processorCount);
   }

   public int ProcessorCount => processorCount;

   public void Enqueue(Action<int> action) => queue.Enqueue(action);

   public void Dispatch()
   {
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      if (queue.AllCount > 0)
      {
         if (multiThreaded)
         {
            var thread = new Thread(() =>
            {
               foreach (var job in jobs)
               {
                  job.JobException += (sender, e) => JobException?.Invoke(sender, e);
                  job.EmptyQueue += balanceQueues;

                  job.Dispatch(queue);
               }

               WaitHandle.WaitAll(manualResetEvents);
            });
            //thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
            //thread.Join();
         }
         else
         {
            foreach (var job in jobs)
            {
               job.Execute(queue);
            }
         }
      }

      stopwatch.Stop();
      Completed?.Invoke(this, new CompletedArgs(stopwatch.Elapsed));
   }

   protected void balanceQueues(object? sender, JobEmptyQueueArgs e)
   {
      if (totalCount() < refillThreshold)
      {
         e.Quit = true;
         return;
      }

      MaybeQueue<Action<int>> newQueue = [];

      for (var i = 0; i < processorCount; i++)
      {
         while (queue.Dequeue(i) is (true, var action))
         {
            newQueue.Enqueue(action);
         }
      }

      queue.ResetCurrentAffinity();
      LazyMaybe<Action<int>> _item = nil;
      while (_item.ValueOf(newQueue.Dequeue, true) is (true, var item))
      {
         queue.Enqueue(item);
      }

      e.Quit = false;
   }

   protected int totalCount()
   {
      var count = 0;
      for (var i = 0; i < processorCount; i++)
      {
         count += queue.Count(i);
      }

      return count;
   }

   public string JobsStatuses
   {
      get
      {
         List<string> list = [];
         var totalCount = 0;
         for (var i = 0; i < processorCount; i++)
         {
            var count = queue.Count(i);
            totalCount += count;
            list.Add($"[{i + 1} | {count,4}]");
         }

         list.Add($"[All | {totalCount,4}]");

         return list.ToString(" ");
      }
   }
}