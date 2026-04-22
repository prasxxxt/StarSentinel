using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Listens for WaveStartedEvent and WaveCompletedEvent on the EventBus
/// and updates: a persistent wave counter and a fading "WAVE N" banner.
/// </summary>
public class WaveHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI waveCounterText;
    [SerializeField] private TextMeshProUGUI waveAnnouncementText;
    [SerializeField] private CanvasGroup announcementCanvasGroup;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float displayDuration = 1.5f;

    private Coroutine announcementRoutine;

    private void OnEnable()
    {
        EventBus.Subscribe<WaveStartedEvent>(OnWaveStarted);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<WaveStartedEvent>(OnWaveStarted);
    }

    private void OnWaveStarted(WaveStartedEvent evt)
    {
        if (waveCounterText != null)
            waveCounterText.text = $"WAVE {evt.WaveNumber}";

        if (announcementRoutine != null)
            StopCoroutine(announcementRoutine);
        announcementRoutine = StartCoroutine(AnnounceRoutine(evt));
    }

    private IEnumerator AnnounceRoutine(WaveStartedEvent evt)
    {
        if (waveAnnouncementText == null || announcementCanvasGroup == null)
            yield break;

        waveAnnouncementText.text = $"WAVE {evt.WaveNumber}";

        // Fade in.
        yield return Fade(0f, 1f);

        // Hold.
        float held = 0f;
        while (held < displayDuration)
        {
            held += Time.deltaTime;
            yield return null;
        }

        // Fade out.
        yield return Fade(1f, 0f);

        announcementRoutine = null;
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            announcementCanvasGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        announcementCanvasGroup.alpha = to;
    }
}