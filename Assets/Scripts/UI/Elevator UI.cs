using UnityEngine;

public class ElevatorUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

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
