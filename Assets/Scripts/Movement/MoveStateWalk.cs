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
        stateHandler.camOrbitController.SetYClamp(Utils.genericCamLock.x, Utils.genericCamLock.y, 1);

        Vector3 currentVelocity = stateHandler.velocity;
        if (!data.Contains(TransitionData.IgnoreVelocityCap))
        {
            stateHandler.CapSpeed(reference.maxVelocity);
        }
    }

    private void Update()
    {
        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, inputMapping, stateHandler.cameraObj);

        // -> ungrounded - allow coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // calculate velocities
        Vector3 oldVelocity = stateHandler.velocity;
        Vector3 newVelocity = ProcessMovement(wishDir, oldVelocity);
        float horizontalSpeed = Utils.GetHorizontal(newVelocity, false).magnitude;
        

        stateHandler.Move(newVelocity);
        stateHandler.SetAnimatorState(horizontalSpeed > 0.05? walkingAnimationState : idleAnimationState);
        stateHandler.SetAnimatorSpeed(horizontalSpeed * animationSpeedMultiplier);
        stateHandler.Rotate(reference.rotationMode);

        // -> sprint + jumping
        AttemptJump(reference, notGroundedState, jumpAnimationTrigger);
        AttemptSprint(reference, horizontalSpeed, onSprintState);
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
}
