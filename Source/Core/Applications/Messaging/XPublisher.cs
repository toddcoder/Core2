using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Configurations;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public class XPublisher
{
   protected string name;
   protected int timeout = 5000;

   public XPublisher(string name)
   {
      this.name = name;
   }

   public Result<Unit> Publish(string topic, Setting payload)
   {
      try
      {
         var nameTopic = $"{name}${topic}";
         using var pipeServer = new NamedPipeServerStream(nameTopic, PipeDirection.Out);
         pipeServer.WriteTimeout = timeout;
         pipeServer.WaitForConnection();

         using var writer = new StreamWriter(pipeServer, Encoding.UTF8);
         writer.AutoFlush = true;
         writer.Write(payload);

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public async Task<Completion<Unit>> PublishAsync(string topic, Setting payload)
   {
      try
      {
         var nameTopic = $"{name}${topic}";
         using var source = new CancellationTokenSource();
         await using var pipeServer = new NamedPipeServerStream(nameTopic, PipeDirection.Out);
         var connectionTask = pipeServer.WaitForConnectionAsync(source.Token);
         var timeoutTask = Task.Delay(timeout, source.Token);
         var completedTask = await Task.WhenAny(connectionTask, timeoutTask);
         if (completedTask == timeoutTask)
         {
            await source.CancelAsync();
            return nil;
         }

         await using var writer = new StreamWriter(pipeServer, Encoding.UTF8);
         await writer.WriteAsync(payload.ToString());
         await writer.FlushAsync();

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}