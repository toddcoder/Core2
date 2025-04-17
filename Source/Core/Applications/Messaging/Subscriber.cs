using System;
using System.Threading.Tasks;
using Core.Collections;

namespace Core.Applications.Messaging;

public class Subscriber<TPayload> : IDisposable where TPayload : notnull
{
   protected string name;
   protected Guid id = Guid.NewGuid();
   protected StringHash<MessageEvent<Publication<TPayload>>> events = [];

   public readonly MessageEvent<Publication<TPayload>> Received = new();

   public Subscriber(string name, bool autoSubscribe = true)
   {
      this.name = name;
      if (autoSubscribe)
      {
         Subscribe();
      }
   }

   public Action<Publication<TPayload>> this[string topic]
   {
      set => events[topic] = new MessageEvent<Publication<TPayload>> { Handler = value };
   }

   public void InvokeTopic(Publication<TPayload> payload)
   {
      if (events.Maybe[payload.Topic] is (true, var handler))
      {
         handler.Invoke(payload);
      }
      else
      {
         Received.Invoke(payload);
      }
   }

   public async Task InvokeTopicAsync(Publication<TPayload> payload)
   {
      if (events.Maybe[payload.Topic] is (true, var handler))
      {
         await handler.InvokeAsync(payload);
      }
      else
      {
         await Received.InvokeAsync(payload);
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

public class Subscriber<TTopic, TPayload> : IDisposable where TTopic : notnull where TPayload : notnull
{
   protected string name;
   protected Guid id = Guid.NewGuid();
   protected Hash<TTopic, MessageEvent<Publication<TTopic, TPayload>>> events = [];

   public readonly MessageEvent<Publication<TTopic, TPayload>> Received = new();

   public Subscriber(string name, bool autoSubscribe = true)
   {
      this.name = name;
      if (autoSubscribe)
      {
         Subscribe();
      }
   }

   public Action<Publication<TTopic, TPayload>> this[TTopic topic]
   {
      set => events[topic] = new MessageEvent<Publication<TTopic, TPayload>> { Handler = value };
   }

   public void InvokeTopic(Publication<TTopic, TPayload> payload)
   {
      if (events.Maybe[payload.Topic] is (true, var handler))
      {
         handler.Invoke(payload);
      }
      else
      {
         Received.Invoke(payload);
      }
   }

   public async Task InvokeTopicAsync(Publication<TTopic, TPayload> payload)
   {
      if (events.Maybe[payload.Topic] is (true, var handler))
      {
         await handler.InvokeAsync(payload);
      }
      else
      {
         await Received.InvokeAsync(payload);
      }
   }

   public Guid Id => id;

   public void Subscribe() => Publisher<TTopic, TPayload>.AddSubscriber(name, this);

   public void Queue() => Publisher<TTopic, TPayload>.QueueSubscriber(name, this);

   public void Unsubscribe() => Publisher<TTopic, TPayload>.RemoveSubscriber(name, this);

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
   protected StringHash<MessageEvent<Publication>> events = [];

   public readonly MessageEvent<Publication> Received = new();

   public Subscriber(string name, bool autoSubscribe = true)
   {
      this.name = name;
      if (autoSubscribe)
      {
         Subscribe();
      }
   }

   public Action<Publication> this[string topic]
   {
      set => events[topic] = new MessageEvent<Publication> { Handler = value };
   }

   public void InvokeTopic(Publication payload)
   {
      if (events.Maybe[payload.Topic] is (true, var handler))
      {
         handler.Invoke(payload);
      }
      else
      {
         Received.Invoke(payload);
      }
   }

   public async Task InvokeTopicAsync(Publication payload)
   {
      if (events.Maybe[payload.Topic] is (true, var handler))
      {
         await handler.InvokeAsync(payload);
      }
      else
      {
         await Received.InvokeAsync(payload);
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