using System.ComponentModel;

namespace Core.WinForms.Components;

public class CoreBackgroundWorker : BackgroundWorker
{
   public event EventHandler<InitializeArgs>? Initialize;

   public CoreBackgroundWorker()
   {
      WorkerSupportsCancellation = true;
   }

   public new void RunWorkerAsync()
   {
      var args = new InitializeArgs();
      Initialize?.Invoke(this, args);
      if (!args.Cancel && !IsBusy)
      {
         var _argument = args.Argument;
         if (_argument is (true, var argument))
         {
            base.RunWorkerAsync(argument);
         }
         else
         {
            base.RunWorkerAsync();
         }
      }
   }

   public new void RunWorkerAsync(object argument)
   {
      var args = new InitializeArgs { Argument = argument };
      Initialize?.Invoke(this, args);
      if (!args.Cancel && !IsBusy)
      {
         var _argument = args.Argument;
         if (_argument is (true, var argumentValue))
         {
            base.RunWorkerAsync(argumentValue);
         }
         else
         {
            base.RunWorkerAsync();
         }
      }
   }
}