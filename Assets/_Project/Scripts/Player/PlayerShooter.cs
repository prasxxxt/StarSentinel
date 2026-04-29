using UnityEngine;

/// <summary>
/// Spawns bullets through the BulletPool service. Supports two power-up
/// modifiers: a fire-rate multiplier (RapidFire) and a triple-shot mode
/// (TripleShot). Both are toggled by the Subclass Sandbox power-up
/// effects via the public Set* methods.
/// </summary>
public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;

    [Header("Fire control")]
    [SerializeField] private float fireInterval = 0.2f;
    [SerializeField] private float bulletSpeed = 15f;

    [Header("Triple-shot spread")]
    [SerializeField] private float spreadAngle = 15f;

    private float nextFireTime;
    private float fireRateMultiplier = 1f;
    private bool tripleShotActive = false;

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

        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + (fireInterval / fireRateMultiplier);
        }
    }

    private void Fire()
    {
        if (firePoint == null || bulletPool == null) return;

        var audio = ServiceLocator.Get<AudioManager>();
        Debug.Log($"[PlayerShooter] Fire. AudioManager = {(audio != null ? "OK" : "NULL")}");
        if (audio != null) audio.Play("shoot");

        SpawnBullet(0f);
        if (tripleShotActive)
        {
            SpawnBullet(-spreadAngle);
            SpawnBullet(spreadAngle);
        }
    }

    private void SpawnBullet(float angleOffset)
    {
        Bullet bullet = bulletPool.Get();
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation * Quaternion.Euler(0, 0, angleOffset);
        bullet.Launch(bullet.transform.up * bulletSpeed);
    }

    // Powerup hooks

    public void SetFireRateMultiplier(float multiplier)
    {
        fireRateMultiplier = Mathf.Max(0.1f, multiplier);
    }

    public void SetTripleShotActive(bool active)
    {
        tripleShotActive = active;
    }
}