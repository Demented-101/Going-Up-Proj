using UnityEngine;

public class CamOrbitObjState : GameStateBehaviour
{
    [SerializeField] private GameObject orbitObject;
    private Transform orbitTrans;

    [SerializeField] private float orbitDistance = 3f;
    [SerializeField] private float sensitivity = 5f;

    private Vector3 faceVector = Vector3.forward;

    public Utils.CameraRotationMode rotationMode;

    public override void Start()
    {
        base.Start();
        orbitTrans = orbitObject.transform;
        faceVector = Vector3.back;
    }

    private void LateUpdate()
    {
        if (!IsActive) { return; }

        if (rotationMode == Utils.CameraRotationMode.FreeOrbit) HandleFreeOribit();

        faceVector.y = 0.4f;
        transform.position = orbitTrans.position + (faceVector * orbitDistance);
        transform.LookAt(orbitTrans.position);
    }

    private void HandleFreeOribit()
    {
        // read mouse input
        Vector2 movement = GetMouseMovement() * Time.deltaTime * sensitivity;
        faceVector = Quaternion.AngleAxis(movement.x, Vector3.up) * faceVector; // rotate around Y axis formovement
    }

    private Vector2 GetMouseMovement()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * sensitivity;
    }

    public void UpdateFaceDirection(Vector3 newVec)
    {
        if (rotationMode == Utils.CameraRotationMode.FollowRotation)
        {
            newVec.y = 0;
            if (newVec.magnitude < 0.05) { return; }
            faceVector = newVec.normalized;
        }
    }
}
