using System.Runtime.CompilerServices;
using Unity.Services.Matchmaker.Models;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class MoveStateSprint : MovementState
{
    public MovementState walkState;
    public MovementState notGroundedState;

    [SerializeField] private MovementStateReference mach3Ref;
    [SerializeField] private MovementStateReference mach4Ref;
    [SerializeField] private MovementStateReference turningReference;
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
    private const int turnBackAnimState = 4;

    private int currentMach = 2;
    private Vector3 input;
    private bool isTurning = false;
    private int turnAnim = -1;
    private enum TurnDirections { right, left, backward };

    // TODO:
    // new 180 animation
    // bonking

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);
        isTurning = false;

        // update animation
        stateHandler.SetAnimatorState(runningAnimationState);

        // update to correct mach
        float currentSpeed = Utils.GetHorizontal(stateHandler.velocity, false).magnitude;
        currentMach = 2;
    }

    private void Update()
    {
        Vector3 newinput = GetMoveDirection(stateHandler.inputManager, Utils.InputMappingMode.ToCameraHorizontal, stateHandler.cameraObj);
        Vector3 velocity = stateHandler.velocity;

        if (newinput.magnitude > 0) input = newinput; 

        // -> midair state - make sure can do coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // -> turning + animation
        ProcessTurning(input, velocity);
        
        Vector3 newVelocity = ProcessMovement(velocity, input);
        float horizontalSpeed = Utils.GetHorizontal(newVelocity, false).magnitude;

        stateHandler.Move(newVelocity);
        stateHandler.Rotate(GetMachRef().characterRotationMode);
        currentMach = ProcessMach(horizontalSpeed);

        // animation
        if (isTurning) stateHandler.SetAnimatorState(turnAnim, sprintingAnimStateName);
        else
        { // either max or normal sprint
            bool useMaxSprint = currentMach >= 4;
            stateHandler.SetAnimatorState(useMaxSprint ? maxSprintAnimState : sprintAnimState, sprintingAnimStateName);
            stateHandler.SetAnimatorSpeed((Utils.GetHorizontal(newVelocity, false).magnitude * animationSpeedMultiplier) + minSprintAnimSpeed);
        }

        // -> walking, jumping
        if (walkState != null & !stateHandler.inputManager.GetWishSprint()) { stateHandler.ChangeState(walkState, new TransitionData[] { TransitionData.IgnoreVelocityCap }); return; }
        AttemptJump(GetMachRef(), notGroundedState, jumpAnimationTrigger, true);
    }

    private Vector3 ProcessMovement(Vector3 velocity, Vector3 wishDir)
    {
        float currentSpeed = velocity.magnitude;

        if (currentSpeed != 0)
        {
            float control = Mathf.Max(GetMachRef().stopSpeed, currentSpeed) * GetMachRef().friction * Time.deltaTime;

            // scale the velocity based off calculated control value
            velocity *= Mathf.Max(currentSpeed - control, 0) / currentSpeed;
        }

        velocity.y = -GetMachRef().gravity;
        return Accelerate(wishDir, velocity, GetMachRef());
    }

    private void ProcessTurning(Vector3 input, Vector3 velocity)
    {
        isTurning = Vector3.Dot(Utils.GetHorizontal(velocity, true), input.normalized) < 0.98f;

        if (isTurning)
        {
            Vector3 leftVector = Vector3.Cross(velocity, Vector3.up).normalized;
            float leftDot = Vector3.Dot(leftVector, input.normalized);
            float rightDot = Vector3.Dot(leftVector * -1, input.normalized);

            if (Mathf.Abs(leftDot - rightDot) < 0.05f) turnAnim = turnBackAnimState;
            if (leftDot > 0) turnAnim = turnLeftAnimState;
            if (rightDot > 0) turnAnim = turnRightAnimState;
        }
    }

    private int ProcessMach(float speed)
    {
        // mach up
        if (currentMach < 4 && !isTurning)
        {
            MovementStateReference nextMach = GetMachRef(currentMach + 1);
            if (speed > nextMach.sprintSpeedRequirement) { return currentMach + 1; }
        }

        // mach down
        if (currentMach > 2)
        {
            if (speed < GetMachRef().sprintSpeedRequirement) { return currentMach - 1; }
        }

        return currentMach;
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
        if (isTurning) return turningReference == null ? reference : turningReference;
        if (currentMach == -1) return reference;
        switch (mach)
        {
            case -1: return GetMachRef(currentMach);
            case 2: return reference;
            case 3: return mach3Ref;
            case 4: return mach4Ref;
        }
        return null;
    }

    public int GetCurrentMach()
    {
        if (isActiveAndEnabled) { return currentMach; }
        return 1;
    }
}
