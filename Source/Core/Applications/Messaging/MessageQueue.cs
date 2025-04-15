using System;
using System.Threading;
using Core.DataStructures;
using Core.Dates.DateIncrements;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public class MessageQueue<TArgument, TResult> where TArgument : notnull where TResult : notnull
{
   public record MessageEnvelope(string Id, TArgument Argument);

   protected const string MESSAGE_QUEUE_NAME = "__$message-queue";

   protected static MaybeQueue<MessageEnvelope> incomingQueue = [];

   public static string MessageQueueName()
   {
      return $"{MESSAGE_QUEUE_NAME}+{getName(typeof(TArgument))}+{getName(typeof(TResult))}";

      string getName(Type type) => type.Namespace ?? type.Name;
   }

   public readonly MessageEvent<TimeSpan> Waiting = new();

   public Optional<TResult> Send(TArgument argument, TimeSpan timeoutSpan)
   {
      var id = Guid.NewGuid().ToString();
      Optional<TResult> _result = nil;

      var subscriber = new Subscriber<TResult>(MessageQueueName());
      subscriber.Received.Handler = p =>
      {
         if (p.Topic == id)
         {
            _result = p.Payload;
         }
      };
      try
      {
         incomingQueue.Enqueue(new MessageEnvelope(id, argument));

         Dates.Timeout timeout = timeoutSpan;
         timeout.Pending.Handler = Waiting.Invoke;
         var sleepSpan = 100.Milliseconds();
         while (!_result && timeout.IsPending())
         {
            Thread.Sleep(sleepSpan);
         }

         if (timeout.Expired)
         {
            return new TimeoutException($"Timeout expired after {timeoutSpan}");
         }
         else
         {
            return _result;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
      finally
      {
         subscriber.Unsubscribe();
      }
   }

   public Optional<TResult> Send(TArgument argument) => Send(argument, 5.Seconds());

   public Maybe<MessageEnvelope> Receive() => incomingQueue.Dequeue();

   public void SendBack(string id, TResult result)
   {
      Publisher<TResult>.Publish(MessageQueueName(), id, result);
   }
}