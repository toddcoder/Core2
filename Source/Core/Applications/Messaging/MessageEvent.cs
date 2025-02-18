using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public class MessageEvent<TPayload> : IList<Action<TPayload>> where TPayload : notnull
{
   protected Maybe<Action<TPayload>> _handler = nil;
   protected List<Action<TPayload>> handlers = [];

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
      else
      {
         foreach (var handlerItem in handlers)
         {
            handlerItem(payload);
            System.Threading.Thread.Sleep(500);
         }
      }
   }

   public async Task InvokeAsync(TPayload payload)
   {
      if (_handler is (true, var asyncHandler))
      {
         await Task.Run(() => asyncHandler(payload));
      }
      else
      {
         foreach (var asyncHandlerItem in handlers)
         {
            await Task.Run(() => asyncHandlerItem);
         }
      }
   }

   public void ClearHandler() => _handler = nil;

   public IEnumerator<Action<TPayload>> GetEnumerator() => handlers.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Add(Action<TPayload> item) => handlers.Add(item);

   public void Clear() => handlers.Clear();

   public bool Contains(Action<TPayload> item) => handlers.Contains(item);

   public void CopyTo(Action<TPayload>[] array, int arrayIndex) => handlers.CopyTo(array, arrayIndex);

   public bool Remove(Action<TPayload> item) => handlers.Remove(item);

   public int Count => handlers.Count;

   public bool IsReadOnly => false;

   public int IndexOf(Action<TPayload> item) => handlers.IndexOf(item);

   public void Insert(int index, Action<TPayload> item) => handlers.Insert(index, item);

   public void RemoveAt(int index) => handlers.RemoveAt(index);

   public Action<TPayload> this[int index]
   {
      get => handlers[index];
      set => handlers[index] = value;
   }
}