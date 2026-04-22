using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Drives the game's enemy progression. Iterates through a list of
/// WaveData assets, spawns the enemies described in each, waits for
/// them to be cleared, then advances. Publishes wave events to the
/// EventBus so UI / audio / persistence can react without holding
/// references here.
/// </summary>
public class WaveManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private float spawnRadius = 12f;

    [Header("Loop on completion")]
    [Tooltip("If true, after the last wave we loop back to the first " +
             "wave with steadily increasing difficulty (count x1.5 each loop).")]
    [SerializeField] private bool loopWaves = true;

    // Tracks living enemies for the current wave. Using a List rather
    // than a counter so off-screen-despawned enemies are detected too.
    private readonly List<Enemy> liveEnemies = new List<Enemy>();
    private int currentWaveIndex = -1;
    private int loopMultiplier = 1;
    private GameManager gameManager;
    private bool wavesRunning = false;

    public int CurrentWaveDisplayNumber => currentWaveIndex + 1;
    public int LiveEnemyCount => liveEnemies.Count;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Start()
    {
        gameManager = ServiceLocator.Get<GameManager>();
        if (gameManager != null && gameManager.CurrentState == GameState.Playing)
            BeginWaves();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void Update()
    {
        // Keep the live list clean of destroyed enemies.
        liveEnemies.RemoveAll(e => e == null);
    }

    private void OnGameStateChanged(GameStateChangedEvent evt)
    {
        if (!wavesRunning && evt.NewState == GameState.Playing)
            BeginWaves();
    }

    private void BeginWaves()
    {
        if (wavesRunning) return;

        if (waves == null || waves.Count == 0)
        {
            Debug.LogWarning("[WaveManager] No waves configured. " +
                "Drag WaveData assets into the Waves list in the Inspector.");
            return;
        }

        // Filter out null entries — these come from empty Inspector slots.
        int removed = waves.RemoveAll(w => w == null);
        if (removed > 0)
        {
            Debug.LogWarning(
                $"[WaveManager] Removed {removed} empty (null) wave slot(s). " +
                "Fill them in the Inspector to use those waves.");
        }

        if (waves.Count == 0)
        {
            Debug.LogError("[WaveManager] All wave slots are empty. " +
                "Drag WaveData assets into the Inspector.");
            return;
        }

        wavesRunning = true;
        StartCoroutine(RunAllWaves());
    }

    private IEnumerator RunAllWaves()
    {
        while (true)
        {
            currentWaveIndex++;

            if (currentWaveIndex >= waves.Count)
            {
                if (!loopWaves)
                {
                    EventBus.Publish(new AllWavesCompletedEvent());
                    yield break;
                }
                currentWaveIndex = 0;
                loopMultiplier++;
            }

            WaveData wave = waves[currentWaveIndex];
            if (wave == null)
            {
                // Should never trigger after BeginWaves filtering, but
                // defensive in case a wave is destroyed mid-game.
                Debug.LogWarning(
                    $"[WaveManager] Wave at index {currentWaveIndex} is null. Skipping.");
                continue;
            }

            yield return new WaitForSeconds(wave.delayBeforeWave);

            EventBus.Publish(new WaveStartedEvent
            {
                WaveNumber = (loopMultiplier - 1) * waves.Count + currentWaveIndex + 1,
                WaveName = wave.waveName,
                TotalEnemies = Mathf.RoundToInt(
                    wave.CountTotalEnemies() * loopMultiplier)
            });

            yield return StartCoroutine(SpawnWave(wave));

            while (liveEnemies.Count > 0)
                yield return null;

            EventBus.Publish(new WaveCompletedEvent
            {
                WaveNumber = currentWaveIndex + 1
            });
        }
    }
}