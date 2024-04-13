using UnityEngine;

/// <summary>
/// Simple bullet that travels forward, damages enemies on contact,
/// and self-destructs after a lifetime. (Phase 2 will replace this with a pooled version.)
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 30f;
    [SerializeField] private int damage = 1;

    private void Start()
    {
        // Schedule self-destruction so off-screen bullets don't accumulate.
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Try to deal damage if the target is destructible.
            if (other.TryGetComponent(out DummyEnemy enemy))
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}