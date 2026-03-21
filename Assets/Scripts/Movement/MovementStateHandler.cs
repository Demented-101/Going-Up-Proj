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
    public CameraGameStateHandler camOrbitController { get; private set; }
    private MovementState[] allStates = null;

    [SerializeField] private GameStatus gameStatus;
    [SerializeField] private MovementState initialState;
    [SerializeField] private GameObject model;
    [SerializeField] private Animator animator;
    [SerializeField] private string animStateName = "State";
    [SerializeField] private string animSpeedName = "Speed";
    [SerializeField] private bool printUpdates;
    [SerializeField] private bool printVelocity;
    
    public MovementState currentState { get; private set; }
    public Vector3 velocity { get; private set; } = Vector3.zero;

    public void Start()
    {
        // setup objects required for sub-states
        inputManager = GetComponent<InputManager>();
        controller = GetComponent<CharacterController>();
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        camOrbitController = cameraObj.GetComponent<CameraGameStateHandler>();

        // setup initial state
        currentState = initialState;
        allStates = GetComponents<MovementState>();

        // clear velocity between levels
        gameStatus.onStateChange += (Utils.GameStates newState) =>
        {
            if (newState != Utils.GameStates.Run) Move(new Vector3(0, 0, 0));
        };
    }

    public void ChangeState(MovementState newState, MovementState.TransitionData[] data = null)
    {
        if (printUpdates) 
        {
            if (data != null) { Debug.Log("new state entered: " + newState + " old state: " + currentState + " data size: " + data.Length); }
            else { Debug.Log("new state entered: " + newState +  "old state: " + currentState); }
        }

        if (newState == null || currentState == null || newState == currentState) { if (printUpdates) Debug.Log("State Chenge invalid; returned."); return;  }

        // Disable old state and start new state.
        MovementState oldState = currentState;
        currentState = newState;

        oldState.enabled = false;
        oldState.onExit();

        newState.enabled = true;
        newState.onEntered(data == null ? Array.Empty<MovementState.TransitionData>() : data); // make sure an array is always passed);

        onStateChanged?.Invoke(newState, oldState);
    }

    public void Update()
    {
        // only the current state is enabled, and is disabled out of game
        foreach (MovementState state in allStates) { state.enabled = state == currentState && gameStatus.gameState == Utils.GameStates.Run; }
        if (printVelocity) { Debug.Log("Velocity: " + velocity + "  horizontal speed: " + Utils.GetHorizontal(velocity, false).magnitude); }
    }

    public void Move(Vector3 newVelocity)
    {
        velocity = newVelocity;
        if(controller != null) controller.Move(velocity * Time.deltaTime);
    }

    public void CapSpeed(float maxSpeed)
    {
        if (velocity.magnitude <= maxSpeed) { return; }
        velocity = velocity.normalized * maxSpeed;
    }

    public void Rotate(Utils.CharacterRotationMode rotationMode)
    {
        if (model == null) { return; }
        Transform transform = model.transform;
        Vector3 newForward = transform.forward;

        switch (rotationMode)
        {
            case Utils.CharacterRotationMode.None: break;

            case Utils.CharacterRotationMode.FollowVelocity:// follow velocity
                if (velocity.magnitude != 0) { newForward = velocity; }
                break;

            case Utils.CharacterRotationMode.FollowVelocityHorizontal: // follow velocity - horizontal only
                Vector3 horizontalVelocity = Utils.GetHorizontal(velocity, false);
                if (horizontalVelocity.magnitude != 0) { newForward = horizontalVelocity; }
                break;

            case Utils.CharacterRotationMode.FollowVelocityReversed:
                Vector3 reverseVelocity = Utils.GetHorizontal(velocity * -1, false);
                if (reverseVelocity.magnitude != 0) { newForward = reverseVelocity; }
                break;
        }

        transform.LookAt(transform.position + newForward.normalized);
    }

    public void SetAnimatorState(int newState, string paramName = "")
    {
        string param = paramName == "" ? animStateName : paramName;
        if (animator != null && AnimatorHasParameter(animator, param))
        {
            animator.SetInteger(param, newState);
        }
    }

    public void SetAnimatorBool(bool newState, string paramName = "")
    {
        string param = paramName == "" ? animStateName : paramName;
        if (animator != null && AnimatorHasParameter(animator, param))
        {
            animator.SetBool(param, newState);
        }
    }

    public void SetAnimatorSpeed(float speed)
    {
        if (animator != null && AnimatorHasParameter(animator, animSpeedName))
        {
            animator.SetFloat(animSpeedName, speed);
        }
    }

    public void SendAnimatorTrigger(string triggerName)
    {
        if (animator != null && AnimatorHasParameter(animator, "Jump"))
        {
            animator.SetTrigger(triggerName);
        }
    }

    private static bool AnimatorHasParameter(Animator animator, string paramName)
    {
        foreach( AnimatorControllerParameter param in animator.parameters )
        {
            if (param.name == paramName) return true;
        }
        Debug.LogError("No Animator parameter named: " + paramName);
        return false;
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.includeLayers == LayerMask.NameToLayer("PropCollision"))
            Debug.Log("hello!");
    }
}
