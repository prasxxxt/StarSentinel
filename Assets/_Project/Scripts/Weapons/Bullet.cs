using UnityEngine;

/// <summary>
/// Pooled projectile. Lives between Get() (OnEnable) and Release()
/// (called when it expires or hits a damageable target).
/// Reset state runs in OnDisable so the same instance can be safely
/// recycled with no carry-over from the previous flight.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;
    private BulletPool pool;
    private float despawnTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Resolve the pool once. Cached for the rest of this instance's life.
        if (pool == null) pool = ServiceLocator.Get<BulletPool>();
        despawnTime = Time.time + lifetime;
    }

    private void OnDisable()
    {
        // Wipe state so the next user of this instance starts cleanly.
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void Update()
    {
        if (Time.time >= despawnTime)
            ReturnToPool();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            ReturnToPool();
        }
    }

    /// <summary>Sets the bullet's velocity. Called by PlayerShooter on Fire.</summary>
    public void Launch(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    private void ReturnToPool()
    {
        if (pool != null) pool.Release(this);
        else Destroy(gameObject); // safety fallback if pool not ready
    }
}