using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using OSPSuite.Utility.Container.Conventions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Container
{
   public interface IAssemblyScanner
   {
      void Build();

      /// <summary>
      ///    Adds an Assembly by name to the scanning operation
      /// </summary>
      void Assembly(string assemblyName);

      /// <summary>
      ///    Adds the Assembly that contains type T to the scanning operation
      /// </summary>
      void AssemblyContainingType<T>();

      /// <summary>
      ///    Adds the currently executing Assembly to the scanning operation
      /// </summary>
      void TheCallingAssembly();

      /// <summary>
      ///    Excludes types that match the Predicate from being scanned
      /// </summary>
      void Exclude(Func<Type, bool> exclude);

      /// <summary>
      ///    Excludes types from being scanned
      /// </summary>
      void ExcludeType<TExclude>();

      /// <summary>
      ///    Excludes types from being scanned
      /// </summary>
      void ExcludeType(Type typeToExclude);

      /// <summary>
      ///    Excludes all types in this nameSpace or its children from the scanning operation
      /// </summary>
      void ExcludeNamespace(string nameSpace);

      /// <summary>
      ///    Excludes all types in this nameSpace or its children from the scanning operation
      /// </summary>
      void ExcludeNamespaceContainingType<T>();

      /// <summary>
      ///    Only includes types matching the Predicate in the scanning operation. You can
      ///    use multiple Include() calls in a single scanning operation
      /// </summary>
      void Include(Func<Type, bool> predicate);

      /// <summary>
      ///    Includes this specific type from the scanning operation
      /// </summary>
      void IncludeType<TInclude>();

      /// <summary>
      ///    Includes this specific type from the scanning operation
      /// </summary>
      void IncludeType(Type typeToInclude);

      /// <summary>
      ///    Only includes types from this nameSpace or its children in the scanning operation.  You can
      ///    use multiple Include() calls in a single scanning operation
      /// </summary>
      void IncludeNamespace(string nameSpace);

      /// <summary>
      ///    Only includes types from this nameSpace or its children in the scanning operation.  You can
      ///    use multiple Include() calls in a single scanning operation
      /// </summary>
      void IncludeNamespaceContainingType<T>();

      /// <summary>
      ///    Register all the types with the given lifecycle in the scanning operation.
      /// </summary>
      void RegisterAs(LifeStyle lifeStyle);

      /// <summary>
      ///    Register all types in the scan with the default convention, i.e., a concrete
      ///    class named "Something" that implements "ISomething" will be automatically
      ///    registered
      /// </summary>
      void WithDefaultConvention();

      /// <summary>
      ///    Adds a registration convention to be applied to all the types in this
      ///    logical "scan" operation
      /// </summary>
      void WithConvention<T>() where T : IRegistrationConvention, new();

      /// <summary>
      ///    Adds a registration convention to be applied to all the types in this
      ///    logical "scan" operation
      /// </summary>
      void WithConvention(IRegistrationConvention registrationConvention);
   }

   public class AssemblyScanner : IAssemblyScanner
   {
      private readonly IContainer _container;
      private readonly IList<Assembly> _assemblies = new List<Assembly>();
      private readonly IList<Func<Type, bool>> _excludes = new List<Func<Type, bool>>();
      private readonly IList<Func<Type, bool>> _includes = new List<Func<Type, bool>>();
      private LifeStyle _lifeStyle = LifeStyle.Transient;
      private IRegistrationConvention _convention = new DefaultRegistrationConvention();

      public AssemblyScanner(IContainer container)
      {
         _container = container;
      }

      public void Build()
      {
         _assemblies.Each(scanTypesInAssembly);
      }

      private void scanTypesInAssembly(Assembly assembly)
      {
         try
         {
            foreach (var type in assembly.GetTypes())
            {
               if (cannotRegister(type)) continue;
               if (!isInTheIncludes(type)) continue;
               if (isInTheExcludes(type)) continue;
               _convention.Process(type, _container, _lifeStyle);
            }
         }
         catch (Exception ex)
         {
            throw new AssemblyScannerException(ex);
         }
      }

      private bool cannotRegister(Type type)
      {
         return type.IsInterface || 
            type.IsAbstract || 
            type.IsEnum ||
            type.GetInterfaces().Length == 0;
      }

      public void AssemblyContainingType<T>()
      {
         Assembly(typeof(T).Assembly);
      }

      public void TheCallingAssembly()
      {
         Assembly(findTheCallingAssembly());
      }

      private void Assembly(Assembly assembly)
      {
         if (!_assemblies.Contains(assembly))
         {
            _assemblies.Add(assembly);
         }
      }

      public void Assembly(string assemblyName)
      {
         Assembly(AppDomain.CurrentDomain.Load(assemblyName));
      }

      private bool isInTheExcludes(Type type)
      {
         if (_excludes.Count == 0) return false;   

         return _excludes.Any(exclude => exclude(type));
      }

      private bool isInTheIncludes(Type type)
      {
         if (_includes.Count == 0) return true;

         return _includes.Any(include => include(type));
      }

      public void Exclude(Func<Type, bool> exclude)
      {
         _excludes.Add(exclude);
      }

      public void ExcludeType<TExclude>()
      {
         Exclude(type => type == typeof(TExclude));
      }

      public void ExcludeType(Type typeToExclude)
      {
         Exclude(type => type == typeToExclude);
      }

      public void ExcludeNamespace(string nameSpace)
      {
         Exclude(type => isInNamespace(type, nameSpace));
      }

      public void ExcludeNamespaceContainingType<T>()
      {
         ExcludeNamespace(typeof(T).Namespace);
      }

      public void Include(Func<Type, bool> predicate)
      {
         _includes.Add(predicate);
      }

      public void IncludeType<TInclude>()
      {
         Include(type => type == typeof(TInclude));
      }

      public void IncludeType(Type typeToInclude)
      {
         Include(type => type == typeToInclude);
      }

      public void IncludeNamespace(string nameSpace)
      {
         Include(type => isInNamespace(type, nameSpace));
      }

      public void IncludeNamespaceContainingType<T>()
      {
         IncludeNamespace(typeof(T).Namespace);
      }

      public void RegisterAs(LifeStyle lifeStyle)
      {
         _lifeStyle = lifeStyle;
      }

      public void WithDefaultConvention()
      {
         WithConvention<DefaultRegistrationConvention>();
      }

      public void WithConvention<T>() where T : IRegistrationConvention, new()
      {
         WithConvention(new T());
      }

      public void WithConvention(IRegistrationConvention convention)
      {
         _convention = convention;
      }

      private bool isInNamespace(Type type, string nameSpace)
      {
         return type.Namespace != null && type.Namespace.StartsWith(nameSpace);
      }

      private static Assembly findTheCallingAssembly()
      {
         var trace = new StackTrace(false);

         Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
         Assembly callingAssembly = null;
         for (int i = 0; i < trace.FrameCount; i++)
         {
            StackFrame frame = trace.GetFrame(i);
            Assembly assembly = frame.GetMethod().DeclaringType.Assembly;
            if (assembly != thisAssembly)
            {
               callingAssembly = assembly;
               break;
            }
         }
         return callingAssembly;
      }
   }
}