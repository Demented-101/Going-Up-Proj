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
    [SerializeField] private GameObject model;
    [SerializeField] private Animator animator;
    [SerializeField] private string animStateName = "State";
    [SerializeField] private string animSpeedName = "Speed";
    [SerializeField] private bool printUpdates;
    
    public MovementState currentState { get; private set; }
    public Vector3 velocity { get; private set; } = Vector3.zero;

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

    public void ChangeState(MovementState newState, string[] data)
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

        Debug.Log(printUpdates);
        if (printUpdates) { Debug.Log("new state entered: " + newState); }

        // Disable old state and start new state.
        MovementState oldState = currentState;
        currentState = newState;

        oldState.enabled = false;
        oldState.onExit();

        newState.enabled = true;
        newState.onEntered(data);

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

    public void Rotate(Utils.CharacterRotationMode rotationMode)
    {
        if (model == null) { return; }
        Transform transform = model.transform;

        switch (rotationMode)
        {
            case Utils.CharacterRotationMode.None:
                break;

            case Utils.CharacterRotationMode.FollowVelocity:
                // follow velocity

                if (velocity.magnitude == 0) { break; }
                transform.LookAt(transform.position + velocity.normalized);

                break;

            case Utils.CharacterRotationMode.FollowVelocityHorizontal:
                // follow velocity - horizontally only

                Vector3 horizontalVelocity = velocity;
                horizontalVelocity.y = 0;
                if (horizontalVelocity.magnitude == 0) { break; }

                transform.LookAt(transform.position + horizontalVelocity.normalized);

                break;

            case Utils.CharacterRotationMode.FollowCamera:
                // follow main camera

                if (cameraObj == null) { Debug.LogError("No camera found - rotation cannot follow camera rot"); break; }
                Vector3 camForward = cameraObj.transform.forward;
                transform.LookAt(transform.position + camForward);
                
                break;
        }
    }

    public void SetAnimatorState(int newState)
    {
        if (animator != null && AnimatorHasParameter(animator, animStateName))
        {
            animator.SetInteger(animStateName, newState);
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

        return false;
    }
}
