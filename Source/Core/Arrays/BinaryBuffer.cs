using System;
using Core.Monads;
using static System.Math;
using static Core.Monads.MonadFunctions;

namespace Core.Arrays;

public class BinaryBuffer
{
   protected byte[] bytes;

   public BinaryBuffer(int length)
   {
      bytes = new byte[length];
      Next = nil;
   }

   public Maybe<BinaryBuffer> Next { get; set; }

   public void Add(byte[] bytesToAdd) => Array.Copy(bytesToAdd, bytes, Min(bytesToAdd.Length, bytes.Length));

   public void CopyTo(byte[] bytes, int index) => this.bytes.CopyTo(bytes, index);
}