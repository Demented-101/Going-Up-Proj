using UnityEditor.SceneManagement;
using UnityEngine;

public class MoveStateSprint : MovementState
{
    public MovementState walkState;
    public bool mapMovementToCamera;
    private float coyoteTime;

    public int runningAnimationState = 1;
    public string startAnimationTrigger = "";
    public float animationSpeedMultiplier = 1.0f;
    public string jumpAnimationTrigger = "";

    public override void onEntered(string[] data)
    {
        base.onEntered(data);
        if (startAnimationTrigger != "") { stateHandler.SendAnimatorTrigger(startAnimationTrigger); }
    }

    private void Update()
    {
        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, mapMovementToCamera, GetCameraGameObject());

        Vector3 oldVelocity = stateHandler.velocity;
        Vector3 newVelocity = ProcessMovement(wishDir, oldVelocity);

        stateHandler.Move(newVelocity);
        stateHandler.SetAnimatorState(runningAnimationState);
        stateHandler.SetAnimatorSpeed(newVelocity.magnitude * animationSpeedMultiplier);
        stateHandler.Rotate(reference.rotationMode);
    }

    private Vector3 ProcessMovement(Vector3 wishDir, Vector3 velocity)
    {
        return Vector3.zero;
    }
}
