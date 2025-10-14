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

    private const float sprintCamMinY = 17;
    private const float sprintCamMaxY = 17;

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
        float turning = GetMoveDirection(stateHandler.inputManager, Utils.InputMappingMode.None).x; // get the A or D input used for turning

        // -> midair state - make sure can do coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { Debug.Log("ungrounded"); stateHandler.ChangeState(notGroundedState); return; }

        // jumping -> midair state (is on ground or coyote time)
        if (stateHandler.inputManager.GetWishJump()) { Debug.Log("Jumped"); Jump(); return; }

        // -> walking
        if (walkState != null & !stateHandler.inputManager.GetWishSprint()) { Debug.Log("lost sprint"); stateHandler.ChangeState(walkState); return; }

        Vector3 oldVelocity = stateHandler.velocity;
        Vector3 newVelocity = ProcessMovement(oldVelocity);

        stateHandler.Move(newVelocity);
        stateHandler.SetAnimatorState(runningAnimationState);
        stateHandler.SetAnimatorSpeed(Utils.GetHorizontal(newVelocity, false).magnitude * animationSpeedMultiplier);
        stateHandler.Rotate(reference.rotationMode);
    }

    private Vector3 ProcessMovement(Vector3 velocity)
    {
        float currentSpeed = velocity.magnitude;
        Vector3 wishDir = Utils.GetHorizontal(stateHandler.cameraObj.transform.forward, true);

        // make sure a minimum speed is kept
        if (velocity.magnitude < reference.sprintSpeedRequirement) { velocity = wishDir * reference.sprintSpeedRequirement; }

        Vector3 targetVelocity = wishDir * reference.maxVelocity;
        velocity.x = Mathf.MoveTowards(velocity.x, targetVelocity.x, reference.acceleration * Time.deltaTime);
        velocity.z = Mathf.MoveTowards(velocity.z, targetVelocity.z, reference.acceleration * Time.deltaTime);

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
