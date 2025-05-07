using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Applications.Messaging;
using Core.Dates.DateIncrements;
using Core.Monads;

namespace Core.Applications.AsyncApplication;

public abstract class Service
{
   protected string[] args = [];

   public readonly MessageEvent Started = new();
   public readonly MessageEvent<Exception> ExceptionRaised = new();
   public readonly MessageEvent Canceled = new();
   public readonly MessageEvent Stopped = new();

   public async Task Main(string[] args)
   {
      this.args = args;

      if (StartingMessage is (true, var startingMessage))
      {
         Console.WriteLine(startingMessage);
      }

      using var cts = new CancellationTokenSource();
      Console.CancelKeyPress += async (_, e) =>
      {
         if (StoppingMessage is (true, var stoppingMessage))
         {
            Console.WriteLine(stoppingMessage);
         }

         await cts.CancelAsync();
         e.Cancel = true;
         await Canceled.InvokeAsync();
      };

      await StartUpAsync(cts.Token);
      await Started.InvokeAsync();
      await RunWrapperAsync(cts.Token);
      await ShutDownAsync(cts.Token);
      await Stopped.InvokeAsync();
   }

   protected async Task RunWrapperAsync(CancellationToken token)
   {
      try
      {
         while (!token.IsCancellationRequested)
         {
            await RunAsync(token);
            Thread.Sleep(SleepTime);
         }
      }
      catch (TaskCanceledException)
      {
         await Canceled.InvokeAsync();
      }
      catch (Exception exception)
      {
         await ExceptionRaised.InvokeAsync(exception);
      }
   }

   public virtual Task StartUpAsync(CancellationToken cancellationToken) => Task.CompletedTask;

   public virtual Task ShutDownAsync(CancellationToken cancellationToken) => Task.CompletedTask;

   public abstract Task RunAsync(CancellationToken cancellationToken);

   public Maybe<string> StartingMessage { get; set; } = "Starting service...";

   public Maybe<string> StoppingMessage { get; set; } = "Stopping service...";

   public TimeSpan SleepTime { get; set; } = 500.Milliseconds();
}