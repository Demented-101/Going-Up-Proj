using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private MoveStateSprint sprintState;
    [SerializeField] private MoveStateWalk walkState;

    private void OnTriggerEnter(Collider other)
    {
        Prop prop = other.gameObject.GetComponent<Prop>();
        if (prop != null)
        {
            prop.Break(sprintState.GetCurrentMach(), walkState);
        }
    }
}
