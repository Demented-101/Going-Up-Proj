using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuUI : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameStatus gameStatus;
    [SerializeField] private GameObject shutter;
    private bool isPaused = false;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameStatus.onPauseChanged += UpdatePaused;
        UpdatePaused();

        // make sure pause menu will always start disabled
        gameStatus.onStateChange += (state) => { if (isPaused) gameManager.TogglePaused(); };
    }

    private void UpdatePaused()
    {
        isPaused = gameStatus.isPaused;

        gameObject.SetActive(isPaused); // only show when paused
        shutter.SetActive(isPaused);
    }

    public void AddPointsTEMP()
    {
        gameStatus.AddScore(1000);
    }

    public void SaveAndQuit()
    {
        gameManager.EndGame(false); // dont save mid-game data, needed info is saved on game start.
    }
}
