using UnityEngine;
using UnityEngine.InputSystem;


public interface InputManager
{
    public Vector3 GetCurrentInput(); // directional movement, such as WASD for the player
    public bool GetIsSprinting(); // can be implemented for other enemies but ideally only for player
}

public class PlayerInput : MonoBehaviour, InputManager
{
    [SerializeField] private InputActionReference movementInput;
    [SerializeField] private InputActionReference sprintInput;

    public Vector3 GetCurrentInput() { return movementInput.action.ReadValue<Vector2>(); } // return WASD input
    public bool GetIsSprinting() { return sprintInput.action.IsPressed(); } // return LShift is pressed
}
