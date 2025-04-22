using Core.Applications.Messaging;

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
}