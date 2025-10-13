using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MovementStateHandler))]
public class MovementState : MonoBehaviour
{
    public enum TransitionData
    {
        Force, IgnoreVelocityCap, IgnoreCoyoteTime
    }

    [SerializeField] public MovementStateReference reference;
    public MovementStateHandler stateHandler { get; private set; }

    private void Start()
    {
        stateHandler = GetComponent<MovementStateHandler>();
    }

    public virtual void onEntered(TransitionData[] data) 
    {
        stateHandler.UpdateCameraLock(reference.camSpeedLock);
    }

    public virtual void onExit() { }

    public static Vector3 GetMoveDirection(InputManager inputManager, Utils.InputMappingMode mapping, GameObject cam = null)
    {
        Vector3 wishDir = Vector3.zero;
        Vector2 input = inputManager.GetCurrentInput();

        switch (mapping)
        {
            case Utils.InputMappingMode.None: // input -> wish movement
                wishDir = new Vector3(input.y, 0, input.x);
                break;

            case Utils.InputMappingMode.ToCamera: //input -> camera rotation
                Vector3 camForward = cam.transform.forward;
                Vector3 camRight = cam.transform.right;

                wishDir += camForward.normalized * input.y;
                wishDir += camRight.normalized * input.x;
                break;

            case Utils.InputMappingMode.ToCameraHorizontal: //input -> camera rotation (remove Y)
                Vector3 camForwardHorz = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);
                Vector3 camRightHorz = new Vector3(cam.transform.right.x, 0, cam.transform.right.z);

                wishDir += camForwardHorz.normalized * input.y;
                wishDir += camRightHorz.normalized * input.x;
                break;
        }

        return wishDir.normalized;
    }

    public static Vector3 Accelerate(Vector3 wishDirection, Vector3 currentVelocity, MovementStateReference reference)
    {
        float delta = Time.deltaTime;

        // how much we speed up/slow down is dependent on the difference between the wish and current velocity
        float currentSpeed = Vector3.Dot(currentVelocity, wishDirection);
        float addSpeed = Mathf.Clamp(reference.maxVelocity - currentSpeed, 0, reference.acceleration * delta); // clamped to stop the player from going too fast
        
        return currentVelocity + (wishDirection * addSpeed);
    }

    public bool CanSprint(float speed)
    {
        return stateHandler.inputManager.GetWishSprint() && reference.sprintSpeedRequirement < speed;
    }

    private GameObject _camObj = null;
    private CamOrbitObjState _camOrbit = null;
    public GameObject GetCameraGameObject()
    {
        if (_camObj != null) { return _camObj; }
        return GameObject.FindGameObjectWithTag("MainCamera");
    }
    public CamOrbitObjState GetCameraOrbitState()
    {
        if (_camOrbit != null) { return _camOrbit; }
        return GetCameraGameObject().GetComponent<CamOrbitObjState>();
    }
}
