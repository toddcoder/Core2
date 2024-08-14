using System.Collections.Generic;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public class ObjectHash<T> where T : notnull
{
   protected Hash<long, T> idToObject = [];
   protected Hash<T, long> objectToId = [];

   public (long id, bool firstTime) GetIdWithFirstTime(T obj)
   {
      var _id = objectToId.Maybe[obj];
      if (_id is (true, var id))
      {
         return (id, false);
      }
      else
      {
         id = objectToId.Count + 1;
         idToObject[id] = obj;
         objectToId[obj] = id;

         return (id, true);
      }
   }

   public long GetId(T obj)
   {
      var (id, _) = GetIdWithFirstTime(obj);
      return id;
   }

   public T this[long id] => idToObject[id];

   public bool ContainsKey(long key) => idToObject.ContainsKey(key);

   public bool ContainsKey(T key) => objectToId.ContainsKey(key);

   public Hash<long, T> IdToObject => idToObject;

   public Hash<T, long> ObjectToId => objectToId;

   public long this[T obj] => objectToId[obj];

   public int Count => idToObject.Count;

   public HashMaybe<long, T> IdToObjectMaybe => idToObject.Maybe;

   public HashMaybe<T, long> ObjectToIdMaybe => objectToId.Maybe;

   public IEnumerable<T> Objects()
   {
      for (var i = 1; i <= idToObject.Count; i++)
      {
         var _obj = idToObject.Maybe[i];
         if (_obj is (true, var obj))
         {
            yield return obj;
         }
      }
   }

   public Maybe<T> Remove(long id)
   {
      var _obj = idToObject.Maybe[id];
      if (_obj is (true, var obj))
      {
         idToObject.Remove(id);
         objectToId.Remove(obj);

         return obj;
      }
      else
      {
         return nil;
      }
   }

   public void Clear()
   {
      idToObject.Clear();
      objectToId.Clear();
   }
}