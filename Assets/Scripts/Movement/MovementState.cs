using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MovementStateHandler))]
public class MovementState : MonoBehaviour
{
    [SerializeField] public MovementStateReference reference;
    public MovementStateHandler stateHandler { get; private set; }

    private void Start()
    {
        stateHandler = GetComponent<MovementStateHandler>();
    }

    public virtual void onEntered() { }
    public virtual void onExit() { }

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
        
        return currentVelocity + (wishDirection * addSpeed);
    }

    private GameObject _camObj = null;
    public GameObject GetCameraGameObject()
    {
        if (_camObj != null) { return _camObj; }
        return GameObject.FindGameObjectWithTag("MainCamera");
    }

    private CamOrbitObjState _camOrbit = null;
    public CamOrbitObjState GetCameraOrbitState()
    {
        if (_camOrbit != null) { return _camOrbit; }
        return GetCameraGameObject().GetComponent<CamOrbitObjState>();
    }
}
