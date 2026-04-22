using UnityEngine;

/// <summary>
/// Tracks the player's health. Supports an invulnerability flag set by
/// the Shield power-up. Publishes PlayerDamagedEvent on hit and
/// PlayerDiedEvent at zero, so listeners (HUD, audio, screen-shake)
/// don't need a direct reference to the player.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    public int CurrentHealth { get; private set; }

    private bool isInvulnerable = false;
    public bool IsInvulnerable => isInvulnerable;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable) return;
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;
        EventBus.Publish(new PlayerDamagedEvent { RemainingHealth = CurrentHealth });

        if (CurrentHealth <= 0)
        {
            EventBus.Publish(new PlayerDiedEvent());
            gameObject.SetActive(false);
        }
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }
}