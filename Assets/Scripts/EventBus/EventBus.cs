using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Scripts.EventBus
{
    public delegate void EventHandler<TEvent>(object sender, ref TEvent e);

    public class EventBus<TBaseEvent> : IEventBus<TBaseEvent>, IDisposable
    {
        private readonly EventListeners _listeners;
        private readonly List<uint> _raiseRecursionsConsumed;
        private readonly ObjectPool<QueuedEvent<TBaseEvent>> _queuedPool;
        private readonly Queue<QueuedEvent<TBaseEvent>> _queue;
        private readonly EventBusService _eventBusService;

        private uint _currentRaiseRecursionDepth;

        public bool IsEventBeingRaiseed => _currentRaiseRecursionDepth > 0;
        public bool CurrentEventIsConsumed => _currentRaiseRecursionDepth > 0 && _raiseRecursionsConsumed.Contains(_currentRaiseRecursionDepth);
        public bool MarkForDestroy { get; set; }

        public EventBus(ObjectPool<EventListenersIterator> listenersPool, EventBusService eventBusService)
        {
            _listeners = new EventListeners(listenersPool);
            _raiseRecursionsConsumed = new List<uint>();
            _queuedPool = new ObjectPool<QueuedEvent<TBaseEvent>>(() => new QueuedEvent<TBaseEvent>());
            _queue = new Queue<QueuedEvent<TBaseEvent>>(30);
            _eventBusService = eventBusService;
        }

        public void Subscribe<TEvent>(EventHandler<TEvent> handler, int priority) where TEvent : TBaseEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), "A reference to the handler is required!");

            _listeners.AddListener(handler, priority);
        }

        public void Unsubscribe<TEvent>(EventHandler<TEvent> handler) where TEvent : TBaseEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), "A reference to the handler is required!");

            _listeners.RemoveListener(handler);
        }

        public void RaiseImmediately<TEvent>(object sender, ref TEvent eventData) where TEvent : TBaseEvent
        {
            _currentRaiseRecursionDepth++;
            try
            {
                foreach (object handler in _listeners)
                {
                    try
                    {
                        if (handler is EventHandler<TEvent> listener)
                            listener?.Invoke(sender, ref eventData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }

                    if (CurrentEventIsConsumed)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                OnAfterRaiseEvent();
            }
        }

        public void Raise<TEvent>(object sender, ref TEvent eventData) where TEvent: TBaseEvent
        {
            if (!IsEventBeingRaiseed)
            {
                RaiseImmediately<TEvent>(sender, ref eventData);
                return;
            }

            EnqueueEvent(sender, eventData);
        }

        public void ConsumeCurrentEvent()
        {
            if (_currentRaiseRecursionDepth == 0)
                return;

            if (!_raiseRecursionsConsumed.Contains(_currentRaiseRecursionDepth))
            {
                _raiseRecursionsConsumed.Add(_currentRaiseRecursionDepth);
            }
        }

        private void OnAfterRaiseEvent()
        {
            _raiseRecursionsConsumed.Remove(_currentRaiseRecursionDepth--);

            if (_currentRaiseRecursionDepth == 0)
            {
                _raiseRecursionsConsumed.Clear();

                while (_queue.Count > 0)
                {
                    QueuedEvent<TBaseEvent> queuedEvent = _queue.Dequeue();
                    queuedEvent.Raise(this);
                }

                if (MarkForDestroy)
                    _eventBusService.DestroyEventBus(this);
            }
        }

        private void EnqueueEvent<TEvent>(object sender, TEvent eventData) where TEvent: TBaseEvent
        {
            QueuedEvent<TBaseEvent> queuedEvent = _queuedPool.Get();
            queuedEvent.EventData = eventData;
            queuedEvent.Sender = sender;
            queuedEvent.Pool = _queuedPool;

            _queue.Enqueue(queuedEvent);
        }

        public void Dispose()
        {
            _queuedPool.Dispose();
        }
    }
}
