using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MovementStateHandler))]
public abstract class MovementState : MonoBehaviour
{
    public enum TransitionData
    {
        Force, IgnoreVelocityCap, IgnoreCoyoteTime, KeepCamRotationMode, IgnoreStumbleTime
    }

    [SerializeField] public MovementStateReference reference;
    public MovementStateHandler stateHandler { get; private set; }

    private void Start()
    {
        stateHandler = GetComponent<MovementStateHandler>();
    }

    public virtual void onEntered(TransitionData[] data) 
    {
        // get state handler
        if (stateHandler == null) { stateHandler = GetComponent<MovementStateHandler>(); }
    }

    public virtual void onExit() { }

    public static Vector3 GetMoveDirection(InputManager inputManager, Utils.InputMappingMode mapping, GameObject cam = null)
    {
        Vector3 wishDir = Vector3.zero;
        Vector2 input = inputManager.GetCurrentInput();

        switch (mapping)
        {
            case Utils.InputMappingMode.None: // input -> wish movement
                wishDir = new Vector3(input.x, 0, input.y);
                break;

            case Utils.InputMappingMode.ToCamera: //input -> camera rotation
                Vector3 camForward = cam.transform.forward;
                Vector3 camRight = cam.transform.right;

                wishDir += camForward.normalized * input.y;
                wishDir += camRight.normalized * input.x;
                break;

            case Utils.InputMappingMode.ToCameraHorizontal: //input -> camera rotation (remove Y)
                Vector3 camForwardHorz = Utils.GetHorizontal(cam.transform.forward, true);
                Vector3 camRightHorz = Utils.GetHorizontal(cam.transform.right, true); ;

                wishDir += camForwardHorz.normalized * input.y;
                wishDir += camRightHorz.normalized * input.x;
                break;
        }

        return wishDir.normalized;
    }

    protected static Vector3 Accelerate(Vector3 wishDirection, Vector3 currentVelocity, MovementStateReference reference)
    {
        float delta = Time.deltaTime;

        // how much we speed up/slow down is dependent on the difference between the wish and current velocity
        float currentSpeed = Vector3.Dot(currentVelocity, wishDirection);
        float addSpeed = Mathf.Clamp(reference.maxVelocity - currentSpeed, 0, reference.acceleration * delta); // clamped to stop the player from going too fast
        
        return currentVelocity + (wishDirection * addSpeed);
    }

    // jumping
    protected virtual bool AttemptJump(MovementStateReference jumpRef, MovementState airState, string animTrigger = "", bool ForceCamMode = false)
    {
        if (airState == null || !stateHandler.inputManager.GetWishJump()) { return false; } // can jump + wish jump
        if (jumpRef.jumpImpulse <= -1) { return false; } // reference allows jumping

        Jump(jumpRef, airState, animTrigger, ForceCamMode);
        return true;
    }
    protected virtual void Jump(MovementStateReference jumpRef, MovementState airState, string animTrigger, bool ForceCamMode = false)
    {
        // use passed reference
        Vector3 newVelocity = stateHandler.velocity * jumpRef.jumpLeapPower;
        newVelocity.y = jumpRef.jumpImpulse;

        stateHandler.Move(newVelocity);
        if (animTrigger != "") { stateHandler.SendAnimatorTrigger(animTrigger); }

        if (airState != this) 
        {
            TransitionData[] transData = { TransitionData.IgnoreCoyoteTime };
            if (ForceCamMode) { transData = new TransitionData[] { TransitionData.IgnoreCoyoteTime, TransitionData.KeepCamRotationMode }; }

            stateHandler.ChangeState(airState, transData);
        }
    }
    
    // sprinting
    protected virtual bool AttemptSprint(MovementStateReference sprintRef, float speed, MovementState sprintState)
    {
        if (sprintState == null || !stateHandler.inputManager.GetWishSprint()) { return false; }
        if (sprintRef.sprintSpeedRequirement > speed) { return false; }

        StartSprint(sprintState);
        return true;
    }
    protected virtual void StartSprint(MovementState sprintState)
    {
       stateHandler.ChangeState(sprintState);
    }
}
