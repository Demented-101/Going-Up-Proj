using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementState : GameState
{
    [SerializeField] private float speed = 2.0f;
    private Vector3 playerVelocity = Vector3.zero;

    [SerializeField] private InputActionReference moveAction;
    private CharacterController controller;
    private GameObject cameraObj;
    private CamOrbitObjState camHandler;

    public override void Start()
    {
        base.Start();

        controller = GetComponent<CharacterController>();
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        camHandler = cameraObj.GetComponent<CamOrbitObjState>();

    }

    private void Update()
    {
        if (!IsActive) { return; }

        Vector3 move = Vector3.zero;
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        Vector3 camForward = cameraObj.transform.forward;
        Vector3 camRight = cameraObj.transform.right;

        // Read movement input and clamp it
        Vector2 input = moveAction.action.ReadValue<Vector2>() * speed;

        // match input to camera POV
        move += camForward * Time.deltaTime * input.y;
        move += camRight * Time.deltaTime * input.x;

        // remove Y component and clamp vector
        move.y = 0;
        move = Vector3.ClampMagnitude(move, 1f);

        controller.Move(move);
    }
}
