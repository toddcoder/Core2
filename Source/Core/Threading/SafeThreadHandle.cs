﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

// ReSharper disable UnusedMember.Global

namespace Core.Threading;

public class SafeThreadHandle : SafeHandleMinusOneIsInvalid
{
   [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
   protected static extern bool CloseHandle(IntPtr handle);

   public SafeThreadHandle() : base(true)
   {
   }

   public SafeThreadHandle(bool ownsHandle) : base(ownsHandle)
   {
   }

   protected override bool ReleaseHandle() => CloseHandle(handle);
}