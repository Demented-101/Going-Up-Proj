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
        if (coyoteTime > 0)
        { 
            coyoteTime -= Time.deltaTime;
            AttemptJump(reference, this, jumpAnimationTrigger);
        }
        else stateHandler.SetAnimatorState(fallAnimationState);
        
        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, inputMapping, stateHandler.cameraObj);

        // -> grounded state
        if (onGroundedState != null && stateHandler.controller.isGrounded) {
            stateHandler.Move(Utils.GetHorizontal(stateHandler.velocity, false)); // remove Y component for landing - improves jump physics
            
            if (!AttemptSprint(reference, stateHandler.velocity.magnitude, onGroundedSprintState)) // attempt to go to sprint, if not continue to grounded
            { 
                stateHandler.ChangeState(onGroundedState); 
            }
            return; 
        }

        Vector3 velocity = stateHandler.velocity;
        velocity.y -= reference.gravity * Time.deltaTime;

        stateHandler.Move(Accelerate(wishDir, velocity, reference));
        stateHandler.Rotate(reference.characterRotationMode);
    }

    protected override void Jump(MovementStateReference jumpRef, MovementState airState, string animTrigger)
    {
        base.Jump(jumpRef, null, animTrigger);
        coyoteTime = 0;
    }
}
