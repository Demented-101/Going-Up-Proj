using System.Linq;
using UnityEngine;

public class MoveStateAir : MovementState
{
    public MovementState onGroundedState;
    public MovementState onGroundedSprintState;
    public Utils.InputMappingMode inputMapping;
    private float coyoteTime;

    [SerializeField] private int fallAnimationState = -1;
    [SerializeField] private string jumpAnimationTrigger = "";

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);
        coyoteTime = data.Contains(TransitionData.IgnoreCoyoteTime) ? 0 : reference.coyoteTime;
    }

    private void Update()
    {
        // coyote time jumping
        if (coyoteTime > 0) coyoteTime -= Time.deltaTime;
        if (AttemptJump(reference, this, jumpAnimationTrigger, true)) coyoteTime = 0;

        // -> grounded state
        if (onGroundedState != null && stateHandler.controller.isGrounded) {
            bool sprinting = AttemptSprint(reference, stateHandler.velocity.magnitude, onGroundedSprintState); // attempt to go to sprint, if not continue to grounded
            if (!sprinting) stateHandler.ChangeState(onGroundedState);
            return; 
        }

        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, inputMapping, stateHandler.cameraObj);

        Vector3 velocity = stateHandler.velocity;
        velocity.y -= reference.gravity * Time.deltaTime;

        stateHandler.Move(Accelerate(wishDir, velocity, reference));
        stateHandler.Rotate(reference.characterRotationMode);
        if (coyoteTime <= 0) stateHandler.SetAnimatorState(fallAnimationState);
    }

    protected override bool AttemptJump(MovementStateReference jumpRef, MovementState airState, string animTrigger = "", bool ForceCamMode = false)
    {
        if (coyoteTime <= 0) { return false; }
        return base.AttemptJump(jumpRef, airState, animTrigger, ForceCamMode);
    }
}
