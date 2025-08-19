using Core.Collections;
using Core.DataStructures;
using System;
using System.Threading.Tasks;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public class TypedPublisher<T> where T : notnull
{
   protected static Memo<string, Hash<Guid, TypedSubscriber<T>>> subscribers = new LazyMemo<string, Hash<Guid, TypedSubscriber<T>>>.Function(_ => []);
   protected static MaybeQueue<TypedSubscriber<T>> pendingSubscribers = [];

   public static void AddSubscriber(TypedSubscriber<T> subscriber) => subscribers[typeof(T).Name][subscriber.Id] = subscriber;

   public static void QueueSubscriber(TypedSubscriber<T> subscriber) => pendingSubscribers.Enqueue(subscriber);

   public static void RemoveSubscriber(TypedSubscriber<T> subscriber)
   {
      var name = typeof(T).Name;
      if (subscribers.Find(name) is (true, var hash))
      {
         hash.Maybe[subscriber.Id] = nil;
         if (hash.Count == 0)
         {
            subscribers.Remove(name);
         }
      }
   }

   protected string name = typeof(T).Name;
   protected object mutex = new();

   public void DequeuePendingSubscriptions()
   {
      while (pendingSubscribers.Dequeue() is (true, var subscriber))
      {
         AddSubscriber(subscriber);
      }
   }

   public void Publish(T payload)
   {
      lock (mutex)
      {
         foreach (var (publisherName, hash) in subscribers)
         {
            if (publisherName == name)
            {
               foreach (var (_, subscriber) in hash)
               {
                  Task.Run(() => subscriber.InvokeTopic(payload));
               }
            }
         }

         DequeuePendingSubscriptions();
      }
   }

   public void PublishSync(T payload)
   {
      foreach (var (publisherName, hash) in subscribers)
      {
         if (publisherName == name)
         {
            foreach (var (_, subscriber) in hash)
            {
               subscriber.InvokeTopic(payload);
            }
         }
      }

      DequeuePendingSubscriptions();
   }

   public async Task PublishAsync(T payload)
   {
      foreach (var (publisherName, hash) in subscribers)
      {
         if (publisherName == name)
         {
            foreach (var (_, subscriber) in hash)
            {
               await subscriber.InvokeTopicAsync(payload);
            }
         }
      }

      DequeuePendingSubscriptions();
   }
}