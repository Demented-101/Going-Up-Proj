using UnityEngine;

public static class Utils
{
    public enum GameStates
    {
        Pregame, Elevator, Run, GameOver,
    }
    public enum CharacterRotationMode
    {
        None, FollowVelocity, FollowVelocityHorizontal, FollowCamera, FollowCameraHorizontal
    }
    public enum InputMappingMode
    {
        None, ToCamera, ToCameraHorizontal
    }


    // multiplayer (unused)
    public const int maxLobbySize = 4;
    
    // save files
    public const string fileName = "/player.json";
    static public string GetSaveFilePath()  { return Application.persistentDataPath + fileName; }

    // other
    public static Vector2 genericCamLock { get; private set; } = new Vector2(-50, 50);

    // floor and building math
    static public int GetFloorPointRequirement(int floor) { return (floor + 10) * 50; }
    static public int GetBuildingFloorCount(int building) { return (building * 10) + 40; }
    static public Vector3 GetHorizontal(Vector3 vectorIn, bool normalize) { 
        Vector3 newVec = new Vector3(vectorIn.x, 0, vectorIn.z);
        return normalize ? newVec.normalized : newVec;
    }
}
