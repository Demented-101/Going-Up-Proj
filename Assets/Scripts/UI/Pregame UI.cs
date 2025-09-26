using UnityEngine;

public class PregameUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void ContinueGame()
    {
        gameManager.StartGame();
    }

    public void NewGame()
    {
        // TODO - load previous save file
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
