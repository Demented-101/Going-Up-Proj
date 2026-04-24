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
        gameManager.StartGame(true); // start game and load the save file
    }

    public void NewGame()
    {
        gameManager.StartGame(false); // start game and load generic 
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
