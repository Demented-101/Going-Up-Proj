using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GenObj : MonoBehaviour
{

    private GenerationHandler handler;
    public Vector2Int gridPosition;
    public int ID { get; private set; }

    [SerializeField] bool persist;

    public void Generate(Dictionary<Utils.PGData, int> data, GenerationHandler newHandler, Vector2Int gridPos)
    {
        handler = newHandler;
        handler.clear += Clear;

        ID = handler.sectionCount;
        handler.sectionCount++;

        gridPosition = gridPos;
        handler.AddGridPosition(gridPosition);

        Debug.Log("ID: " + ID + "  my grid pos: " + gridPosition);

        data[Utils.PGData.RemainingSize]--;
        

        // start new branch
        if (data[Utils.PGData.BranchCountdown] == 0)
        {
            Dictionary<Utils.PGData, int> newData = InitializeDictionary(data);
            if (AttemptMakeSegement(newData)) data[Utils.PGData.BranchCountdown] = Random.Range(Utils.branchDeltaMin, Utils.branchDeltaMax);
        }
        else
        {
            data[Utils.PGData.BranchCountdown]--;
        }

            // continue down main my branch
            AttemptMakeSegement(data);
    }

    private bool AttemptMakeSegement(Dictionary<Utils.PGData, int> data)
    {
        foreach (Vector2Int direction in GetRandomDirections())
        {
            if (data[Utils.PGData.RemainingSize] < 0) return false; // -> reached max depth, return out

            // get positions
            Vector2Int nextPosition = gridPosition + direction;
            Vector3 nextWorldPos = new Vector3(nextPosition.x * Utils.gridSize, 0, nextPosition.y * Utils.gridSize);

            // -> position is taken, continue to next direction
            if (handler.IsGridPositionUsed(nextPosition)) continue; 

            // create next segment
            GameObject newSegment = Instantiate(handler.cornerObj, nextWorldPos, Quaternion.identity);
            newSegment.transform.SetParent(handler.transform);
            newSegment.GetComponent<GenObj>()?.Generate(data, handler, nextPosition);
            return true;
        }

        return false; // didnt successfully create new segment
    }

    private static Vector2Int[] GetRandomDirections()
    {
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
            {Utils.PGData.BranchCountdown, -1 }, // -1 makes sure no new branches are started past this point
        };
        return dict;
    }

    private void Clear()
    {
        if (persist) return;

        handler.clear -= Clear;
        DestroyImmediate(this.gameObject);
    }
}
