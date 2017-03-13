using System;

namespace OSPSuite.Utility.Extensions
{
   public static class LatchableExtensions
   {
      public static void DoWithinLatch(this ILatchable latchable, Action action)
      {
         if (latchable.IsLatched) return;

         try
         {
            latchable.IsLatched = true;
            action();
         }
         finally
         {
            latchable.IsLatched = false;
         }
      }
   }
}