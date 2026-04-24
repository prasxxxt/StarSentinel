using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens to EnemyDiedEvent. With a configurable probability, spawns
/// a random power-up at the death position. Pure event-driven design —
/// no references to the wave system or to the enemy.
///
/// Defensive: filters null entries from the power-up list at startup
/// so empty Inspector slots don't crash the random pick (same pattern
/// as the WaveManager fix in Patch 3.1).
/// </summary>
public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private PowerUp powerUpPrefab;
    [SerializeField] private List<PowerUpData> availablePowerUps;
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 0.3f;

    private void Awake()
    {
        // Strip null entries left by empty Inspector slots.
        if (availablePowerUps != null)
        {
            int removed = availablePowerUps.RemoveAll(p => p == null);
            if (removed > 0)
            {
                Debug.LogWarning(
                    $"[PowerUpSpawner] Removed {removed} empty (null) " +
                    "power-up slot(s). Fill them in the Inspector to enable.");
            }
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
    }

    private void OnEnemyDied(EnemyDiedEvent evt)
    {
        if (powerUpPrefab == null) return;
        if (availablePowerUps == null || availablePowerUps.Count == 0) return;
        if (Random.value > dropChance) return;

        PowerUpData data = availablePowerUps[Random.Range(0, availablePowerUps.Count)];
        PowerUp spawned = Instantiate(powerUpPrefab, evt.Position, Quaternion.identity);
        spawned.Initialize(data);
    }
}