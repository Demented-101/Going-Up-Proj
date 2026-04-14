using System.Linq;
using UnityEngine;

public class MoveStateSprint : MovementState
{
    public MovementState walkState;
    public MovementState notGroundedState;
    public MovementState stumbleState;

    public SprintRefProvider sprintRef;
    [SerializeField] private int runningAnimationState = 2;
    [SerializeField] private string sprintingAnimStateName = "SprintState";
    [SerializeField] private float minSprintAnimSpeed = 1.5f;
    [SerializeField] private float animationSpeedMultiplier = 0.1f;
    [SerializeField] private string jumpAnimationTrigger = "";
    [SerializeField] private float maxDashTimer = 1f;
    [SerializeField] private float maxDashCooldown = 1f;

    private const int sprintAnimState = 1;
    private const int maxSprintAnimState = 2;
    private const int turnLeftAnimState = 3;
    private const int turnRightAnimState = 4;

    private const float bumpCastYOffset = 0.36f;
    private const float bumpCastDistance = 0.6f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldown = 0f;
    private Vector3 dashDirection;

    private int currentMach = 2;
    private Vector3 input;
    private bool isTurning = false;
    private int turnAnim = -1;
    private enum TurnDirections { right, left, backward };


    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);
        isTurning = false;
        isDashing = false;

        // update animation
        stateHandler.SetAnimatorState(runningAnimationState);

        // update to mach 2
        currentMach = 2;

        // attempt enter on mach 4
        if (data.Contains(TransitionData.StartMach4)){
            currentMach = 4;

            Vector3 newinput = GetMoveDirection(stateHandler.inputManager, Utils.InputMappingMode.ToCameraHorizontal, stateHandler.cameraObj);
            if (newinput.magnitude > 0)
            {
                // start dash to confirm mach 4
                StartDash(newinput);
                dashTimer = 0.3f;
            }
        }
    }

    private void Update()
    {
        Vector3 newinput = GetMoveDirection(stateHandler.inputManager, Utils.InputMappingMode.ToCameraHorizontal, stateHandler.cameraObj);
        Vector3 velocity = stateHandler.velocity;

        if (newinput.magnitude > 0) input = newinput; 

        // -> midair state + stumble + dash
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }
        if (stumbleState != null && ProcessBump(Utils.GetHorizontal(velocity, true))) {stateHandler.ChangeState(stumbleState); return; }
        if (stateHandler.inputManager.GetWishDash()) { 
            if (isDashing) { EndDash(); }
            else if (dashCooldown <= 0) { StartDash(velocity); }
        }

        Vector3 newVelocity;
        if (isDashing)
        {
            newVelocity = ProcessDash(velocity);
            isTurning = false;
        }
        else
        {
            if (dashCooldown >= 0) dashCooldown -= Time.deltaTime;
            ProcessTurning(input, velocity);
            newVelocity = ProcessMovement(velocity, input);
        }

        float horizontalSpeed = Utils.GetHorizontal(newVelocity, false).magnitude;

        stateHandler.Move(newVelocity);
        stateHandler.Rotate(GetMachRef().characterRotationMode);
        currentMach = ProcessMach(horizontalSpeed);

        // animation
        if (isTurning) stateHandler.SetAnimatorState(turnAnim, sprintingAnimStateName);
        else
        { // either dash or sprint anim
            stateHandler.SetAnimatorState(isDashing ? maxSprintAnimState : sprintAnimState, sprintingAnimStateName);
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
            float backDot = Vector3.Dot(input.normalized, Utils.GetHorizontal(velocity, true) * -1);
            float leftDot = Vector3.Dot(leftVector, input.normalized);
            float rightDot = Vector3.Dot(leftVector * -1, input.normalized);

            if (backDot > 0.85f) { stateHandler.ChangeState(stumbleState, new TransitionData[] {TransitionData.IgnoreStumbleTime}); }
            if (leftDot > 0f) { turnAnim = turnLeftAnimState; }
            if (rightDot > 0f) { turnAnim = turnRightAnimState; }
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
    
    private bool ProcessBump(Vector3 forward)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + new Vector3(0, bumpCastYOffset, 0), forward, out hit, bumpCastDistance);

        return hit.collider != null;
    }

    private Vector3 ProcessDash(Vector3 velocity)
    {
        dashTimer -= Time.deltaTime;
        if (dashTimer < 0) EndDash();

        return ProcessMovement(velocity, dashDirection);
    }

    private void StartDash(Vector3 velocity)
    {
        Debug.Log("Dash started");
        isDashing = true;
        dashTimer = maxDashTimer;
        stateHandler.SendAnimatorTrigger("Dash");
        dashDirection = velocity.normalized;
    }

    private void EndDash()
    {
        Debug.Log("Dash ended");
        isDashing = false;
        dashTimer = 0;
        dashCooldown = maxDashCooldown;
    }

    protected override bool AttemptSprint(MovementStateReference nextSprintRef, float speed, MovementState sprintState)
    {
        if (nextSprintRef == null) return false;
        if (speed < nextSprintRef.sprintSpeedRequirement && !isTurning) { return false; }

        StartSprint(sprintState);
        return true;
    }
    protected override void StartSprint(MovementState sprintState) { currentMach++; }

    private MovementStateReference GetMachRef(int mach = -1) // uses the default "reference" if one isnt selected, otherwise selects from the sprintRef
    {
        if (isDashing) return sprintRef.Dash == null ? reference : sprintRef.Dash;
        if (isTurning) return sprintRef.Turning == null ? reference : sprintRef.Turning;
        if (currentMach == -1) return reference;

        switch (mach)
        {
            case -1: return GetMachRef(currentMach);
            case 2: return sprintRef.Mach2;
            case 3: return sprintRef.Mach3;
            case 4: return sprintRef.Mach4;
        }
        return reference;
    }

    public int GetCurrentMach()
    {
        if (isActiveAndEnabled) 
        { 
            if (isDashing) { return 4; }
            return currentMach; 
        }
        return 1;
    }
}
