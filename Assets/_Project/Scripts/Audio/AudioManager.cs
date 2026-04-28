using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays SFX in response to gameplay events. Holds a small pool of
/// AudioSources (so concurrent sounds don't truncate each other) and
/// a lookup of SoundData assets keyed by id.
/// Subscribed handlers translate gameplay events into Play(id) calls.
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

    private void Awake()
    {
        if (ServiceLocator.IsRegistered<AudioManager>())
        {
            Destroy(gameObject);
            return;
        }

        ServiceLocator.Register(this);
        DontDestroyOnLoad(gameObject);

        // Build lookup table.
        foreach (var s in sounds)
            if (s != null && !string.IsNullOrEmpty(s.id))
                lookup[s.id] = s;

        // Pre-create SFX sources as children.
        for (int i = 0; i < sfxSourceCount; i++)
        {
            var go = new GameObject($"SFX_{i}");
            go.transform.parent = transform;
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            sfxSources.Add(src);
        }

        // Music source.
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
        // Translate gameplay events to sound effects.
        EventBus.Subscribe<EnemyDiedEvent>(e => Play("enemy_die"));
        EventBus.Subscribe<PlayerDamagedEvent>(e => Play("player_hit"));
        EventBus.Subscribe<PlayerDiedEvent>(e => Play("player_die"));
        EventBus.Subscribe<PowerUpCollectedEvent>(e => Play("pickup"));
        EventBus.Subscribe<PowerUpExpiredEvent>(e => Play("powerup_expire"));
        EventBus.Subscribe<WaveStartedEvent>(e => Play("wave_start"));
    }

    /// <summary>Play a sound by id. Picks an idle pool source.</summary>
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

        // All busy — overwrite the first one.
        return sfxSources.Count > 0 ? sfxSources[0] : null;
    }
}