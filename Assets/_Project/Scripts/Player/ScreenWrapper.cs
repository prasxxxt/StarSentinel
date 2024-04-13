using UnityEngine;

/// <summary>
/// Teleports the GameObject to the opposite edge of the camera's view
/// when it crosses a viewport boundary. Used for the player ship.
/// Enemies do NOT have this component (per design).
/// </summary>
public class ScreenWrapper : MonoBehaviour
{
    [Tooltip("How far past the edge before we wrap. Prevents flicker.")]
    [SerializeField] private float padding = 0.2f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        Vector3 newPos = transform.position;
        bool wrapped = false;

        if (viewportPos.x < 0f)
        {
            // Off the left edge → teleport to the right
            newPos = new Vector3(GetRightEdgeX() - padding, newPos.y, newPos.z);
            wrapped = true;
        }
        else if (viewportPos.x > 1f)
        {
            // Off the right edge → teleport to the left
            newPos = new Vector3(GetLeftEdgeX() + padding, newPos.y, newPos.z);
            wrapped = true;
        }

        if (viewportPos.y < 0f)
        {
            // Off the bottom edge → teleport to the top
            newPos = new Vector3(newPos.x, GetTopEdgeY() - padding, newPos.z);
            wrapped = true;
        }
        else if (viewportPos.y > 1f)
        {
            // Off the top edge → teleport to the bottom
            newPos = new Vector3(newPos.x, GetBottomEdgeY() + padding, newPos.z);
            wrapped = true;
        }

        if (wrapped)
        {
            transform.position = newPos;
        }
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