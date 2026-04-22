using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data definition for a single wave: which enemy types to spawn,
/// how many of each, and the timing between spawns. Designers create
/// new waves as asset files; no code changes required to add or
/// rebalance a wave.
/// </summary>
[CreateAssetMenu(fileName = "WaveData", menuName = "StarSentinel/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Identity")]
    public string waveName = "Wave";

    [Header("Spawning")]
    public List<SpawnGroup> spawnGroups;

    [Header("Timing")]
    [Tooltip("Pause before this wave begins (after the previous wave ends).")]
    public float delayBeforeWave = 2f;

    public int CountTotalEnemies()
    {
        int total = 0;
        if (spawnGroups != null)
            foreach (var g in spawnGroups) total += g.count;
        return total;
    }
}

/// <summary>
/// One contiguous group of enemies of the same type to spawn within a wave.
/// </summary>
[System.Serializable]
public class SpawnGroup
{
    public EnemyData enemyType;
    [Min(1)] public int count = 3;
    [Tooltip("Seconds between each spawn within this group.")]
    public float spawnInterval = 0.5f;
}