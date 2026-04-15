using UnityEngine;

/// <summary>
/// Travels forward, damages the first IDamageable it touches, then despawns.
/// Doesn't care whether the target is an Enemy, an Asteroid, or anything else
/// the design team adds later — interfaces decouple senders from receivers
/// of damage in the same way the EventBus decouples publishers from subscribers.
/// </summary>
public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}