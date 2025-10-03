using UnityEngine;
using UnityEngine.InputSystem;


public interface InputManager
{
    public Vector3 GetCurrentInput();
    public bool GetIsSprinting();
}

public class PlayerInput : MonoBehaviour, InputManager
{
    [SerializeField] private InputActionReference MovementInput;

    public Vector3 GetCurrentInput() { return MovementInput.action.ReadValue<Vector2>(); }
    public bool GetIsSprinting() { return Input.GetKey(KeyCode.LeftShift); }
}
