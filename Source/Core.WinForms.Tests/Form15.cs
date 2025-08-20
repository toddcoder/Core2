using System.Diagnostics;
using Core.Dates;
using Core.WinForms.Components;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form15 : Form
{
   protected readonly UiAction uiDisplay = new();
   protected readonly UiAction uiStandard = new();
   protected readonly UiAction uiChannel = new();
   protected readonly UiAction uiElapsedTime = new();
   protected readonly UiAction uiTest = new();

   public Form15()
   {
      InitializeComponent();

      uiDisplay.Message("ready");

      uiStandard.Button("Standard");
      uiStandard.Click += (_, _) =>
      {
         var standard = new Standard(uiDisplay);
         standard.Finalized.Handler = () => uiElapsedTime.Message(standard.Elapsed.ToLongString(true));
         standard.RunWorkerAsync();
      };
      uiStandard.ClickText = "Using InvokeRequired/Invoke";

      uiChannel.Button("Channel");
      uiChannel.Click += (_, _) =>
      {
         var channel = new Channel(uiDisplay);
         channel.Finalized.Handler = () => uiElapsedTime.Message(channel.Elapsed.ToLongString(true));
         channel.RunWorkerAsync();
      };
      uiChannel.ClickText = "Using UiActionChannel";

      uiElapsedTime.Message("ready");

      uiTest.Button("Test");
      uiTest.Click += (_, _) => { };
      uiTest.ClickText = "For tests purposes";

      var builder = new TableLayoutBuilder(tableLayoutPanel1);
      _ = builder.Col * 2 * 50f;
      _ = builder.Row + 60 + 60 + 60 + 100;
      builder.SetUp();

      (builder + uiDisplay).SpanCol(2).Row();

      (builder + uiStandard).Next();
      (builder + uiChannel).Row();

      (builder + uiElapsedTime).Next();
      (builder + uiTest).Row();

      this.Tuck(uiElapsedTime);
   }
}

public class Standard(UiAction uiAction) : Background
{
   protected Stopwatch stopwatch = new();

   public TimeSpan Elapsed => stopwatch.Elapsed;

   public override void Initialize()
   {
      stopwatch.Start();
      uiAction.Busy(true);
   }

   public override void DoWork()
   {
      for (var i = 0; i < 500_000; i++)
      {
         if (i % 100 == 0)
         {
            Work(i);
         }
      }
   }

   public virtual void Work(int i)
   {
      uiAction.Do(() => uiAction.Busy(i.ToString()));
      Application.DoEvents();
   }

   public override void RunWorkerCompleted()
   {
      stopwatch.Stop();
      uiAction.Success("Done");
      Finalized.Invoke();
   }
}

public class Channel(UiAction uiAction) : Standard(uiAction)
{
   protected UiActionChannel channel = new(uiAction);

   public override void Initialize()
   {
      stopwatch.Start();
      channel.Subscribe();
      channel.Send(new UiActionState.Busy(true));
   }

   public override void Work(int i)
   {
      channel.Send(new UiActionState.Standard(i.ToString(), UiActionType.BusyText));
      Application.DoEvents();
   }

   public override void RunWorkerCompleted()
   {
      base.RunWorkerCompleted();
      channel.Unsubscribe();
   }
}