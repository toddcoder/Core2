using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Assertions;
using Core.Enumerables;

namespace Core.Objects;

public readonly struct CoreSpan<T> : IEquatable<CoreSpan<T>>, IEnumerable<T>
{
   public struct Enumerator(CoreSpan<T> span) : IEnumerator<T>
   {
      private readonly CoreSpan<T> span = span;
      private int index = -1;

      public bool MoveNext()
      {
         var nextIndex = index + 1;
         if (nextIndex < span.Length)
         {
            index = nextIndex;
            return true;
         }
         else
         {
            return false;
         }
      }

      public void Reset() => index = -1;

      public T Current => span[index];

      object? IEnumerator.Current => Current;

      public void Dispose()
      {
      }
   }

   public static implicit operator CoreSpan<T>(T[] array) => new(array);

   public static CoreSpan<T> Empty => new([]);

   private readonly T[] reference;
   private readonly int length;

   public CoreSpan(T[] array)
   {
      reference = array;
      length = array.Length;
   }

   public CoreSpan(T[] array, int start, int length)
   {
      (start + length).Must().BeLessThanOrEqual(array.Length).OrThrow();
      reference = [.. array.Skip(start).Take(length)];
   }

   public T this[int index]
   {
      get
      {
         index.Must().BeLessThan(length).OrThrow();
         return reference[index];
      }
   }

   public int Length => length;

   public bool IsEmpty => length == 0;

   public bool Equals(CoreSpan<T> other) => reference.Equals(other.reference) && length == other.length;

   public IEnumerator<T> GetEnumerator() => new Enumerator(this);

   public override bool Equals(object? obj) => obj is CoreSpan<T> other && Equals(other);

   public override int GetHashCode() => HashCode.Combine(reference, length);

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public static bool operator ==(CoreSpan<T> left, CoreSpan<T> right) => left.Equals(right);

   public static bool operator !=(CoreSpan<T> left, CoreSpan<T> right) => !left.Equals(right);

   public CoreSpan<T> Copy()
   {
      var destination = new T[length];
      Array.Copy(reference, destination, length);

      return new CoreSpan<T>(destination);
   }

   public override string ToString()
   {
      if (typeof(T) == typeof(char))
      {
         return reference.ToString("");
      }
      else
      {
         return $"CoreSpan<{typeof(T).Name}>[{length}]";
      }
   }

   public CoreSpan<T> Slice(int start)
   {
      start.Must().BeLessThan(length).OrThrow();
      return new CoreSpan<T>([.. reference.Skip(start)]);
   }

   public CoreSpan<T> Slice(int start, int length)
   {
      (start + length).Must().BeLessThanOrEqual(this.length).OrThrow();
      return new CoreSpan<T>([..reference.Skip(start).Take(length)]);
   }

   public T[] ToArray()
   {
      var array = new T[length];
      Array.Copy(reference, array, length);

      return array;
   }
}