using UnityEngine;

[RequireComponent(typeof(MovementStateHandler))]
public class MovementState : MonoBehaviour
{
    private MovementStateHandler stateHandler;

    private InputManager inputManager;
    private CharacterController characterController;

    private void Start()
    {
        stateHandler = GetComponent<MovementStateHandler>();

        inputManager = stateHandler.inputManager;
        characterController = stateHandler.controller;
    }
}
