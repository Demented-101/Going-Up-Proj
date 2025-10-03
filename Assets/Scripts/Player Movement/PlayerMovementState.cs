using UnityEngine;

public class PlayerMovementState : MonoBehaviour
{
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<PlayerMovementGameState>().controller;
    }
}
