using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class CamOrbitObjState : GameState
{
    [SerializeField] private GameObject orbitObject;
    private Transform orbitTrans;

    [SerializeField] private float orbitDistance = 3f;
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float YMin = -50f;
    [SerializeField] private float YMax = 50f;

    private float currentX;
    private float currentY;

    public override void Start()
    {
        base.Start();
        orbitTrans = orbitObject.transform;
    }

    private void Update()
    {
        if (!IsActive) { return; }

        // read mouse input
        currentX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        currentY += -Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // clamp Y rotation
        currentY = Mathf.Clamp(currentY, YMin, YMax);

        // get cam position + look at object
        Vector3 Direction = new Vector3(0, 0, -orbitDistance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = orbitTrans.position + rotation * Direction;

        transform.LookAt(orbitTrans.position);
    }
}
