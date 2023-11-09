using System;
using Core.Dates.Now;
using Core.Objects;

namespace Core.Dates;

public class Trigger
{
   public static implicit operator Trigger(string interval) => new(interval.Value().TimeSpan());

   protected DateTime targetTime;
   protected TimeSpan interval;

   public Trigger(TimeSpan interval)
   {
      this.interval = interval;
      setTargetTime();
   }

   public bool Triggered
   {
      get
      {
         if (NowServer.Now > targetTime)
         {
            setTargetTime();
            return true;
         }
         else
         {
            return false;
         }
      }
   }

   protected void setTargetTime() => targetTime = NowServer.Now + interval;

   public void Reset() => setTargetTime();

   public override string ToString() => interval.ToString();
}