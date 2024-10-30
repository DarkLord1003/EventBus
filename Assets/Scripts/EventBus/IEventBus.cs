namespace Scripts.EventBus
{
    public interface IEventBus<TBaseEvent>
    {
        bool CurrentEventIsConsumed { get; }
        bool IsEventBeingRaiseed { get; }

        void ConsumeCurrentEvent();
        void RaiseImmediately<TEvent>(object sender, ref TEvent eventData) where TEvent : TBaseEvent;
        void Subscribe<TEvent>(EventHandler<TEvent> handler, int priority) where TEvent : TBaseEvent;
        void Unsubscribe<TEvent>(EventHandler<TEvent> handler) where TEvent : TBaseEvent;
    }
}