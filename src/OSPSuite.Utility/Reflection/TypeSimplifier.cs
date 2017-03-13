using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Reflection
{
   public interface ITypeSimplifier
   {
      IEnumerable<TypeForGenericType> Simplify(IEnumerable<TypeForGenericType> implementationsToSimplify);
      IEnumerable<Type> Simplify(IEnumerable<Type> typesToSimplify);
   }

   public class TypeSimplifier : ITypeSimplifier
   {
      public IEnumerable<TypeForGenericType> Simplify(IEnumerable<TypeForGenericType> implementationsToSimplify)
      {
         var distinctGenericType = implementationsToSimplify.Distinct().ToList();
         var simplifiedTypes = Simplify(distinctGenericType.Select(tg => tg.DeclaredType));

         return from tg in distinctGenericType
            where simplifiedTypes.Contains(tg.DeclaredType)
            select tg;
      }

      public IEnumerable<Type> Simplify(IEnumerable<Type> typesToSimplify)
      {
         var distinctTypes = typesToSimplify.Distinct().ToList();
         var simplifiedTypes = simplifyTypes(distinctTypes).ToList();
         if (simplifiedTypes.Count.Equals(distinctTypes.Count))
            return simplifiedTypes;

         return Simplify(simplifiedTypes);
      }

      private IEnumerable<Type> simplifyTypes(IList<Type> typesToSimplify)
      {
         if (!typesToSimplify.Any())
            yield break;

         if (typesToSimplify.Count == 1)
         {
            yield return typesToSimplify[0];
            yield break;
         }

         foreach (var typeChecked in typesToSimplify)
         {
            //type is not derived from any availalbe type. return that type
            if (typeHasNoDerivedTypeIn(typeChecked, typesToSimplify))
               yield return typeChecked;
         }
      }

      private bool typeHasNoDerivedTypeIn(Type checkedType, IEnumerable<Type> availableTypes)
      {
         //at least one type derived from checked type that is not checked type
         return !availableTypes.Where(type => type != checkedType)
            .Any(type => type.IsAnImplementationOf(checkedType));
      }
   }
}