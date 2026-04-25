using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Appears when GameStateChangedEvent reports GameOver. Displays the
/// final score and the persisted high score. Restart and Main Menu
/// buttons reload scenes.
/// </summary>
public class GameOverPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        Debug.Log($"[GameOverPanel] Awake. panelRoot={(panelRoot != null ? panelRoot.name : "NULL")}");
        if (panelRoot != null) panelRoot.SetActive(false);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestart);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    private void OnEnable()
    {
        Debug.Log("[GameOverPanel] OnEnable - subscribing");
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        Debug.Log("[GameOverPanel] OnDisable - unsubscribing");
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent evt)
    {
        Debug.Log($"[GameOverPanel] OnGameStateChanged - new state: {evt.NewState}");
        if (evt.NewState != GameState.GameOver) return;
        Show();
    }

    private void Show()
    {
        Debug.Log("[GameOverPanel] Show called");
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
            Debug.Log($"[GameOverPanel] panelRoot activated. Active in hierarchy: {panelRoot.activeInHierarchy}");
        }
        else
        {
            Debug.LogError("[GameOverPanel] panelRoot is NULL — cannot show!");
        }

        var save = ServiceLocator.Get<SaveManager>();
        var gm = ServiceLocator.Get<GameManager>();

        if (finalScoreText != null && gm != null)
            finalScoreText.text = $"Score: {gm.CurrentScore:D6}";

        if (highScoreText != null && save != null)
            highScoreText.text = $"Best: {save.Data.highScore:D6}";
    }

    private void OnRestart()
    {
        Time.timeScale = 1f;
        EventBus.ClearAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMainMenu()
    {
        Time.timeScale = 1f;
        EventBus.ClearAll();
        SceneManager.LoadScene("MainMenu");
    }
}