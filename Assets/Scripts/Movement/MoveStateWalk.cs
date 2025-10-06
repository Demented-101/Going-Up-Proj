using Unity.VisualScripting;
using UnityEngine;

public class MoveStateWalk : MovementState
{
    public MovementState notGroundedState;
    public MovementState onSprintState;
    public bool mapMovementToCamera;

    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        // get input direction
        Vector3 wishDir = GetMoveDirection(GetInputManager(), mapMovementToCamera, GetCameraGameObject());

        // -> midair state
        if (notGroundedState != null && !GetCharacter().isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // -> sprint state
        if (onSprintState != null && GetInputManager().GetIsSprinting()) {stateHandler.ChangeState(onSprintState); return; }

        float currentSpeed = velocity.magnitude;

        if (currentSpeed != 0)
        {
            float control = Mathf.Max(reference.stopSpeed, currentSpeed) * reference.friction * Time.deltaTime;

            // scale the velocity based off calculated friction
            velocity *= Mathf.Max(currentSpeed - control, 0) / currentSpeed;
        }

        velocity = Accelerate(wishDir, velocity, reference);
        GetCharacter().Move(velocity);
    }
}
