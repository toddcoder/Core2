using System;
using System.Linq;
using Core.Collections;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Matching;

public class Replacer(Pattern pattern)
{
   public Optional<string> Replace(string source, Func<int, int, string, Maybe<string>> replacement)
   {
      try
      {
         Slicer slicer = source;
         var _result = pattern.MatchedBy(source);
         if (_result is (true, var result))
         {
            var any = false;
            var matchIndex = 0;
            foreach (var match in result)
            {
               var groupIndex = 0;
               foreach (var group in match)
               {
                  var _replaced = replacement(matchIndex, groupIndex++, group.Text);
                  if (_replaced is (true, var replaced))
                  {
                     slicer[group.Index, group.Length] = replaced;
                     any = true;
                  }
               }

               matchIndex++;
            }

            if (any)
            {
               return slicer.ToString();
            }
            else
            {
               return nil;
            }
         }
         else if (_result.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<string> ReplaceAllGroups(string source, Func<string[], Maybe<string[]>> replacement)
   {
      var _result = pattern.MatchedBy(source);
      if (_result is (true, var result))
      {
         string[] groups = [.. result.Groups(0).Skip(1)];
         var _replaced = replacement(groups);
         if (_replaced is (true, var replaced))
         {
            for (var i = 0; i < replaced.Length; i++)
            {
               result[0, i + 1] = replaced[i];
            }

            return result.ToString();
         }
         else
         {
            return nil;
         }
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }

   public Optional<string> ReplaceAllGroups(string source, Action<StringHash> replacement, params string[] keys)
   {
      var _result = pattern.MatchedBy(source);
      if (_result is (true, var result))
      {
         var groups = result.Groups(0);
         if (groups.Length != keys.Length + 1)
         {
            return fail("Length of keys must be one less than number of groups");
         }

         StringHash stringHash = [];
         StringHash<int> indexes = [];

         stringHash["$text"] = groups[0];

         var index = 1;
         foreach (var (key, value) in keys.Zip(groups.Skip(1)))
         {
            stringHash[key] = value;
            indexes[key] = index++;
         }

         replacement(stringHash);

         if (stringHash["$text"] != groups[0])
         {
            return stringHash["$text"];
         }
         else
         {
            foreach (var (key, groupIndex) in indexes)
            {
               result[0, groupIndex] = stringHash.Maybe[key] | "";
            }

            return result.ToString();
         }
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }
}