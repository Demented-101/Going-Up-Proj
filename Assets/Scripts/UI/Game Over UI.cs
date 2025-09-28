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
        gameStatus.LoadFromGeneric();
        gameManager.EndGame();
    }
}
