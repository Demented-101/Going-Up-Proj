using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class MoveStateStumble : MovementState
{
    public MovementState returnState;
    public MovementState notGroundedState;
    [SerializeField] private string stumbleAnimParam = "";
    [SerializeField] private float animSpeed = 1f;
    [SerializeField] private AudioSource bumpSfx;

    private float stumbleTime;
    private const float stumbleTimeMax = 0.15f;
    private Vector3 startMomentum;

    public override void onEntered(TransitionData[] data)
    {
        base.onEntered(data);

        startMomentum = Utils.GetHorizontal(stateHandler.velocity, false) * 1.5f;
        stateHandler.Move((startMomentum * -1) - new Vector3(0, reference.gravity, 0) );
        if (data.Contains(TransitionData.IgnoreStumbleTime)) stumbleTime = 0;
        else stumbleTime = stumbleTimeMax;

        stateHandler.SetAnimatorBool(true, stumbleAnimParam);
        stateHandler.SetAnimatorSpeed(animSpeed);
        bumpSfx.Play();
    }

    public override void onExit()
    {
        base.onExit();

        stateHandler.SetAnimatorBool(false, stumbleAnimParam);
    }

    private void Update()
    {
        Vector3 velocity = stateHandler.velocity;
        Vector3 newVelocity = ProcessMovement(velocity, Vector3.zero);

        stateHandler.Move(newVelocity);

        // once "near" 0 speed, start reducing stumbleState time
        if (Utils.GetHorizontal(newVelocity, false).magnitude <= 0.05f)
        {
            stumbleTime -= Time.deltaTime; 
            if (stumbleTime <= 0f) stateHandler.ChangeState(returnState); // once stumbleState time 0, return
        }
    }

    private Vector3 ProcessMovement(Vector3 velocity, Vector3 wishDir)
    {
        float currentSpeed = velocity.magnitude;

        if (currentSpeed != 0)
        {
            float control = Mathf.Max(reference.stopSpeed, currentSpeed) * reference.friction * Time.deltaTime;

            // scale the velocity based off calculated control value
            velocity *= Mathf.Max(currentSpeed - control, 0) / currentSpeed;
        }

        velocity.y = -reference.gravity;
        return Accelerate(wishDir, velocity, reference);
    }
}
