using Unity.VisualScripting;
using UnityEngine;

/// <summary>The four high-level states the game can be in.</summary>
public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver
}

/// <summary>
/// Top-level game controller. Owns the Finite State Machine that
/// drives the high-level game state, and pumps the EventBus once
/// per frame so queued events get delivered to subscribers.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("FSM")]
    [Tooltip("Which state to enter on scene load.")]
    [SerializeField] private GameState initialState = GameState.Playing;

    public GameState CurrentState { get; private set; }
    public int CurrentScore { get; private set; }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        ServiceLocator.Register(this);
    }

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

    private void Start()
    {
        Debug.Log("HELLO FROM " + gameObject.name);
        ChangeState(initialState);
    }

    private void Update()
    {
        // The Event Queue pump
        // Every frame, drain queued events and deliver them to handlers.
        EventBus.ProcessQueue();

        // Pause toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (CurrentState == GameState.Playing)
                ChangeState(GameState.Paused);
            else if (CurrentState == GameState.Paused)
                ChangeState(GameState.Playing);
        }
    }

    // FSM core
    /// <summary>Transition the FSM to a new state, firing exit/enter logic.</summary>
    public void ChangeState(GameState newState)
    {
        if (newState == CurrentState) return;

        var previous = CurrentState;
        OnStateExit(previous);
        CurrentState = newState;
        OnStateEnter(newState);

        EventBus.Publish(new GameStateChangedEvent
        {
            NewState = newState,
            PreviousState = previous
        });
    }

    private void OnStateEnter(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                Time.timeScale = 0f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }

    private void OnStateExit(GameState state)
    {
        // Per-state cleanup. Nothing yet; we'll fill this in later phases.
    }

    // Score handling event listener

    private void OnEnemyDied(EnemyDiedEvent evt)
    {
        if (CurrentState != GameState.Playing) return;

        CurrentScore += evt.ScoreValue;
        EventBus.Publish(new ScoreChangedEvent { NewScore = CurrentScore });
    }

    private void OnPlayerDied(PlayerDiedEvent evt)
    {
        ChangeState(GameState.GameOver);
    }
}