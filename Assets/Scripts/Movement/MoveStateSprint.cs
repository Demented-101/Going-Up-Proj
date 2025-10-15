using Unity.Services.Matchmaker.Models;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MoveStateSprint : MovementState
{
    public MovementState walkState;
    public MovementState notGroundedState;

    [SerializeField] private MovementStateReference mach3Ref;
    [SerializeField] private MovementStateReference mach4Ref;
    [SerializeField] private MovementStateReference turningReference;
    [SerializeField] private int runningAnimationState = 1;
    [SerializeField] private string startAnimationTrigger = "";
    [SerializeField] private float animationSpeedMultiplier = 1.0f;
    [SerializeField] private string jumpAnimationTrigger = "";

    private const float sprintCamMinY = 17;
    private const float sprintCamMaxY = 17;

    private bool isTurning = false;
    private Vector3 turnVector;
    private bool boost = false;
    private int currentMach = 2;
    private enum TurnDirections { right, left, backward };

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);
        if (startAnimationTrigger != "") { stateHandler.SendAnimatorTrigger(startAnimationTrigger); }
        if (stateHandler.camOrbitController != null) { stateHandler.camOrbitController.SetYClamp(sprintCamMinY, sprintCamMaxY, 1); }

        // update to correct mach
        currentMach = 2;
        float currentSpeed = Utils.GetHorizontal(stateHandler.velocity, false).magnitude;
        AttemptSprint(mach3Ref, currentSpeed, this);
        AttemptSprint(mach4Ref, currentSpeed, this);
    }

    public override void onExit()
    {
        base.onExit();
        if (stateHandler.camOrbitController != null) { stateHandler.camOrbitController.SetYClamp(-50, 50, 1); }
        isTurning = false;
    }

    private void Update()
    {
        Vector3 turning = GetMoveDirection(stateHandler.inputManager, Utils.InputMappingMode.None); // get the A or D input used for turnings
        bool wishNextMach = turning.z > 0;

        // -> midair state - make sure can do coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // -> turning
        if (!isTurning) { 
            if (turning.x > 0) { StartTurn(TurnDirections.right); } // turn right
            if (turning.x < 0) { StartTurn(TurnDirections.left); } // turn left
        }

        Vector3 oldVelocity = stateHandler.velocity;
        Vector3 newVelocity = ProcessMovement(oldVelocity);
        float horizontalSpeed = Utils.GetHorizontal(newVelocity, false).magnitude;

        stateHandler.Move(newVelocity);
        stateHandler.SetAnimatorState(runningAnimationState);
        stateHandler.SetAnimatorSpeed(Utils.GetHorizontal(newVelocity, false).magnitude * animationSpeedMultiplier);
        stateHandler.Rotate(GetMachRef().rotationMode);

        // -> walking, jumping + next mach
        if (walkState != null & !stateHandler.inputManager.GetWishSprint()) { stateHandler.ChangeState(walkState); return; }
        AttemptJump(GetMachRef(), notGroundedState, jumpAnimationTrigger);
        AttemptSprint(GetMachRef(currentMach + 1), horizontalSpeed, this);
    }

    private Vector3 ProcessMovement(Vector3 velocity)
    {
        if (isTurning) { return ProcessMovementTurn(velocity); }

        Vector3 wishDir = Utils.GetHorizontal(stateHandler.cameraObj.transform.forward, true);

        // make sure a minimum speed is kept
        if (velocity.magnitude < GetMachRef().sprintSpeedRequirement) { velocity = wishDir * GetMachRef().sprintSpeedRequirement; }

        Vector3 targetVelocity = wishDir * GetMachRef().maxVelocity;
        float acceleration = GetMachRef().acceleration;
        if (boost) { acceleration += GetMachRef().acceleration * 1.3f; boost = false; }

        velocity.x = Mathf.MoveTowards(velocity.x, targetVelocity.x, acceleration * Time.deltaTime);
        velocity.z = Mathf.MoveTowards(velocity.z, targetVelocity.z, acceleration * Time.deltaTime);
        velocity.y = -GetMachRef().gravity;

        return velocity;
    }

    private Vector3 ProcessMovementTurn(Vector3 velocity)
    {
        float currentSpeed = Utils.GetHorizontal(velocity, false).magnitude;
        MovementStateReference realRef = turningReference == null ? GetMachRef() : turningReference;

        if (currentSpeed != 0)
        {
            float control = Mathf.Max(realRef.stopSpeed, currentSpeed) * realRef.friction * Time.deltaTime;

            // scale the velocity based off calculated friction
            velocity *= Mathf.Max(currentSpeed - control, 0) / currentSpeed;
        }

        // have completed rotation
        if (velocity.magnitude > realRef.sprintSpeedRequirement && Vector3.Dot(Utils.GetHorizontal(velocity, true), turnVector) >= 0.95) { EndTurn(); }

        // make sure minimum speed is kept
        if (velocity.magnitude < realRef.sprintSpeedRequirement) { velocity += turnVector * realRef.sprintSpeedRequirement; }

        velocity.y = -realRef.gravity;
        return Accelerate(turnVector, velocity, realRef);
    }

    private void StartTurn(TurnDirections direction)
    {
        isTurning = true;
        float camRotSpeed = 0.75f;

        switch (direction)
        {
            case TurnDirections.right:
                turnVector = stateHandler.cameraObj.transform.right;
                stateHandler.camOrbitController.StartHorzRotation(90, camRotSpeed);
                break;
            case TurnDirections.left:
                turnVector = stateHandler.cameraObj.transform.right * -1;
                stateHandler.camOrbitController.StartHorzRotation(-90, camRotSpeed);
                break;
            case TurnDirections.backward:
                turnVector = stateHandler.cameraObj.transform.forward * -1;
                stateHandler.camOrbitController.StartHorzRotation(180, camRotSpeed);
                break;
        }
    }

    private void EndTurn()
    {
        isTurning = false;
        boost = true;
    }

    protected override bool AttemptSprint(MovementStateReference nextSprintRef, float speed, MovementState sprintState)
    {
        if (nextSprintRef == null) return false;
        if (speed < nextSprintRef.sprintSpeedRequirement) { return false; }

        StartSprint(sprintState);
        currentMach++;
        return true;
    }

    private MovementStateReference GetMachRef(int mach = -1)
    {
        switch (mach)
        {
            case -1: return GetMachRef(currentMach);
            case 2: return reference;
            case 3: return mach3Ref;
            case 4: return mach4Ref;
        }
        return null;
    }
}
