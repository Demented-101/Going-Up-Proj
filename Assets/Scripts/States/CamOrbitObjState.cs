using UnityEngine;

public class CamOrbitObjState : GameStateBehaviour
{
    [SerializeField] private GameObject orbitObject;
    private Transform orbitTrans;

    [SerializeField] private float orbitDistance = 3f;
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float YMin = -50f;
    [SerializeField] private float YMax = 50f;

    public float oldYMin;
    public float oldYMax;
    public float transitionTime = 0;
    public float transitionSpeed = 0;

    private float currentX;
    private float currentY;

    public Vector2 movementLock;

    public override void Start()
    {
        base.Start();
        orbitTrans = orbitObject.transform;
    }

    private void LateUpdate()
    {
        if (!IsActive) { return; }

        // read mouse input
        Vector2 movement = GetMouseMovement();

        currentX += movement.x * Time.deltaTime;
        currentY += movement.y * Time.deltaTime;

        // clamp Y rotation
        Vector2 YClamp = HandleTransition();
        currentY = Mathf.Clamp(currentY, YClamp.x, YClamp.y);

        // get cam position + look at object
        Vector3 Direction = new Vector3(0, 0, -orbitDistance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = orbitTrans.position + rotation * Direction;

        transform.LookAt(orbitTrans.position);
    }

    private Vector2 GetMouseMovement()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (movementLock.x > -1)
        {
            float sign = movement.x < 0 ? -1.0f : 1.0f;
            float absMovementX = Mathf.Min(Mathf.Abs(movement.x), movementLock.x);
            movement.x = absMovementX * sign;
        }
        if (movementLock.y > -1)
        {
            float sign = movement.y < 0 ? -1.0f : 1.0f;
            float absMovementY = Mathf.Min(Mathf.Abs(movement.y), movementLock.y);
            movement.y = absMovementY * sign;
        }

        return movement * sensitivity;
    }

    private Vector2 HandleTransition()
    {
        if(transitionTime < 1) { transitionTime += Time.deltaTime * transitionSpeed; }

        float newMin = Mathf.Lerp(oldYMin, YMin, transitionTime);
        float newMax = Mathf.Lerp(oldYMax, YMax, transitionTime);

        return new Vector2(newMin, newMax);
    }

    public void SetYClamp(float Min, float Max, float speed)
    {
        oldYMax = YMax; oldYMin = YMin;
        YMax = Max; YMin = Min;
        transitionTime = 0.01f;
        transitionSpeed = speed;
    }
}
