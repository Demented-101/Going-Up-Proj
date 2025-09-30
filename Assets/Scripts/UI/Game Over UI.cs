using UnityEngine;
using static Utils;

public class GameOverUI : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] GameStatus gameStatus;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void ReturnToMenu()
    {
        gameStatus.LoadFromSave(true);
        gameManager.EndGame(true); // Overrides the save file to ensure that the player cannot just go back and try again.
    }
}
