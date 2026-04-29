using System.IO;
using UnityEngine;

/// <summary>
/// Reads and writes the persistent save file as JSON to
/// Application.persistentDataPath/savegame.json. Registers itself with
/// the ServiceLocator so any system can read or update saved values
/// without holding a direct reference here.
///
/// Listens to ScoreChangedEvent and WaveCompletedEvent on the EventBus
/// to update the high score and best wave. Saves to disk on
/// PlayerDiedEvent (game-over moment) and on application quit.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public GameSaveData Data { get; private set; } = new GameSaveData();

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "savegame.json");

    private void Awake()
    {
        // Persist across scenes.
        if (ServiceLocator.IsRegistered<SaveManager>())
        {
            Destroy(gameObject);
            return;
        }

        ServiceLocator.Register(this);
        DontDestroyOnLoad(gameObject);
        Load();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
        EventBus.Subscribe<WaveCompletedEvent>(OnWaveCompleted);
        EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        EventBus.Unsubscribe<WaveCompletedEvent>(OnWaveCompleted);
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public void SetMasterVolume(float v)
    {
        Data.masterVolume = Mathf.Clamp01(v);
        AudioListener.volume = Data.masterVolume;
        Save();
    }

    public void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(Data, true);
            File.WriteAllText(SavePath, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveManager] Save failed: {ex.Message}");
        }
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                Data = new GameSaveData();
                return;
            }
            string json = File.ReadAllText(SavePath);
            Data = JsonUtility.FromJson<GameSaveData>(json) ?? new GameSaveData();
            AudioListener.volume = Data.masterVolume;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveManager] Load failed: {ex.Message}");
            Data = new GameSaveData();
        }
    }

    private void OnScoreChanged(ScoreChangedEvent evt)
    {
        if (evt.NewScore > Data.highScore)
        {
            Data.highScore = evt.NewScore;
            EventBus.Publish(new HighScoreUpdatedEvent
            {
                NewHighScore = Data.highScore
            });
        }
    }

    private void OnWaveCompleted(WaveCompletedEvent evt)
    {
        if (evt.WaveNumber > Data.bestWave)
        {
            Data.bestWave = evt.WaveNumber;
            EventBus.Publish(new BestWaveUpdatedEvent
            {
                NewBestWave = Data.bestWave
            });
        }
    }

    private void OnPlayerDied(PlayerDiedEvent evt)
    {
        Save();
    }
}