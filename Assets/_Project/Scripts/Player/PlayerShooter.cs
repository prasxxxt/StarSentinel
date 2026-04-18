using UnityEngine;

/// <summary>
/// Spawns bullets via the BulletPool service when Fire is pressed.
/// Gates firing on the GameManager's state (no shooting while paused
/// or game-over). Both dependencies are resolved through the
/// ServiceLocator — no direct references to other systems.
/// </summary>
public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;

    [Header("Fire control")]
    [SerializeField] private float fireInterval = 0.2f;
    [SerializeField] private float bulletSpeed = 15f;

    private float nextFireTime;
    private GameManager gameManager;
    private BulletPool bulletPool;

    private void Start()
    {
        gameManager = ServiceLocator.Get<GameManager>();
        bulletPool = ServiceLocator.Get<BulletPool>();
    }

    private void Update()
    {
        if (gameManager == null || gameManager.CurrentState != GameState.Playing)
            return;

        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireInterval;
        }
    }

    private void Fire()
    {
        if (firePoint == null || bulletPool == null) return;

        Bullet bullet = bulletPool.Get();
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.Launch(firePoint.up * bulletSpeed);
    }
}