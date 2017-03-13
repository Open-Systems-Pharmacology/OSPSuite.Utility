using System;
using System.Collections.Generic;
using System.Text;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Visitor
{
   public class AmbiguousVisitMethodException : Exception
   {
      public AmbiguousVisitMethodException(IEnumerable<TypeForGenericType> allPossibleImplementations, Type typeOfVisitor, Type typeOfObjectToVisit)
      {
         var sb = new StringBuilder();
         sb.AppendLine($"Ambiguous implementation of 'visit' in visitor '{typeOfVisitor}' for '{typeOfObjectToVisit}'.");
         sb.AppendLine("Possible implementations are:");
         foreach (var possibleImplementation in allPossibleImplementations)
         {
            sb.AppendLine($"\tIVisitor<{possibleImplementation.DeclaredType.Name}>");
         }

         Message = sb.ToString();
      }

      public override string Message { get; } = string.Empty;
   }
}