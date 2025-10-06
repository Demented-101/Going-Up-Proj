using UnityEngine;

public class MoveStateWalk : MovementState
{
    public MovementState notGroundedState;
    public MovementState onSprintState;
    public bool mapMovementToCamera;

    private Vector3 velocity;

    private void Update()
    {
        Debug.Log(velocity);
        // get input direction
        Vector3 wishDir = GetMoveDirection(inputManager, mapMovementToCamera, camObj);

        // -> midair state
        if (notGroundedState != null && !characterController.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // -> sprint state
        if (onSprintState != null && inputManager.GetIsSprinting()) {stateHandler.ChangeState(onSprintState); return; }

        float currentSpeed = velocity.magnitude;

        if (currentSpeed != 0)
        {
            float control = Mathf.Max(reference.stopSpeed, currentSpeed) * reference.friction * Time.deltaTime;

            // scale the velocity based off calculated friction
            velocity *= Mathf.Max(currentSpeed - control, 0) / currentSpeed;
        }

        Debug.Log(Accelerate(wishDir, velocity, reference));
        velocity = Accelerate(wishDir, velocity, reference);
        Debug.Log(velocity);
        characterController.Move(velocity);
    }
}
