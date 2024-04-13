using UnityEngine;

/// <summary>
/// Spawns bullets from the FirePoint when the player holds Fire (LMB or Space).
/// Rate-limited via a fire interval. Phase 2 will rewire this to use object pooling.
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

    private void Update()
    {
        bool firePressed = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);
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
            // firePoint.up = the ship's local "forward" direction in world space.
            bulletRb.linearVelocity = firePoint.up * bulletSpeed;
        }
    }
}