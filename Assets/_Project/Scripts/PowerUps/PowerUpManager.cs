using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks all currently active power-ups, runs their countdown timers,
/// and triggers expiration. Publishes activation / expiration events
/// through the EventBus. Registered via the ServiceLocator so power-up
/// pickups can call Activate without holding a reference here.
///
/// Defensive: looks up the player lazily so it remains correct after
/// scene reloads or player respawns (relevant for Phase 5's Game Over
/// → Restart flow).
/// </summary>
public class PowerUpManager : MonoBehaviour
{
    public class ActivePowerUp
    {
        public PowerUpData data;
        public float timeRemaining;
        public float totalDuration;
    }

    private readonly List<ActivePowerUp> active = new List<ActivePowerUp>();
    public IReadOnlyList<ActivePowerUp> Active => active;

    private GameObject player;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    /// <summary>
    /// Resolve the player on demand. Called whenever we need it instead
    /// of caching once at Start, so the manager survives a player respawn.
    /// </summary>
    private GameObject ResolvePlayer()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        return player;
    }

    public ActivePowerUp GetActive(PowerUpData data)
    {
        return active.Find(a => a.data == data);
    }

    public void Activate(PowerUpData data)
    {
        if (data == null) return;
        var p = ResolvePlayer();
        if (p == null) return;

        var existing = GetActive(data);
        if (existing != null)
        {
            existing.timeRemaining = data.duration;
            existing.totalDuration = data.duration;
            return;
        }

        data.Apply(p);
        active.Add(new ActivePowerUp
        {
            data = data,
            timeRemaining = data.duration,
            totalDuration = data.duration
        });

        EventBus.Publish(new PowerUpActivatedEvent
        {
            Data = data,
            Duration = data.duration
        });
    }

    private void Update()
    {
        if (active.Count == 0) return;
        var p = ResolvePlayer();

        for (int i = active.Count - 1; i >= 0; i--)
        {
            active[i].timeRemaining -= Time.deltaTime;
            if (active[i].timeRemaining <= 0f)
            {
                if (p != null) active[i].data.Remove(p);
                EventBus.Publish(new PowerUpExpiredEvent { Data = active[i].data });
                active.RemoveAt(i);
            }
        }
    }
}