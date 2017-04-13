using System;

namespace OSPSuite.Utility.Reflection
{
   public class MissingParameterLessConstructorException : Exception
   {
      private const string _errorMessage = "'{0}' is missing a parameterless constructor.";

      public MissingParameterLessConstructorException(Type type) : base(string.Format(_errorMessage, type.AssemblyQualifiedName))
      {
      }

      public MissingParameterLessConstructorException(Type type, Exception e)
         : base(string.Format(_errorMessage, type), e)
      {
      }
   }
}