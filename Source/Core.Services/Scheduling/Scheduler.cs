using Core.Dates.Now;
using Core.Enumerables;
using Core.Monads;
using Core.Matching;
using static Core.Monads.MonadFunctions;

namespace Core.Services.Scheduling;

public class Scheduler
{
   public static implicit operator Scheduler(string source) => new(source);

   protected Schedule[] schedules;
   protected int lastScheduleIndex;
   protected Maybe<DateTime> _lastTargetDateTime;
   protected int nextScheduleIndex;
   protected Maybe<DateTime> _nextTargetDateTime;

   public Scheduler(string source, bool autoNext = false, bool noSchedules = false)
   {
      schedules = [.. source.Unjoin("/s* ',' /s*; f").Select(s => new Schedule(s, autoNext))];
      lastScheduleIndex = -1;
      nextScheduleIndex = -1;
      _lastTargetDateTime = nil;
      _nextTargetDateTime = nil;

      if (!noSchedules)
      {
         setNextTargetDateTime();
      }
   }

   public bool AutoNext
   {
      get => schedules.All(s => s.AutoNext);
      set
      {
         foreach (var schedule in schedules)
         {
            schedule.AutoNext = value;
         }
      }
   }

   protected void setNextTargetDateTime()
   {
      setLastTargetDateTime();

      for (var i = 0; i < schedules.Length; i++)
      {
         var schedule = schedules[i];
         schedule.Next();
         if (_nextTargetDateTime is (true, var nextTargetDateTime) && nextTargetDateTime > schedule.TargetDateTime || !_nextTargetDateTime)
         {
            nextScheduleIndex = i;
            _nextTargetDateTime = schedule.TargetDateTime;
         }
      }
   }

   protected void setLastTargetDateTime()
   {
      if (_nextTargetDateTime is (true, var nextTargetDateTime))
      {
         _lastTargetDateTime = nextTargetDateTime;
         lastScheduleIndex = nextScheduleIndex;
      }
   }

   public virtual bool Triggered
   {
      get
      {
         var date = assertNextTargetDateTime();

         var now = NowServer.Now;
         var triggered = now >= date;
         var schedule = schedules[nextScheduleIndex];
         if (triggered && schedule.AutoNext)
         {
            Next();
         }

         return schedule.Bracket.Within(now) && triggered;
      }
   }

   protected DateTime assertLastTargetDateTime() => _lastTargetDateTime.Required("Last target date/time not determined");

   protected DateTime assertNextTargetDateTime() => _nextTargetDateTime.Required("Next target date/time not determined");

   public virtual void Next()
   {
      setLastTargetDateTime();

      schedules[nextScheduleIndex].Next();
      _nextTargetDateTime = nil;
      for (var i = 0; i < schedules.Length; i++)
      {
         var schedule = schedules[i];
         if (_nextTargetDateTime is (true, var nextTargetDateTime) && nextTargetDateTime > schedule.TargetDateTime || !_nextTargetDateTime)
         {
            nextScheduleIndex = i;
            _nextTargetDateTime = schedule.TargetDateTime;
         }
      }
   }

   public DateTime LastTargetTime => assertLastTargetDateTime();

   public DateTime NextTargetDateTime => assertNextTargetDateTime();

   public Schedule LastSchedule
   {
      get
      {
         _ = assertLastTargetDateTime();
         return schedules[lastScheduleIndex];
      }
   }

   public Schedule NextSchedule
   {
      get
      {
         _ = assertNextTargetDateTime();
         return schedules[nextScheduleIndex];
      }
   }

   public override string ToString() => schedules.ToString(", ");
}