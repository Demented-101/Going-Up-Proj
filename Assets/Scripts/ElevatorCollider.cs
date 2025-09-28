using UnityEngine;

public class ElevatorCollider : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameStatus gameStatus;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && gameStatus.gameState == Utils.GameStates.Run)
        {
            gameManager.EndRun();
        }
    }
}
