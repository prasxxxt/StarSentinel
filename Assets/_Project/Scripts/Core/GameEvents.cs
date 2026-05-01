using UnityEngine;

/// <summary>
/// Base class for every event that can travel through the EventBus.
/// Concrete events extend this and add the data their listeners need.
/// </summary>
public abstract class GameEvent { }

//Gameplay events
public class EnemyDiedEvent : GameEvent
{
    public int ScoreValue;
    public Vector3 Position;
}
public class ScoreChangedEvent : GameEvent
{
    public int NewScore;
}
public class PlayerDamagedEvent : GameEvent
{
    public int RemainingHealth;
}
public class PlayerDiedEvent : GameEvent { }

//State events
public class GameStateChangedEvent : GameEvent
{
    public GameState NewState;
    public GameState PreviousState;
}
//Wave events
public class WaveStartedEvent : GameEvent
{
    public int WaveNumber;
    public string WaveName;
    public int TotalEnemies;
}
public class WaveCompletedEvent : GameEvent
{
    public int WaveNumber;
}
public class AllWavesCompletedEvent : GameEvent { }

//Power-up events

public class PowerUpCollectedEvent : GameEvent
{
    public PowerUpData Data;
}

public class PowerUpActivatedEvent : GameEvent
{
    public PowerUpData Data;
    public float Duration;
}

public class PowerUpExpiredEvent : GameEvent
{
    public PowerUpData Data;
}

//Save events

public class HighScoreUpdatedEvent : GameEvent
{
    public int NewHighScore;
}

public class BestWaveUpdatedEvent : GameEvent
{
    public int NewBestWave;
}