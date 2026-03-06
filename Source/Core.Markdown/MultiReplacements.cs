using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markdown;

public class MultiReplacements : IHash<string, Replacements>
{
   protected StringHash<Replacements> replacements = [];
   protected Maybe<Replacements> _currentReplacements = nil;
   protected Maybe<string> _replacementKey = nil;
   protected StringHash hash = [];

   public string this[string key]
   {
      set => hash[key] = value;
   }

   public bool ContainsKey(string key) => replacements.ContainsKey(key);

   public Hash<string, Replacements> GetHash() => replacements;

   public HashInterfaceMaybe<string, Replacements> Items => replacements.Items;

   public Replacements CurrentReplacements => _currentReplacements.Required("Begin() has not been called");

   public bool Transacting => _currentReplacements;

   public Replacements Begin(string replacementKey, params string[] keys)
   {
      var currentReplacements = new Replacements(keys);
      _currentReplacements = currentReplacements;
      _replacementKey = replacementKey;
      hash = [];

      return currentReplacements;
   }

   public void Commit()
   {
      if (_currentReplacements is (true, var currentReplacements) && _replacementKey is (true, var replacementKey) && hash.Count == currentReplacements.KeyCount)
      {
         currentReplacements.Add(hash);
         replacements[replacementKey] = currentReplacements;
      }

      _currentReplacements = nil;
      _replacementKey = nil;
      hash = [];
   }

   public void RollBack()
   {
      _currentReplacements = nil;
      _replacementKey = nil;
      hash = [];
   }

   Replacements IHash<string, Replacements>.this[string key] => replacements[key];

   public IHashMaybe<string, Replacements> Maybe => new(replacements);
}