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

    public int currentMach { get; private set; } = 2;
    private Vector3 currentDirection;

    private bool isTurning = false;
    private int turnAnim = -1;
    private enum TurnDirections { right, left, backward };

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);
        isTurning = false;
        currentDirection = stateHandler.velocity;

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
        Vector3 input = GetMoveDirection(stateHandler.inputManager, Utils.InputMappingMode.ToCameraHorizontal, stateHandler.cameraObj);

        currentDirection = input.normalized * currentDirection.magnitude;
        Vector3 velocity = stateHandler.velocity;

        Debug.Log(isTurning);

        // -> midair state - make sure can do coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // -> turning + animation
        if (Vector3.Dot(input.normalized, Utils.GetHorizontal(velocity, true)) > 0.95f)
        {
            isTurning = false;
        }
        else
        {
            isTurning = true;

            Vector3 LeftVector = Vector3.Cross(velocity, Vector3.up).normalized;
            Vector3 RightVector = LeftVector * -1;

            if (Vector3.Dot(LeftVector, input) > Vector3.Dot(RightVector, input)) StartTurn(TurnDirections.left);  // turn left
            else StartTurn(TurnDirections.right); // turn right
        }
        
        Vector3 newVelocity = ProcessMovement(velocity, input);
        float horizontalSpeed = Utils.GetHorizontal(newVelocity, false).magnitude;

        stateHandler.Move(newVelocity);
        stateHandler.Rotate(GetMachRef().characterRotationMode);

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

    private void StartTurn(TurnDirections direction)
    {
        switch (direction)
        {
            case TurnDirections.right:
                turnAnim = turnRightAnimState;
                break;

            case TurnDirections.left:
                turnAnim = turnLeftAnimState;
                break;

            case TurnDirections.backward:
                turnAnim = turnBackAnimState;
                return;
        }
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
        return reference;
    }
}
