using System.Collections.Generic;
using System.Text;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Xml
{
   public class SchemaValidationResult
   {
      public IList<string> Messages { get; private set; }

      public SchemaValidationResult()
      {
         Messages = new List<string>();
      }

      public bool Success
      {
         get { return Messages.Count == 0; }
      }

      public string FullMessageLog
      {
         get
         {
            var builder = new StringBuilder();
            Messages.Each(m => builder.AppendLine(m));
            return builder.ToString();
         }
      }
   }
}