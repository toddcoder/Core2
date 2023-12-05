using System;
using System.Collections;
using System.Collections.Generic;
using Core.DataStructures;
using Core.Monads;

namespace Core.Arrays;

public class ResizableMatrix<T> : IEnumerable<T> where T : notnull
{
   protected int rows;
   protected int columns;
   protected Either<T, Func<T>> defaultValue;
   protected T[,] array;

   public ResizableMatrix(int rows, int columns, T defaultValue)
   {
      this.rows = rows;
      this.columns = columns;
      this.defaultValue = defaultValue;

      array = new T[rows, columns];
   }

   public ResizableMatrix(int rows, int columns, Func<T> defaultValue)
   {
      this.rows = rows;
      this.columns = columns;
      this.defaultValue = defaultValue;

      array = new T[rows, columns];
   }

   public ResizableMatrix(int rows, int columns, T defaultValue, IEnumerable<T> enumerable) : this(rows, columns, defaultValue)
   {
      fill(enumerable);
   }

   public ResizableMatrix(int rows, int columns, Func<T> defaultValue, IEnumerable<T> enumerable) : this(rows, columns, defaultValue)
   {
      fill(enumerable);
   }

   public ResizableMatrix(int rows, int columns, T defaultValue, params T[] array) : this(rows, columns, defaultValue)
   {
      fill(array);
   }

   public ResizableMatrix(int rows, int columns, Func<T> defaultValue, params T[] array) : this(rows, columns, defaultValue)
   {
      fill(array);
   }

   public bool AutoSize { get; set; }

   protected Func<T> getDefaultFunc() => defaultValue switch
   {
      (true, var value, _) => () => value,
      (false, _, var defaultFunc) => defaultFunc,
      _ => () => default!
   };

   protected void fill(IEnumerable<T> enumerable)
   {
      var queue = new MaybeQueue<T>(enumerable);
      var func = getDefaultFunc();

      for (var row = 0; row < rows; row++)
      {
         for (var column = 0; column < columns; column++)
         {
            this[row, column] = queue.Dequeue() | func;
         }
      }
   }

   protected void resize(int rowCount, int columnCount)
   {
      var queue = new MaybeQueue<T>();
      for (var row = 0; row < rows; row++)
      {
         for (var column = 0; column < columns; column++)
         {
            queue.Enqueue(array[row, column]);
         }
      }

      rows = rowCount;
      columns = columnCount;
      var newArray = new T[rows, columns];
      var func = getDefaultFunc();
      for (var row = 0; row < rows; row++)
      {
         for (var column = 0; column < columns; column++)
         {
            newArray[row, column] = queue.Dequeue() | func;
         }
      }

      array = newArray;
   }

   protected void ensureSize(int row, int column)
   {
      if (AutoSize)
      {
         var rowCount = row + 1;
         var columnCount = column + 1;
         if (rowCount > rows || columnCount > columns)
         {
            resize(rowCount, columnCount);
         }
      }
   }

   public T this[int row, int column]
   {
      get
      {
         ensureSize(row, column);
         return array[row, column];
      }
      set
      {
         ensureSize(row, column);
         array[row, column] = value;
      }
   }

   public IEnumerator<T> GetEnumerator()
   {
      for (var row = 0; row < rows; row++)
      {
         for (var column = 0; column < columns; column++)
         {
            yield return array[row, column];
         }
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public IEnumerable<(int row, int column, T item)> WithCoordinates()
   {
      for (var row = 0; row < rows; row++)
      {
         for (var column = 0; column < columns; column++)
         {
            yield return (row, column, this[row, column]);
         }
      }
   }

   public void Resize(int rowCount, int columnCount) => resize(rowCount, columnCount);

   public void Resize(int rowCount, int columnCount, T defaultValue)
   {
      this.defaultValue = defaultValue;
      resize(rowCount, columnCount);
   }

   public void Resize(int rowColumn, int columnColumn, Func<T> defaultValue)
   {
      this.defaultValue = defaultValue;
      resize(rowColumn, columnColumn);
   }
}