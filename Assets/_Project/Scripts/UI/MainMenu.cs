using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Drives the main menu: shows the saved high score / best wave,
/// handles Play / Options / Quit, exposes a master volume slider that
/// writes to the SaveManager.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Title screen")]
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI bestWaveText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;

    [Header("Options panel")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button backButton;

    [Header("Scene names")]
    [SerializeField] private string gameSceneName = "Game";

    private void Start()
    {
        var save = ServiceLocator.Get<SaveManager>();

        // If no SaveManager exists yet (very first launch), spawn one.
        // Normal flow: SaveManager persists from gameplay via DontDestroyOnLoad.
        if (save == null)
        {
            var go = new GameObject("SaveManager");
            save = go.AddComponent<SaveManager>();
        }

        // Display the saved values.
        if (highScoreText != null)
            highScoreText.text = $"Best Score: {save.Data.highScore:D6}";
        if (bestWaveText != null)
            bestWaveText.text = $"Best Wave: {save.Data.bestWave}";
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(save.Data.masterVolume);
            volumeSlider.onValueChanged.AddListener(save.SetMasterVolume);
        }

        // Wire buttons.
        if (playButton != null) playButton.onClick.AddListener(OnPlay);
        if (optionsButton != null) optionsButton.onClick.AddListener(OnOptions);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuit);
        if (backButton != null) backButton.onClick.AddListener(OnBack);

        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    private void OnPlay()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    private void OnBack()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    private void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}