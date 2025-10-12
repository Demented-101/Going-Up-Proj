using UnityEditor.SceneManagement;
using UnityEngine;

public class MoveStateAir : MovementState
{
    public MovementState onGroundedState;
    public bool mapMovementToCamera;
    private float coyoteTime;

    public int fallAnimationState; // if character has high downward vertical velocity
    public int leapAnimationState; // if character has high horizontal velocity

    public override void onEntered()
    {
        base.onEntered();
        coyoteTime = reference.coyoteTime;
    }

    private void Update()
    {
        if (coyoteTime > 0) 
        { 
            coyoteTime -= Time.deltaTime; 
            if (stateHandler.inputManager.GetWishJump()) { Jump(); }
        }
        else
        {
            stateHandler.SetAnimatorState(fallAnimationState);
        }

        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, mapMovementToCamera, GetCameraGameObject());

        // -> grounded state
        if (onGroundedState != null && stateHandler.controller.isGrounded) { stateHandler.ChangeState(onGroundedState); return; }

        Vector3 velocity = stateHandler.velocity;
        velocity.y -= reference.gravity * Time.deltaTime;

        stateHandler.Move(Accelerate(wishDir, velocity, reference));
        stateHandler.Rotate(reference.rotationMode);
    }

    private void Jump()
    {
        coyoteTime = 0;

        Vector3 newVelocity = stateHandler.velocity * reference.jumpLeapPower;
        newVelocity.y += reference.jumpImpulse;

        stateHandler.Move(newVelocity);
        stateHandler.SetAnimatorState(leapAnimationState);
    }
}
