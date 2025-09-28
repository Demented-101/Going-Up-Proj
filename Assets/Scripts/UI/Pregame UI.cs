using UnityEngine;
using static Utils;

public class PregameUI : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] GameStatus gameStatus;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void ContinueGame()
    {
        // TODO - load previous save file
        gameManager.StartGame();
    }

    public void NewGame()
    {
        gameStatus.Reset();
        gameManager.StartGame();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
