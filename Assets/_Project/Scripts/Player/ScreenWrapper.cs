using UnityEngine;

/// <summary>
/// Teleports the GameObject to the opposite edge of the camera's view
/// when it crosses a viewport boundary. On wrap, spawns a brief flash
/// effect at both the exit and entry positions for visual polish.
/// </summary>
public class ScreenWrapper : MonoBehaviour
{
    [Tooltip("How far past the edge before we wrap. Prevents flicker.")]
    [SerializeField] private float padding = 0.2f;

    [Tooltip("Optional: particle prefab spawned at exit and entry points on wrap.")]
    [SerializeField] private GameObject teleportFlashPrefab;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        Vector3 oldPos = transform.position;
        Vector3 newPos = oldPos;
        bool wrapped = false;

        if (viewportPos.x < 0f)
        {
            newPos = new Vector3(GetRightEdgeX() - padding, newPos.y, newPos.z);
            wrapped = true;
        }
        else if (viewportPos.x > 1f)
        {
            newPos = new Vector3(GetLeftEdgeX() + padding, newPos.y, newPos.z);
            wrapped = true;
        }

        if (viewportPos.y < 0f)
        {
            newPos = new Vector3(newPos.x, GetTopEdgeY() - padding, newPos.z);
            wrapped = true;
        }
        else if (viewportPos.y > 1f)
        {
            newPos = new Vector3(newPos.x, GetBottomEdgeY() + padding, newPos.z);
            wrapped = true;
        }

        if (wrapped)
        {
            // Flash at exit point (where we vanished from)
            SpawnFlash(oldPos);
            // Flash at entry point (where we reappear)
            SpawnFlash(newPos);

            transform.position = newPos;
        }
    }

    private void SpawnFlash(Vector3 position)
    {
        var audio = ServiceLocator.Get<AudioManager>();
        if (audio != null) audio.Play("teleport");
        if (teleportFlashPrefab == null) return;
        Instantiate(teleportFlashPrefab, position, Quaternion.identity);
    }

    private float GetLeftEdgeX() =>
        mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, -mainCamera.transform.position.z)).x;
    private float GetRightEdgeX() =>
        mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, -mainCamera.transform.position.z)).x;
    private float GetBottomEdgeY() =>
        mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, -mainCamera.transform.position.z)).y;
    private float GetTopEdgeY() =>
        mainCamera.ViewportToWorldPoint(new Vector3(0f, 1f, -mainCamera.transform.position.z)).y;
}