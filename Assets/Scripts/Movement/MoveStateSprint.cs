using System.Runtime.CompilerServices;
using Unity.Services.Matchmaker.Models;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public class MoveStateSprint : MovementState
{
    public MovementState walkState;
    public MovementState notGroundedState;

    [SerializeField] private MovementStateReference mach3Ref;
    [SerializeField] private MovementStateReference mach4Ref;
    [SerializeField] private MovementStateReference turningReference;
    [SerializeField] private float maxTurnDelay = 0.3f;
    [SerializeField] private int runningAnimationState = 2;
    [SerializeField] private string sprintingAnimStateName = "SprintState";
    [SerializeField] private float minSprintAnimSpeed = 1.5f;
    [SerializeField] private float animationSpeedMultiplier = 0.1f;
    [SerializeField] private string jumpAnimationTrigger = "";
    [SerializeField] private string startAnimationTrigger = "";

    private const int sprintAnimState = 1;
    private const int maxSprintAnimState = 2;
    private const int turnLeftAnimState = 3;
    private const int turnRightAnimState = 4;

    public int currentMach { get; private set; } = 2;

    private bool isTurning = false;
    private Vector3 turnVector;
    private float turnDelay = -1;
    private int turnAnim = -1;
    private enum TurnDirections { right, left, backward };

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);
        isTurning = false;
        
        // update animation
        stateHandler.SetAnimatorState(runningAnimationState);

        // update to correct mach
        float currentSpeed = Utils.GetHorizontal(stateHandler.velocity, false).magnitude;
        currentMach = 2; 
        AttemptSprint(mach3Ref, currentSpeed, this);
        AttemptSprint(mach4Ref, currentSpeed, this);
    }

    private void Update()
    {
        Vector3 turning = GetMoveDirection(stateHandler.inputManager, Utils.InputMappingMode.None); // get the A or D input used for turnings

        // -> midair state - make sure can do coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // -> turning + animation
        if (!isTurning) {
            if (turnDelay > 0) turnDelay -= Time.deltaTime;
            else {
                if (turning.x > 0) { StartTurn(TurnDirections.right); } // turn right
                if (turning.x < 0) { StartTurn(TurnDirections.left); } // turn left
            }
        }
        
        Vector3 oldVelocity = stateHandler.velocity;
        Vector3 newVelocity = ProcessMovement(oldVelocity);
        float horizontalSpeed = Utils.GetHorizontal(newVelocity, false).magnitude;

        stateHandler.Move(newVelocity);
        stateHandler.Rotate(GetMachRef().characterRotationMode);
        stateHandler.camOrbitController.UpdateFaceDirection(-Utils.GetHorizontal(newVelocity, true));

        // animation
        if (isTurning) stateHandler.SetAnimatorState(turnAnim, sprintingAnimStateName);
        else
        { // either max or normal sprint
            bool useMaxSprint = currentMach >= 4;
            stateHandler.SetAnimatorState(useMaxSprint ? maxSprintAnimState : sprintAnimState, sprintingAnimStateName);
            stateHandler.SetAnimatorSpeed((Utils.GetHorizontal(newVelocity, false).magnitude * animationSpeedMultiplier) + minSprintAnimSpeed);
        }

        // -> walking, jumping + next mach
        if (walkState != null & !stateHandler.inputManager.GetWishSprint()) { stateHandler.ChangeState(walkState, new TransitionData[] { TransitionData.IgnoreVelocityCap }); return; }
        AttemptJump(GetMachRef(), notGroundedState, jumpAnimationTrigger, true); // ensure the camera is always kept forward
        AttemptSprint(GetMachRef(currentMach + 1), horizontalSpeed, this);
    }

    private Vector3 ProcessMovement(Vector3 velocity)
    {
        if (isTurning) { return ProcessMovementTurn(velocity); }

        Vector3 wishDir = Utils.GetHorizontal(stateHandler.cameraObj.transform.forward, true);

        // make sure a minimum speed is kept
        if (velocity.magnitude < GetMachRef().sprintSpeedRequirement) { velocity = wishDir * GetMachRef().sprintSpeedRequirement; }

        velocity = Vector3.MoveTowards(velocity, wishDir * GetMachRef().maxVelocity, GetMachRef().acceleration * Time.deltaTime);
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

        if (velocity.magnitude > GetMachRef().sprintSpeedRequirement && Vector3.Dot(Utils.GetHorizontal(velocity, true), turnVector.normalized) >= 0.99f) // completed turn
        { 
            EndTurn();
            return (turnVector * Utils.GetHorizontal(velocity, false).magnitude) - (Vector3.up * realRef.gravity);
        }

        velocity.y = -realRef.gravity;
        return Accelerate(turnVector, velocity, realRef);
    }

    private void StartTurn(TurnDirections direction)
    {
        isTurning = true;

        switch (direction)
        {
            case TurnDirections.right:
                turnVector = stateHandler.cameraObj.transform.right;
                turnAnim = turnRightAnimState;
                break;

            case TurnDirections.left:
                turnVector = stateHandler.cameraObj.transform.right * -1;
                turnAnim = turnLeftAnimState;
                break;
        }
    }

    private void EndTurn()
    {
        isTurning = false;
        turnDelay = maxTurnDelay;

        // update to correct mach
        float currentSpeed = Utils.GetHorizontal(stateHandler.velocity, false).magnitude;
        currentMach = 2;
        AttemptSprint(mach3Ref, currentSpeed, this);
        AttemptSprint(mach4Ref, currentSpeed, this);
    }

    protected override bool AttemptSprint(MovementStateReference nextSprintRef, float speed, MovementState sprintState)
    {
        if (nextSprintRef == null) return false;
        if (speed < nextSprintRef.sprintSpeedRequirement && !isTurning) { return false; }

        StartSprint(sprintState);
        return true;
    }
    protected override void StartSprint(MovementState sprintState) { currentMach++; }

    private MovementStateReference GetMachRef(int mach = -1)
    {
        if (currentMach == -1) return reference;
        switch (mach)
        {
            case -1: return GetMachRef(currentMach);
            case 2: return reference;
            case 3: return mach3Ref;
            case 4: return mach4Ref;
        }
        return reference;
    }
}
