using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] private Utils.GameStates listenForState;
    public bool IsActive { get; private set; }
    public GameStatus gameStatus;
    public bool printUpdates;

    public virtual void Start()
    {
        gameStatus.onStateChange += OnStateChanged;
        OnStateChanged(Utils.GameStates.Pregame);
    }

    public virtual void OnStateChanged(Utils.GameStates newState)
    {
        IsActive = newState == listenForState;
        if (printUpdates) { Debug.Log("State update - " + IsActive.ToString() + ". Im listening for " + listenForState.ToString()); }
    }
}
