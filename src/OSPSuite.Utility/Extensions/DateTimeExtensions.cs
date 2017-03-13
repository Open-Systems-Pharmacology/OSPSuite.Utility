using System;
using System.Globalization;

namespace OSPSuite.Utility.Extensions
{
   public static class DateTimeExtensions
   {
      /// <summary>
      ///    Returns a string formatted using the ISO 8601 Format (yyyy-MM-dd HH:mm:ss)
      /// </summary>
      /// <param name="dateTime">Date to format</param>
      /// <param name="withTime">if <c>true</c> the time will be added to the date. Default is <c>true</c></param>
      /// <param name="withSeconds">
      ///    if <c>true</c> the second will be added to the date time. Default is <c>false</c>. If
      ///    <paramref name="withTime" /> is <c>false</c>, this parameter has no effect
      /// </param>
      /// <returns></returns>
      public static string ToIsoFormat(this DateTime dateTime, bool withTime = true, bool withSeconds = false)
      {
         var dateFormat = "yyyy-MM-dd";
         if (withTime)
            dateFormat = $"{dateFormat} HH':'mm{(withSeconds ? "':'ss" : "")}";

         return dateTime.ToString(dateFormat, CultureInfo.InvariantCulture);
      }
   }
}