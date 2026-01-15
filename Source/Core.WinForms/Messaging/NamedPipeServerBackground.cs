using Core.Applications.LongMessaging;
using Core.Monads;
using Core.WinForms.Components;
using Core.WinForms.Controls;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Messaging;

public class NamedPipeServerBackground(UiAction uiMessage, string pipeName) : Background
{
   protected Maybe<CancellationTokenSource> _cts = nil;

   public override void Initialize()
   {
      uiMessage.Status = StatusType.Busy;
      uiMessage.Message("IPC server starting");
      _cts = new CancellationTokenSource();
   }

   public override void DoWork()
   {
      if (_cts is (true, var cts))
      {
         NamedPipeIpc.StartServerAsync(pipeName, msg => uiMessage.Do(() => uiMessage.Message(msg)), cts.Token).GetAwaiter().GetResult();
      }
   }

   public override void RunWorkerCompleted()
   {
      if (_cts is (true, var cts))
      {
         cts.Dispose();
         _cts = nil;
      }

      uiMessage.Do(() => uiMessage.Success("IPC stopped"));
      Finalized.Invoke();
   }

   public void Stop()
   {
      if (_cts is (true, var cts))
      {
         cts.Cancel();
      }
   }
}