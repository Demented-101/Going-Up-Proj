using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MoveStateWalk : MovementState
{
    [SerializeField] private MovementState notGroundedState;
    [SerializeField] private MovementState onSprintState;
    [SerializeField] private Utils.InputMappingMode inputMapping;

    [SerializeField] private int idleAnimationState = 0;
    [SerializeField] private int walkingAnimationState = 1;
    [SerializeField] private float animationSpeedMultiplier = 1.0f;
    [SerializeField] private string jumpAnimationTrigger = "";

    public bool isHitting { get; private set; }
    private float hitTimer;
    private float hitDelay;

    public Action onHit;

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);
        if (!data.Contains(TransitionData.IgnoreVelocityCap)) stateHandler.CapSpeed(reference.maxVelocity);

        isHitting = false;
    }

    private void Update()
    {
        // get input direction
        Vector3 wishDir = GetMoveDirection(stateHandler.inputManager, inputMapping, stateHandler.cameraObj);

        // -> ungrounded - allow coyote time
        if (notGroundedState != null && !stateHandler.controller.isGrounded) { stateHandler.ChangeState(notGroundedState); return; }

        // -> attack/hit
        if (stateHandler.inputManager.GetWishDash()) { Hit(); }
        HandleHit();

        // calculate velocities
        Vector3 oldVelocity = stateHandler.velocity;
        Vector3 newVelocity = ProcessMovement(wishDir, oldVelocity);
        float horizontalSpeed = Utils.GetHorizontal(newVelocity, false).magnitude;

        
        stateHandler.Move(newVelocity);
        stateHandler.SetAnimatorState(horizontalSpeed > 0.05? walkingAnimationState : idleAnimationState);
        stateHandler.SetAnimatorSpeed(horizontalSpeed * animationSpeedMultiplier);
        stateHandler.Rotate(reference.characterRotationMode);

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

            // scale the velocity based off calculated control value
            velocity *= Mathf.Max(currentSpeed - control, 0) / currentSpeed;
        }

        if (isHitting) { velocity += velocity * 10f * Time.deltaTime; }

        if (isHitting) { wishDir += wishDir * 2; }
        velocity.y = -reference.gravity;
        return Accelerate(wishDir, velocity, reference);
    }

    private void Hit()
    {
        if (hitDelay > 0 || hitTimer > 0) return;

        onHit.Invoke();
        isHitting = true;
        hitTimer = 0.2f;
        stateHandler.SendAnimatorTrigger("Hit");
    }

    private void HandleHit()
    {
        if (hitTimer > 0) 
        { 
            onHit.Invoke();
            hitTimer -= Time.deltaTime;

            if (hitTimer <= 0)
            {
                isHitting = false;
                hitDelay = 1f;
            } 
        }

        hitDelay -= Time.deltaTime;
    }
}
