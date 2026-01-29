using UnityEngine;
using UnityEngine.InputSystem;


public interface InputManager
{
    public Vector3 GetCurrentInput(); // directional movement, such as WASD for the player
    public bool GetWishSprint(); // can be implemented for other enemies but ideally only for player
    public bool GetWishJump(); // ditto
    public bool GetWishDash(); // ditto

}

public class PlayerInput : MonoBehaviour, InputManager
{
    [SerializeField] private InputActionReference movementInput;
    [SerializeField] private InputActionReference sprintInput;
    [SerializeField] private InputActionReference jumpInput;

    public Vector3 GetCurrentInput() { return movementInput.action.ReadValue<Vector2>(); } // return WASD input
    public bool GetWishSprint() { return sprintInput.action.IsPressed(); } // return LShift is pressed
    public bool GetWishJump() { return false; } // return space pressed this frame - unsed
    public bool GetWishDash() { return jumpInput.action.IsPressed(); } // return space for dash this frame - used instead
}
