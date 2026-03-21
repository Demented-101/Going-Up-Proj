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
        None, FollowVelocity, FollowVelocityHorizontal, FollowVelocityReversed
    }
    public enum InputMappingMode
    {
        None, ToCamera, ToCameraHorizontal
    }
    public enum PGData 
    { 
        Seed, RemainingSize, BranchCountdown, isMainBranch
    }
    
    // save files
    public const string fileName = "/player.json";
    static public string GetSaveFilePath()  { return Application.persistentDataPath + fileName; }

    // proc generation - seed generation
    private const int seedPrefixSize = 3; // the size of the buidling side of the seed
    private const int seedSuffixSize = 5; // the size of the floor sude of the seed
    private const string seedPadding = "00000"; // ! - must be longer than both pre/suffix sizes, and is a digit. is added to the front of both ends to ensure size before its cut down
        
    // proc generation - map layout and settings
    public const int gridSize = 30; // ! - always add one to integer maxs since range is excusive max

    public const int floorMinSize = 15;
    public const int floorMaxSize = 25;
    public const int forceEndSize = 50;

    public const int branchDeltaMin = 2;
    public const int branchDeltaMax = 3;
    public const int branchSizeMin = 2;
    public const int branchSizeMax = 4;
    public const int largeOfficeChance = 75;

    public const int minFloorCount = 30;
    public const int floorsPerBuilding = 5;

    // floor and building constants
    public const int winPointCost = 4000;
    public const int floorPointCost = 500;

    // floor and building math
    static public int GetBuildingFloorCount(int building) { return minFloorCount + ((building - 1) * floorsPerBuilding); }
    static public int GetFloorSeed(int floor, int building, bool isNegative = false)
    {
        // match both sides of the seed to the correct sizes
        string seedPrefix = seedPadding + building.ToString();
        if (seedPrefix.Length > seedPrefixSize) seedPrefix = seedPrefix.Substring(seedPrefix.Length - seedPrefixSize);

        string seedSuffix = seedPadding + floor.ToString();
        if (seedSuffix.Length > seedSuffixSize) seedSuffix = seedSuffix.Substring(seedSuffix.Length - seedSuffixSize); 

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
