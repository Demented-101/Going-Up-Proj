using UnityEngine;

public class TeleportGameState : GameState
{
    [SerializeField] private GameObject anchor;
    [SerializeField] private bool copyPosition;
    [SerializeField] private bool copyRotation;
    private bool shouldMove;
    private bool hasMoved;
    private CharacterController characterController;

    public override void Start()
    {
        base.Start();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Delay movement by a frame to ensure it moves correctly
        // this is because the order is not ensured, so if an anchor runs after this on the same frame, the movement will be cancelled.
        if (IsActive)
        {
            // if i havent moved but also shouldnt on this frame
            if (!shouldMove && !hasMoved)
            {
                shouldMove = true;
                return;
            }

            // if i should move this frame but havent yet
            if (!hasMoved && shouldMove)
            {
                hasMoved = true;
                shouldMove = false;

                // disable and re-enable any character controllers to override their position bias. This will force the new position
                if (characterController != null) { characterController.enabled = false; }

                if (copyPosition) { transform.position = anchor.transform.position; }
                if (copyRotation) { transform.rotation = anchor.transform.rotation; }

                if (characterController != null) { characterController.enabled = true; }

                return;
            }

            
        }

        else if (!IsActive)
        {
            hasMoved = false;
            shouldMove = false;
        }
    }
}
