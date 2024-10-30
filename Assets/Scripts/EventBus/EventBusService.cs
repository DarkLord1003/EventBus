using System.Collections.Generic;
using System;
using UnityEngine.Pool;

namespace Scripts.EventBus
{
    public class EventBusService
    {
        private readonly Dictionary<EventBusBindID, EventBusBindInfo> _bindings;
        private readonly ObjectPool<EventListenersIterator> _iteratorsPool;

        public EventBusService()
        {
            _bindings = new Dictionary<EventBusBindID, EventBusBindInfo>();
            _iteratorsPool = new ObjectPool<EventListenersIterator>(OnCreateFunc);
        }

        public EventBus<TBaseEvent> Register<TBaseEvent>()
        {
            EventBusBindID bindID = new EventBusBindID(typeof(TBaseEvent));
            if (_bindings.ContainsKey(bindID))
                throw new InvalidOperationException($"EventBus for {typeof(TBaseEvent).Name} is already registered!");

            EventBus<TBaseEvent> eventBus = new EventBus<TBaseEvent>(_iteratorsPool, this);
            _bindings[bindID] = new EventBusBindInfo(eventBus);

            return eventBus;
        }

        public EventBus<TBaseEvent> GetEventBus<TBaseEvent>()
        {
            EventBusBindID bindID = new EventBusBindID(typeof(TBaseEvent));
            if (_bindings.TryGetValue(bindID, out EventBusBindInfo eventBusBindInfo))
            {
                EventBus<TBaseEvent> eventBus = eventBusBindInfo.EventBus as EventBus<TBaseEvent>;
                if (eventBus != null)
                    return eventBus;

                throw new InvalidOperationException("The registered event bus does not match the requested type!");
            }

            throw new InvalidOperationException("This event bus was not registered!");
        }

        public void Unregister<TBaseEvent>()
        {
            EventBusBindID bindID = new EventBusBindID(typeof(TBaseEvent));
            if (_bindings.ContainsKey(bindID))
            {
                EventBus<TBaseEvent> eventBus = _bindings[bindID].EventBus as EventBus<TBaseEvent>;
                if (eventBus != null)
                {
                    if (eventBus.IsEventBeingRaiseed)
                    {
                        eventBus.MarkForDestroy = true;
                        return;
                    }

                    DestroyEventBus(eventBus);
                }

                throw new InvalidOperationException("The registered event bus does not match the requested type!");
            }

            throw new InvalidOperationException("This event bus was not registered!");
        }

        public void DestroyEventBus<TBaseEvent>(EventBus<TBaseEvent> eventBus)
        {
            if (eventBus == null)
                throw new ArgumentNullException(nameof(eventBus), "A reference to the event bus is required!");

            eventBus.Dispose();
        }

        private EventListenersIterator OnCreateFunc()
        {
            return new EventListenersIterator();
        }
    }
}
