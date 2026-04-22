using UnityEngine;

/// <summary>
/// Scene-resident wrapper around an ObjectPool&lt;Bullet&gt;.
/// Registers itself in the ServiceLocator so PlayerShooter (and any
/// future weapon) can fetch bullets without holding a direct reference.
/// </summary>
public class BulletPool : MonoBehaviour
{
    [Header("Pool config")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int initialSize = 30;
    [SerializeField] private int maxSize = 100;

    private ObjectPool<Bullet> pool;

    private void Awake()
    {
        ServiceLocator.Register(this);
        pool = new ObjectPool<Bullet>(bulletPrefab, initialSize, maxSize, transform);
    }

    public Bullet Get() => pool.Get();
    public void Release(Bullet bullet) => pool.Release(bullet);
}