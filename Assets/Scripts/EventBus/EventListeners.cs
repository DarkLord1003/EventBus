using Scripts.Utility.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Scripts.EventBus
{
    public class EventListeners : IEnumerable<object>
    {
        private readonly List<Listener> _listeners;
        private readonly List<EventListenersIterator> _activeIterator;
        private readonly ObjectPool<EventListenersIterator> _pool;

        public int CountListeners => _listeners.Count;
        public List<Listener> Listeners => _listeners;

        public EventListeners(ObjectPool<EventListenersIterator> pool)
        {
            _listeners = new List<Listener>();
            _activeIterator = new List<EventListenersIterator>();
            _pool = pool;
        }

        public void AddListener(object handler, int priority)
        {
            Listener listener = new(handler, priority);
            int index = _listeners.InsertIntoSortedList(listener);

            foreach (EventListenersIterator activeIterator in _activeIterator)
            {
                if (activeIterator.Index > index)
                {
                    activeIterator.Index++;
                }
            }
        }

        public void RemoveListener(object handler)
        {
            for (int i = CountListeners - 1; i >= 0; i--)
            {
                if (!Equals(_listeners[i].Handler, handler))
                    continue;

                _listeners.RemoveAt(i);
                foreach (EventListenersIterator activeIterator in _activeIterator)
                {
                    if (activeIterator.Index >= i && activeIterator.Index > 0)
                    {
                        activeIterator.Index--;
                    }
                }
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            EventListenersIterator iterator = _pool?.Get();
            iterator.Owner = this;
            iterator.Pool = _pool;
            iterator.Index = 0;

            _activeIterator.Add(iterator);
            return iterator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
