using UnityEngine;

public static class Utils
{
    public enum GameStates
    {
        Pregame, Elevator, Run, GameOver,
    }

    // multiplayer (unused)
    public const int maxLobbySize = 4;
    
    // save files
    public const string fileName = "/player.json";

    static public string GetSaveFilePath()  { return Application.persistentDataPath + fileName; }

    // floor and building math
    static public int GetFloorPointRequirement(int floor) { return (floor + 10) * 50; }
    static public int GetBuildingFloorCount(int building) { return (building * 10) + 40; }
}
