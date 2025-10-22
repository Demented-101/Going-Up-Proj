using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerationHandler : MonoBehaviour
{
    [SerializeField] private GameStatus status;
    [SerializeField] private GenObj initialObject;

    public GameObject cornerObj;
    public Action clear;
    public int sectionCount = 0;
    public List<Vector2Int> usedGridPositions;

    private int seed;

    private void Start()
    {
        status.onStateChange += (Utils.GameStates state) => { 
            if (state == Utils.GameStates.Run) Generate(status.currentFloor, status.currentBuilding); 
        };
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

        Debug.Log("Starting Generation with seed: " + seed);
        
        Dictionary<Utils.PGData, int> data = InitializeDictionary(5);
        initialObject.Generate(data, this, Vector2Int.zero);
    }

    public void Clear()
    {
        try { clear?.Invoke(); } catch { }
        clear = null;
    }

    private Dictionary<Utils.PGData, int> InitializeDictionary(int size)
    {
        return new Dictionary<Utils.PGData, int>()
        {
            {Utils.PGData.Seed, seed},
            {Utils.PGData.RemainingBranches,  size},
            {Utils.PGData.RemainingSize, size},
        };
    }

}
