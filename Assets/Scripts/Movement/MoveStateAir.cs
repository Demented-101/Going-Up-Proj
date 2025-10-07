using UnityEditor.SceneManagement;
using UnityEngine;

public class MoveStateAir : MovementState
{
    public MovementState onGroundedState;
    public bool mapMovementToCamera;
    private float coyoteTime;

    public override void onEntered()
    {
        coyoteTime = reference.coyoteTime;
    }

    private void Update()
    {
        if (coyoteTime > 0) 
        { 
            coyoteTime -= Time.deltaTime; 
            if (stateHandler.inputManager.GetWishJump()) { Jump(); }
        }

        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, mapMovementToCamera, GetCameraGameObject());

        // -> grounded state
        if (onGroundedState != null && stateHandler.controller.isGrounded) { stateHandler.ChangeState(onGroundedState); return; }

        Vector3 velocity = stateHandler.velocity;
        velocity.y -= reference.gravity * Time.deltaTime;
        stateHandler.Move(Accelerate(wishDir, velocity, reference));
    }

    private void Jump()
    {
        coyoteTime = 0;

        Vector3 newVelocity = stateHandler.velocity * reference.jumpLeapPower;
        newVelocity.y += reference.jumpImpulse;

        stateHandler.Move(newVelocity);
    }
}
