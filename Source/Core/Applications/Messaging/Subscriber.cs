using System;
using Core.Monads;

namespace Core.Applications.Messaging;

public class Subscriber<TPayload> : IDisposable where TPayload : notnull
{
   protected Guid id = Guid.NewGuid();

   public readonly MessageEvent<Publication<TPayload>> Received = [];

   public Subscriber()
   {
      Subscribe();
   }

   public Guid Id => id;

   public string Reader { get; set; } = "";

   public void Subscribe() => Publisher<TPayload>.AddSubscriber(this);

   public void Unsubscribe() => Publisher<TPayload>.RemoveSubscriber(this);

   public void Dispose()
   {
      Unsubscribe();
      GC.SuppressFinalize(this);
   }

   ~Subscriber() => Unsubscribe();
}

public class Subscriber : Subscriber<Unit>
{
   public new readonly MessageEvent<Publication> Received = [];

   public Subscriber()
   {
      Subscribe();
   }
}