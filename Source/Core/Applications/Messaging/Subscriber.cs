using System;

namespace Core.Applications.Messaging;

public class Subscriber<TPayload> : IDisposable where TPayload : notnull
{
   protected string name;
   protected Guid id = Guid.NewGuid();

   public readonly MessageEvent<Publication<TPayload>> Received = [];

   public Subscriber(string name, bool autoSubscribe = true)
   {
      this.name = name;
      if (autoSubscribe)
      {
         Subscribe();
      }
   }

   public Guid Id => id;

   public void Subscribe() => Publisher<TPayload>.AddSubscriber(name, this);

   public void Queue() => Publisher<TPayload>.QueueSubscriber(name, this);

   public void Unsubscribe() => Publisher<TPayload>.RemoveSubscriber(name, this);

   public void Dispose()
   {
      Unsubscribe();
      GC.SuppressFinalize(this);
   }

   ~Subscriber() => Unsubscribe();
}

public class Subscriber : IDisposable
{
   protected string name;
   protected Guid id = Guid.NewGuid();

   public readonly MessageEvent<Publication> Received = [];

   public Subscriber(string name, bool autoSubscribe = true)
   {
      this.name = name;
      if (autoSubscribe)
      {
         Subscribe();
      }
   }

   public Guid Id => id;

   public void Subscribe() => Publisher.AddSubscriber(name, this);

   public void Queue() => Publisher.QueueSubscriber(name, this);

   public void Unsubscribe() => Publisher.RemoveSubscriber(name, this);

   public void Dispose()
   {
      Unsubscribe();
      GC.SuppressFinalize(this);
   }

   ~Subscriber() => Unsubscribe();
}