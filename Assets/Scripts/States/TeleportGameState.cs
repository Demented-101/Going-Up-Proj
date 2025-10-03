using UnityEngine;

public class TeleportGameState : GameStateBehaviour
{
    [SerializeField] private GameObject anchor;
    [SerializeField] private int frameDelay = 1;
    [SerializeField] private bool copyPosition;
    [SerializeField] private bool copyRotation;
    private int moveState = -1; // -1 has moved, 0 move this frame, 1+ frames until move

    public override void OnStateChanged(Utils.GameStates newState)
    {
        base.OnStateChanged(newState);
        if (IsActive) { moveState = frameDelay; }
    }

    private void Update()
    {
        // Delay movement by a frame to ensure it moves correctly
        // this is because the order is not ensured, so if an anchor runs after this on the same frame, the movement will be cancelled.
        if (IsActive)
        {
            // dont move on this frame
            if (moveState > 0) { moveState--; }

            // move on this frame
            else if (moveState == 0)
            {
                moveState = -1;

                // for this to work on CharacterControllers, enable auto sync transforms.
                if (copyPosition) { transform.position = anchor.transform.position; }
                if (copyRotation) { transform.rotation = anchor.transform.rotation; }
            }
        }

        else // not active
        {
            moveState = -1;
        }
    }
}
