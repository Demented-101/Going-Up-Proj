using UnityEngine;

public static class Utils
{
    public enum GameStates
    {
        Pregame, Elevator, Run, GameOver,
    }

    public const int maxLobbySize = 4;
    public const string fileName = "/player.json";

    public static string GetSaveFilePath()
    {
        return Application.persistentDataPath + fileName;
    }
}
