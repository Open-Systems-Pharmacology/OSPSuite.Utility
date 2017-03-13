using System;

namespace OSPSuite.Utility.Container
{
   public class InterfaceResolutionException : Exception
   {
      public const string InterfaceResolutionMessageFormat = "Failed to resolve an implementation of {0}";
      public const string InterfaceResolutionMessageFormatWithKey = "Failed to resolve an implementation of {0} with key {1}";

      public InterfaceResolutionException(Type interfaceThatCouldNotBeResolved, Exception e)
         : base(string.Format(InterfaceResolutionMessageFormat, interfaceThatCouldNotBeResolved.Name), e)
      {
      }

      public InterfaceResolutionException(Type interfaceThatCouldNotBeResolved, string key, Exception e)
         : base(string.Format(InterfaceResolutionMessageFormatWithKey, interfaceThatCouldNotBeResolved.Name, key), e)
      {
      }
   }
}