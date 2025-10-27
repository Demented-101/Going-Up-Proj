using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class LargeOfficeDecorator : MonoBehaviour, Decorator
{
    public GenObj generator;

    public List<Vector2Int> officeConnections { get; private set; } = new List<Vector2Int> { };
    [SerializeField] private GameObject doorObject;
    [SerializeField] private GameObject WallObject;

    public void Decorate(GenObj genObj)
    {
        generator = genObj;
        if (generator != null)
        {
            propegate();
        }

        GenerateRoom();
    }

    public void GenerateRoom()
    {
        List<Vector2Int> remainingDirections = new List<Vector2Int> { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        if (generator != null)
        {
            foreach (Vector2Int hallDirection in generator.connections) // generate doors
            {
                remainingDirections.Remove(hallDirection);

                GameObject newHall = Instantiate(doorObject, transform.position, Quaternion.identity);
                newHall.transform.parent = transform;
                newHall.transform.LookAt(transform.position + new Vector3(-hallDirection.y, 0, hallDirection.x));
            }
        }

        foreach (Vector2Int gapDirection in officeConnections) // skip gaps - connections between offices
        {
            remainingDirections.Remove(gapDirection);
        }

        foreach (Vector2Int wallDirection in remainingDirections) // generate final walls
        {
            GameObject newWall = Instantiate(WallObject, transform.position, Quaternion.identity);
            newWall.transform.parent = transform;
            newWall.transform.LookAt(transform.position + new Vector3(-wallDirection.y, 0, wallDirection.x));
        }
    }

    private void propegate()
    {
        foreach (Vector2Int direction in new Vector2Int[] { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down })
        {
            Vector2Int nextPosition = generator.gridPosition + direction;
            if (generator.handler.IsGridPositionUsed(nextPosition)) continue;

            MakeSegment(nextPosition, direction);
            generator.handler.AddGridPosition(nextPosition);
        }
    }

    private void MakeSegment(Vector2Int gridPos, Vector2Int gridDir)
    {
        Vector3 nextWorldPos = new Vector3(gridPos.x * Utils.gridSize, 0, gridPos.y * Utils.gridSize);

        GameObject newSegment = Instantiate(gameObject, nextWorldPos, Quaternion.identity);
        newSegment.transform.SetParent(transform.parent);

        officeConnections.Add(gridDir);
        LargeOfficeDecorator nextDecorator = newSegment.GetComponent<LargeOfficeDecorator>();
        if (nextDecorator != null)
        {
            nextDecorator.officeConnections.Add(-gridDir);
            nextDecorator.Decorate(null);
        }
    }
}
