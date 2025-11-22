using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerationHandler : MonoBehaviour
{
    [SerializeField] private GameStatus status;
    [SerializeField] private GenObj initialObject;

    public GameObject cornerObj;
    public GameObject officeSection;

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

        bool doNextPass = true;
        while (doNextPass)
        {
            doNextPass = false;
            foreach (GameObject DecorObj in GameObject.FindGameObjectsWithTag("Decorator"))
            {
                Decorator[] decorators = DecorObj.GetComponents<Decorator>();
                foreach (Decorator decorator in decorators)
                {
                    bool completedDecor = decorator.AttemptDecorate();
                    if (completedDecor) { doNextPass = true; }
                }
            }
        }
    }

    public void Clear()
    {
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
    }

    public void AddGridPosition(Vector2Int gridPos)
    {
        usedGridPositions.Add(gridPos);
    }
    public bool IsGridPositionUsed(Vector2Int gridPos)
    {
        foreach (Vector2Int usedPos in usedGridPositions)
        {
            if (usedPos.x == gridPos.x && usedPos.y == gridPos.y) { return true; }
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
            {Utils.PGData.isMainBranch, 1 }, // 1 for main branch/true, 0 for other branch/false
        };
        return dict;
    }

}
