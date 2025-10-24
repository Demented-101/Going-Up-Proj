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
    public enum CameraRotationMode
    {
        FreeOrbit, FollowRotation, Locked
    }
    public enum InputMappingMode
    {
        None, ToCamera, ToCameraHorizontal
    }
    public enum PGData 
    { 
        Seed, RemainingSize, BranchCountdown
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
        
    public const int gridSize = 30; // ! - always add one to integer maxs since range is excusive max
    public const int floorMinSize  = 25;
    public const int floorMaxSize  = 31; 
    public const int branchDeltaMin = 3;
    public const int branchDeltaMax = 5; 
    public const int branchSizeMin = 1;
    public const int branchSizeMax = 3;

    // floor and building math
    static public int GetFloorPointRequirement(int floor) { return (floor + 10) * 50; }
    static public int GetBuildingFloorCount(int building) { return (building * 10) + 40; }
    static public int GetFloorSeed(int floor, int building, bool isNegative = false)
    {
        // match both sides of the seed to the correct sizes
        string seedPrefix = building.ToString() + seedPadding ;
        if (seedPrefix.Length > seedPrefixSize) seedPrefix = seedPrefix.Substring(0, seedPrefixSize);

        string seedSuffix = floor.ToString() + seedPadding;
        if (seedSuffix.Length > seedSuffixSize) seedSuffix = seedSuffix.Substring(0, seedSuffixSize); 

        int result = int.Parse(seedPrefix + seedSuffix);
        return isNegative ? -result : result;
    }

    static public Vector3 GetHorizontal(Vector3 vectorIn, bool normalize) { 
        Vector3 newVec = new Vector3(vectorIn.x, 0, vectorIn.z);
        return normalize ? newVec.normalized : newVec;
    }

    public static int GridDirectionIndex(Vector2Int gridDir)
    {
        if (gridDir.x > 0 && gridDir.y == 0) { return 0; }
        if (gridDir.x == 0 && gridDir.y > 0) { return 1; }
        if (gridDir.x < 0 && gridDir.y == 0) { return 2; }
        if (gridDir.x == 0 && gridDir.y < 0) { return 3; }

        return -1;
    }

    public static Vector2Int GetGridDirection(int index)
    {
        switch (index)
        {
            case 0: return Vector2Int.right;
            case 1: return Vector2Int.up;
            case 2: return Vector2Int.left;
            case 3: return Vector2Int.down;
        }
        return Vector2Int.zero;
    }
}
