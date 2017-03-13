using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Compression;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_StringCompression : ContextSpecification<IStringCompression>
   {
      protected string _stringToCompress;

      protected override void Context()
      {
         _stringToCompress = "toto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tontontoto tata titi tutu tonton";
         var compression = A.Fake<ICompression>();
         A.CallTo(() => compression.Compress(A<byte[]>._)).ReturnsLazily(x => x.GetArgument<byte[]>(0));
         A.CallTo(() => compression.Decompress(A<byte[]>._)).ReturnsLazily(x => x.GetArgument<byte[]>(0));
         sut = new StringCompression(compression);
      }
   }

   public class When_decompressing_a_compressed_string_with_the_sharp_lib_compression : concern_for_StringCompression
   {
      private string _compressedString;

      protected override void Context()
      {
         base.Context();
         _compressedString = sut.Compress(_stringToCompress);
      }

      [Observation]
      public void should_return_the_orginal_string()
      {
         sut.Decompress(_compressedString).ShouldBeEqualTo(_stringToCompress);
      }
   }
}