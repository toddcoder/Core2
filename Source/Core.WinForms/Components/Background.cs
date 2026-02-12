using Core.Applications.Messaging;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Components;

public abstract class Background
{
   protected CoreBackgroundWorker worker = new();
   protected bool cancel = false;

   public readonly MessageEvent Canceled = new();
   public readonly MessageEvent Finalized = new();

   public Background()
   {
      worker.Initialize += (_, e) =>
      {
         Initialize();
         if (cancel)
         {
            e.Cancel = true;
            Canceled.Invoke();
         }
      };
      worker.DoWork += (_, _) => DoWork();
      worker.RunWorkerCompleted += (_, _) =>
      {
         RunWorkerCompleted();
         Finalized.Invoke();
      };
   }

   public virtual void Initialize()
   {
   }

   public virtual void DoWork()
   {
   }

   public virtual void RunWorkerCompleted()
   {
   }

   public void RunWorkerAsync() => worker.RunWorkerAsync();

   public Optional<Unit> RunWorkerAndWait(Maybe<TimeSpan> _timeout)
   {
      var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

      worker.Initialize += (_, e) =>
      {
         if (e.Cancel)
         {
            tcs.TrySetCanceled();
         }
      };
      worker.RunWorkerCompleted += (_, _) => tcs.TrySetResult(true);

      try
      {
         worker.RunWorkerAsync();
         if (!worker.IsBusy && !tcs.Task.IsCompleted)
         {
            return fail("Background worker did not start");
         }

         if (_timeout is (true, var timeout))
         {
            if (!tcs.Task.Wait(timeout))
            {
               return fail("Background worker timed out");
            }
         }
         else
         {
            tcs.Task.Wait();
         }

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<Unit> RunWorkerAndWait() => RunWorkerAndWait(nil);

   public async Task<Completion<Unit>> RunWorkerAndWaitAsync(Maybe<TimeSpan> _timeout)
   {
      var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
      worker.Initialize += (_, e) =>
      {
         if (e.Cancel)
         {
            tcs.TrySetCanceled();
         }
      };
      worker.RunWorkerCompleted += (_, _) => tcs.TrySetResult(true);
      try
      {
         worker.RunWorkerAsync();
         if (await Task.WhenAny(tcs.Task, Task.Delay(_timeout | (() => TimeSpan.FromSeconds(30)))) == tcs.Task)
         {
            await tcs.Task;
         }
         else
         {
            return fail("Background worker timed out");
         }
      }
      catch (Exception exception)
      {
         return exception;
      }

      return unit;
   }
}