﻿using System;
using Core.Applications.Messaging;
using System.Runtime.InteropServices;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications;

public class Idle(int idleThreshold = 60)
{
   [StructLayout(LayoutKind.Sequential)]
   protected struct LastInputInfo
   {
      public int cbSize;
      public uint dwTime;
   }

   [DllImport("user32.dll")]
   protected static extern bool GetLastInputInfo(ref LastInputInfo lastInputInfo);

   public readonly MessageEvent<int> UserIdle = new();
   public readonly MessageEvent InputResumed = new();
   public readonly MessageEvent<int> IsIdle = new();
   protected bool invoked;

   protected static int getIdleTimeInSeconds()
   {
      var lastInputInfo = new LastInputInfo();
      lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
      if (GetLastInputInfo(ref lastInputInfo))
      {
         var idleTicks = (uint)Environment.TickCount - lastInputInfo.dwTime;
         return (int)(idleTicks / 1000);
      }
      else
      {
         return 0;
      }
   }

   public void CheckIdleTime()
   {
      idleThreshold.Must().Not.BeZero().OrThrow("idleThreshold cannot be 0");

      var idleTimeInSeconds = getIdleTimeInSeconds();
      switch (idleTimeInSeconds)
      {
         case 0 when invoked:
            InputResumed.Invoke();
            invoked = false;
            break;
         case > 0 when idleTimeInSeconds % idleThreshold == 0:
         {
            var maximumSeconds = MaximumSeconds | 0;
            if (maximumSeconds == 0 || idleTimeInSeconds < maximumSeconds)
            {
               UserIdle.Invoke(idleTimeInSeconds);
            }

            invoked = true;
            break;
         }
         default:
            IsIdle.Invoke(idleTimeInSeconds);
            break;
      }
   }

   public Maybe<int> MaximumSeconds { get; set; } = nil;
}