using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Plays SFX in response to gameplay events. Holds a small pool of
/// AudioSources so concurrent sounds don't truncate each other, and a
/// lookup of SoundData assets keyed by id.
///
/// Music plays continuously from when the AudioManager wakes up until
/// the application quits. Subscriptions are re-established on every
/// scene load to recover from EventBus.ClearAll() wipes.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Sound library")]
    [SerializeField] private List<SoundData> sounds;

    [Header("Music")]
    [SerializeField] private AudioClip musicClip;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.4f;

    [Header("SFX pool")]
    [SerializeField] private int sfxSourceCount = 8;

    private readonly Dictionary<string, SoundData> lookup =
        new Dictionary<string, SoundData>();
    private readonly List<AudioSource> sfxSources = new List<AudioSource>();
    private AudioSource musicSource;

    private bool subscribed = false;

    private void Awake()
    {
        // Singleton - style guard: if a duplicate appears, kill it so the
        // original keeps running. Preserves music continuity.
        if (ServiceLocator.IsRegistered<AudioManager>())
        {
            Destroy(gameObject);
            return;
        }

        ServiceLocator.Register(this);
        DontDestroyOnLoad(gameObject);

        // Build the id  -> data lookup.
        foreach (var s in sounds)
            if (s != null && !string.IsNullOrEmpty(s.id))
                lookup[s.id] = s;

        // Pre create SFX pool sources as children.
        for (int i = 0; i < sfxSourceCount; i++)
        {
            var go = new GameObject($"SFX_{i}");
            go.transform.parent = transform;
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            sfxSources.Add(src);
        }

        // Music source - starts immediately and never stops while the
        // application is running.
        var musicGo = new GameObject("Music");
        musicGo.transform.parent = transform;
        musicSource = musicGo.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    private void OnEnable()
    {
        Subscribe();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Unsubscribe();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        // Defensive: if some external code called EventBus.ClearAll() and
        // wiped our subscriptions, restore them next frame. Cheap to check.
        if (!subscribed) Subscribe();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Belt and braces - also re-subscribe on scene load.
        Subscribe();
    }

    private void Subscribe()
    {
        // Always start by removing any existing handlers so we don't
        // accidentally double-subscribe (which would play SFX twice).
        Unsubscribe();

        EventBus.Subscribe<EnemyDiedEvent>(HandleEnemyDied);
        EventBus.Subscribe<PlayerDamagedEvent>(HandlePlayerDamaged);
        EventBus.Subscribe<PlayerDiedEvent>(HandlePlayerDied);
        EventBus.Subscribe<PowerUpCollectedEvent>(HandlePowerUpCollected);
        EventBus.Subscribe<PowerUpExpiredEvent>(HandlePowerUpExpired);
        EventBus.Subscribe<WaveStartedEvent>(HandleWaveStarted);

        subscribed = true;
    }

    private void Unsubscribe()
    {
        EventBus.Unsubscribe<EnemyDiedEvent>(HandleEnemyDied);
        EventBus.Unsubscribe<PlayerDamagedEvent>(HandlePlayerDamaged);
        EventBus.Unsubscribe<PlayerDiedEvent>(HandlePlayerDied);
        EventBus.Unsubscribe<PowerUpCollectedEvent>(HandlePowerUpCollected);
        EventBus.Unsubscribe<PowerUpExpiredEvent>(HandlePowerUpExpired);
        EventBus.Unsubscribe<WaveStartedEvent>(HandleWaveStarted);

        subscribed = false;
    }

    // Event handler - named methods so Subscribe/Unsubscribe match

    private void HandleEnemyDied(EnemyDiedEvent e) => Play("enemy_die");
    private void HandlePlayerDamaged(PlayerDamagedEvent e) => Play("player_hit");
    private void HandlePlayerDied(PlayerDiedEvent e) => Play("player_die");
    private void HandlePowerUpCollected(PowerUpCollectedEvent e) => Play("pickup");
    private void HandlePowerUpExpired(PowerUpExpiredEvent e) => Play("powerup_expire");
    private void HandleWaveStarted(WaveStartedEvent e) => Play("wave_start");

    // Public Play API

    public void Play(string id)
    {
        if (!lookup.TryGetValue(id, out var data)) return;
        if (data.clip == null) return;

        var source = GetIdleSource();
        if (source == null) return;

        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = Random.Range(data.minPitch, data.maxPitch);
        source.Play();
    }

    private AudioSource GetIdleSource()
    {
        foreach (var src in sfxSources)
            if (!src.isPlaying) return src;
        return sfxSources.Count > 0 ? sfxSources[0] : null;
    }
}