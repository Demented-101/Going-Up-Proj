using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputManager))]
public class MovementStateHandler : MonoBehaviour
{
    public Action<MovementState, MovementState> onStateChanged; // new state, old state

    public InputManager inputManager { get; private set; }
    public CharacterController controller { get; private set; }
    public GameObject cameraObj { get; private set; }
    public CamOrbitObjState camOrbitController { get; private set; }

    [SerializeField] private GameStatus gameStatus;
    [SerializeField] private MovementState initialState;
    public MovementState currentState { get; private set; }
    public Vector3 velocity = Vector3.zero;

    public void Start()
    {
        // setup objects required for sub-states
        inputManager = GetComponent<InputManager>();
        controller = GetComponent<CharacterController>();
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        Debug.Log(cameraObj);
        camOrbitController = cameraObj.GetComponent<CamOrbitObjState>();

        // setup initial state
        currentState = initialState;
    }

    public void ChangeState(MovementState newState)
    {
        if (newState == currentState) { return; }

        if (newState == null)
        {
            Debug.LogError("State passed is null");
            return;
        }
        if (currentState == null)
        {
            Debug.Log("Old state is null");
            return;
        }

        // Disable old state and start new state.
        MovementState oldState = currentState;
        currentState = newState;

        oldState.enabled = false;
        oldState.onExit();

        newState.enabled = true;
        newState.onEntered();

        onStateChanged?.Invoke(newState, oldState);
    }

    public void Update()
    {
        // make sure the player isnt enabled on incorrect states
        currentState.enabled = gameStatus.gameState == Utils.GameStates.Run;
    }

    public void Move(Vector3 newVelocity)
    {
        velocity = newVelocity;

        if(controller != null && velocity != Vector3.zero)
        {
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
