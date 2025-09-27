using UnityEngine;

public class AnchorGameState : GameState
{
    [SerializeField] private GameObject anchor;
    [SerializeField] private bool copyPosition;
    [SerializeField] private bool copyRotation;
    [SerializeField] private bool onlyOnEnter;
    private bool hasMoved;

    private void Update()
    {
        if (!IsActive)
        {
            hasMoved = false;
            return;
        }

        if (anchor != null && !hasMoved)
        {
            if (onlyOnEnter) { hasMoved = true; }

            if (copyPosition) { transform.position = anchor.transform.position; }
            if (copyRotation) { transform.rotation = anchor.transform.rotation; }
        }
    }
}
