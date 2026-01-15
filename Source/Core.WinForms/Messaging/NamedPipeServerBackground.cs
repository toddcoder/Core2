using Core.Applications.LongMessaging;
using Core.Applications.Messaging;
using Core.Monads;
using Core.WinForms.Components;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Messaging;

public class NamedPipeServerBackground(string pipeName) : Background
{
   protected Maybe<CancellationTokenSource> _cts = nil;

   public readonly MessageEvent Initialized = new();
   public readonly MessageEvent<string> MessageSent = new();
   public readonly MessageEvent Cancelled = new();

   public override void Initialize()
   {
      Initialized.Invoke();
      _cts = new CancellationTokenSource();
   }

   public override void DoWork()
   {
      if (_cts is (true, var cts))
      {
         NamedPipeIpc.StartServerAsync(pipeName, msg => MessageSent.Invoke(msg), cts.Token).GetAwaiter().GetResult();
      }
   }

   public override void RunWorkerCompleted()
   {
      if (_cts is (true, var cts))
      {
         cts.Dispose();
         _cts = nil;
      }

      Finalized.Invoke();
   }

   public void Stop()
   {
      if (_cts is (true, var cts))
      {
         cts.Cancel();
         Cancelled.Invoke();
      }
   }
}