using UnityEngine;

/// <summary>
/// Spawns bullets from the FirePoint when the fire key is held.
/// Refuses to fire unless the game is in the Playing state — this
/// prevents bullets from spawning during pause / game-over screens.
/// Game state is queried via the ServiceLocator pattern.
/// </summary>
public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Fire control")]
    [Tooltip("Seconds between shots. Lower = faster fire.")]
    [SerializeField] private float fireInterval = 0.2f;

    [Tooltip("Speed at which spawned bullets travel.")]
    [SerializeField] private float bulletSpeed = 15f;

    private float nextFireTime;
    private GameManager gameManager;

    private void Start()
    {
        // Resolve the GameManager once via the Service Locator.
        // Cached because Service Locator lookups, while cheap, aren't free.
        gameManager = ServiceLocator.Get<GameManager>();
    }

    private void Update()
    {
        // Gate: only fire when the game is actively being played.
        if (gameManager == null || gameManager.CurrentState != GameState.Playing)
            return;

        bool firePressed = Input.GetKey(KeyCode.Space);
        if (firePressed && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireInterval;
        }
    }

    private void Fire()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (bullet.TryGetComponent(out Rigidbody2D bulletRb))
        {
            bulletRb.linearVelocity = firePoint.up * bulletSpeed;
        }
    }
}