namespace Core.WinForms.Components;

public abstract class Background
{
   protected CoreBackgroundWorker worker = new();
   protected bool cancel = false;

   public Background()
   {
      worker.Initialize += (_, e) =>
      {
         Initialize();
         if (cancel)
         {
            e.Cancel = true;
         }
      };
      worker.DoWork += (_, _) => DoWork();
      worker.RunWorkerCompleted += (_, _) => RunWorkerCompleted();
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