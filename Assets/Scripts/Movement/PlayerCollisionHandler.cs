using UnityEngine;

[RequireComponent (typeof(MoveStateSprint))]
[RequireComponent (typeof(CharacterController))]
public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private MoveStateSprint sprintState;
    [SerializeField] private Collider damageCollider;

    private const int playerCollisionLayer = 6;
    private const int Mach2CollisionLayer = 7;
    private const int Mach3CollisionLayer = 8;
    private const int Mach4CollisionLayer = 9;

    // character controller collider = player
    // as mach changes, layers for mach2 -> 4 should be enabled and disabled

    // damage collider = default
    // if mach can break, then it will


    private void Update()
    {
        int currentMach = sprintState.GetCurrentMach();

        LayerMask newMask = new LayerMask(); 
        if (currentMach < 2) { newMask.value += Mach2CollisionLayer; }
        if (currentMach < 3) { newMask.value += Mach3CollisionLayer; }
        if (currentMach < 4) { newMask.value += Mach4CollisionLayer; }

        damageCollider.excludeLayers = newMask;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("uhm");
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Prop")
        {
            Debug.Log(other.gameObject.name);
        }
    }
}
