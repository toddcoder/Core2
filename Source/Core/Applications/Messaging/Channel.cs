using System;
using System.Linq;
using System.Reflection;
using Core.Collections;

namespace Core.Applications.Messaging;

public abstract class Channel<TQuery, TResponse>(string name, bool autoSubscribe = true) : IDisposable where TQuery : notnull
   where TResponse : notnull
{
   protected Subscriber<TQuery> subscriberQuery = new($"query-{name}", autoSubscribe);
   protected Subscriber<TResponse> subscriberResponse = new($"response-{name}", autoSubscribe);

   public void Subscribe()
   {
      subscriberQuery.Subscribe();
      subscriberResponse.Subscribe();
   }

   protected static void subscribeAll<T>(Channel<TQuery, TResponse> channel, Type type, string prefix, Subscriber<T> subscriber) where T : notnull
   {
      var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith(prefix));
      foreach (var methodInfo in methods)
      {
         var parameters = methodInfo.GetParameters();
         if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Publication<T>))
         {
            var topic = methodInfo.Name[prefix.Length..];
            subscriber[topic] = publication => methodInfo.Invoke(channel, [publication]);
         }
      }

      var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.Name.StartsWith(prefix));
      foreach (var propertyInfo in properties)
      {
         if (propertyInfo.PropertyType == typeof(Action<Publication<T>>))
         {
            var topic = propertyInfo.Name[prefix.Length..];
            subscriber[topic] = publication => ((Action<Publication<T>>)propertyInfo.GetValue(channel)!)(publication);
         }
      }
   }

   public virtual void Start()
   {
      var type = GetType();
      subscribeAll(this, type, "Query", subscriberQuery);
      subscribeAll(this, type, "Response", subscriberResponse);
   }

   public void Dispose()
   {
      subscriberQuery.Unsubscribe();
      subscriberResponse.Unsubscribe();
   }
}