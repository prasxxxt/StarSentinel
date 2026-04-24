using UnityEngine;

/// <summary>
/// World-space pickup. Rotates for visibility. On player contact,
/// asks the PowerUpManager to activate its data, then despawns.
/// Reads visuals from its data via Initialize.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpData data;
    [SerializeField] private float rotationSpeed = 90f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (data != null) ApplyVisuals();
    }

    public void Initialize(PowerUpData newData)
    {
        data = newData;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        if (data == null) return;
        if (data.icon != null) spriteRenderer.sprite = data.icon;
        spriteRenderer.color = data.color;
        gameObject.name = $"PowerUp ({data.displayName})";
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (data == null) return;

        var manager = ServiceLocator.Get<PowerUpManager>();
        if (manager != null) manager.Activate(data);

        EventBus.Publish(new PowerUpCollectedEvent { Data = data });
        Destroy(gameObject);
    }
}