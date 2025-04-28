using System;
using System.Threading.Tasks;
using Core.DataStructures;

namespace Core.Applications;

public class Deference : IDisposable
{
   protected record DeferenceTask(Action Action, bool IsAsync);

   protected MaybeStack<DeferenceTask> deferenceStack = [];

   public void Defer(Action action) => deferenceStack.Push(new DeferenceTask(action, false));

   public void DeferAsync(Action action) => deferenceStack.Push(new DeferenceTask(action, true));

   protected void runDeferences()
   {
      while (deferenceStack.Pop() is (true, var deferenceTask))
      {
         if (deferenceTask.IsAsync)
         {
            Task.Run(deferenceTask.Action);
         }
         else
         {
            deferenceTask.Action();
         }
      }
   }

   public void Dispose()
   {
      runDeferences();
      GC.SuppressFinalize(this);
   }

   ~Deference() => runDeferences();
}