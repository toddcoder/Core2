using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Core.Monads;
using Microsoft.Win32.SafeHandles;
using static Core.Monads.MonadFunctions;

namespace Core.Computers;

public class Impersonation
{
   public const int LOGON32_PROVIDER_DEFAULT = 0;
   public const int LOGON32_LOGON_INTERACTIVE = 2;

   [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
   public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider,
      out SafeAccessTokenHandle phToken);

   protected string domainName;
   protected string userName;
   protected string password;

   public Impersonation(string domainName, string userName, string password)
   {
      this.domainName = domainName;
      this.userName = userName;
      this.password = password;
   }

   public Result<Unit> Impersonate(Action action)
   {
      try
      {
         var returnValue = LogonUser(userName, domainName, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, out var safeAccessTokenHandle);
         if (returnValue)
         {
            WindowsIdentity.RunImpersonated(safeAccessTokenHandle, action);
            return unit;
         }
         else
         {
            var error = Marshal.GetLastWin32Error();
            return new Win32Exception(error);
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}