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
        data[Utils.PGData.RemainingSize]--;

        Debug.Log("ID: " + ID + "  my grid pos: " + gridPosition);

        foreach (Vector2Int direction in GetRandomDirections())
        {
            // -> reached max depth, return out
            if (data[Utils.PGData.RemainingSize] < 0) return;

            // get positions
            Vector2Int nextPosition = gridPosition + direction;
            Vector3 nextWorldPos = new Vector3(nextPosition.x * Utils.gridSize, 0, nextPosition.y * Utils.gridSize);

            // -> position is taken, continue to next direction
            if (handler.usedGridPositions.Contains(nextPosition)) continue;

            // create next corner
            GameObject newCorner = Instantiate(handler.cornerObj, nextWorldPos, Quaternion.identity);
            newCorner.transform.SetParent(handler.transform);
            newCorner.GetComponent<GenObj>()?.Generate(data, handler, nextPosition);
        }
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

    private void Clear()
    {
        if (persist) return;

        handler.clear -= Clear;
        DestroyImmediate(this.gameObject);
    }
}
