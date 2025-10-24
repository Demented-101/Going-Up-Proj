using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerationHandler : MonoBehaviour
{
    [SerializeField] private GameStatus status;
    [SerializeField] private GenObj initialObject;

    public GameObject cornerObj;

    public Action clear;
    public int sectionCount = 0;
    public List<Vector2Int> usedGridPositions { get; private set; }

    private int seed;

    private void Start()
    {
        if (status != null)
        {
            status.onStateChange += (Utils.GameStates state) =>
            {
                if (state == Utils.GameStates.Run) Generate(status.currentFloor, status.currentBuilding);
            };
        }
    }

    public void Generate(int floor, int building)
    {
        Clear();
        sectionCount = 0;
        usedGridPositions = new List<Vector2Int>
        {
            new Vector2Int(0, 0) // starting room (elevator)
        };

        seed = Utils.GetFloorSeed(floor, building);
        UnityEngine.Random.InitState(seed);
        int floorSize = UnityEngine.Random.Range(Utils.floorMinSize, Utils.floorMaxSize);

        Debug.Log("Starting Generation with seed: " + seed);
        
        Dictionary<Utils.PGData, int> data = InitializeDictionary(floorSize);
        initialObject.Generate(data, this, Vector2Int.zero, Vector2Int.zero);
    }

    public void Clear()
    {
        try { clear?.Invoke(); } catch { }
        clear = null;
    }

    public void AddGridPosition(Vector2Int gridPos)
    {
        usedGridPositions.Add(gridPos);
    }
    public bool IsGridPositionUsed(Vector2Int gridPos)
    {
        foreach (Vector2Int usedPos in usedGridPositions)
        {
            Debug.Log(usedPos);
            if (usedPos.x == gridPos.x && usedPos.y == gridPos.y) { Debug.Log("AAA"); return true; }
        }
        return false;
    }

    private Dictionary<Utils.PGData, int> InitializeDictionary(int size)
    {
        Dictionary<Utils.PGData, int> dict = new Dictionary<Utils.PGData, int>()
        {
            {Utils.PGData.Seed, seed},
            {Utils.PGData.RemainingSize, size},
            {Utils.PGData.BranchCountdown, UnityEngine.Random.Range(Utils.branchDeltaMin, Utils.branchDeltaMax) },
        };
        return dict;
    }

}
