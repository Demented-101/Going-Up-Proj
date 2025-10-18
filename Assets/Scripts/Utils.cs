using Unity.Networking.Transport.Error;
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

    // proc generation
    private const int seedPrefixSize = 3; // the size of the buidling side of the seed
    private const int seedSuffixSize = 5; // the size of the floor sude of the seed
    private const string seedPadding = "00000"; // ! - must be longer than both pre/suffix sizes, and is a digit. is added to the front of both ends to ensure size before its cut down

    // other
    public static Vector2 genericCamLock { get; private set; } = new Vector2(-50, 50);

    // floor and building math
    static public int GetFloorPointRequirement(int floor) { return (floor + 10) * 50; }
    static public int GetBuildingFloorCount(int building) { return (building * 10) + 40; }
    static public int GetFloorSeed(int floor, int building, bool isNegative = false)
    {
        // match both sides of the seed to the correct sizes
        string seedPrefix = seedPadding + building.ToString();
        if (seedPrefix.Length > seedPrefixSize) seedPrefix = seedPrefix.Substring(0, seedPrefixSize); 

        string seedSuffix = seedPadding + floor.ToString();
        if (seedSuffix.Length > seedSuffixSize) seedSuffix = seedSuffix.Substring(0, seedSuffixSize); 

        int result = int.Parse(seedPrefix + seedSuffix);
        return isNegative ? -result : result;
    }

    static public Vector3 GetHorizontal(Vector3 vectorIn, bool normalize) { 
        Vector3 newVec = new Vector3(vectorIn.x, 0, vectorIn.z);
        return normalize ? newVec.normalized : newVec;
    }
}
