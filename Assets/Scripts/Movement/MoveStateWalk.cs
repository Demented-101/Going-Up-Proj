using Unity.VisualScripting;
using UnityEngine;

public class MoveStateWalk : MovementState
{
    public MovementState notGroundedState;
    public MovementState onSprintState;
    public bool mapMovementToCamera;

    private float coyoteTime;

    private void Update()
    {
        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, mapMovementToCamera, GetCameraGameObject());

        // -> midair state
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // jumping -> midair state (is on ground or coyote time)
        if (stateHandler.inputManager.GetWishJump()) { Jump(); return; }

        // -> sprint state
        if (onSprintState != null && stateHandler.inputManager.GetWishSprint()) {stateHandler.ChangeState(onSprintState); return; }

        Vector3 oldVelocity = stateHandler.velocity;
        stateHandler.Move(ProcessMovement(wishDir, oldVelocity));
    }

    private Vector3 ProcessMovement(Vector3 wishDir, Vector3 velocity)
    {
        float currentSpeed = velocity.magnitude;

        if (currentSpeed != 0)
        {
            float control = Mathf.Max(reference.stopSpeed, currentSpeed) * reference.friction * Time.deltaTime;

            // scale the velocity based off calculated friction
            velocity *= Mathf.Max(currentSpeed - control, 0) / currentSpeed;
        }

        return Accelerate(wishDir, velocity, reference);
    }

    private void Jump()
    {
        Vector3 newVelocity = stateHandler.velocity * reference.jumpLeapPower;
        newVelocity.y += reference.jumpImpulse;

        stateHandler.ChangeState(notGroundedState);
        stateHandler.Move(newVelocity);
    }
}
