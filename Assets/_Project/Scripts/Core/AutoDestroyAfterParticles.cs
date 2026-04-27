using UnityEngine;

/// <summary>
/// Destroys the GameObject once its ParticleSystem finishes emitting.
/// Used so one-shot effects (explosions, etc.) clean up automatically.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class AutoDestroyAfterParticles : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!ps.IsAlive(true))
            Destroy(gameObject);
    }
}