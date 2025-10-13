using NUnit.Framework.Internal.Filters;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MoveStateWalk : MovementState
{
    public MovementState notGroundedState;
    public MovementState onSprintState;
    public Utils.InputMappingMode inputMapping;

    public int idleAnimationState = 0;
    public int walkingAnimationState = 1;
    public float animationSpeedMultiplier = 1.0f;
    public string jumpAnimationTrigger = "";

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);

        Vector3 currentVelocity = stateHandler.velocity;
        if (!data.Contains(TransitionData.IgnoreVelocityCap))
        {
            stateHandler.CapSpeed(reference.maxVelocity);
        }
    }

    private void Update()
    {
        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, inputMapping, GetCameraGameObject());

        // -> midair state - make sure can do coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // jumping -> midair state (is on ground or coyote time)
        if (stateHandler.inputManager.GetWishJump()) { Jump(); return; }

        // calculate velocities
        Vector3 oldVelocity = stateHandler.velocity;
        Vector3 newVelocity = ProcessMovement(wishDir, oldVelocity);
        float horizontalSpeed = new Vector3(newVelocity.x, 0, newVelocity.z).magnitude;
        
        // -> sprint state
        if (onSprintState != null && CanSprint(horizontalSpeed)) {stateHandler.ChangeState(onSprintState); return; }

        stateHandler.Move(newVelocity);
        stateHandler.SetAnimatorState(horizontalSpeed > 0.05? walkingAnimationState : idleAnimationState);
        stateHandler.SetAnimatorSpeed(horizontalSpeed * animationSpeedMultiplier);
        stateHandler.Rotate(reference.rotationMode);
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

        velocity.y -= reference.gravity;
        return Accelerate(wishDir, velocity, reference);
    }

    private void Jump()
    {

        Vector3 newVelocity = stateHandler.velocity * reference.jumpLeapPower;
        newVelocity.y = reference.jumpImpulse;

        stateHandler.ChangeState(notGroundedState, new TransitionData[] {TransitionData.IgnoreCoyoteTime}); // do not allow coyote time
        stateHandler.Move(newVelocity);
        if (jumpAnimationTrigger != "") { stateHandler.SendAnimatorTrigger(jumpAnimationTrigger); }
    }
}
