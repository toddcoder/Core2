using System;

namespace Core.Applications.Messaging;

public class Subscriber<TPayload> : IDisposable where TPayload : notnull
{
   protected Guid id = Guid.NewGuid();

   public readonly MessageEvent<Publication<TPayload>> Received = new();

   public Subscriber()
   {
      Subscribe();
   }

   public Guid Id => id;

   public void Subscribe() => Publisher<TPayload>.AddSubscriber(this);

   public void Unsubscribe() => Publisher<TPayload>.RemoveSubscriber(this);

   public void Dispose()
   {
      Unsubscribe();
      GC.SuppressFinalize(this);
   }

   ~Subscriber() => Unsubscribe();
}