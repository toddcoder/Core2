using System;
using System.Linq;
using Core.Collections;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public class DataContainerEvaluator : IEvaluator, IHash<string, object>, IHash<Signature, object>
{
   protected DataContainer data;

   public DataContainerEvaluator(DataContainer data) => this.data = data;

   object? IEvaluator.this[string signature]
   {
      get => data[signature];
      set => data[signature] = value!;
   }

   public bool ContainsKey(string key) => data.ContainsKey(key);

   Hash<string, object> IHash<string, object>.GetHash()
   {
      return Signatures.Select(s => (key: s.Name, value: data[s.Name])).ToHash(i => i.key, i => i.value);
   }

   HashInterfaceMaybe<Signature, object> IHash<Signature, object>.Items => new(this);

   HashInterfaceMaybe<string, object> IHash<string, object>.Items => new(this);

   object? IEvaluator.this[Signature signature]
   {
      get => data[signature.Name];
      set => data[signature.Name] = value!;
   }

   public bool ContainsKey(Signature key) => data.ContainsKey(key.Name);

   Hash<Signature, object> IHash<Signature, object>.GetHash()
   {
      return Signatures.Select(s => (key: s, value: data[s.Name])).ToHash(i => i.key, i => i.value);
   }

   public Type Type(string signature)
   {
      var _value = data.Maybe[signature];
      if (_value is (true, var value))
      {
         return value.GetType();
      }
      else
      {
         throw fail("Value isn't set");
      }
   }

   public Type Type(Signature signature) => Type(signature.Name);

   public bool Contains(string signature) => data.ContainsKey(signature);

   public Signature[] Signatures => [.. data.KeyArray().Select(key => new Signature(key))];

   object IHash<string, object>.this[string key] => data[key];

   object IHash<Signature, object>.this[Signature key] => data[key.Name];
}