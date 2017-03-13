using System;

namespace OSPSuite.Utility
{
   public static class SystemTime
   {
      /// <summary>
      ///    Defines a function returning the current time
      /// </summary>
      public static Func<DateTime> Now = () => DateTime.Now;

      /// <summary>
      ///    Defines a function returning the current time in UTC
      /// </summary>
      public static Func<DateTime> UtcNow = () => DateTime.UtcNow;
   }
}