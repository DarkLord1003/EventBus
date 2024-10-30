
namespace Scripts.EventBus
{
    public interface IEventBusService
    {
        void DestroyEventBus<TBaseEvent>(EventBus<TBaseEvent> eventBus);
        EventBus<TBaseEvent> GetEventBus<TBaseEvent>();
        EventBus<TBaseEvent> Register<TBaseEvent>();
        void Unregister<TBaseEvent>();
    }
}