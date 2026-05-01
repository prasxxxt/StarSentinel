using UnityEngine;

/// <summary>
/// The "object" half of the Type Object pattern.
/// One generic enemy MonoBehaviour, parameterised by an EnemyData asset.
/// Reads its appearance, stats, and movement style from the data on enable.
/// Implements IDamageable so any source of damage (bullets today, lasers later)
/// can hurt it without knowing the concrete subclass.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable
{
    [Tooltip("The 'type' driving this instance. Drag an EnemyData asset here.")]
    [SerializeField] private EnemyData data;

    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    private Transform playerTransform;
    private Vector3 startPosition;
    private Vector3 driftDirection;
    private float spawnTime;

    private Color baseColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // Apply the type's properties on reactivation. Designed so this
        // also works for pooled enemies later.
        if (data != null) ApplyData();

        startPosition = transform.position;
        spawnTime = Time.time;

        // If this is a drifting enemy, pick a random direction once on spawn.
        if (data != null && data.movementType == MovementType.Drift)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            driftDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
        }
    }

    private void Start()
    {
        // Cache the player reference once. FindWithTag is fine hereit
        // it only runs once per spawned enemy, never per frame.
        var player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    /// <summary>
    /// Inject a data type at runtime. Used by the future spawner so the same
    /// prefab can be turned into any kind of enemy at instantiation time.
    /// </summary>
    public void Initialize(EnemyData newData)
    {
        data = newData;
        if (gameObject.activeInHierarchy) ApplyData();
    }

    private void ApplyData()
    {
        if (data.sprite != null) spriteRenderer.sprite = data.sprite;
        spriteRenderer.color = data.color;
        baseColor = data.color;                             // NEW
        transform.localScale = new Vector3(data.scale, data.scale, 1f);
        currentHealth = data.maxHealth;
        gameObject.name = $"Enemy ({data.displayName})";
    }
    private void Update()
    {
        if (data == null) return;
        UpdateMovement();

        // Despawn if too far from origin to avoid lingering off-screen.
        if (transform.position.magnitude > 25f)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateMovement()
    {
        switch (data.movementType)
        {
            case MovementType.Stationary:
                // intenttionally no movement
                break;

            case MovementType.LinearTowardPlayer:
                if (playerTransform != null)
                {
                    Vector3 dir =
                        (playerTransform.position - transform.position).normalized;
                    transform.position += dir * data.moveSpeed * Time.deltaTime;
                }
                break;

            case MovementType.Sine:
                {
                    float t = Time.time - spawnTime;
                    Vector3 p = startPosition;
                    p.x += Mathf.Sin(t * 2f) * 2f;
                    p.y -= t * data.moveSpeed * 0.5f;
                    transform.position = p;
                    break;
                }

            case MovementType.Orbit:
                {
                    float angle = (Time.time - spawnTime) * data.moveSpeed;
                    float radius = Mathf.Max(0.5f, startPosition.magnitude);
                    transform.position = new Vector3(
                        Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle) * radius,
                        0f);
                    break;
                }
            case MovementType.Drift:
                {
                    transform.position += driftDirection * data.moveSpeed * Time.deltaTime;
                    break;
                }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        // Hit flash + SFX
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(HitFlash());
        var audio = ServiceLocator.Get<AudioManager>();
        if (audio != null) audio.Play("enemy_hit");

        if (currentHealth <= 0) Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PlayerHealth health))
            {
                health.TakeDamage(data.contactDamage);
            }
            Die();
        }
    }

    private void Die()
    {
        EventBus.Publish(new EnemyDiedEvent
        {
            ScoreValue = data.scoreValue,
            Position = transform.position
        });
        Destroy(gameObject);
    }
    private System.Collections.IEnumerator HitFlash()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.06f);
        spriteRenderer.color = baseColor;
        flashRoutine = null;
    }
}