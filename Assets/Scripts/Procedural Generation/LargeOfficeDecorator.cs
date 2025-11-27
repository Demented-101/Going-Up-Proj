using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class LargeOfficeDecorator : Decorator
{
    public List<Vector2Int> officeConnections { get; private set; } = new List<Vector2Int> { };
    [SerializeField] private GameObject doorObject;
    [SerializeField] private GameObject WallObject;

    public override void Decorate()
    {
        if (generator != null && generator.handler != null)
        {
            propegate();
        }
        GenerateRoom();
    }

    public void GenerateRoom()
    {
        List<Vector2Int> remainingDirections = new List<Vector2Int> { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        if (generator != null && generator.connections != null)
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
        Vector2Int direction = new Vector2Int[] { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down }[Random.Range(0, 4)];
        Vector2Int usedDirection = Vector2Int.zero;

        for (int i = 0; i < 4; i++)
        {
            direction = new Vector2Int(-direction.y, direction.x);

            Vector2Int nextPosition = generator.gridPosition + direction;
            if (usedDirection != Vector2Int.zero && direction != -usedDirection) continue;
            if (generator.handler.IsGridPositionUsed(nextPosition)) continue;

            usedDirection = direction;
            MakeSegment(nextPosition, direction);
            generator.handler.AddGridPosition(nextPosition);
        }
    }

    private void MakeSegment(Vector2Int gridPos, Vector2Int gridDir)
    {
        Vector3 nextWorldPos = new Vector3(gridPos.x * Utils.gridSize, 0, gridPos.y * Utils.gridSize);

        GameObject newSegment = Instantiate(gameObject, nextWorldPos, Quaternion.identity, transform.parent);

        officeConnections.Add(gridDir);
        LargeOfficeDecorator nextDecorator = newSegment.GetComponent<LargeOfficeDecorator>();
        if (nextDecorator != null)
        {
            nextDecorator.officeConnections.Add(-gridDir);
            nextDecorator.GenerateRoom();
        }
    }
}
