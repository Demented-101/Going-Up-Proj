using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "MovementStateReference", menuName = "Scriptable Objects/MovementStateReference")]
public class MovementStateReference : ScriptableObject
{
    // used for easy customization of movement states
    public float maxVelocity = 0.6f; // the plauers top-speed
    public float acceleration = 6f; // how fast the player speeds up
    public float stopSpeed = 1.5f; // the minimun speed at which the player stops - otherwise current speed is used
    public float gravity = 9.81f; 
    public float friction = 4.0f; // multiplies slow-down speed

    public float jumpImpulse = 2.0f;
    public float jumpLeapPower = 0.9f; // horizonal movement multiplier on-jump
    public float coyoteTime = 0.3f; // additional time to perform a jump when leaving the ground

    // max air velocity is omitted since air velocity should use a seperate movement state ref

    // camera + character transform controls
    public Utils.CameraStates cameraState = Utils.CameraStates.FreeOrbit;
    public Vector2 camSpeedLock = new Vector2(-1, -1); // -1 is uncapped, 0 means no control, 1 means only 1 unit per second max
    public Utils.CharacterRotationMode rotationMode;
}
