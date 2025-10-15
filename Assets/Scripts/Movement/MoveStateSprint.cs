using UnityEditor.SceneManagement;
using UnityEngine;

public class MoveStateSprint : MovementState
{
    public MovementState walkState;
    public MovementState notGroundedState;

    [SerializeField] private MovementStateReference turningReference;

    public int runningAnimationState = 1;
    public string startAnimationTrigger = "";
    public float animationSpeedMultiplier = 1.0f;
    public string jumpAnimationTrigger = "";

    private const float sprintCamMinY = 17;
    private const float sprintCamMaxY = 17;

    private bool isTurning = false;
    private Vector3 turnVector;
    private bool boost = false;
    private enum TurnDirections { right, left, backward };

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
        isTurning = false;
    }

    private void Update()
    {
        Vector3 turning = GetMoveDirection(stateHandler.inputManager, Utils.InputMappingMode.None); // get the A or D input used for turning

        // -> midair state - make sure can do coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { Debug.Log("ungrounded"); stateHandler.ChangeState(notGroundedState); return; }

        // jumping -> midair state (is on ground or coyote time)
        if (stateHandler.inputManager.GetWishJump()) { Debug.Log("Jumped"); Jump(); return; }

        // -> walking
        if (walkState != null & !stateHandler.inputManager.GetWishSprint()) { Debug.Log("lost sprint"); stateHandler.ChangeState(walkState); return; }

        if (!isTurning) { 
            if (turning.x > 0) { StartTurn(TurnDirections.right); } // turn right
            if (turning.x < 0) { StartTurn(TurnDirections.left); } // turn left
            if (turning.z < 0) { StartTurn(TurnDirections.backward); } // 180 turn
        }

        Vector3 oldVelocity = stateHandler.velocity;
        Vector3 newVelocity = ProcessMovement(oldVelocity);

        stateHandler.Move(newVelocity);
        stateHandler.SetAnimatorState(runningAnimationState);
        stateHandler.SetAnimatorSpeed(Utils.GetHorizontal(newVelocity, false).magnitude * animationSpeedMultiplier);
        stateHandler.Rotate(reference.rotationMode);
    }

    private Vector3 ProcessMovement(Vector3 velocity)
    {
        if (isTurning) { return ProcessMovementTurn(velocity); }

        Vector3 wishDir = Utils.GetHorizontal(stateHandler.cameraObj.transform.forward, true);

        // make sure a minimum speed is kept
        if (velocity.magnitude < reference.sprintSpeedRequirement) { velocity = wishDir * reference.sprintSpeedRequirement; }

        Vector3 targetVelocity = wishDir * reference.maxVelocity;
        float boostSpeed = boost ? 0.1f : 0;
        velocity.x = Mathf.MoveTowards(velocity.x, targetVelocity.x, (reference.acceleration + boostSpeed) * Time.deltaTime);
        velocity.z = Mathf.MoveTowards(velocity.z, targetVelocity.z, (reference.acceleration + boostSpeed) * Time.deltaTime);

        return velocity;
    }

    private Vector3 ProcessMovementTurn(Vector3 velocity)
    {
        float currentSpeed = Utils.GetHorizontal(velocity, false).magnitude;
        MovementStateReference realRef = turningReference == null ? reference : turningReference;

        if (currentSpeed != 0)
        {
            float control = Mathf.Max(realRef.stopSpeed, currentSpeed) * realRef.friction * Time.deltaTime;

            // scale the velocity based off calculated friction
            velocity *= Mathf.Max(currentSpeed - control, 0) / currentSpeed;
        }

        if (velocity.magnitude > realRef.sprintSpeedRequirement && Vector3.Dot(Utils.GetHorizontal(velocity, true), turnVector) >= 0.95) { EndTurn(); }

        velocity.y -= realRef.gravity;
        return Accelerate(turnVector, velocity, realRef);
    }

    private void StartTurn(TurnDirections direction)
    {
        isTurning = true;

        switch (direction)
        {
            case TurnDirections.right:
                turnVector = stateHandler.cameraObj.transform.right;
                break;
            case TurnDirections.left:
                turnVector = stateHandler.cameraObj.transform.right * -1;
                break;
            case TurnDirections.backward:
                turnVector = stateHandler.cameraObj.transform.forward * -1;
                break;
        }
    }

    private void EndTurn()
    {
        isTurning = false;
        boost = true;
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
