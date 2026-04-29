using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central Event Queue. Producers call Publish to enqueue events.
/// Consumers register handlers with Subscribe. Once per frame the
/// game pump calls ProcessQueue to deliver events to handlers.
///
/// This decouples senders from receivers in BOTH space (no direct
/// references) AND time (events are deferred until the next pump).
/// </summary>
public static class EventBus
{
    // The pending event queue. FIFO ordering preserves causality.
    private static readonly Queue<GameEvent> _queue = new Queue<GameEvent>();

    // Map from event type to its registered handlers.
    // We store as Delegate so we can invoke any Action<T> via DynamicInvoke.
    private static readonly Dictionary<Type, List<Delegate>> _subscribers =
        new Dictionary<Type, List<Delegate>>();

    /// <summary>
    /// Register a handler for events of type T.
    /// Always pair with Unsubscribe in OnDisable to avoid leaks.
    /// </summary>
    public static void Subscribe<T>(Action<T> handler) where T : GameEvent
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Delegate>();
        _subscribers[type].Add(handler);
    }

    /// <summary>Remove a previously-registered handler.</summary>
    public static void Unsubscribe<T>(Action<T> handler) where T : GameEvent
    {
        var type = typeof(T);
        if (_subscribers.TryGetValue(type, out var list))
            list.Remove(handler);
    }

    /// <summary>Enqueue an event for delivery on the next pump.</summary>
    public static void Publish(GameEvent evt)
    {
        if (evt == null) return;
        _queue.Enqueue(evt);
    }

    /// <summary>
    /// Drain the queue and dispatch each event to its handlers.
    /// Called once per frame from GameManager.Update.
    /// </summary>
    public static void ProcessQueue()
    {
        // Process until empty. Handlers may publish more events; those
        // are appended to the queue and processed in the same pump.
        while (_queue.Count > 0)
        {
            var evt = _queue.Dequeue();
            var type = evt.GetType();

            if (_subscribers.TryGetValue(type, out var handlers))
            {
                // Snapshot in case a handler unsubscribes during iteration.
                var snapshot = handlers.ToArray();
                foreach (var handler in snapshot)
                {
                    try
                    {
                        handler.DynamicInvoke(evt);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(
                            $"EventBus handler error for {type.Name}: {ex}");
                    }
                }
            }
        }
    }

    /// Wipe queue and subscribers. Call on scene reload.
    public static void ClearAll()
    {
        _queue.Clear();
        _subscribers.Clear();
    }
}