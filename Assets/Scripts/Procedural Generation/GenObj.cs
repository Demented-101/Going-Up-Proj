using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GenObj : MonoBehaviour
{
    public GenerationHandler handler { get; private set; }
    public Vector2Int gridPosition;
    public int ID { get; private set; }
    public bool isBranch;
    public List<Vector2Int> connections { get; private set; }

    [SerializeField] bool isInitial;

    public void Generate(Dictionary<Utils.PGData, int> data, GenerationHandler newHandler, Vector2Int gridPos, Vector2Int incomingDirection)
    {
        handler = newHandler;

        // ID and section counts
        ID = handler.sectionCount;
        handler.sectionCount++;
        data[Utils.PGData.RemainingSize]--;

        // grid positions
        gridPosition = gridPos;
        handler.AddGridPosition(gridPosition);

        // connections for decorator
        connections = new List<Vector2Int>();
        if (incomingDirection.magnitude != 0) connections.Add(incomingDirection);

        // start new branch
        if (data[Utils.PGData.BranchCountdown] == 0 && !isInitial && data[Utils.PGData.RemainingSize] > 0)
        {
            Dictionary<Utils.PGData, int> newData = InitializeDictionary(data);
            if (AttemptMakeSegement(newData, true)) { data[Utils.PGData.BranchCountdown] = Random.Range(Utils.branchDeltaMin, Utils.branchDeltaMax); }
        }
        else data[Utils.PGData.BranchCountdown]--;

        // continue down main my branch
        AttemptMakeSegement(data);

        // on backtrace, add self reference to all decorators if needed
        if (gameObject.GetComponent<Decorator>() != null && !isInitial)
        {
            Decorator[] decorators = gameObject.GetComponents<Decorator>();
            foreach (Decorator decor in decorators) { decor.generator = this; }
        }
    }

    private bool AttemptMakeSegement(Dictionary<Utils.PGData, int> data, bool onlyMakeOne = false)
    {
        bool hasMadeSegment = false;

        foreach (Vector2Int direction in GetRandomDirections())
        {
            if (data[Utils.PGData.RemainingSize] < 0) return false; // -> reached max depth, return out
            Vector2Int nextPosition = gridPosition + direction;

            // -> position is taken, continue to next direction
            if (handler.IsGridPositionUsed(nextPosition)) continue;

            // create next segment
            MakeSegment(nextPosition, direction, data, handler.sectionCount >= Utils.forceEndSize);
            if (onlyMakeOne) return true;
            hasMadeSegment = true;
        }

        return hasMadeSegment; // didnt successfully create new segment
    }

    public void MakeSegment(Vector2Int gridPos, Vector2Int gridDir, Dictionary<Utils.PGData, int> data, bool forceOffice = false)
    {
        Vector3 nextWorldPos = new Vector3(gridPos.x * Utils.gridSize, 0, gridPos.y * Utils.gridSize);
        bool isOfficeSection = data[Utils.PGData.RemainingSize] == 0 || forceOffice;

        GameObject roomObj = isOfficeSection ? handler.officeSection : handler.cornerObj;

        GameObject newSegment = Instantiate(roomObj, nextWorldPos, Quaternion.identity);
        newSegment.transform.SetParent(handler.transform);
        AddConnection(gridDir);

        GenObj generationComponent = newSegment.GetComponent<GenObj>();
        if (generationComponent != null)
        {
            generationComponent.Generate(data, handler, gridPos, gridDir * -1);
        }
    }

    private Vector2Int[] GetRandomDirections()
    {
        if (isInitial) { return new Vector2Int[] { Vector2Int.up }; }

        Vector2Int[] directions = new Vector2Int[] { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        for (int i = 0; i < directions.Length -1; i++)
        {
            int num = Random.Range(0, directions.Length);

            Vector2Int temp = directions[i];
            directions[i] = directions[num];
            directions[num] = temp;
        }

        return directions;
    }

    private Dictionary<Utils.PGData, int> InitializeDictionary(Dictionary<Utils.PGData, int> mainBranchData)
    {
        Dictionary<Utils.PGData, int> dict = new Dictionary<Utils.PGData, int>()
        {
            {Utils.PGData.Seed, mainBranchData[Utils.PGData.Seed]},
            {Utils.PGData.RemainingSize, Random.Range(Utils.branchSizeMin, Utils.branchSizeMax)},
            {Utils.PGData.BranchCountdown, -1}, // dont allow further branches
            {Utils.PGData.isMainBranch, 0 },
        };
        return dict;
    }

    public void AddConnection(Vector2Int direction)
    {
        connections.Add(direction);
    }
}
