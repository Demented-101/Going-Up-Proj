using System;
using System.Linq.Expressions;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementGameState : GameState
{
    public Action<PlayerMovementState, PlayerMovementState> onStateChanged; // new state, old state

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private GameObject controllerGameObject;
    public CharacterController controller { get; private set; }
    private GameObject cameraObj;
    private CamOrbitObjState camHandler;

    [SerializeField] private bool resetOnDisable;
    [SerializeField] private PlayerMovementState initialState;
    public PlayerMovementState currentState { get; private set; }

    public override void Start()
    {
        base.Start();

        controller = controllerGameObject?.GetComponent<CharacterController>();
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        camHandler = cameraObj.GetComponent<CamOrbitObjState>();

        initialState.enabled = true;
    }

    public override void OnStateChanged(Utils.GameStates newState)
    {
        base.OnStateChanged(newState);

        if (resetOnDisable && !IsActive) { ChangeState(initialState); } // reset to initial state on disable
        currentState.enabled = IsActive;
    }

    public void ChangeState(PlayerMovementState newState)
    {
        PlayerMovementState oldState = currentState;
        currentState = newState;

        oldState.enabled = false;
        newState.enabled = true;

        onStateChanged.Invoke(newState, oldState);
    }
}
