using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Assertions;
using Core.Enumerables;
using Core.Monads;
using Core.Monads.Lazy;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.DataStructures;

public class MaybeStack<T> : IEnumerable<T> where T : notnull
{
   protected Stack<T> stack;
   protected Maybe<T> _last;

   public MaybeStack()
   {
      stack = new Stack<T>();
      _last = nil;
   }

   public MaybeStack(IEnumerable<T> collection)
   {
      stack = new Stack<T>(collection);
      _last = nil;
   }

   public MaybeStack(int capacity)
   {
      stack = new Stack<T>(capacity);
      _last = nil;
   }

   public int Count => stack.Count;

   public void Clear() => stack.Clear();

   public bool Contains(T item) => stack.Contains(item);

   public Result<T[]> ToArray(int arrayIndex = 0)
   {
      return
         from assertion in arrayIndex.Must().BeBetween(0).Until(Count).OrFailure()
         from array in tryTo(() =>
         {
            var result = new T[Count];
            stack.CopyTo(result, arrayIndex);

            return result;
         })
         select array;
   }

   public Result<T> Item(int index)
   {
      T[] getArray() => [.. stack.Skip(index).Take(1)];

      return
         from assertion in index.Must().BeBetween(0).Until(Count).OrFailure()
         from item in tryTo(() => getArray()[0])
         select item;
   }

   public Maybe<T> Peek() => maybe<T>() & IsNotEmpty & (() => stack.Peek());

   public Maybe<T> Pop() => maybe<T>() & IsNotEmpty & (() => stack.Pop());

   public void Push(T item) => stack.Push(item);

   public void Add(T item) => stack.Push(item);

   public T[] ToArray() => [.. stack];

   public IEnumerator<T> GetEnumerator() => stack.GetEnumerator();

   public override string ToString() => stack.ToString(", ");

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void TrimExcess() => stack.TrimExcess();

   public bool IsEmpty => stack.Count == 0;

   public bool IsNotEmpty => stack.Count > 0;

   public ResultStack<T> TryTo => new(this);

   public IEnumerable<T> Popping()
   {
      LazyMaybe<T> _item = nil;
      while (_item.ValueOf(Pop(), true))
      {
         yield return _item;
      }
   }

   public bool More()
   {
      _last = Pop();
      return _last;
   }

   public Maybe<T> Last => _last;
}