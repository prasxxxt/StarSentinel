using UnityEngine;

/// <summary>
/// Sets PlayerHealth.IsInvulnerable while active. Player takes no damage.
/// </summary>
[CreateAssetMenu(fileName = "ShieldPowerUp", menuName = "StarSentinel/PowerUp/Shield")]
public class ShieldPowerUpData : PowerUpData
{
    public override void Apply(GameObject player)
    {
        if (player.TryGetComponent(out PlayerHealth health))
            health.SetInvulnerable(true);
    }

    public override void Remove(GameObject player)
    {
        if (player.TryGetComponent(out PlayerHealth health))
            health.SetInvulnerable(false);
    }
}