using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float defaultDuration = 0.25f;
    [SerializeField] private float defaultMagnitude = 3f;
    [SerializeField] private float deathDuration = 0.6f;
    [SerializeField] private float deathMagnitude = 5f;

    private Vector3 originalPosition;
    private float shakeTimeRemaining = 0f;
    private float currentDuration = 0f;
    private float currentMagnitude = 0f;

    private void Awake()
    {
        originalPosition = transform.position;
        Debug.Log($"[CameraShake] Awake. originalPosition = {originalPosition}");
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnPlayerDamaged(PlayerDamagedEvent evt)
    {
        StartShake(defaultDuration, defaultMagnitude);
    }

    private void OnPlayerDied(PlayerDiedEvent evt)
    {
        StartShake(deathDuration, deathMagnitude);
    }

    private void StartShake(float duration, float magnitude)
    {
        currentDuration = duration;
        currentMagnitude = magnitude;
        shakeTimeRemaining = duration;
    }

    private void LateUpdate()
    {
        if (shakeTimeRemaining > 0f)
        {
            shakeTimeRemaining -= Time.unscaledDeltaTime;

            float damper = Mathf.Clamp01(shakeTimeRemaining / currentDuration);
            float offsetX = (Random.value * 2f - 1f) * currentMagnitude * damper;
            float offsetY = (Random.value * 2f - 1f) * currentMagnitude * damper;

            Vector3 newPos = new Vector3(
                originalPosition.x + offsetX,
                originalPosition.y + offsetY,
                originalPosition.z
            );

            transform.position = newPos;
            Debug.Log($"[CameraShake] frame: pos={newPos}, offset=({offsetX:F2}, {offsetY:F2})");

            if (shakeTimeRemaining <= 0f)
            {
                transform.position = originalPosition;
            }
        }
    }
}