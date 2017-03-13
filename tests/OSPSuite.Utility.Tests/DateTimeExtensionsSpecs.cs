using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_DateTimeExtensions : StaticContextSpecification
   {
      protected DateTime _date;

      protected override void Context()
      {
         _date = new DateTime(2010, 05, 24, 22, 10, 50);
      }
   }

   public class When_formatting_a_date_to_iso_format : concern_for_DateTimeExtensions
   {
      [Observation]
      public void should_return_the_expected_string()
      {
         _date.ToIsoFormat().ShouldBeEqualTo("2010-05-24 22:10");
         _date.ToIsoFormat(withSeconds: true).ShouldBeEqualTo("2010-05-24 22:10:50");
         _date.ToIsoFormat(withTime: false).ShouldBeEqualTo("2010-05-24");
         _date.ToIsoFormat(withTime: false, withSeconds: true).ShouldBeEqualTo("2010-05-24");
      }
   }
}