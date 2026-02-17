using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private MoveStateSprint sprintState;
    [SerializeField] private MoveStateWalk walkState;

    private List<Collider> colliding;

    private void Start()
    {
        colliding = new List<Collider>();
        walkState.onHit += Hit;
    }

    private void OnTriggerEnter(Collider other)
    {
        colliding.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        colliding.Remove(other);
    }

    private void Update()
    {
        if (sprintState.GetCurrentMach() > 0 && colliding.Count > 0)
        {
            for(int i = 0; i < colliding.Count; i++) 
            {
                Prop prop = colliding[i].gameObject.GetComponent<Prop>();

                if (prop != null) 
                    if ( prop.Break(sprintState.GetCurrentMach()) ) { colliding.Remove(colliding[i]); }
            }
        }
    }

    private void Hit()
    {
        if (colliding.Count <= 0) return;

        for (int i = 0; i < colliding.Count; i++)
        {
            Prop prop = colliding[i].gameObject.GetComponent<Prop>();

            if (prop != null)
                if (prop.Break(5)) { colliding.Remove(colliding[i]); }
        }
    }


}
