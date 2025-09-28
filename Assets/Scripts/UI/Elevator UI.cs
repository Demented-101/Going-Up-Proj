using UnityEngine;

public class ElevatorUI : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void StartRun()
    {
        gameManager.StartRun();
    }

    public void SaveAndQuit()
    {
        //TODO - Save game data to fileSt
        gameManager.EndGame();
    }
}
