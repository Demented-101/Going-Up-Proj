using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MovementStateHandler))]
public class MovementState : MonoBehaviour
{
    [SerializeField] public MovementStateReference reference;

    public MovementStateHandler stateHandler { get; private set; }
    public InputManager inputManager { get; private set; }
    public CharacterController characterController { get; private set; }
    public GameObject camObj { get; private set; }
    public CamOrbitObjState camOrbit { get; private set; }


    private void Start()
    {
        stateHandler = GetComponent<MovementStateHandler>();

        inputManager = stateHandler.inputManager;
        characterController = stateHandler.controller;
        camObj = stateHandler.cameraObj;
        camOrbit = stateHandler.camOrbitController;
    }

    public static Vector3 GetMoveDirection(InputManager inputManager, bool mapToCam, GameObject cam = null)
    {
        Vector3 wishDir = Vector3.zero;
        Vector2 input = inputManager.GetCurrentInput();

        if (mapToCam && cam != null)
        {
            // input -> wish movement, rotated to fit camera perspective
            Vector3 camForward = cam.transform.forward;
            camForward.y = 0;

            Vector3 camRight = cam.transform.right;
            camRight.y = 0;

            wishDir += camForward.normalized * input.y;
            wishDir += camRight.normalized * input.x;
        }
        else
        {
            // input -> wish movement
            wishDir = new Vector3(input.y, 0, input.x);

        }

        wishDir.Normalize();
        return wishDir;
    }

    public static Vector3 Accelerate(Vector3 wishDirection, Vector3 currentVelocity, MovementStateReference reference)
    {
        float delta = Time.deltaTime;

        // how much we speed up/slow down is dependent on the difference between the wish and current velocity
        float currentSpeed = Vector3.Dot(currentVelocity, wishDirection);
        float addSpeed = Mathf.Clamp(reference.maxVelocity - currentSpeed, 0, reference.acceleration * delta); // clamped to stop the player from going too fast
        Debug.Log(currentSpeed + "    " + addSpeed);

        return currentVelocity + (wishDirection * addSpeed);
    }
}
