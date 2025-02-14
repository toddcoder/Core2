using System;

namespace Core.Applications.Messaging;

public class Subscriber<TPayload> : IDisposable where TPayload : notnull
{
   protected Guid id = Guid.NewGuid();
   protected string topic;

   public readonly MessageEvent<TPayload> Received = new();

   public Subscriber(string topic)
   {
      this.topic = topic;

      Publisher<TPayload>.AddSubscriber(this);
   }

   public Guid Id => id;

   public string Topic => topic;

   public void Subscribe() => Publisher<TPayload>.AddSubscriber(this);

   public void Unsubscribe() => Publisher<TPayload>.RemoveSubscriber(this);

   public void Dispose()
   {
      Publisher<TPayload>.RemoveSubscriber(this);
      GC.SuppressFinalize(this);
   }

   ~Subscriber()
   {
      Publisher<TPayload>.RemoveSubscriber(this);
   }
}