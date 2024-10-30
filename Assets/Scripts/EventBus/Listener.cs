using System;

namespace Scripts.EventBus
{
    public readonly struct Listener : IEquatable<Listener>, IComparable<Listener>
    {
        public readonly object Handler;
        public readonly int Priority;

        public Listener(object handler, int priority)
        {
            Handler = handler;
            Priority = priority;
        }

        public int CompareTo(Listener other)
        {
            return other.Priority.CompareTo(Priority);
        }

        public bool Equals(Listener other)
        {
            return Handler.Equals(other.Handler);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) 
                return false;

            if (obj is Listener other)
                return Equals(other);

            return false;
        }

        public override int GetHashCode()
        {
            return Handler.GetHashCode();
        }
    }
}
