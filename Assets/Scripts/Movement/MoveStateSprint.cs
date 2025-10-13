using UnityEditor.SceneManagement;
using UnityEngine;

public class MoveStateSprint : MovementState
{
    public MovementState walkState;
    public MovementState notGroundedState;

    public int runningAnimationState = 1;
    public string startAnimationTrigger = "";
    public float animationSpeedMultiplier = 1.0f;
    public string jumpAnimationTrigger = "";

    private const float sprintCamMinY = 15;
    private const float sprintCamMaxY = 20;

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);
        if (startAnimationTrigger != "") { stateHandler.SendAnimatorTrigger(startAnimationTrigger); }
        if (stateHandler.camOrbitController != null) { stateHandler.camOrbitController.SetYClamp(sprintCamMinY, sprintCamMaxY, 1); }
    }

    public override void onExit()
    {
        base.onExit();
        if (stateHandler.camOrbitController != null) { stateHandler.camOrbitController.SetYClamp(-50, 50, 1); }
    }

    private void Update()
    {
        float turning = GetMoveDirection(stateHandler.inputManager, Utils.InputMappingMode.None).x;

        // -> midair state - make sure can do coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { Debug.Log("ungrounded"); stateHandler.ChangeState(notGroundedState); return; }

        // jumping -> midair state (is on ground or coyote time)
        if (stateHandler.inputManager.GetWishJump()) { Debug.Log("Jumped"); Jump(); return; }

        Vector3 oldVelocity = stateHandler.velocity;
        Vector3 newVelocity = ProcessMovement(oldVelocity);
        float horizontalSpeed = new Vector3(newVelocity.x, 0, newVelocity.z).magnitude;

        // -> walking (either sprint is released or max speed isnt met)
        if (walkState != null & !stateHandler.inputManager.GetWishSprint()) { Debug.Log("lost sprint"); stateHandler.ChangeState(walkState); return; }

        stateHandler.Move(newVelocity);
        stateHandler.SetAnimatorState(runningAnimationState);
        stateHandler.SetAnimatorSpeed(horizontalSpeed * animationSpeedMultiplier);
        stateHandler.Rotate(reference.rotationMode);
    }

    private Vector3 ProcessMovement(Vector3 velocity)
    {
        float currentSpeed = velocity.magnitude;
        Vector3 wishDir = stateHandler.cameraObj.transform.forward;
        wishDir.y = 0;

        velocity = Accelerate(wishDir.normalized, velocity, reference);
        if (velocity.magnitude < reference.sprintSpeedRequirement) { velocity = wishDir.normalized * reference.sprintSpeedRequirement; }
        if (velocity.magnitude > reference.maxVelocity) { stateHandler.CapSpeed(reference.maxVelocity); }

        velocity.y -= reference.gravity;
        return velocity;
    }

    private void Jump()
    {
        Debug.Log("JUMP");

        Vector3 newVelocity = stateHandler.velocity * reference.jumpLeapPower;
        newVelocity.y = reference.jumpImpulse;

        stateHandler.ChangeState(notGroundedState, new TransitionData[] { TransitionData.IgnoreCoyoteTime }); // do not allow coyote time
        stateHandler.Move(newVelocity);
        if (jumpAnimationTrigger != "") { stateHandler.SendAnimatorTrigger(jumpAnimationTrigger); }
    }
}
