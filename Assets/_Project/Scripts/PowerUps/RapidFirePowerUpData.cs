using UnityEngine;

/// <summary>
/// Multiplies the player's fire rate while active.
/// </summary>
[CreateAssetMenu(fileName = "RapidFirePowerUp", menuName = "StarSentinel/PowerUp/RapidFire")]
public class RapidFirePowerUpData : PowerUpData
{
    [Header("Effect")]
    [Tooltip("How many times faster than normal the player shoots while active.")]
    public float fireRateMultiplier = 3f;

    public override void Apply(GameObject player)
    {
        if (player.TryGetComponent(out PlayerShooter shooter))
            shooter.SetFireRateMultiplier(fireRateMultiplier);
    }

    public override void Remove(GameObject player)
    {
        if (player.TryGetComponent(out PlayerShooter shooter))
            shooter.SetFireRateMultiplier(1f);
    }
}