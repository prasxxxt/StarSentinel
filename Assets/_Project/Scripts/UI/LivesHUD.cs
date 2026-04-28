using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays the player's remaining lives as a row of icons.
/// Listens to PlayerDamagedEvent on the EventBus and hides the
/// rightmost icon as the player loses health. Reset on scene load.
/// </summary>
public class LivesHUD : MonoBehaviour
{
    [Tooltip("Drag each life icon GameObject in. Order: leftmost first.")]
    [SerializeField] private List<GameObject> lifeIcons;

    private void OnEnable()
    {
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
    }

    private void OnPlayerDamaged(PlayerDamagedEvent evt)
    {
        UpdateIcons(evt.RemainingHealth);
    }

    private void UpdateIcons(int remaining)
    {
        // Hide icons beyond the remaining health count.
        for (int i = 0; i < lifeIcons.Count; i++)
        {
            if (lifeIcons[i] != null)
                lifeIcons[i].SetActive(i < remaining);
        }
    }
}