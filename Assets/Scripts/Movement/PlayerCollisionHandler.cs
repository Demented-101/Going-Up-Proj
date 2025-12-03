using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private MoveStateSprint sprintState;
    [SerializeField] private Collider damageCollider;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        Prop prop = other.gameObject.GetComponent<Prop>();
        if (prop != null)
        {
            prop.Break();
        }
    }
}
