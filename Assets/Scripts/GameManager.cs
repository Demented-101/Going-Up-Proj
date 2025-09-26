using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Utils.GameStates currentState { get; private set; }
    [SerializeField] private GameStatus gameStatus;

    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentState = Utils.GameStates.Pregame;
    }

    // updates the current game state to reduce repeated code.
    private void ChangeState(Utils.GameStates newState)
    {
        currentState = newState;
        if (gameStatus != null)
        {
            gameStatus.gameState = newState;
            gameStatus.onStateChange?.Invoke(newState);
            Debug.Log("Game State changed - " + newState.ToString());
        }
    }
    
    // pregame menu -> first elevator
    public void StartGame()
    {
        ChangeState(Utils.GameStates.Elevator);
    }

    // ends the game (any state -> pregame menu)
    public void EndGame()
    {
        ChangeState(Utils.GameStates.Pregame);
    }

    // elevator -> new run
    public void StartRun()
    {
        ChangeState(Utils.GameStates.Run);
    }

    // run -> elevator
    public void EndRun()
    {
        ChangeState(Utils.GameStates.Elevator);
    }

}
