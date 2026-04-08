using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private GameStatus status;
    [SerializeField] private MusicTrack[] aud;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string mixerVolExposedName;

    enum Tracks { Pregame, Run, GameOver }
    private Tracks currentTrack;
    private MusicTrack currentPlayer;

    private void Start()
    {
        status.onStateChange += onStateChanged;
        onStateChanged(Utils.GameStates.Pregame);

        SelectPlayer(0);
    }

    private void onStateChanged(Utils.GameStates state)
    {
        switch (state)
        {
            case Utils.GameStates.Pregame:
                if (currentTrack == Tracks.GameOver) break;
                currentTrack = Tracks.Pregame;
                SelectPlayer(0);
                break;

            case Utils.GameStates.Elevator:
                if (currentTrack == Tracks.Pregame) break;
                currentTrack = Tracks.Pregame;
                SelectPlayer(0);
                break;

            case Utils.GameStates.Run:
                currentTrack = Tracks.Run;
                SelectPlayer(1);
                break;

            case Utils.GameStates.GameOver:
                currentTrack = Tracks.GameOver;
                SelectPlayer(2);
                break;

        }
    }

    private void SelectPlayer(int index)
    {
        if (currentPlayer != null) currentPlayer.Stop();

        currentPlayer = aud[index];
        currentPlayer.Select();
    }

    public void MuteMusic(bool muted)
    {
        float volume = 0f;
        if (muted) volume = -80f;
        mixer.SetFloat(mixerVolExposedName, volume);
    }


    // for each player, play from 0 when selected
    // slowly turn up the volume when selected, and decrease a bit faster when not selected
}
