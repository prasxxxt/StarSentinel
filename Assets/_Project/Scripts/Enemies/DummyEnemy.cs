using UnityEngine;

/// <summary>
/// Placeholder enemy used only for Phase 1 testing.
/// Phase 2 replaces this entirely with the Type Object pattern (Enemy + EnemyData ScriptableObject).
/// </summary>
public class DummyEnemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"DummyEnemy took {amount} damage, {currentHealth} HP remaining");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("DummyEnemy destroyed");
        Destroy(gameObject);
    }
}