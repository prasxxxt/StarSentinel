using UnityEngine;

/// <summary>
/// Abstract base for all power-up types. Holds shared display data
/// (name, icon, colour, duration) and declares the Apply/Remove
/// primitives that subclasses must implement.
///
/// Subclass Sandbox pattern: subclasses use a small set of provided
/// primitives (writing to PlayerHealth / PlayerShooter) without
/// duplicating activation, timer, or HUD plumbing.
/// </summary>
public abstract class PowerUpData : ScriptableObject
{
    [Header("Identity")]
    public string displayName = "Power Up";

    [Header("Visuals")]
    public Sprite icon;
    public Color color = Color.white;

    [Header("Timing")]
    [Min(0.5f)] public float duration = 8f;

    /// <summary>Apply the effect to the player. Called when picked up.</summary>
    public abstract void Apply(GameObject player);

    /// <summary>Remove the effect from the player. Called when the timer expires.</summary>
    public abstract void Remove(GameObject player);
}