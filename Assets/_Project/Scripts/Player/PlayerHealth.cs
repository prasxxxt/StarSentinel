using UnityEngine;

/// <summary>
/// Tracks the player's health. Publishes PlayerDamagedEvent on hit
/// and PlayerDiedEvent at zero. Listeners (HUD, audio, screen-shake)
/// react to those events without holding a reference to the player.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    public int CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHealth <= 0) return; // already dead, ignore

        CurrentHealth -= amount;

        EventBus.Publish(new PlayerDamagedEvent
        {
            RemainingHealth = CurrentHealth
        });

        if (CurrentHealth <= 0)
        {
            EventBus.Publish(new PlayerDiedEvent());
            // We don't destroy the player GameObject here.
            // GameManager handles the GameOver state transition.
            gameObject.SetActive(false);
        }
    }
}