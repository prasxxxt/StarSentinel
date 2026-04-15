using UnityEngine;

/// <summary>
/// Base class for every event that can travel through the EventBus.
/// Concrete events extend this and add the data their listeners need.
/// </summary>
public abstract class GameEvent { }

// ---------- Gameplay events ----------

/// <summary>Published when an enemy is destroyed by the player.</summary>
public class EnemyDiedEvent : GameEvent
{
    public int ScoreValue;
    public Vector3 Position;
}

/// <summary>Published whenever the score changes.</summary>
public class ScoreChangedEvent : GameEvent
{
    public int NewScore;
}

/// <summary>Published whenever the player takes damage.</summary>
public class PlayerDamagedEvent : GameEvent
{
    public int RemainingHealth;
}

/// <summary>Published when the player dies.</summary>
public class PlayerDiedEvent : GameEvent { }

// ---------- State events ----------

/// <summary>Published whenever the game's overall state changes.</summary>
public class GameStateChangedEvent : GameEvent
{
    public GameState NewState;
    public GameState PreviousState;
}