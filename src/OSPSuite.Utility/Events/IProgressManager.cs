namespace OSPSuite.Utility.Events
{
   /// <summary>
   ///    Simple interface that should be implemented in client application. Usage is as follow
   ///    <code>
   /// using(var updater = progressManager.Create()
   /// {
   ///   //Do expensive stuff here and no need to call terminate on the progressUpdater
   /// }
   /// </code>
   /// </summary>
   public interface IProgressManager
   {
      IProgressUpdater Create();
   }
}