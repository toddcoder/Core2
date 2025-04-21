using System;
using System.Linq;
using System.Timers;
using Core.Applications.Messaging;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections.Expiring;

public class ExpiringCache<TKey, TValue> : IHash<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected Hash<TKey, TValue> cache;
   protected Hash<TKey, ExpirationPolicy<TValue>> expirationPolicies;
   protected Maybe<Timer> _timer;
   protected object locker;
   protected Func<ExpirationPolicy<TValue>> newPolicy;

   public readonly MessageEvent<ExpirationArgs<TKey, TValue>> Expired = new();

   public ExpiringCache(TimeSpan activeMonitoringInterval)
   {
      cache = [];
      expirationPolicies = [];
      var newTimer = new Timer(activeMonitoringInterval.TotalMilliseconds);
      locker = new object();
      newTimer.Elapsed += (_, _) =>
      {
         lock (locker)
         {
            TKey[] expiredKeys = [.. cache.Where(i => expirationPolicies.Maybe[i.Key].Map(v => v.ItemEvictable(i.Value)) | false).Select(i => i.Key)];
            foreach (var key in expiredKeys)
            {
               var args = new ExpirationArgs<TKey, TValue>(key, cache[key]);
               Expired.Invoke(args);
               if (!args.CancelEviction)
               {
                  cache.Remove(key);
                  expirationPolicies.Remove(key);
               }
            }
         }
      };

      _timer = newTimer;
      newPolicy = () => new NonExpiration<TValue>();
   }

   public ExpiringCache()
   {
      cache = [];
      expirationPolicies = [];
      _timer = nil;
      locker = new object();
      newPolicy = () => new NonExpiration<TValue>();
   }

   public Func<ExpirationPolicy<TValue>> NewPolicy
   {
      get => newPolicy;
      set => newPolicy = value;
   }

   public void StartMonitoring()
   {
      if (_timer is (true, var timer))
      {
         timer.Enabled = true;
      }
   }

   public void StopMonitoring()
   {
      if (_timer is (true, var timer))
      {
         timer.Enabled = false;
      }
   }

   public TValue this[TKey key]
   {
      get
      {
         lock (locker)
         {
            var _value = cache.Maybe[key];
            if (_value is (true, var value))
            {
               var policy = expirationPolicies.Find(key, _ => newPolicy(), true);
               policy.Reset();
               if (policy.ItemEvictable(value))
               {
                  cache.Remove(key);
                  expirationPolicies.Remove(key);

                  return default!;
               }
               else
               {
                  return value;
               }
            }
            else
            {
               return default!;
            }
         }
      }
      set
      {
         lock (locker)
         {
            cache[key] = value;
            expirationPolicies[key] = newPolicy();
         }
      }
   }

   public bool ContainsKey(TKey key)
   {
      lock (locker)
      {
         return cache.ContainsKey(key);
      }
   }

   public Hash<TKey, TValue> GetHash() => cache;

   public HashInterfaceMaybe<TKey, TValue> Items => new(this);

   public void Remove(TKey key)
   {
      lock (locker)
      {
         cache.Remove(key);
         expirationPolicies.Remove(key);
      }
   }
}