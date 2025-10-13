using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MoveStateAir : MovementState
{
    public MovementState onGroundedState;
    public Utils.InputMappingMode inputMapping;
    private float coyoteTime;

    [SerializeField] private int fallAnimationState = -1;
    [SerializeField] private string jumpAnimationTrigger = "";

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);
        coyoteTime = data.Contains(TransitionData.IgnoreCoyoteTime) ? 0 : reference.coyoteTime;
    }

    private void Update()
    {
        if (coyoteTime > 0) 
        { 
            coyoteTime -= Time.deltaTime; 
            if (stateHandler.inputManager.GetWishJump()) { Jump(); }
        }
        else
        {
            stateHandler.SetAnimatorState(fallAnimationState);
        }

        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, inputMapping, GetCameraGameObject());

        // -> grounded state
        if (onGroundedState != null && stateHandler.controller.isGrounded) {
            stateHandler.Move(new Vector3(stateHandler.velocity.x, 0, stateHandler.velocity.z)); // remove Y component for landing - improves jump physics
            stateHandler.ChangeState(onGroundedState); 
            return; 
        }

        Vector3 velocity = stateHandler.velocity;
        velocity.y -= reference.gravity * Time.deltaTime;

        stateHandler.Move(Accelerate(wishDir, velocity, reference));
        stateHandler.Rotate(reference.rotationMode);
    }

    private void Jump()
    {
        coyoteTime = 0;

        Vector3 newVelocity = stateHandler.velocity * reference.jumpLeapPower;
        newVelocity.y = reference.jumpImpulse;

        stateHandler.Move(newVelocity);
        if (jumpAnimationTrigger != "") { stateHandler.SendAnimatorTrigger(jumpAnimationTrigger); }
    }
}
