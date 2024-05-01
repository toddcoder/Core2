﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.DataStructures;

public class MaybeQueue<T> : IQueue<T>, IEnumerable<T> where T : notnull
{
   protected Queue<T> queue;
   protected Maybe<T> _last;

   public MaybeQueue()
   {
      queue = new Queue<T>();
      _last = nil;
   }

   public MaybeQueue(IEnumerable<T> collection)
   {
      queue = new Queue<T>(collection);
      _last = nil;
   }

   public MaybeQueue(int capacity)
   {
      queue = new Queue<T>(capacity);
      _last = nil;
   }

   public int Count => queue.Count;

   public void Clear() => queue.Clear();

   public bool Contains(T item) => queue.Contains(item);

   public Result<T[]> ToArray(int arrayIndex = 0)
   {
      return
         from assertion in arrayIndex.Must().BeBetween(0).Until(Count).OrFailure()
         from array in tryTo(() =>
         {
            var result = new T[Count];
            queue.CopyTo(result, arrayIndex);

            return result;
         })
         select array;
   }

   public Result<T> Item(int index)
   {
      return
         from assertion in index.Must().BeBetween(0).Until(Count).OrFailure()
         from item in tryTo(() => ((T[]) [.. queue.Skip(index).Take(1)])[0])
         select item;
   }

   public Maybe<T> Dequeue() => maybe<T>() & IsNotEmpty & (() => queue.Dequeue());

   public void Enqueue(T item) => queue.Enqueue(item);

   public void Add(T item) => queue.Enqueue(item);

   public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();

   public override bool Equals(object? obj) => obj is MaybeQueue<T> q && q == this;

   public override int GetHashCode() => queue.GetHashCode();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public Maybe<T> Peek() => maybe<T>() & IsNotEmpty & (() => queue.Peek());

   public T[] ToArray() => [.. queue];

   public void TrimExcess() => queue.TrimExcess();

   public bool IsEmpty => queue.Count == 0;

   public bool IsNotEmpty => queue.Count > 0;

   public ResultQueue<T> TryTo => new(this);

   public IEnumerable<T> Dequeuing()
   {
      while (Dequeue() is (true, var item))
      {
         yield return item;
      }
   }

   public bool More()
   {
      _last = Dequeue();
      return _last;
   }

   public Maybe<T> Last => _last;
}