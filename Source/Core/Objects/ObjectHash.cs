using Core.Monads;
using System.Runtime.CompilerServices;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public class ObjectHash<T> where T : notnull
{
   protected const int NUM_BINS = 4;

   protected static readonly int[] sizes =
   [
      5,
      11,
      29,
      47,
      97,
      197,
      397,
      797,
      1597,
      3203,
      6421,
      12853,
      25717,
      51437,
      102877,
      205759,
      411527,
      823117,
      1646237,
      3292489,
      6584983
   ];

   protected static readonly int[] sizesWithMaxArraySwitch =
   [
      5,
      11,
      29,
      47,
      97,
      197,
      397,
      797,
      1597,
      3203,
      6421,
      12853,
      25717,
      51437,
      102877,
      205759,
      411527,
      823117,
      1646237,
      3292489,
      6584983,
      13169977,
      26339969,
      52679969,
      105359939,
      210719881,
      421439783
   ];

   protected int currentCount = 1;
   protected int currentSize = sizes[0];
   protected long[] ids;
   protected Maybe<T>[] objects;

   public ObjectHash()
   {
      ids = new long[currentSize * 4];
      objects = new Maybe<T>[currentSize * 4];
      for (var i = 0; i < objects.Length; i++)
      {
         objects[i] = nil;
      }
   }

   protected (int index, bool found) find(T obj)
   {
      var hashCode = RuntimeHelpers.GetHashCode(obj);
      var num1 = 1 + (hashCode & int.MaxValue) % (currentSize - 2);
      while (true)
      {
         var num2 = (hashCode & int.MaxValue) % currentSize * 4;
         for (var element = num2; element < num2 + 4; element++)
         {
            if (!objects[element])
            {
               return (element, false);
            }

            if (objects[element] is (true, var objAtIndex) && objAtIndex.Equals(obj))
            {
               return (element, true);
            }
         }

         hashCode += num1;
      }
   }

   public (long id, bool firstTime) GetIdWithFirstTime(T obj)
   {
      var (index, found) = find(obj);
      if (found)
      {
         return (ids[index], false);
      }
      else
      {
         objects[index] = obj;
         ids[index] = currentCount++;
         var id = ids[index];
         if (currentCount > currentSize * 4 / 2)
         {
            rehash();
         }

         return (id, true);
      }
   }

   public long GetId(T obj)
   {
      var (id, _) = GetIdWithFirstTime(obj);
      return id;
   }

   protected void rehash()
   {
      var newSizes = sizes;
      var originalIndex = 0;
      var thisCurrentSize = currentSize;
      while (originalIndex < newSizes.Length && newSizes[originalIndex] <= thisCurrentSize)
      {
         ++originalIndex;
      }

      if (originalIndex == newSizes.Length)
      {
         throw fail("Too many elements");
      }

      currentSize = newSizes[originalIndex];
      var newIds = new long[currentSize * 4];
      var newObjects = new Maybe<T>[currentSize * 4];
      for (var i = 0; i < newObjects.Length; i++)
      {
         newObjects[i] = nil;
      }
      var thisIds = ids;
      var thisObjects = objects;
      ids = newIds;
      objects = newObjects;
      for (var i = 0; i < thisObjects.Length; ++i)
      {
         if (thisObjects[i] is (true, var obj))
         {
            var (element, _) = find(obj);
            objects[element] = obj;
            ids[element] = thisIds[i];
         }
      }
   }
}