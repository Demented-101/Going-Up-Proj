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
    [SerializeField] private string boostAnimationTrigger = "";

    private const int sprintAnimState = 1;
    private const int maxSprintAnimState = 2;
    private const int turnLeftAnimState = 3;
    private const int turnRightAnimState = 4;
    private const float boostSpeed = 1.3f;

    public int currentMach { get; private set; } = 2;

    private bool isTurning = false;
    private Vector3 turnVector;
    private float turnDelay = -1;
    private enum TurnDirections { right, left, backward };

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);

        // update to correct mach
        currentMach = 2;


        float currentSpeed = Utils.GetHorizontal(stateHandler.velocity, false).magnitude;
        AttemptSprint(mach3Ref, currentSpeed, this);
        AttemptSprint(mach4Ref, currentSpeed, this);
    }

    public override void onExit()
    {
        base.onExit();
        isTurning = false;
    }

    private void Update()
    {
        Vector3 turning = GetMoveDirection(stateHandler.inputManager, Utils.InputMappingMode.None); // get the A or D input used for turnings

        // -> midair state - make sure can do coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // -> turning
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
        stateHandler.SetAnimatorState(runningAnimationState);
        stateHandler.SetAnimatorSpeed((Utils.GetHorizontal(newVelocity, false).magnitude * animationSpeedMultiplier) + minSprintAnimSpeed);
        stateHandler.Rotate(GetMachRef().characterRotationMode);
        stateHandler.camOrbitController.UpdateFaceDirection(-Utils.GetHorizontal(newVelocity, true));

        // sprint animation
        if (!isTurning) 
        {
            if (currentMach >= 4) stateHandler.SetAnimatorState(maxSprintAnimState, sprintingAnimStateName);
            else stateHandler.SetAnimatorState(sprintAnimState, sprintingAnimStateName);
        }

        // -> walking, jumping + next mach
        if (walkState != null & !stateHandler.inputManager.GetWishSprint()) { stateHandler.ChangeState(walkState, new TransitionData[] {TransitionData.IgnoreVelocityCap}); return; }
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

        velocity = Vector3.MoveTowards(velocity, targetVelocity, acceleration * Time.deltaTime);
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


        // make sure minimum speed is kept
        if (velocity.magnitude < realRef.sprintSpeedRequirement) { velocity += turnVector * realRef.sprintSpeedRequirement; }

        // have completed rotation
        if (velocity.magnitude > realRef.sprintSpeedRequirement && Vector3.Dot(Utils.GetHorizontal(velocity, true), turnVector.normalized) >= 0.99) 
        { 
            EndTurn();
            stateHandler.SendAnimatorTrigger(boostAnimationTrigger);
            return (turnVector * velocity.magnitude) - (Vector3.up * realRef.gravity);
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
                stateHandler.SetAnimatorState(turnRightAnimState, sprintingAnimStateName);
                break;
            case TurnDirections.left:
                turnVector = stateHandler.cameraObj.transform.right * -1;
                stateHandler.SetAnimatorState(turnLeftAnimState, sprintingAnimStateName);
                break;
            case TurnDirections.backward:
                turnVector = stateHandler.cameraObj.transform.forward * -1;
                stateHandler.SetAnimatorState(turnLeftAnimState, sprintingAnimStateName);
                break;
        }
    }

    private void EndTurn()
    {
        isTurning = false;
        turnDelay = maxTurnDelay;
        if (currentMach == 4) currentMach = 3;
        stateHandler.SetAnimatorState(sprintAnimState, sprintingAnimStateName);
    }

    protected override bool AttemptSprint(MovementStateReference nextSprintRef, float speed, MovementState sprintState)
    {
        if (nextSprintRef == null) return false;
        if (speed < nextSprintRef.sprintSpeedRequirement && !isTurning) { return false; }

        StartSprint(sprintState);
        return true;
    }

    protected override void StartSprint(MovementState sprintState)
    {
        currentMach++;
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
