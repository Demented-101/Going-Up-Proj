using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))]
public class CameraGameStateHandler : MonoBehaviour
{
    [SerializeField] private bool activePregame;
    [SerializeField] private bool activeElevator;
    [SerializeField] private bool activeRun;
    [SerializeField] private bool activeGameOver;

    [SerializeField] private GameStatus gameStatus;
    private Camera cameraComp;
    private AudioListener audioListener;

    public void Start()
    {
        cameraComp = gameObject.GetComponent<Camera>();
        audioListener = gameObject.GetComponent<AudioListener>();

        gameStatus.onStateChange += GameStateUpdate; 
    }

    private void GameStateUpdate(Utils.GameStates newState)
    {
        switch (newState)
        {
            case Utils.GameStates.Pregame:
                SetEnabled(activePregame); break;

            case Utils.GameStates.Elevator:
                SetEnabled(activeElevator); break;

            case Utils.GameStates.Run:
                SetEnabled(activeRun); break;

            case Utils.GameStates.GameOver:
                SetEnabled(activeGameOver); break;
        }
    }

    private void SetEnabled(bool enabled)
    {
        cameraComp.enabled = enabled;
        audioListener.enabled = enabled;
    }
}
