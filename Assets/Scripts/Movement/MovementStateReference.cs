using UnityEngine;

[CreateAssetMenu(fileName = "MovementStateReference", menuName = "Scriptable Objects/MovementStateReference")]
public class MovementStateReference : ScriptableObject
{
    // used for easy customization of movement states
    public float maxVelocity = 0.6f;
    public float acceleration = 6f;
    public float stopSpeed = 1.5f; // how fast the player slows down. use as friction
    public float gravity = -9.81f; // used only when not grounded
    public float friction = 4.0f;

    // max air velocity is omitted since air velocity should use a seperate movement state ref

    public Utils.CameraStates cameraState = Utils.CameraStates.FreeOrbit;
    public float camSensitivityMultiplier = 1.0f;
}
