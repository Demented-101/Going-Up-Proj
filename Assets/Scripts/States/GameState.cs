using Unity.VisualScripting;
using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField] private Utils.GameStates listenForState;
    public bool IsActive { get; private set; }
    public GameStatus gameStatus;

    public virtual void Start()
    {
        gameStatus.onStateChange += OnStateChanged;
        OnStateChanged(Utils.GameStates.Pregame);
    }

    public virtual void OnStateChanged(Utils.GameStates newState)
    {
        IsActive = newState == listenForState;
    }
}
