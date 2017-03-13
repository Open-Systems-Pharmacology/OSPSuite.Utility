using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Tests
{
   public class When_resolving_the_name_of_an_expression : StaticContextSpecification
   {
      [Observation]
      public void should_return_the_defined_name()
      {
         Expression<Func<IAnInterface, string>> exp = x => x.FirstName;
         Expression<Func<IAnInterface, IAnInterface>> exp2 = x => x;
         MySimpleClass obj = new MySimpleClass();
         Expression<Action<MySimpleVisitor>> exp3 = x => x.Visit(obj);
         Expression<Func<IList<IAnInterface>, IEnumerable<IAnInterface>>> exp4 = x => x.All();
         exp.Name().ShouldBeEqualTo("FirstName");
         exp2.Name().ShouldBeEqualTo("x");
         exp3.Name().ShouldBeEqualTo("Visit");
         exp4.Name().ShouldBeEqualTo("All");
      }
   }
}