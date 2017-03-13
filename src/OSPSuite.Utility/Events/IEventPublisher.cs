namespace OSPSuite.Utility.Events
{
   public interface IEventPublisher
   {
      /// <summary>
      ///    Publishes given event to all available listener
      /// </summary>
      void PublishEvent<T>(T eventToPublish);

      /// <summary>
      ///    Adds listener to the event publisher if this listener does not already exist
      /// </summary>
      void AddListener(IListener listenerToAdd);

      /// <summary>
      ///    Removes listener from the system of listener was registered
      /// </summary>
      void RemoveListener(IListener listenerToRemove);

      /// <summary>
      ///    Returns true if the publisher contains the listener otherwise false
      /// </summary>
      bool Contains(IListener listener);
   }
}