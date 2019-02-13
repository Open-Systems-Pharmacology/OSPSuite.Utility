using System.Drawing;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public class When_converting_a_color_to_hex : StaticContextSpecification
   {
      [Observation]
      public void should_return_the_expected_value()
      {
         var color = Color.FromKnownColor(KnownColor.Red);
         var hex = color.ToHexString();

         hex.ShouldBeEqualTo("#FF0000");
      }
   }
}