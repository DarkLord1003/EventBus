using System;

namespace Scripts.EventBus
{
    public readonly struct EventBusBindID : IEquatable<EventBusBindID>
    {
        public readonly Type EventBusType;

        public EventBusBindID(Type eventType)
        {
            EventBusType = eventType;
        }

        public bool Equals(EventBusBindID other)
        {
            return EventBusType.Equals(other.EventBusType);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is EventBusBindID other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return EventBusType.GetHashCode();
        }
    }
}
