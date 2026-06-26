using System;
using System.Linq;
using System.Reflection;

namespace Core.Applications.Messaging;

public abstract class Channel<TQuery, TResponse>(string name, bool autoSubscribe = true) : IDisposable where TQuery : notnull
   where TResponse : notnull
{
   protected const string QUERY = "Query";
   protected const string RESPONSE = "Response";

   protected Subscriber<TQuery> subscriberQuery = new($"query-{name}", autoSubscribe);
   protected Subscriber<TResponse> subscriberResponse = new($"response-{name}", autoSubscribe);
   protected Publisher<TQuery> publisherQuery = new($"query-{name}");

   public void Subscribe()
   {
      subscriberQuery.Subscribe();
      subscriberResponse.Subscribe();
   }

   protected void setUpQuery(Type type)
   {
      var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith(QUERY));
      foreach (var methodInfo in methods)
      {
         var returnType = methodInfo.ReturnType;
         var parameters = methodInfo.GetParameters();
         if (returnType == typeof(TResponse) && parameters.Length == 1)
         {
            var topic = methodInfo.Name[QUERY.Length..];
            var parameterType = parameters[0].ParameterType;
            if (parameterType == typeof(TQuery))
            {
               subscriberQuery.SetTopic(topic,  (TQuery query) =>
               {
                  var response = (TResponse)methodInfo.Invoke(this, [query])!;
                  subscriberResponse.InvokeTopic(new Publication<TResponse>(topic, response));
               });
            }
            else if (parameterType == typeof(Func<TQuery>))
            {
               subscriberQuery.SetTopic(topic, func =>
               {
                  var response = (TResponse)methodInfo.Invoke(this, [func()])!;
                  subscriberResponse.InvokeTopic(new Publication<TResponse>(topic, response));
               });
            }
         }
         else if (returnType == typeof(void) && parameters.Length == 1)
         {
            var topic = methodInfo.Name[QUERY.Length..];
            var parameterType = parameters[0].ParameterType;
            if (parameterType == typeof(TQuery))
            {
               subscriberQuery.SetTopic(topic, (TQuery query) => methodInfo.Invoke(this, [query]));
            }
            else if (parameterType == typeof(Func<TQuery>))
            {
               subscriberQuery.SetTopic(topic, func => methodInfo.Invoke(this, [func()]));
            }
         }
      }
   }

   protected void setUpResponse(Type type)
   {
      var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith(RESPONSE));
      foreach (var methodInfo in methods)
      {
         var parameters = methodInfo.GetParameters();
         if (parameters.Length == 1 && parameters[0].ParameterType == typeof(TResponse))
         {
            var topic = methodInfo.Name[RESPONSE.Length..];
            subscriberResponse.SetTopic(topic, (TResponse response) => methodInfo.Invoke(this, [response]));
         }
      }
   }

   public void Send(string topic, TQuery query) => publisherQuery.Publish(topic, query);

   public virtual void Start()
   {
      var type = GetType();
      setUpQuery(type);
      setUpResponse(type);
   }

   public void QuerySuspend() => subscriberQuery.Suspend();

   public void QueryResume() => subscriberQuery.Resume();

   public void ResponseSuspend() => subscriberResponse.Suspend();

   public void ResponseResume() => subscriberResponse.Resume();

   public void Suspend()
   {
      QuerySuspend();
      ResponseSuspend();
   }

   public void Resume()
   {
      QueryResume();
      ResponseResume();
   }

   public void Dispose()
   {
      subscriberQuery.Unsubscribe();
      subscriberResponse.Unsubscribe();
   }
}