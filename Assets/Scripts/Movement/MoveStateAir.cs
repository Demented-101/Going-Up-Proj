using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MoveStateAir : MovementState
{
    public MovementState onGroundedState;
    public bool mapMovementToCamera;
    private float coyoteTime;

    [SerializeField] private int fallAnimationState = -1;
    [SerializeField] private string jumpAnimationTrigger = "";

    public override void onEntered(string[] data)
    {
        base.onEntered(data);
        coyoteTime = data.Contains("do Coyote Time") ? reference.coyoteTime : 0;
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
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, mapMovementToCamera, GetCameraGameObject());

        // -> grounded state
        if (onGroundedState != null && stateHandler.controller.isGrounded) {
            stateHandler.Move(stateHandler.velocity + new Vector3(1, 0, 1)); // remove Y component for landing - improves jump physics
            stateHandler.ChangeState(onGroundedState, new string[0]); 
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
