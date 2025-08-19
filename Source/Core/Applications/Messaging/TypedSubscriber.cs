using Core.Collections;
using System;
using System.Threading.Tasks;

namespace Core.Applications.Messaging;

public class TypedSubscriber<T> : IDisposable where T : notnull
{
   protected Guid id = Guid.NewGuid();
   protected Hash<string, MessageEvent<T>> events = [];

   public readonly MessageEvent<T> Received = new();

   public TypedSubscriber(bool autoSubscribe = true)
   {
      if (autoSubscribe)
      {
         Subscribe();
      }
   }

   public Action<T> Handler
   {
      set => events[value.GetType().Name] = new MessageEvent<T> { Handler = value };
   }

   public void InvokeTopic(T payload)
   {
      if (events.Maybe[payload.GetType().Name] is (true, var handler))
      {
         handler.Invoke(payload);
      }
      else
      {
         Received.Invoke(payload);
      }
   }

   public async Task InvokeTopicAsync(T payload)
   {
      if (events.Maybe[payload.GetType().Name] is (true, var handler))
      {
         await handler.InvokeAsync(payload);
      }
      else
      {
         await Received.InvokeAsync(payload);
      }
   }

   public Guid Id => id;

   public void Subscribe() => TypedPublisher<T>.AddSubscriber(this);

   public void Queue() => TypedPublisher<T>.QueueSubscriber(this);

   public void Unsubscribe() => TypedPublisher<T>.RemoveSubscriber(this);

   public void Dispose()
   {
      Unsubscribe();
      GC.SuppressFinalize(this);
   }

   ~TypedSubscriber() => Unsubscribe();
}