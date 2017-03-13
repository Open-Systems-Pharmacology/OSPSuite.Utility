using System.IO;
using System.Text;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Logging;
using OSPSuite.Utility.Logging.TextWriterLogging;

namespace OSPSuite.Utility.Tests
{
   public class When_told_to_log_an_informational_message_with_the_text_writer_logger : ContextSpecification<ILogger>
   {
      private StringBuilder backing_field;

      protected override void Context()
      {
         backing_field = new StringBuilder();
         sut = new TextWriterLogger(new StringWriter(backing_field));
      }

      protected override void Because()
      {
         sut.Informational("blah");
      }

      [Observation]
      public void should_output_the_message_to_the_console()
      {
         backing_field.ToString().ShouldBeEqualTo("blah\r\n");
      }
   }
}