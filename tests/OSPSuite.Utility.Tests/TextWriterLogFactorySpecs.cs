using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Logging;
using OSPSuite.Utility.Logging.TextWriterLogging;

namespace OSPSuite.Utility.Tests
{
   public class when_told_to_create_a_log_for_a_type : ContextSpecification<ILogFactory>
   {
      private ILogger result;

      protected override void Context()
      {
         sut = new TextWriterLogFactory();
      }

      protected override void Because()
      {
         result = sut.CreateFor(GetType());
      }

      [Observation]
      public void should_create_a_text_writer_log()
      {
         result.ShouldBeAnInstanceOf<TextWriterLogger>();
      }
   }
}