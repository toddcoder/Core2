using System;
using System.IO;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Computers;

public static class FullPathFunctions
{
   internal static Result<string> ValidatePath(IFullPath fullPathObject, bool allowRelativePaths = false)
   {
      try
      {
         var path = fullPathObject.FullPath;
         var fullPath = Path.GetFullPath(path);
         if (allowRelativePaths)
         {
            return Path.IsPathRooted(path).Must().BeTrue().OrFailure("Path not rooted").Map(_ => fullPath);
         }
         else
         {
            var root = Path.GetPathRoot(path);
            if (root is not null)
            {
               return root.Trim('\\', '/').Must().Not.BeNullOrEmpty().OrFailure("Root nothing but slashes and backslashes").Map(_ => fullPath);
            }
            else
            {
               return fail("Root is null");
            }
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}