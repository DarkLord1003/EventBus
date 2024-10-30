using System.Collections.Generic;
using UnityEngine.Pool;

namespace Scripts.EventBus
{
    public class EventListenersIterator : IEnumerator<object>
    {
        public EventListeners Owner;
        public ObjectPool<EventListenersIterator> Pool;
        public int Index;

        public object Current { get; private set; }

        public bool MoveNext()
        {
            if (Index >= Owner.CountListeners)
                return false;

            Current = Owner.Listeners[Index++].Handler;
            return true;
        }

        public void Reset()
        {
            Index = 0;
        }
        public void Dispose()
        {
            Pool?.Release(this);
            Reset();
        }
    }
}
