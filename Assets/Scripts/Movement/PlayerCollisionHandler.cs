using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private MoveStateSprint sprintState;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        Prop prop = other.gameObject.GetComponent<Prop>();
        if (prop != null)
        {
            prop.Break(sprintState.GetCurrentMach());
        }
    }
}
