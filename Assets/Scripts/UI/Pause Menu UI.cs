using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameStatus gameStatus;
    private bool isPaused = false;

    private void Start()
    {
        gameStatus.onPauseChanged += UpdatePaused;
    }

    private void UpdatePaused()
    {
        isPaused = gameStatus.isPaused;

        gameObject.SetActive(!isPaused); // only show when paused
    }


    public void Unpause()
    {
        if (!isPaused) { return; }
        gameManager.TogglePaused();
    }

    public void SaveAndQuit()
    {
        // TODO - save game
        gameManager.EndGame();
    }
}
