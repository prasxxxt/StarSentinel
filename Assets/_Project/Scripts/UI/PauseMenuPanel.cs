using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shows a pause overlay when GameStateChangedEvent reports the
/// Paused state. Resume returns to Playing; Restart reloads the
/// gameplay scene; Main Menu loads the menu scene.
///
/// Same controller / panel structure as GameOverPanel — the script
/// lives on a parent that stays active, the panel root it controls
/// starts inactive and is shown/hidden by the script.
/// </summary>
public class PauseMenuPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        if (restartButton != null) restartButton.onClick.AddListener(OnRestart);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenu);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameStateChangedEvent evt)
    {
        if (panelRoot == null) return;

        // Show on entering Paused, hide on leaving it.
        panelRoot.SetActive(evt.NewState == GameState.Paused);
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