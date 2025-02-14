using System;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public class MessageEvent<TPayload> where TPayload : notnull
{
   protected Maybe<Action<TPayload>> _handler = nil;

   public Action<TPayload> Handler
   {
      set => _handler = value;
   }

   public void Invoke(TPayload payload)
   {
      if (_handler is (true, var handler))
      {
         handler(payload);
      }
   }

   public async Task InvokeAsync(TPayload payload)
   {
      if (_handler is (true, var asyncHandler))
      {
         await Task.Run(() => asyncHandler(payload));
      }
   }

   public void ClearHandler() => _handler = nil;
}