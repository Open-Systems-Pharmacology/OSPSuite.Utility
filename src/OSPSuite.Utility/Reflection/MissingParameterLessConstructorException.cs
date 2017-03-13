using System;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Reflection
{
   public class MissingParameterLessConstructorException : Exception
   {
      private const string _errorMessage = "'{0}' is missing a parameterless constructor.";

      public MissingParameterLessConstructorException(Type type) : base(_errorMessage.FormatWith(type.AssemblyQualifiedName))
      {
      }

      public MissingParameterLessConstructorException(Type type, Exception e)
         : base(_errorMessage.FormatWith(type), e)
      {
      }
   }
}