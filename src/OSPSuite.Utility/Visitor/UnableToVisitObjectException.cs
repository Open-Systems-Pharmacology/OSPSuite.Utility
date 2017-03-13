using System;
using System.Text;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Visitor
{
   public class UnableToVisitObjectException : Exception
   {
      public UnableToVisitObjectException(IVisitor visitor, Type typeToVisit) : this(visitor.GetType(), typeToVisit)
      {
      }

      public UnableToVisitObjectException(Type visitorType, Type typeToVisit)
      {
         var sb = new StringBuilder();
         sb.AppendLine($"Unable to find an implementation of 'visit' in visitor '{visitorType}' for '{typeToVisit}'.");
         sb.AppendLine("Possible implementations are:");
         var allPossibleImplementations = visitorType.GetDeclaredTypesForGeneric(typeof(IVisitor<>));

         foreach (var possibleImplementation in allPossibleImplementations)
         {
            sb.AppendLine($"\tIVisitor<{possibleImplementation.DeclaredType.Name}>");
         }

         Message = sb.ToString();
      }

      public override string Message { get; } = string.Empty;
   }
}