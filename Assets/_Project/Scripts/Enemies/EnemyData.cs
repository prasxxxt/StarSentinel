using UnityEngine;

/// <summary>
/// The "type" half of the Type Object pattern.
/// A pure data asset describing one variety of enemy: appearance,
/// stats, and movement style. Shared across all Enemy instances of
/// this type - the runtime entities (Enemy.cs) hold a reference here.
///
/// New enemy types are created in the Unity editor as new asset files;
/// no new C# code is needed to introduce a new variant.
/// </summary>
[CreateAssetMenu(
    fileName = "NewEnemyData",
    menuName = "StarSentinel/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Display name used in UI / debug.")]
    public string displayName = "Enemy";

    [Header("Visuals")]
    public Sprite sprite;
    public Color color = Color.white;
    [Tooltip("Multiplier applied to the prefab's base scale.")]
    public float scale = 1f;

    [Header("Stats")]
    public int maxHealth = 3;
    public int scoreValue = 100;
    public int contactDamage = 1;

    [Header("Movement")]
    public MovementType movementType = MovementType.Stationary;
    [Tooltip("Speed in world-units per second.")]
    public float moveSpeed = 2f;
}

/// <summary>
/// The repertoire of movement behaviours an Enemy can exhibit.
/// Adding a new entry here also requires handling it in Enemy.UpdateMovement().
/// </summary>
public enum MovementType
{
    Stationary,
    LinearTowardPlayer,
    Sine,
    Orbit,
    Drift
}