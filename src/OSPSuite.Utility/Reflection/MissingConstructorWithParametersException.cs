using System;
using System.Linq;
using System.Text;

namespace OSPSuite.Utility.Reflection
{
   public class MissingConstructorWithParametersException : Exception
   {
      public MissingConstructorWithParametersException(Type type, object[] constructorParameters)
      {
         var sb = new StringBuilder();
         sb.AppendLine($"'{type}' is missing a constructor with following parameter type{(constructorParameters.Length == 1 ? "" : "s")}:.");

         foreach (var paramType in constructorParameters.Select(param => param.GetType()))
         {
            sb.AppendLine($"\t{paramType}");
         }

         Message = sb.ToString();
      }

      public override string Message { get; } = string.Empty;
   }
}