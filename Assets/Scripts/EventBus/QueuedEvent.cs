using UnityEngine.Pool;

namespace Scripts.EventBus
{
    public class QueuedEvent<TBaseEvent>
    {
        public TBaseEvent EventData;
        public object Sender;
        public ObjectPool<QueuedEvent<TBaseEvent>> Pool;

        public void Raise(EventBus<TBaseEvent> eventBus)
        {
            eventBus?.Raise(Sender, ref EventData);

            EventData = default(TBaseEvent);
            Sender = null;
            Pool?.Release(this);
        }
    }
}
