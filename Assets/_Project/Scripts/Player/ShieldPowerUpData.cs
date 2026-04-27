using UnityEngine;

public override void Apply(GameObject player)
{
    if (player.TryGetComponent(out PlayerHealth health))
        health.SetInvulnerable(true);
    if (player.TryGetComponent(out PlayerVisuals visuals))
        visuals.SetShieldVisible(true);
}

public override void Remove(GameObject player)
{
    if (player.TryGetComponent(out PlayerHealth health))
        health.SetInvulnerable(false);
    if (player.TryGetComponent(out PlayerVisuals visuals))
        visuals.SetShieldVisible(false);
}