using TMPro;
using UnityEngine;

/// <summary>
/// Pure event listener. Subscribes to ScoreChangedEvent and updates
/// the on-screen score text. Holds no references to GameManager,
/// the player, or anything else — it just listens.
/// </summary>
public class HUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void Start()
    {
        // Initial display before any event has fired.
        if (scoreText != null)
            scoreText.text = "SCORE: 000000";
    }

    private void OnScoreChanged(ScoreChangedEvent evt)
    {
        if (scoreText != null)
            scoreText.text = $"SCORE: {evt.NewScore:D6}";
    }

    private void OnGameStateChanged(GameStateChangedEvent evt)
    {
        // Hook for showing/hiding HUD elements based on game state.
        // We'll flesh this out in later phases.
    }
}