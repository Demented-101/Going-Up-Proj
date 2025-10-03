using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputManager))]
public class MovementStateHandler : MonoBehaviour
{
    public Action<MovementState, MovementState> onStateChanged; // new state, old state

    public InputManager inputManager { get; private set; }
    public CharacterController controller { get; private set; }

    [SerializeField] private GameStatus gameStatus;
    [SerializeField] private MovementState initialState;
    public MovementState currentState { get; private set; }

    public void Start()
    {
        inputManager = GetComponent<InputManager>();
        controller = GetComponent<CharacterController>();

        currentState = initialState;
    }

    public void ChangeState(MovementState newState)
    {
        MovementState oldState = currentState;
        currentState = newState;

        oldState.enabled = false;
        newState.enabled = true;

        onStateChanged.Invoke(newState, oldState);
    }

    public void Update()
    {
        currentState.enabled = gameStatus.gameState == Utils.GameStates.Run
    }
}
