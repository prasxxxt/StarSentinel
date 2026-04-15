using UnityEngine;

/// <summary>
/// Placeholder enemy. Stationary in this build.
///   - Bullets damage it via TakeDamage().
///   - Player contact damages the player and destroys the enemy.
///   - On death, publishes EnemyDiedEvent through the EventBus.
/// Phase 2B replaces this with a Type-Object-driven Enemy.
/// </summary>
public class DummyEnemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int scoreValue = 100;
    [SerializeField] private int contactDamage = 1;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerHealth health))
            {
                health.TakeDamage(contactDamage);
            }
            Die();
        }
    }

    private void Die()
    {
        EventBus.Publish(new EnemyDiedEvent
        {
            ScoreValue = scoreValue,
            Position = transform.position
        });
        Destroy(gameObject);
    }
}