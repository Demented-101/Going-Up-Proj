using UnityEditor.SceneManagement;
using UnityEngine;

public class MoveStateAir : MovementState
{
    public MovementState onGroundedState;
    public bool mapMovementToCamera;

    private void Update()
    {
        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, mapMovementToCamera, GetCameraGameObject());

        // -> grounded state
        if (onGroundedState != null && stateHandler.controller.isGrounded) { stateHandler.ChangeState(onGroundedState); return; }

        Vector3 velocity = stateHandler.velocity;
        velocity.y -= reference.gravity * Time.deltaTime;
        stateHandler.Move(Accelerate(wishDir, velocity, reference));
    }

}
