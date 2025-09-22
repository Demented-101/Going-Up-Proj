using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    private const float gravityConst = 9.81f;
    private Vector3 playerVelocity = Vector3.zero;

    public InputActionReference moveAction;
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        // Read movement input and clamp it
        Vector2 input = moveAction.action.ReadValue<Vector2>() * speed;
        Vector3 move = new Vector3(input.y, 0, input.x);
        move = Vector3.ClampMagnitude(move, 1f);

        controller.Move(move);
    }
}
