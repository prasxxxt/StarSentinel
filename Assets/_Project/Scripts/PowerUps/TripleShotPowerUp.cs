using UnityEngine;

/// <summary>
/// Enables the triple-shot spread while active.
/// </summary>
[CreateAssetMenu(fileName = "TripleShotPowerUp", menuName = "StarSentinel/PowerUp/TripleShot")]
public class TripleShotPowerUpData : PowerUpData
{
    public override void Apply(GameObject player)
    {
        if (player.TryGetComponent(out PlayerShooter shooter))
            shooter.SetTripleShotActive(true);
    }

    public override void Remove(GameObject player)
    {
        if (player.TryGetComponent(out PlayerShooter shooter))
            shooter.SetTripleShotActive(false);
    }
}