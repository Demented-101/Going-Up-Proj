using UnityEngine;

public class AnchorGameState : GameState
{
    [SerializeField] private GameObject anchor;
    [SerializeField] private bool copyPosition;
    [SerializeField] private bool copyRotation;

    private void Update()
    {
        if (anchor != null && IsActive)
        {
            if (copyPosition) { transform.position = anchor.transform.position; }
            if (copyRotation) { transform.rotation = anchor.transform.rotation; }
        }
    }
}
