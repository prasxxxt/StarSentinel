using UnityEngine;

/// <summary>
/// Aggregates visual feedback elements on the player ship that other
/// systems (power-ups, hit reactions) can toggle.
/// </summary>
public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private GameObject shieldVisual;

    public void SetShieldVisible(bool visible)
    {
        if (shieldVisual != null) shieldVisual.SetActive(visible);
    }
}