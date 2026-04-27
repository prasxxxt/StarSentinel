using UnityEngine;

public class EffectsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject playerExplosionPrefab;

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
        EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
        EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnEnemyDied(EnemyDiedEvent evt)
    {
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, evt.Position, Quaternion.identity);
    }

    private void OnPlayerDied(PlayerDiedEvent evt)
    {
        // Player position not in the event; find the player's last spot.
        var player = GameObject.FindWithTag("Player");
        if (player != null && playerExplosionPrefab != null)
            Instantiate(playerExplosionPrefab, player.transform.position, Quaternion.identity);
    }
}