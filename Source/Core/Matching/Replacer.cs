using System;
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
}